using System.Text;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.Identity.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Kysect.Shreks.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentityConfiguration(
        this IServiceCollection collection,
        IConfigurationSection identityConfigurationSection,
        Action<DbContextOptionsBuilder> dbContextAction)
    {
        var identityConfiguration = identityConfigurationSection.Get<IdentityConfiguration>();

        collection.AddSingleton(identityConfiguration);
        collection.AddDbContext<ShreksIdentityContext>(dbContextAction);

        collection.AddIdentity<ShreksIdentityUser, ShreksIdentityRole>()
            .AddEntityFrameworkStores<ShreksIdentityContext>()
            .AddDefaultTokenProviders();
        
        collection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = identityConfiguration.Audience,
                ValidIssuer = identityConfiguration.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(identityConfiguration.Secret)),
            };
        });
    }
}