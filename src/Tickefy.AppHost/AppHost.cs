var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("database")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var db = postgres.AddDatabase("Postgres");

builder.AddProject<Projects.Tickefy_API>("tickefy-api")
    .WithReference(db)
    .WaitFor(db);

await builder.Build().RunAsync();
