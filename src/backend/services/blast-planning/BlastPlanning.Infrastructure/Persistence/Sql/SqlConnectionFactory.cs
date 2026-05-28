using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace BlastPlanning.Infrastructure.Persistence.Sql;

public sealed class SqlConnectionFactory(IOptions<SqlOptions> options)
{
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(options.Value.ConnectionString);
    }
}