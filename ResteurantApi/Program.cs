using AutoMapper;

using ResteurantApi.Entities;
using ResteurantApi.Services;

using System.Reflection;
using System.Text;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using ResteurantApi;
using ResteurantApi.Authorization;
using ResteurantApi.Middleware;
using ResteurantApi.Models;
using ResteurantApi.Models.Validators;


var builder = WebApplication.CreateBuilder();

//u¿ycie nLoga dotnet6
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

//configure services

        var authenticationsSettings = new AuthenticationSettings();
        //³¹czymy wartosc z appsettings do zmiennej z nowej klasy. Bind s³u¿y do ³¹czenia 
        builder.Configuration.GetSection("Authentication").Bind(authenticationsSettings);
        
        //rejestracja w kontenerze zaleznosci ustawien autentykacji 
        builder.Services.AddSingleton(authenticationsSettings);
        //dodanie serwisu autentykacji,  ustalamy w jaki sposob ma byc przeprowadzana autentykacja
        builder.Services.AddAuthentication(option =>
        {
            //ustawiamy ze dla wszystkich przypadków schemat bedzie sprawdzal po tokenie w nag³ówku autentykacji
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;//nie wymuszamy protokolu https
            cfg.SaveToken = true; //chcemy aby server zapisa³ token
            cfg.TokenValidationParameters = new TokenValidationParameters()//ustalamy parametry walidacji tokena
            {
                ValidIssuer = authenticationsSettings.JwtIssuer, //wydawca tokenu
                ValidAudience = authenticationsSettings.JwtIssuer, //okreslamy kto moze uzywac tokenu - tutaj generujemy token tylko dla siebie
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationsSettings.JwtKey)), //klucz prywatny wygenerowany na podstawie zmiennej w pliku appsettings
            };
        });

        //sprawdzamy czy token zawiera w sobie narodowasc 
        //twowrzenie naszych wlasnych zasad autentykacji
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Poland"));
            options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequiermant(20)));
            options.AddPolicy("CreatedAtleast2", builder => builder.AddRequirements(new CreatedMultipleResteurantRequirement(2)));
        });
        //u¿ycie naszej polityki autoryzacji
        builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequiermantHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleResteurantRequirementHandler>();


        builder.Services.AddRazorPages();
        //u¿ywanie kontrolerów oraz dodanie implementacji bibloteki walidatora
        builder.Services.AddControllers().AddFluentValidation();
        //implementacja bazy danych
        builder.Services.AddDbContext<ResteurantDBContext>();
        //zaplenienie bazzy danych danymi
        builder.Services.AddScoped<ResteurantSeeder>();
        //mapper s³u¿acy do mapowania danych i ich wysylania
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        //implementacja serwisu
        builder.Services.AddScoped<IResteurantService, ResteurantService>();
        builder.Services.AddScoped<IDishService, DishService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        //implemetnacja obslugi wyjatkow
        builder.Services.AddScoped<ErrorHandlingMiddleware>();
        builder.Services.AddScoped<RequestTimeMiddleware>();
        //implemetnacja hashera hasel dla uzytkownika
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        //implemenacja walidatora sprawdzajacego uzytkownika podczas rejestracji
        builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        //implemetnacja walidatora ilosci stron podczas wyswietlania wynikow
        builder.Services.AddScoped<IValidator<ResteurantQuery>, ResteurantQueryValidator>();
        //implemetnacja contextu uzytkownika
        builder.Services.AddScoped<IUserContextService, UserContextService>();
        builder.Services.AddHttpContextAccessor();

        //dodanie swaggera
        builder.Services.AddSwaggerGen();

        //mozliwosc ³¹czenia sie z frontendem
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendEndClient", policybuilder =>
                policybuilder
                    .AllowAnyMethod()
                    .WithOrigins(builder.Configuration["AllowedOrigins"])
            );
        });

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

//configure app
var app = builder.Build();

        //configure seeder
        var scope = app.Services.CreateScope();
        //pobranie z kontenera dependency injection
        var seeder = scope.ServiceProvider.GetRequiredService<ResteurantSeeder>();


        //mechanizm cachowania
        app.UseResponseCaching();
        //pobieranie plikow statycznych
        app.UseStaticFiles();
        app.UseCors("FrontendEndClient");


        //dodanie seedera ktory wype³ni baze danych danymi poczatkowymi
        seeder.Seed();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        //sprawdzanie wyjatkow
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestTimeMiddleware>();
        //uruchomienie autentykacji
        app.UseAuthentication();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseDefaultFiles();
        app.UseStaticFiles();



        //uzycie swaggera
        app.UseSwagger();
        //implemetnacja intrefejsu
        app.UseSwaggerUI(c =>
        {

            //ustalenie endpointu swaggera i wskazanie na glowny wygenerowany plik
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resteruant API");
        });
        app.UseRouting();
        //autoryzacja uzytkownika
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();

        });

app.Run();