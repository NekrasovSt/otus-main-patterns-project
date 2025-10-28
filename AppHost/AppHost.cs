var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder.AddMongoDB("mongo")
    .WithLifetime(ContainerLifetime.Session)
    .WithMongoExpress();


var apiService = 
    builder.AddProject<Projects.RuleEditor>("rule-editor")
        .WithReference(mongodb)
        .WaitFor(mongodb);

builder.Build().Run();