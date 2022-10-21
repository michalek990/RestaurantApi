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
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
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
            services.AddRazorPages();
            //u�ywanie kontroler�w oraz dodanie implementacji bibloteki walidatora
            services.AddControllers().AddFluentValidation();
            //implementacja bazy danych
            services.AddDbContext<ResteurantDBContext>();
            //zaplenienie bazzy danych danymi
            services.AddScoped<ResteurantSeeder>();
            //mapper s�u�acy do mapowania danych i ich wysylania
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
            //dodanie swaggera
            services.AddSwaggerGen();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ResteurantSeeder seeder)
        {
            //dodanie seedera ktory wype�ni baze danych danymi poczatkowymi
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

            });
        }
    }
}
