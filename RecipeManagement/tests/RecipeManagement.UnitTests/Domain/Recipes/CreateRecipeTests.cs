namespace RecipeManagement.UnitTests.Domain.Recipes;

using RecipeManagement.SharedTestHelpers.Fakes.Recipe;
using RecipeManagement.Domain.Recipes;
using RecipeManagement.Domain.Recipes.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateRecipeTests
{
    private readonly Faker _faker;

    public CreateRecipeTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_recipe()
    {
        // Arrange
        var recipeToCreate = new FakeRecipeForCreationDto().Generate();
        
        // Act
        var fakeRecipe = Recipe.Create(recipeToCreate);

        // Assert
        fakeRecipe.Title.Should().Be(recipeToCreate.Title);
        fakeRecipe.Directions.Should().Be(recipeToCreate.Directions);
        fakeRecipe.RecipeSourceLink.Should().Be(recipeToCreate.RecipeSourceLink);
        fakeRecipe.Description.Should().Be(recipeToCreate.Description);
        fakeRecipe.ImageLink.Should().Be(recipeToCreate.ImageLink);
        fakeRecipe.Visibility.Should().Be(recipeToCreate.Visibility);
        fakeRecipe.DateOfOrigin.Should().Be(recipeToCreate.DateOfOrigin);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange + Act
        var fakeRecipe = FakeRecipe.Generate();

        // Assert
        fakeRecipe.DomainEvents.Count.Should().Be(1);
        fakeRecipe.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(RecipeCreated));
    }
}