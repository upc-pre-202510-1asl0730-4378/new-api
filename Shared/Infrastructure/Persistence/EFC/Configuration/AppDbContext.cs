using eb4341u202318323.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using eb4395u202318323.API.Assets.Infrastructure.Persistence.EFC.Configuration.Extensions;
using eb4395u202318323.API.Operations.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;

namespace eb4395u202318323.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Add the created and updated interceptor
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyOperationsConfiguration();
        builder.ApplyAssetsConfiguration();
        // Apply Naming convention Policy
        builder.UseSnakeCaseNamingConvention();
        
        
    }
}