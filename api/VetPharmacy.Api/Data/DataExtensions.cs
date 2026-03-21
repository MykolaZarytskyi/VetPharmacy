using Microsoft.EntityFrameworkCore;

namespace VetPharmacy.Api.Data;

public static class DataExtensions
{
    public static WebApplicationBuilder AddVetPharmacyNpgsql<TContext>(
    this WebApplicationBuilder builder,
    string connectionStringName//,
    //TokenCredential credential
) where TContext : DbContext
    {
        var connString = builder.Configuration.GetConnectionString(connectionStringName);

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddNpgsql<TContext>(connString);
        }
        else
        {
            // builder.Services.AddNpgsql<TContext>(connString, dbContextOptionsBuilder =>
            // {
            //     dbContextOptionsBuilder.ConfigureDataSource(dataSourceBuilder =>
            //     {
            //         dataSourceBuilder.UsePeriodicPasswordProvider(
            //             async (_, cancellationToken) =>
            //             {
            //                 var token = await credential.GetTokenAsync(
            //                     new TokenRequestContext([postgreSqlScope]),
            //                     cancellationToken
            //                 );

            //                 return token.Token;
            //             },
            //             TimeSpan.FromHours(24),
            //             TimeSpan.FromSeconds(10)
            //         );
            //     });
            // });
        }

        return builder;
    }
}