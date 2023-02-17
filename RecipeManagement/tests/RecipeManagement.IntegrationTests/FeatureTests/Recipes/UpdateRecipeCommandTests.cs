namespace RecipeManagement.IntegrationTests.FeatureTests.Recipes;

using RecipeManagement.SharedTestHelpers.Fakes.Recipe;
using RecipeManagement.Domain.Recipes.Dtos;
using SharedKernel.Exceptions;
using RecipeManagement.Domain.Recipes.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateRecipeCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_recipe_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeRecipeOne = FakeRecipe.Generate(new FakeRecipeForCreationDto().Generate());
        var updatedRecipeDto = new FakeRecipeForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeRecipeOne);

        var recipe = await testingServiceScope.ExecuteDbContextAsync(db => db.Recipes
            .FirstOrDefaultAsync(r => r.Id == fakeRecipeOne.Id));
        var id = recipe.Id;

        // Act
        var command = new UpdateRecipe.Command(id, updatedRecipeDto);
        await testingServiceScope.SendAsync(command);
        var updatedRecipe = await testingServiceScope.ExecuteDbContextAsync(db => db.Recipes.FirstOrDefaultAsync(r => r.Id == id));

        // Assert
        updatedRecipe.Title.Should().Be(updatedRecipeDto.Title);
        updatedRecipe.Directions.Should().Be(updatedRecipeDto.Directions);
        updatedRecipe.RecipeSourceLink.Should().Be(updatedRecipeDto.RecipeSourceLink);
        updatedRecipe.Description.Should().Be(updatedRecipeDto.Description);
        updatedRecipe.ImageLink.Should().Be(updatedRecipeDto.ImageLink);
        updatedRecipe.Visibility.Should().Be(updatedRecipeDto.Visibility);
        updatedRecipe.DateOfOrigin.Should().Be(updatedRecipeDto.DateOfOrigin);
    }
}