namespace SmartTeethCare.API.Extensions
{
    public static class CorsServiceExtension
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:8081",
                            "http://localhost:5039",
                            "http://localhost:8080",
                            "https://dental-clinic-project-ten.vercel.app"  // frontend url for production
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}