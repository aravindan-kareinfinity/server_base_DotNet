using PlanItNoww.AwsS3;
using PlanItNoww.Services;
using PlanItNoww.Utils;

namespace PlanItNoww
{
    public static class ServicesConfiguration
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<UserSocketService>();
            services.AddSingleton<IQueryBuilderProvider, QueryBuilderProvider>();
            services.AddTransient<CustomCryptography>();

            // AWS S3
            services.AddSingleton<AwsS3Service>();

            //DB
            services.AddTransient<FilesService>();
          
         
            services.AddTransient<UserSessionService>();
            services.AddTransient<UsersService>();
            services.AddTransient<ReferenceTypeService>();
            services.AddScoped<ReferenceValueService>();
            services.AddTransient<UserProfilesService>();
            services.AddTransient<RolesService>();
            services.AddTransient<AadhaarVerificationsService>();
            services.AddTransient<OTPsService>();
            services.AddTransient<PaymentsService>();
            services.AddTransient<PermissionsReferenceService>();


            //        services.AddScoped<OrganisationServiceTimingService>();


        }
    }
}
