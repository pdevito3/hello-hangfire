namespace RecipeManagement.Domain.Recipes.Features;

using RecipeManagement.Domain.Recipes.Services;
using RecipeManagement.Domain.Recipes;
using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Services;
using SharedKernel.Exceptions;
using MapsterMapper;
using MediatR;

public static class AddRecipe
{
    public sealed class Command : IRequest<RecipeDto>
    {
        public readonly RecipeForCreationDto RecipeToAdd;

        public Command(RecipeForCreationDto recipeToAdd)
        {
            RecipeToAdd = recipeToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, RecipeDto>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public Handler(IRecipeRepository recipeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = Recipe.Create(request.RecipeToAdd);
            await _recipeRepository.Add(recipe, cancellationToken);

            await _unitOfWork.CommitChanges(cancellationToken);

            var recipeAdded = await _recipeRepository.GetById(recipe.Id, cancellationToken: cancellationToken);
            return _mapper.Map<RecipeDto>(recipeAdded);
        }
    }
}