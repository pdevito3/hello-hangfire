namespace RecipeManagement.IntegrationTests.FeatureTests.Recipes;

using RecipeManagement.SharedTestHelpers.Fakes.Recipe;
using RecipeManagement.Domain.Recipes.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class RecipeQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_recipe_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeRecipeOne = FakeRecipe.Generate(new FakeRecipeForCreationDto().Generate());
        await testingServiceScope.InsertAsync(fakeRecipeOne);

        // Act
        var query = new GetRecipe.Query(fakeRecipeOne.Id);
        var recipe = await testingServiceScope.SendAsync(query);

        // Assert
        recipe.Title.Should().Be(fakeRecipeOne.Title);
        recipe.Directions.Should().Be(fakeRecipeOne.Directions);
        recipe.RecipeSourceLink.Should().Be(fakeRecipeOne.RecipeSourceLink);
        recipe.Description.Should().Be(fakeRecipeOne.Description);
        recipe.ImageLink.Should().Be(fakeRecipeOne.ImageLink);
        recipe.Visibility.Should().Be(fakeRecipeOne.Visibility);
        recipe.DateOfOrigin.Should().Be(fakeRecipeOne.DateOfOrigin);
    }

    [Fact]
    public async Task get_recipe_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetRecipe.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}