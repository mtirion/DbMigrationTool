# Sample Console App for database migration using ASP.NET infrastructure

This application is a sample to demonstrate how to setup a .NET Core Console Application with the use of [.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1), the [Options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.1) and [dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1). It's using .NET Core 3.1 LTS.

If a new .NET Core Console App is created with Visual Studio 2019, it has no default infrastructure for configuration or dependency injection. As the standard documentation is mostly aimed at ASP.NET Core, there is no formal documentation how to achieve this in a console application. There are however a few blogposts explaining how to do this. For this demonstrator these blogposts were used:

* [How to use Configuration API in .net core console application](https://garywoodfine.com/configuration-api-net-core-console-application/) by Gary Woodfine
* [.net core console application IOptions<T> configuration](https://medium.com/@kritner/net-core-console-application-ioptions-t-configuration-ae74bfafe1c5) by Russel Hammett Jr
* [Dependency injection in .NE Core Console Application](https://pradeeploganathan.com/dotnet/dependency-injection-in-net-core-console-application/) by Pradeep Logathan

## Running the sample

To run this sample, open the solution and build it. Go to the `bin/Debug/netcoreapp3.1` folder and run DbMigrationTool.exe from the commandline. If you leave it like it is, the output is this:

```text
Environment = Development

Server = sqlserver001
Database = mainDatabase
Username =
Password =
```

These settings are coming from the environment and `appsettings.json`. The username and password is not something we want in a readable text file. So we might want to add this though an environment variable. If you add CONNECTION__SQLSERVERUSERNAME=admin to your environemt (NOTE the double underscode between CONNECTION and SQLSERVERUSERNAME!) and run it again, you should see this:

```text
Environment = Development

Server = sqlserver001
Database = mainDatabase
Username = admin
Password =
```

Another way to add parameters is through command line arguments. Try this as a command:

```shell
DbMigrationTool --Connection:SQLServerPassword=secret!
```

The output is now:

```text
Environment = Development

Server = sqlserver001
Database = mainDatabase
Username = admin
Password = secret!
```

Of course you can add other implementations to retrieve the settings. For more information, see [.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)

## Structure of this sample

To maintain code standards, I've added StyleCop and FxCop. There is a `GlobalSuppressions.cs` with some exceptions for this project.

### Program.cs

This is the main entry point of the console application. The `Main` method is the first to be called. In `Main` we setup the configuration and create and use the `Startup` class to mimic the ASP.NET Core behavior.

The configuration is created in the `CreateConfigBuilder` method:

```csharp
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
    Console.WriteLine($"Environment: {env}");

    return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
}
```

The implementation of the `Main` method is like this:

```csharp
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

```

### Startup.cs

This class contains the main implementation to setup the environment and call into the actual logic. The constructor gets the configuration as parameter. The `ConfigureServices` method sets up the services like the `Connection` settings and the `DatabaseMigrationService`. 

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // add Options pattern
    services.AddOptions();

    // add settings
    var x = this.Configuration.GetSection(nameof(Connection));
    services.Configure<Connection>(this.Configuration.GetSection(nameof(Connection)));

    // add application services
    services.AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();
}
```

The `Run` method finally runs the business logic.

```csharp
public void Run(IServiceScope scope)
{
    Precondition.NotNull(scope);

    // execute the migration of the database when required
    scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>().MigrateDatabase();
}
```

### appsettings.json

This is the only settings file in this project currently, as it is just a demo. The contents is:

```json
{
  "Connection": {
    "SqlServer": "sqlserver001",
    "Database": "mainDatabase"
  }
}
```

### Helpers

I've added the `Precondition` helper class to validate preconditions for methods, like null checks. To make StyleCop aware that this check is made, we have also added the `ValidatedNotNullAttribute`. This attribute is added to the appropriate methods in `Precondition`.

### Configuration

This folder contains all configuration classes to be used with the Options pattern. In this sample there is just one: a `Connection` class. The classname is also used as the section name in the settings (see `appsettings.json`).

```csharp
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
```

### Interfaces

This folder contains the interface definitions of the services. In this sample we have only one service, so also one interface: `IDatabaseMigrationService`.

```csharp
public interface IDatabaseMigrationService
{
    /// <summary>
    /// Migrate the database.
    /// </summary>
    /// <returns>Migration executed TRUE/FALSE.</returns>
    bool MigrateDatabase();
}
```

Using this mechanism provides a way to have multiple implementations of this service and add it to the services in `Startup.cs`.

### Services

All service implementations are stored in this folder. In this sample we have only one: `DatabaseMigrationService`. I haven't done an actual implementation of the migration, there are just placeholder methods. The constructor gets the Connection configuration settings through dependecy injection. The `MigrateDatabase` method outputs the configuration settings to the console.

```csharp
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
```
