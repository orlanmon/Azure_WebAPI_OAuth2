
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;




namespace OAuth2.Web.API
{
    public class Program
    {
        public static void Main(string[] args)
        {


            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Support for Minimal APIs
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddOpenApi();


            // Add the following to Conifgure CORS Settings

            
            builder.Services.AddCors(options =>
            {
                // Configure SpecificOriginsPolicy
                var specificOriginsSection = builder.Configuration.GetSection("CorsSettings:SpecificOriginsPolicy");
                if (specificOriginsSection.Exists())
                {
                    var allowedOrigins = specificOriginsSection["AllowedOrigins"]?.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var allowedHeaders = specificOriginsSection["AllowedHeaders"]?.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var allowedMethods = specificOriginsSection["AllowedMethods"]?.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var allowCredentials = specificOriginsSection.GetValue<bool>("AllowCredentials");

                    options.AddPolicy("SpecificOriginsPolicy", policy =>
                    {
                        if (allowedOrigins != null && allowedOrigins.Length > 0)
                        {
                            policy.WithOrigins(allowedOrigins);
                        }
                        if (allowedHeaders != null && allowedHeaders.Length > 0)
                        {
                            policy.WithHeaders(allowedHeaders);
                        }
                        if (allowedMethods != null && allowedMethods.Length > 0)
                        {
                            policy.WithMethods(allowedMethods);
                        }
                        if (allowCredentials)
                        {
                            policy.AllowCredentials();
                        }
                    });
                };
            });



            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://sts.windows.net/02ab6bc4-9edc-4e50-b34f-f804fb5ff6fa/";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Set this to map standard User.IsInRole to your custom JWT claim key
                    RoleClaimType = "roles", // or "role" depending on your token payload claim name
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ValidIssuer = "https://sts.windows.net/02ab6bc4-9edc-4e50-b34f-f804fb5ff6fa/",
                    //ValidAudience = "api://1a983a44-a729-4269-a083-254a2e0b6745"
                    
                };
            });



            /*
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            .AddMicrosoftIdentityWebApi();
            */



            /*
            .AddMicrosoftAccount(microsoftOptions =>
             {
                 microsoftOptions.ClientId = "feb6fe33-8229-4760-b244-a428a9f437ab";
                 microsoftOptions.ClientSecret = "5aB8Q~guVMdZOkCRhHRjJdlclD39gKnAsS7d9bjd";

             });
            */




            //string tenantID = "02ab6bc4-9edc-4e50-b34f-f804fb5ff6fa";


            /*
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                {

                    options.Authority = $"https://login.microsoftonline.com/{tenantID}/";
                    //options.Audience = "6e34bd85-1b40-486f-a68c-014bf4de156a";
                    options.Audience = "api://6e34bd85-1b40-486f-a68c-014bf4de156a";

                    // Token validation parameters here...
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Write a breakpoint here or log the error
                            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        }
                    };
                }
            });
            */

            /*            
            builder.Services.AddAuthorizationCore(options =>
            {
                // Policy for a single scope requirement
                options.AddPolicy("Web.API.Access.Role.Policy", policy =>
                    policy.RequireAssertion(context => {
                        context.User.HasClaim(c => c.Type == "roles" && c.Value.Split(' ').Contains("Web.API.Access.Role") } )
                         )
                    
                    );
            });
            */

            // The Below is Not needed if you shut off the default Token Parsing and then the Roles Claim will correctly parse and work with 
            // In the Controller Action method with [Authorize(Roles = "Web.API.Access.Role")]

            /*
            builder.Services.AddAuthorizationCore(opt =>
            {
                opt.AddPolicy("Web.API.Access.Role.Policy", policyBuilder => policyBuilder
                        //.RequireAuthenticatedUser()
                        //.RequireRole("admin")
                        .RequireAssertion(ctx =>
                        {
                           // Some custom assertion for your "meh" policy
                           // Put a breakpoint here to check what is passed as context.
                           return ctx.User.Claims.Single(s => s.Type == "roles")?.Value == "Web.API.Access.Role";
                        })
                        .Build());
            });
            */




            var app = builder.Build();

            // Use CORS Policy
            app.UseCors("SpecificOriginsPolicy");

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();


            //}


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();  // Maps attribute-routed controllers

            app.Run();
        }
    }
}
