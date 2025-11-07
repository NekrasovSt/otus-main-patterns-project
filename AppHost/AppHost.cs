var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder.AddMongoDB("mongo")
    .WithLifetime(ContainerLifetime.Session)
    .WithMongoExpress();

var rabbitmq = builder
    .AddRabbitMQ("notification", 
        userName: builder.AddParameter("username", "1", secret: true),
        password: builder.AddParameter("password", "1", secret: true))
    .WithManagementPlugin();

var ruleEditor = 
    builder.AddProject<Projects.RuleEditor>("rule-editor")
        .WithReference(mongodb)
        .WaitFor(mongodb)
        .WithReference(rabbitmq)
        .WaitFor(rabbitmq);

var authService = 
    builder.AddProject<Projects.Auth>("auth")
        .WithReference(mongodb)
        .WaitFor(mongodb);

var authClient = builder.AddProject<Projects.Auth_Client>("auth-client")
    .WaitFor(authService)
    .WithReference(authService);

var linkServer = builder.AddProject<Projects.LinkServer>("link-server")
    .WaitFor(ruleEditor)
    .WithReference(ruleEditor)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);


builder.Build().Run();