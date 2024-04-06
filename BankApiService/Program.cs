using System;
using BankApiService.Controllers;
using BankApiService.CsvHelperService;
using BankApiService.Dependcies;
using BankApiService.Dependcies.LifeCycle;
using BankApiService.Models;

namespace BankApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = builder.Environment.EnvironmentName;

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Add services to the container.

            var configuration = builder.Configuration;

            builder.Services.AddControllers();

            // DI
            builder.Services.AddSingleton<CsvService<Account>>();
            builder.Services.AddSingleton<CsvService<Transaction>>();

            builder.Services.AddSingleton<SecondTestService>();
            builder.Services.AddSingleton<FirstTestService>();

            builder.Services.AddTransient<RequestService>();
            builder.Services.AddSingleton<SingletonDep>();
            builder.Services.AddTransient<TransientDep>();
            builder.Services.AddScoped<ScopedDep>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            // Add Cors
            builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            };

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.MapControllers();

            app.Run();
        }
    }
}
