namespace DbMigrationTool.Configuration
{
    /// <summary>
    /// Configuration class for the database.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Gets or sets the sql server name.
        /// </summary>
        public string SqlServer { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the sql server username.
        /// </summary>
        public string SQLServerUsername { get; set; }

        /// <summary>
        /// Gets or sets the sql server password.
        /// </summary>
        public string SQLServerPassword { get; set; }
    }
}
