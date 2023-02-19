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
[Route("api/hangfire")]
[ApiVersion("1.0")]
public sealed class HangfireController: ControllerBase
{
    private readonly IMediator _mediator;

    public HangfireController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public class PrintAJob
    {
        [Queue("loop-queue")]
        [AutomaticRetry(Attempts = 3)]
        public void Handle(int jobNumber)
        {
            // Generate a random number between 1 and 10
            var randomNumber = new Random().Next(1, 11);

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
        var handler = new PrintAJob();
        for (var jobNumber = 1; jobNumber <= 300; jobNumber++)
        {
            BackgroundJob.Enqueue(() => handler.Handle(jobNumber));
        }

        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpPost("enqueue-delayed")]
    public IActionResult EnqueueDelayed()
    {
        var handler = new PrintAJob();
        for (var jobNumber = 1; jobNumber <= 300; jobNumber++)
        {
            var jobId = BackgroundJob.Schedule(() => handler.Handle(jobNumber), TimeSpan.FromSeconds(5));
        }

        return Ok();
    }
    
    [HttpPost("enqueue-recurring")]
    public IActionResult EnqueueRecurring()
    {
        var handler = new PrintAJob();
        RecurringJob.AddOrUpdate("PrintAJob", () => handler.Handle(1), Cron.Minutely);

        return Ok();
    }
}
