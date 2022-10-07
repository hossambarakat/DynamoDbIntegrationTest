using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DynamoDbStore.Tests;

public class IntegrationTestBase: IAsyncLifetime
{
    protected const string TABLE_NAME = "Persons";
    protected AmazonDynamoDBClient client;
    private TestcontainersContainer _testContainer;

    public virtual async Task InitializeAsync()
    {
        _testContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("amazon/dynamodb-local:latest")
            .WithPortBinding(8000, assignRandomHostPort:true)
            .WithDockerEndpoint(Environment.GetEnvironmentVariable("DOCKER_HOST") ?? "unix:///var/run/docker.sock")
            .Build();
        await _testContainer.StartAsync();
        var credentials = new BasicAWSCredentials("FAKE", "FAKE");
        var amazonDynamoDbConfig = new AmazonDynamoDBConfig()
        {
            ServiceURL = $"http://localhost:{_testContainer.GetMappedPublicPort(8000)}",
            AuthenticationRegion = "us-east-1"
        };
        client = new AmazonDynamoDBClient(credentials, amazonDynamoDbConfig);

        var createTableRequest = new CreateTableRequest
        {
            TableName = TABLE_NAME,
            KeySchema = new List<KeySchemaElement>()
            {
                new("id", KeyType.HASH)
            },
            AttributeDefinitions = new List<AttributeDefinition>()
            {
                new()
                {
                    AttributeName = "id",
                    AttributeType = ScalarAttributeType.S
                }
            },
            BillingMode = BillingMode.PAY_PER_REQUEST
        };
        await client.CreateTableAsync(createTableRequest);
        var response = await client.DescribeTableAsync(TABLE_NAME);
        if (response == null)
        {
            throw new NullReferenceException("Table was not created within the expected time");
        }
    }

    public virtual Task DisposeAsync()
    {
        return _testContainer.DisposeAsync().AsTask();
    }
}