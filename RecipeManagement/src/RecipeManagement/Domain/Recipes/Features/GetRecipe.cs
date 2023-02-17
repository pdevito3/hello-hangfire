namespace RecipeManagement.Domain.Recipes.Features;

using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Domain.Recipes.Services;
using SharedKernel.Exceptions;
using MapsterMapper;
using MediatR;

public static class GetRecipe
{
    public sealed class Query : IRequest<RecipeDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, RecipeDto>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public Handler(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _mapper = mapper;
            _recipeRepository = recipeRepository;
        }

        public async Task<RecipeDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _recipeRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return _mapper.Map<RecipeDto>(result);
        }
    }
}