namespace DbMigrationTool
{
    using DbMigrationTool.Configuration;
    using DbMigrationTool.Helpers;
    using DbMigrationTool.Interfaces;
    using DbMigrationTool.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure services.
        /// </summary>
        /// <param name="services">Services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // add Options pattern
            services.AddOptions();

            // add settings
            services.Configure<Connection>(this.Configuration.GetSection(nameof(Connection)));

            // add application services
            services.AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();
        }

        /// <summary>
        /// Main entry point of Startup to begin logic of this application.
        /// </summary>
        /// <param name="scope">Service scope.</param>
        public void Run(IServiceScope scope)
        {
            Precondition.NotNull(scope);

            // execute the migration of the database when required
            scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>().MigrateDatabase();
        }
    }
}
