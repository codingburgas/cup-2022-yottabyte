using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Azure;
using Tailwind;
using Microsoft.AspNetCore.StaticFiles;
using Yottabyte.Server.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Yottabyte.Data;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Services;

namespace Yottabyte.Server;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")!, o =>
           {
               o.MigrationsAssembly("Yottabyte.Server");
               o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
           }));

        // For Identity
        services
            .AddIdentity<User, IdentityRole>(options =>
            {
                /*options.SignIn.RequireConfirmedEmail = true;*/
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddLogging(conf =>
            {
                conf.ClearProviders();

                // conf.AddSeq(configuration.GetSection("Seq"));
                conf.AddAzureWebAppDiagnostics();
                conf.AddConsole();
            })
            .Configure<AzureFileLoggerOptions>(options =>
            {
                options.FileName = "first-azure-log";
                options.FileSizeLimit = 50 * 1024;
                options.RetainedFileCountLimit = 10;
            });

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]!)),
                };
            });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder.WithOrigins("http://localhost:19006", "https://localhost:19006", "http://mapit.studio", "https://mapit.studio", "http://www.mapit.studio", "https://www.mapit.studio", "https://ashy-cliff-062a3df03.2.azurestaticapps.net", "http://ashy-cliff-062a3df03.2.azurestaticapps.net")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddServices();
        services.AddSwagger();

        services.AddAutoMapper(typeof(MappingProfile));


        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(Configuration["AzureStorage:ConnectionString:blob"], preferMsi: true);
            builder.AddQueueServiceClient(Configuration["AzureStorage:ConnectionString:queue"], preferMsi: true);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
            app.RunTailwind("tailwind", "../Client/");
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        var contentTypes = new FileExtensionContentTypeProvider();
        contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";

        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = contentTypes
        });

        app.UseCors();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
