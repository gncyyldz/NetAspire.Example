var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "gncy", secret: true);
var password = builder.AddParameter("password", "123", secret: true);
var rabbitMQ = builder.AddRabbitMQ("rabbitMQ", username, password)
    .WithManagementPlugin();

//var keycloak = builder.AddKeycloak("keycloak", 1111, username, password);
var keycloak = builder.AddKeycloak("keycloak", 80, username, password);

var redis = builder.AddRedis("redis");

var sqlServer = builder.AddSqlServer("sqlServer")
    .WithContainerName("sqlServer")
    .WithLifetime(ContainerLifetime.Persistent);
var database = sqlServer.AddDatabase("exampleDB");

var _servicea = builder.AddProject<Projects.ServiceA>("servicea");

var _serviceb = builder.AddProject<Projects.ServiceB>("serviceb");

_servicea
    .WithReference(rabbitMQ)
    .WithReference(keycloak)
    .WithReference(redis)
    .WithReference(_serviceb)
    .WithReference(database);

_serviceb
    .WithReference(rabbitMQ);

builder.Build().Run();