using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace BlastPlanning.Infrastructure.Persistence.Sql;

public sealed class SqlConnectionFactory(IOptions<SqlOptions> options)
{
    public SqlConnection CreateConnection()
    {
        if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
        {
            throw new InvalidOperationException(
                "SQL connection string is not configured.");
        }

        return new SqlConnection(options.Value.ConnectionString);
    }
}