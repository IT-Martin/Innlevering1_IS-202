using Reachout.DataAccess.Repositories;

namespace Reachout.Tests;

/* Testklasse for testing av ResourceRepository */
public class ResourceRepositoryTests
{
    /* Test for å sikre at GetByAccountIdAsync returnerer kun ressurser som tilhører den spesifiserte kontoen,
       i synkende rekkefølge etter ressurs-ID, og at relaterte data (Location og ResourceType) er inkludert.*/
    [Fact]
    public async Task GetByAccountIdAsync_ReturnsOnlyThatAccountsResourcesInDescendingOrder()
    {
        await using var context = TestDatabase.CreateContext();
        var older = TestDatabase.CreateResource(context, name: "Eldre");
        var otherOwner = TestDatabase.CreateResource(context, accountId: 2, name: "Annen eier");
        var newer = TestDatabase.CreateResource(context, name: "Nyere");
        var repository = new ResourceRepository(context);

        var resources = await repository.GetByAccountIdAsync(1);

        Assert.Equal(new[] { newer.resource_id, older.resource_id }, resources.Select(item => item.resource_id));
        Assert.DoesNotContain(resources, item => item.resource_id == otherOwner.resource_id);
        Assert.All(resources, item =>
        {
            Assert.NotNull(item.Location);
            Assert.NotNull(item.ResourceType);
        });
    }

    /* Test for å sikre at GetByIdForAccountAsync returnerer null når ressursen tilhører en annen konto,
       og at relaterte data ikke blir lastet inn.*/
    [Fact]
    public async Task GetByIdForAccountAsync_DoesNotReturnAnotherAccountsResource()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context, accountId: 2);
        var repository = new ResourceRepository(context);

        var result = await repository.GetByIdForAccountAsync(resource.resource_id, 1);

        Assert.Null(result);
    }

    /* Test for å sikre at GetByIdForAccountAsync returnerer ressursen med relaterte data (Location og ResourceType). */
    [Fact]
    public async Task GetByIdAsync_ReturnsResourceWithRelatedData()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context);
        var repository = new ResourceRepository(context);

        var result = await repository.GetByIdForAccountAsync(resource.resource_id, 1);

        Assert.NotNull(result);
        Assert.NotNull(result.Location);
        Assert.NotNull(result.ResourceType);
    }

    /* Test for å sikre at AddAsync lagrer ressursen og returnerer den med en generert ID. */
    [Fact]
    public async Task AddAsync_PersistsResourceAndReturnsGeneratedId()
    {
        await using var context = TestDatabase.CreateContext();
        var repository = new ResourceRepository(context);
        var resource = new Reachout.Models.Entities.Resource
        {
            account_id = 1,
            resource_name = "Ny ressurs",
            description = "Beskrivelse",
            contact_details = "Kontakt",
            verification_status = "Pending",
            resource_type_id = 1,
            ResourceType = context.ResourceTypes.Single(type => type.resource_type_id == 1),
            Location = new Reachout.Models.Entities.Location
            {
                address_or_area = "Teststed",
                latitude = 59.9m,
                longitude = 10.7m
            }
        };

        var added = await repository.AddAsync(resource);

        Assert.True(added.resource_id > 0);
        Assert.Equal("Ny ressurs", (await context.Resources.FindAsync(added.resource_id))?.resource_name);
    }

    /* Test for å sikre at UpdateAsync oppdaterer ressursen i databasen med de nye verdiene. */
    [Fact]
    public async Task UpdateAsync_PersistsResourceChanges()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context);
        var repository = new ResourceRepository(context);
        resource.resource_name = "Oppdatert ressurs";

        await repository.UpdateAsync(resource);

        Assert.Equal("Oppdatert ressurs", (await context.Resources.FindAsync(resource.resource_id))?.resource_name);
    }

    /* Test for å sikre at DeleteAsync sletter ressursen fra databasen når den tilhører den spesifiserte kontoen,
       og at den ikke sletter ressurser som tilhører andre kontoer.*/
    [Fact]
    public async Task DeleteAsync_DeletesOnlyWhenResourceBelongsToAccount()
    {
        await using var context = TestDatabase.CreateContext();
        var ownedResource = TestDatabase.CreateResource(context);
        var otherResource = TestDatabase.CreateResource(context, accountId: 2);
        var repository = new ResourceRepository(context);

        Assert.False(await repository.DeleteAsync(otherResource.resource_id, 1));
        Assert.True(await repository.DeleteAsync(ownedResource.resource_id, 1));
        Assert.Null(await context.Resources.FindAsync(ownedResource.resource_id));
        Assert.NotNull(await context.Resources.FindAsync(otherResource.resource_id));
    }

    /* Test for å sikre at DeleteAsync returnerer false når ressursen ikke finnes i databasen. */
    [Fact]
    public async Task DeleteAsync_ReturnsFalseForUnknownResource()
    {
        await using var context = TestDatabase.CreateContext();
        var repository = new ResourceRepository(context);

        Assert.False(await repository.DeleteAsync(999, 1));
    }
}
