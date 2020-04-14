using System.Text;
using Amazon.DynamoDBv2;
using AutoMapper;
using InventoryBooks.ActionFilters;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Application.Services;
using InventoryBooks.Helper;
using InventoryBooks.Infrastructure;
using InventoryBooks.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace InventoryBooks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            CorsPolicyBuilder corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder.Build());             
               
            });
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //aws option
            var awsOptions = Configuration.GetAWSOptions();
            services.AddDefaultAWSOptions(awsOptions);
            var dynamoDbOption = new DynamoDbOptions();
            ConfigurationBinder.Bind(Configuration.GetSection("DynamoDbTables"), dynamoDbOption);
            var client = awsOptions.CreateServiceClient<IAmazonDynamoDB>();

            services.AddSingleton<IDynamoDbContext<UserDetails>>(prop => new DynamoDbContext<UserDetails>(client, dynamoDbOption.UserDetails));
            services.AddSingleton<IDynamoDbContext<Roles>>(prop => new DynamoDbContext<Roles>(client, dynamoDbOption.Roles));
            services.AddSingleton<IDynamoDbContext<BookDetails>>(prop => new DynamoDbContext<BookDetails>(client, dynamoDbOption.BookDetails));
            services.AddSingleton<IDynamoDbContext<UserServicesItem>>(prop => new DynamoDbContext<UserServicesItem>(client, dynamoDbOption.IssueBookInventory));
            services.AddSingleton<IDynamoDbContext<InventoryHistory>>(prop => new DynamoDbContext<InventoryHistory>(client, dynamoDbOption.InventoryHistory));
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<IInventoryServices, InventoryServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IAdminServices, AdminServices>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IReminderServices, ReminderServices>();
            services.AddScoped<IProfileServices, ProfileServices>();

            var appSettings = new AppSettings();
            ConfigurationBinder.Bind(Configuration.GetSection("AppSettings"), appSettings);
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            // JwtBearerDefaults
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "",
                        ValidAudience = "",
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            services.AddScoped<SecurityFilter>();
            services.AddAutoMapper(typeof(Startup));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerDocumentation();
            services.AddMvc();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks($"/health");
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSwaggerDocumentation();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
