namespace DbMigrationTool.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Public interface of the database migration service.
    /// </summary>
    public interface IDatabaseMigrationService
    {
        /// <summary>
        /// Migrate the database.
        /// </summary>
        /// <returns>Migration executed TRUE/FALSE.</returns>
        bool MigrateDatabase();
    }
}
