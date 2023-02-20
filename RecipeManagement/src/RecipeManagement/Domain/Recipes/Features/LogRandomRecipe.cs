namespace RecipeManagement.Domain.Recipes.Features;

using System.Text.Json;
using Hangfire;
using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Domain.Recipes.Services;
using MapsterMapper;
using RecipeManagement.Services;

public class LogRandomRecipe
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;
    
    public LogRandomRecipe(IRecipeRepository recipeRepository, IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    [Queue("recipe-logger")]
    public async Task Handle()
    {
        var recipe = _recipeRepository.GetRandomRecipe();
        var recipeToPrint = _mapper.Map<RecipeDto>(recipe);
        Console.WriteLine(JsonSerializer.Serialize(recipeToPrint, new JsonSerializerOptions { WriteIndented = true }));
    }
}
