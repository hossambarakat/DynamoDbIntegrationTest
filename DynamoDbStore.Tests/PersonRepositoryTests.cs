using Amazon.DynamoDBv2;
using Shouldly;

namespace DynamoDbStore.Tests;

public class PersonRepositoryTests: IntegrationTestBase
{
    [Fact]
    public async Task End_To_End()
    {
        var personRepository = new PersonRepository(base.client);
        var newPerson = new Person()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Hossam",
            LastName = "Barakat"
        };
        await personRepository.Insert(newPerson);
        
        var person = await personRepository.Get(newPerson.Id);

        person.ShouldNotBeNull();
        person.Id.ShouldBe(newPerson.Id);
    }
}