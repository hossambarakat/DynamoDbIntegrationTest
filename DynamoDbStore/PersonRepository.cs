using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDbStore;

[DynamoDBTable("Persons")]
public class Person
{
    [DynamoDBHashKey("id")] //Partition key
    public string Id { get; set; }
    [DynamoDBProperty]
    public string FirstName { get; set; }
    [DynamoDBProperty]
    public string LastName { get; set; }
}

public class PersonRepository
{
    private readonly AmazonDynamoDBClient _dynamoDbClient;

    public PersonRepository(AmazonDynamoDBClient dynamoDbClient)
    {
        _dynamoDbClient = dynamoDbClient;
    }
    public Task Insert(Person person)
    {
        DynamoDBContext context = new DynamoDBContext(_dynamoDbClient);
        return context.SaveAsync(person);
    }

    public Task<Person> Get(string personId)
    {
        DynamoDBContext context = new DynamoDBContext(_dynamoDbClient);
        return context.LoadAsync<Person>(personId);
    }
}