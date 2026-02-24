
using YourPubQuiz.Services;
using YourPubQuiz.Singletons;

namespace YourPubQuiz;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<OpenTdbService>();
        builder.Services.AddSingleton<QuestionData>();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AngularClient", policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Clacks-Overhead"] = "GNU Terry Pratchett";
            await next();
        });
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseAuthorization();
        app.UseCors("AngularClient");
        
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapFallbackToFile("index.html");
        
        app.MapControllers();

        app.Run();
    }
}