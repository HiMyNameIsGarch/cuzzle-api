using cuzzle_api.Services.TokenService;
using cuzzle_api.Services.AuthService;
using cuzzle_api.Services.PuzzleService;
using cuzzle_api.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbServicesExtension
    {
        public static IServiceCollection AddDbContext(
             this IServiceCollection services, IConfiguration config)
        {
            // Authentication things
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IDbService, CuzzleEntity>();

            // Models
            services.AddSingleton<IPuzzleService, PuzzleService>();
            
            // Http
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
