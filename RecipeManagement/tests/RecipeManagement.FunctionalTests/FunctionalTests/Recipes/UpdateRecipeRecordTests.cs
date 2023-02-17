namespace RecipeManagement.FunctionalTests.FunctionalTests.Recipes;

using RecipeManagement.SharedTestHelpers.Fakes.Recipe;
using RecipeManagement.FunctionalTests.TestUtilities;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdateRecipeRecordTests : TestBase
{
    [Fact]
    public async Task put_recipe_returns_nocontent_when_entity_exists()
    {
        // Arrange
        var fakeRecipe = FakeRecipe.Generate(new FakeRecipeForCreationDto().Generate());
        var updatedRecipeDto = new FakeRecipeForUpdateDto().Generate();
        await InsertAsync(fakeRecipe);

        // Act
        var route = ApiRoutes.Recipes.Put(fakeRecipe.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedRecipeDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}