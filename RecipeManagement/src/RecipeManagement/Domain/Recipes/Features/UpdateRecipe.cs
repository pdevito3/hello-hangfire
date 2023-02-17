namespace RecipeManagement.Domain.Recipes.Features;

using RecipeManagement.Domain.Recipes;
using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Domain.Recipes.Services;
using RecipeManagement.Services;
using SharedKernel.Exceptions;
using MapsterMapper;
using MediatR;

public static class UpdateRecipe
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid Id;
        public readonly RecipeForUpdateDto UpdatedRecipeData;

        public Command(Guid id, RecipeForUpdateDto updatedRecipeData)
        {
            Id = id;
            UpdatedRecipeData = updatedRecipeData;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IRecipeRepository recipeRepository, IUnitOfWork unitOfWork)
        {
            _recipeRepository = recipeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipeToUpdate = await _recipeRepository.GetById(request.Id, cancellationToken: cancellationToken);

            recipeToUpdate.Update(request.UpdatedRecipeData);
            _recipeRepository.Update(recipeToUpdate);
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}