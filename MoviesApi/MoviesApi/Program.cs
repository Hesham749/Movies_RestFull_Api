using Microsoft.OpenApi.Models;
using MoviesApi.Data;
using MoviesApi.Interfaces;
using MoviesApi.Repositories;

namespace MoviesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors();
            builder.Services.AddTransient<IUnitOfWorks, UnitOfWorks>();
            builder.Services.AddDbContext<ApplicationDbContext>(op =>
            {
                op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #region Swagger configurations
            builder.Services.AddSwaggerGen(op =>
                {
                    op.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "My Movies Api",
                        Version = "v1",
                        Contact = new OpenApiContact { Email = "HeshamElsayedAhmed@outlock.com", Name = "Me" }
                    });

                    op.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter your token",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                    });

                    op.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                           }
                       },
                       new List<string>()
                    }

                    });
                });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
