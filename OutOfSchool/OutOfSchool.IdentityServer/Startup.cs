using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OutOfSchool.Common.Models;
using OutOfSchool.IdentityServer.Config.ExternalUriModels;
using OutOfSchool.IdentityServer.Extensions;
using OutOfSchool.IdentityServer.Validators;
using OutOfSchool.IdentityServer.ViewModels;

namespace OutOfSchool.IdentityServer;

public static class Startup
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        services.Configure<IdentityServerConfig>(config.GetSection(IdentityServerConfig.Name));
        var migrationsAssembly = config["MigrationsAssembly"];

        // TODO: Move version check into an extension to reuse code across apps
        var mySQLServerVersion = config["MySQLServerVersion"];
        var serverVersion = new MySqlServerVersion(new Version(mySQLServerVersion));
        if (serverVersion.Version.Major < Constants.MySQLServerMinimalMajorVersion)
        {
            throw new Exception("MySQL Server version should be 8 or higher.");
        }

        var connectionString = config.GetMySqlConnectionString<IdentityConnectionOptions>(
            "DefaultConnection",
            options => new MySqlConnectionStringBuilder
            {
                Server = options.Server,
                Port = options.Port,
                UserID = options.UserId,
                Password = options.Password,
                Database = options.Database,
                GuidFormat = options.GuidFormat.ToEnum(MySqlGuidFormat.Default),
            });
        services
            .AddDbContext<OutOfSchoolDbContext>(options => options
                .UseMySql(
                    connectionString,
                    serverVersion,
                    optionsBuilder =>
                        optionsBuilder
                            .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)
                            .MigrationsAssembly(migrationsAssembly)));

        services.AddCustomDataProtection("IdentityServer");

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddAuthentication("Bearer")
            .AddIdentityServerAuthentication("Bearer", options =>
            {
                options.ApiName = "outofschoolapi";
                options.Authority = config["Identity:Authority"];

                options.RequireHttpsMetadata = false;
            });

        var issuerSection = config.GetSection(IssuerConfig.Name);
        services.Configure<IssuerConfig>(issuerSection);

        // GRPC options
        services.Configure<GRPCConfig>(config.GetSection(GRPCConfig.Name));

        // ExternalUris options
        services.Configure<AngularClientScopeExternalUrisConfig>(config.GetSection(AngularClientScopeExternalUrisConfig.Name));

        services.ConfigureIdentity(
            connectionString,
            issuerSection["Uri"],
            serverVersion,
            migrationsAssembly,
            identityBuilder =>
            {
                identityBuilder.AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);
            },
            identityServerBuilder =>
            {
                identityServerBuilder.AddAspNetIdentity<User>();
                identityServerBuilder.AddProfileService<ProfileService>();
            });

        services.ConfigureApplicationCookie(c =>
        {
            c.Cookie.Name = "IdentityServer.Cookie";
            c.LoginPath = "/Auth/Login";
            c.LogoutPath = "/Auth/Logout";
        });

        var mailConfig = config
            .GetSection(EmailOptions.SectionName)
            .Get<EmailOptions>();
        services.AddEmailSender(
            builder.Environment.IsDevelopment(),
            mailConfig.SendGridKey,
            emailOptions => emailOptions.Bind(config.GetSection(EmailOptions.SectionName)));

        services.AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            });

        services.AddProxy();
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient(typeof(IEntityRepository<,>), typeof(EntityRepository<,>));
        services.AddTransient<IProviderAdminRepository, ProviderAdminRepository>();
        services.AddTransient<IParentRepository, ParentRepository>();
        services.AddTransient<IProviderAdminService, ProviderAdminService>();
        services.AddTransient<IUserManagerAdditionalService, UserManagerAdditionalService>();
        services.AddTransient<IInstitutionAdminRepository, InstitutionAdminRepository>();
        services.AddTransient<IRegionAdminRepository, RegionAdminRepository>();
        services.AddTransient<ICommonMinistryAdminService<MinistryAdminBaseDto>,
            CommonMinistryAdminService<Guid, InstitutionAdmin, MinistryAdminBaseDto, IInstitutionAdminRepository>>();
        services.AddTransient<ICommonMinistryAdminService<RegionAdminBaseDto>,
            CommonMinistryAdminService<long, RegionAdmin, RegionAdminBaseDto, IRegionAdminRepository>>();
        services.AddTransient<IProviderAdminChangesLogService, ProviderAdminChangesLogService>();

        // Register the Permission policy handlers
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

        services.AddFluentValidationAutoValidation();

        services.AddScoped<IValidator<RegisterViewModel>, RegisterViewModelValidator>();

        services.AddGrpc();

        services.AddHostedService<AdditionalClientsHostedService>();

        services.AddHealthChecks()
            .AddDbContextCheck<OutOfSchoolDbContext>(
                "Database",
                tags: new[] { "readiness" });
    }

    public static void Configure(this WebApplication app)
    {
        var proxyOptions = app.Configuration.GetSection(ReverseProxyOptions.Name).Get<ReverseProxyOptions>();
        app.UseProxy(proxyOptions);

        app.UseSecurityHttpHeaders(app.Environment.IsDevelopment());

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        var supportedCultures = new[]
        {
                new CultureInfo("en"),
                new CultureInfo("uk"),
        };

        var requestLocalization = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("uk"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures,
            RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new CustomRequestCultureProvider(context =>
                    {
                        if (!context.Request.Query.TryGetValue("ui-culture", out var selectedUiCulture) &
                            !context.Request.Query.TryGetValue("culture", out var selectedCulture))
                        {
                            var encodedPathAndQuery = context.Request.GetEncodedPathAndQuery();
                            var decodedUrl = WebUtility.UrlDecode(encodedPathAndQuery);
                            var dictionary = QueryHelpers.ParseQuery(decodedUrl);
                            dictionary.TryGetValue("ui-culture", out selectedUiCulture);
                            dictionary.TryGetValue("culture", out selectedCulture);
                        }

                        if (selectedCulture.FirstOrDefault() is null ^ selectedUiCulture.FirstOrDefault() is null)
                        {
                            return Task.FromResult(new ProviderCultureResult(
                                selectedCulture.FirstOrDefault() ??
                                selectedUiCulture.FirstOrDefault()));
                        }

                        return Task.FromResult(new ProviderCultureResult(
                            selectedCulture.FirstOrDefault(),
                            selectedUiCulture.FirstOrDefault()));
                    }),
                },
        };

        app.UseRequestLocalization(requestLocalization);

        app.UseRouting();

        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

        app.UseStaticFiles();

        app.UseSerilogRequestLogging();

        app.UseIdentityServer();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("readiness"),
                AllowCachingResponses = false,
            })
            .WithMetadata(new AllowAnonymousAttribute());

        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

        var gRPCConfig = app.Configuration.GetSection(GRPCConfig.Name).Get<GRPCConfig>();

        if (gRPCConfig.Enabled)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ProviderAdminServiceGRPC>().RequireHost($"*:{gRPCConfig.Port}");
            });
        }
    }
}
