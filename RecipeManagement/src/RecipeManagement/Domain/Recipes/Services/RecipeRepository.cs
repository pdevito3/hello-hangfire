namespace RecipeManagement.Domain.Recipes.Services;

using RecipeManagement.Domain.Recipes;
using RecipeManagement.Databases;
using RecipeManagement.Services;

public interface IRecipeRepository : IGenericRepository<Recipe>
{
    Recipe GetRandomRecipe();
}

public sealed class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
{
    private readonly RecipesDbContext _dbContext;

    public RecipeRepository(RecipesDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Recipe GetRandomRecipe()
    {
        var totalRecords = _dbContext.Recipes.Count();
        var randomRecord = new Random().Next(0, totalRecords);
        return _dbContext.Recipes.Skip(randomRecord).Take(1).FirstOrDefault();
    }
}
