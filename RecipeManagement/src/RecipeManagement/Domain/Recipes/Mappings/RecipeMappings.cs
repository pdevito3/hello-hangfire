namespace RecipeManagement.Domain.Recipes.Mappings;

using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Domain.Recipes;
using Mapster;

public sealed class RecipeMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Recipe, RecipeDto>();
        config.NewConfig<RecipeForCreationDto, Recipe>()
            .TwoWays();
        config.NewConfig<RecipeForUpdateDto, Recipe>()
            .TwoWays();
    }
}