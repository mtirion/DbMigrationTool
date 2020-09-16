namespace DbMigrationTool.Services
{
    using System;
    using DbMigrationTool.Configuration;
    using DbMigrationTool.Helpers;
    using DbMigrationTool.Interfaces;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Database migration service implementation.
    /// </summary>
    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private readonly Connection connectionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseMigrationService"/> class.
        /// </summary>
        /// <param name="connectionOptions">Configuration of the database.</param>
        public DatabaseMigrationService(IOptions<Connection> connectionOptions)
        {
            Precondition.NotNull(connectionOptions, "Connection options must be provided.");
            Precondition.NotNull(connectionOptions.Value, "Connection options must be provided.");

            this.connectionOptions = connectionOptions.Value;
        }

        /// <inheritdoc />
        public bool MigrateDatabase()
        {
            // TODO: call into the assembly for the actual migration

            // output the connection settings for demo purposes.
            Console.WriteLine($"Server = {this.connectionOptions.SqlServer}");
            Console.WriteLine($"Database = {this.connectionOptions.Database}");
            Console.WriteLine($"Username = {this.connectionOptions.SQLServerUsername}");
            Console.WriteLine($"Password = {this.connectionOptions.SQLServerPassword}");

            // Fake that we have done the migration
            return true;
        }
    }
}
