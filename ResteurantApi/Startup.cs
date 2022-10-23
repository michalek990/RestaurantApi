using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResteurantApi.Entities;
using ResteurantApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ResteurantApi.Authorization;
using ResteurantApi.Middleware;
using ResteurantApi.Models;
using ResteurantApi.Models.Validators;

namespace ResteurantApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var authenticationsSettings = new AuthenticationSettings();
            //³¹czymy wartosc z appsettings do zmiennej z nowej klasy. Bind s³u¿y do ³¹czenia 
            Configuration.GetSection("Authentication").Bind(authenticationsSettings);

            //rejestracja w kontenerze zaleznosci ustawien autentykacji 
            services.AddSingleton(authenticationsSettings);
            //dodanie serwisu autentykacji,  ustalamy w jaki sposob ma byc przeprowadzana autentykacja
            services.AddAuthentication(option =>
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
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Poland"));
                options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequiermant(20)));
                options.AddPolicy("CreatedAtleast2", builder => builder.AddRequirements(new CreatedMultipleResteurantRequirement(2)));
            });
            //u¿ycie naszej polityki autoryzacji
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequiermantHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, CreatedMultipleResteurantRequirementHandler>();


            services.AddRazorPages();
            //u¿ywanie kontrolerów oraz dodanie implementacji bibloteki walidatora
            services.AddControllers().AddFluentValidation();
            //implementacja bazy danych
            services.AddDbContext<ResteurantDBContext>();
            //zaplenienie bazzy danych danymi
            services.AddScoped<ResteurantSeeder>();
            //mapper s³u¿acy do mapowania danych i ich wysylania
            services.AddAutoMapper(this.GetType().Assembly);
            //implementacja serwisu
            services.AddScoped<IResteurantService, ResteurantService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IAccountService, AccountService>();
            //implemetnacja obslugi wyjatkow
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<RequestTimeMiddleware>();
            //implemetnacja hashera hasel dla uzytkownika
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            //implemenacja walidatora sprawdzajacego uzytkownika podczas rejestracji
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            //implemetnacja walidatora ilosci stron podczas wyswietlania wynikow
            services.AddScoped<IValidator<ResteurantQuery>, ResteurantQueryValidator>();
            //implemetnacja contextu uzytkownika
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();

            //dodanie swaggera
            services.AddSwaggerGen();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ResteurantSeeder seeder)
        {
            //dodanie seedera ktory wype³ni baze danych danymi poczatkowymi
            seeder.Seed();
            if (env.IsDevelopment())
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
        }
    }
}
