using System.Data;
using System.Data.SqlClient;
using EzDomain.EventSourcing.EventStores.Sql.Factories;

namespace EzDomain.EventSourcing.EventStores.SqlServer.Factories
{
    public sealed class SqlServerConnectionFactory
        : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString) => new SqlConnection(connectionString);
    }
}