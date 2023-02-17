namespace RecipeManagement.Controllers.v1;

using RecipeManagement.Domain.Recipes.Features;
using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Wrappers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using Hangfire;
using MediatR;

[ApiController]
[Route("api/recipes")]
[ApiVersion("1.0")]
public sealed class RecipesController: ControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    
    public class DoThisThingHandler
    {
        [Queue("loop-queue")]
        [AutomaticRetry(Attempts = 3)]
        public void Handle(int jobNumber)
        {
            // Generate a random number between 1 and 10
            int randomNumber = new Random().Next(1, 11);

            // If the random number is greater than 5, throw an exception
            if (randomNumber > 5)
            {
                Console.WriteLine("Failed job number: {0}", jobNumber);
                throw new Exception($"Error processing job {jobNumber}");
            }
            
            Console.WriteLine("Completed job number: {0}", jobNumber);
        }
    }
    
    [HttpPost("enqueue")]
    public Task<IActionResult> Enqueue()
    {
        var handler = new DoThisThingHandler();
        for (int i = 1; i <= 300; i++)
        {
            int jobNumber = i;

            // Enqueue the job
            BackgroundJob.Enqueue(() => handler.Handle(jobNumber));
        }

        return Task.FromResult<IActionResult>(Ok());
    }
    

    /// <summary>
    /// Gets a list of all Recipes.
    /// </summary>
    [HttpGet(Name = "GetRecipes")]
    public async Task<IActionResult> GetRecipes([FromQuery] RecipeParametersDto recipeParametersDto)
    {
        var query = new GetRecipeList.Query(recipeParametersDto);
        var queryResponse = await _mediator.Send(query);

        var paginationMetadata = new
        {
            totalCount = queryResponse.TotalCount,
            pageSize = queryResponse.PageSize,
            currentPageSize = queryResponse.CurrentPageSize,
            currentStartIndex = queryResponse.CurrentStartIndex,
            currentEndIndex = queryResponse.CurrentEndIndex,
            pageNumber = queryResponse.PageNumber,
            totalPages = queryResponse.TotalPages,
            hasPrevious = queryResponse.HasPrevious,
            hasNext = queryResponse.HasNext
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single Recipe by ID.
    /// </summary>
    [HttpGet("{id:guid}", Name = "GetRecipe")]
    public async Task<ActionResult<RecipeDto>> GetRecipe(Guid id)
    {
        var query = new GetRecipe.Query(id);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Recipe record.
    /// </summary>
    [HttpPost(Name = "AddRecipe")]
    public async Task<ActionResult<RecipeDto>> AddRecipe([FromBody]RecipeForCreationDto recipeForCreation)
    {
        var command = new AddRecipe.Command(recipeForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetRecipe",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing Recipe.
    /// </summary>
    [HttpPut("{id:guid}", Name = "UpdateRecipe")]
    public async Task<IActionResult> UpdateRecipe(Guid id, RecipeForUpdateDto recipe)
    {
        var command = new UpdateRecipe.Command(id, recipe);
        await _mediator.Send(command);

        return NoContent();
    }


    /// <summary>
    /// Deletes an existing Recipe record.
    /// </summary>
    [HttpDelete("{id:guid}", Name = "DeleteRecipe")]
    public async Task<ActionResult> DeleteRecipe(Guid id)
    {
        var command = new DeleteRecipe.Command(id);
        await _mediator.Send(command);

        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
