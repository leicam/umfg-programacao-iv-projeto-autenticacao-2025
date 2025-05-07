
using umfgcloud.autenticacao.webapi.Extensions;

namespace umfgcloud.autenticacao.webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile($"appsettings.json", false);
            builder.Configuration.AddJsonFile(
                $"appsettings.{Environment
                .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();
            builder.Services.AddCorsPolicy(builder.Configuration);
            builder.Services.AddAutenticacao(builder.Configuration);
            builder.Services.RegistrarServicos(builder.Configuration);

            var app = builder.Build();

            app.UseCorsPolicy();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
