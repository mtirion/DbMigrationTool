namespace DbMigrationTool
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The main program class.
    /// </summary>
    internal class Program
    {
        private static IServiceProvider serviceProvider;

        /// <summary>
        /// Create configuration build object to parse various configuration methods.
        /// Currently supported:
        /// - appsettings.json
        /// - appsettings.[environment].json
        /// - environment variables
        /// - command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Configuration builder object.</returns>
        public static IConfigurationBuilder CreateConfigBuilder(string[] args)
        {
            // Get the environment setting
            string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
            {
                // no environment set, so fall back to default Development
                env = "Development";
            }

            // TODO: remove console output if not needed
            Console.WriteLine($"Environment = {env}\n");

            return new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: false)
                 .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                 .AddEnvironmentVariables()
                 .AddCommandLine(args);
        }

        /// <summary>
        /// Main entry point for the console application.
        /// </summary>
        /// <param name="args">Commandline arguments.</param>
        private static void Main(string[] args)
        {
            // setup configuration handler(s)
            var configuration = CreateConfigBuilder(args).Build();

            // create the startup class for configuration
            Startup startup = new Startup(configuration);

            // create a service collection and let Startup configure the services
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider(true);

            // run the application logic
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                startup.Run(scope);
            }

            // cleanup
            ((IDisposable)serviceProvider).Dispose();
        }
    }
}
