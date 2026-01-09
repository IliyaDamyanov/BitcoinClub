namespace BitcoinClub.Infrastructure.Database
{
    public static class ConnectionStringValidator
    {
        public static void ValidatePostgresConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is required.", nameof(connectionString));
            }

            if (!connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("PostgreSQL connection string must include 'Host'.", nameof(connectionString));
            }

            if (!connectionString.Contains("Database=", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("PostgreSQL connection string must include 'Database'.", nameof(connectionString));
            }

            if (!connectionString.Contains("Username=", StringComparison.OrdinalIgnoreCase) &&
                !connectionString.Contains("User ID=", StringComparison.OrdinalIgnoreCase) &&
                !connectionString.Contains("UserId=", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("PostgreSQL connection string must include 'Username' (or 'User ID').", nameof(connectionString));
            }
        }
    }
}
