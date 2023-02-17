namespace RecipeManagement.SharedTestHelpers.Fakes.Recipe;

using RecipeManagement.Domain.Recipes;
using RecipeManagement.Domain.Recipes.Dtos;

public class FakeRecipeBuilder
{
    private RecipeForCreationDto _creationData = new FakeRecipeForCreationDto().Generate();

    public FakeRecipeBuilder WithDto(RecipeForCreationDto dto)
    {
        _creationData = dto;
        return this;
    }
    
    public Recipe Build()
    {
        var result = Recipe.Create(_creationData);
        return result;
    }
}