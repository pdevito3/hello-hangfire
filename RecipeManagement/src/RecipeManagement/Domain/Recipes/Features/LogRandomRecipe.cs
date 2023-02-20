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
    private readonly IUnitOfWork _unitOfWork;
    
    public LogRandomRecipe(IRecipeRepository recipeRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    [Queue("recipe-logger")]
    public async Task Handle()
    {
        var recipe = _recipeRepository.GetRandomRecipe();
        var recipeToPrint = _mapper.Map<RecipeDto>(recipe);
        Console.WriteLine(JsonSerializer.Serialize(recipeToPrint, new JsonSerializerOptions { WriteIndented = true }));
    }
}

public class LogSpecificRecipe
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    
    public LogSpecificRecipe(IRecipeRepository recipeRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    [Queue("recipe-logger")]
    public async Task Handle(Guid recipeId, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetById(recipeId, cancellationToken: cancellationToken);
        recipe.SetDescription("this was lit of fire 🔥");
        await _unitOfWork.CommitChanges(cancellationToken);
        var updatedRecipe = await _recipeRepository.GetById(recipe.Id, cancellationToken: cancellationToken);
        var recipeToPrint = _mapper.Map<RecipeDto>(updatedRecipe);
        Console.WriteLine(JsonSerializer.Serialize(recipeToPrint, new JsonSerializerOptions { WriteIndented = true }));
    }
}