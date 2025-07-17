using PlanItNoww.Middlewares;
using PlanItNoww.Utils;

namespace PlanItNoww
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = CreateWebApplication(args);
            try
            {
                var preset = new Preset();
                await preset.Start(app);
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadKey();
            }
        }
        public static WebApplication CreateWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<ApplicationEnvironment>(builder.Configuration.GetSection("ApplicationSettings"));
            var appSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationEnvironment>();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddLog4Net("log4net.config");

            builder.Services.AddControllersWithViews().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                //options.CustomSchemaIds(type => type.ToString());
                options.CustomSchemaIds(type => type.FullName.Replace("+", "."));
            });
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<AppState>();
            builder.Services.AddScoped<RequestState>();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            builder.Services.AddScoped<IDbProvider, PostgreSQLProvider>();
            builder.Services.AddCustomServices();
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    cookieContext.CookieOptions.SameSite = SameSiteMode.None;
                options.OnDeleteCookie = cookieContext =>
                    cookieContext.CookieOptions.SameSite = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always; // required for chromium-based browsers

            });
            var app = builder.Build();
            app.Use((context, next) =>
            {
                context.Response.Headers["X-Frame-Options"] = "ALLOWALL";
                return next.Invoke();
            });
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            app.UseCookiePolicy();
            //app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseWebSockets();
            app.UseMiddleware<JwtMiddleware>();
            app.UseStaticFiles();
            app.UseMiddleware<MobileRedirectMiddleware>();
            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapFallbackToFile("index.html");
            return app;
        }
    }
}

