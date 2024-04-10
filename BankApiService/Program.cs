using BankApiService.Context;
using BankApiService.Services;

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


            builder.Services.AddEntityFrameworkSqlite()
                            .AddDbContext<BankContext>();

            var serviceProvide = builder.Services.BuildServiceProvider();
            var context = serviceProvide.GetRequiredService<BankContext>();

            //Check database exists, if not create it
            context.Database.EnsureCreated();


            // DI
            builder.Services.AddSingleton<IAccountsService, AccountsService>();
            builder.Services.AddSingleton<ITransactionsService, TransactionsService>();
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
