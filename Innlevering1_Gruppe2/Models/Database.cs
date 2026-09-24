using MySqlConnector;

namespace Innlevering1_Gruppe2.Models
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDB")
            ?? throw new InvalidOperationException(
                "Fant ikke tilkoblinger til MariaDB.");
        }


        public async Task TestConnectionAsync()
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
        }
    }
}
