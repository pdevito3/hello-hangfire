namespace RecipeManagement.Domain.Recipes;

using SharedKernel.Exceptions;
using RecipeManagement.Domain.Recipes.Dtos;
using RecipeManagement.Domain.Recipes.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Sieve.Attributes;


public class Recipe : BaseEntity
{
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Title { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Directions { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string RecipeSourceLink { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Description { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string ImageLink { get; private set; }

    private VisibilityEnum _visibility;
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Visibility
    {
        get => _visibility.Name;
        private set
        {
            if (!VisibilityEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Visibility), value);

            _visibility = parsed;
        }
    }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual DateOnly? DateOfOrigin { get; private set; }


    public static Recipe Create(RecipeForCreationDto recipeForCreationDto)
    {
        var newRecipe = new Recipe();

        newRecipe.Title = recipeForCreationDto.Title;
        newRecipe.Directions = recipeForCreationDto.Directions;
        newRecipe.RecipeSourceLink = recipeForCreationDto.RecipeSourceLink;
        newRecipe.Description = recipeForCreationDto.Description;
        newRecipe.ImageLink = recipeForCreationDto.ImageLink;
        newRecipe.Visibility = recipeForCreationDto.Visibility;
        newRecipe.DateOfOrigin = recipeForCreationDto.DateOfOrigin;

        newRecipe.QueueDomainEvent(new RecipeCreated(){ Recipe = newRecipe });
        
        return newRecipe;
    }

    public Recipe Update(RecipeForUpdateDto recipeForUpdateDto)
    {
        Title = recipeForUpdateDto.Title;
        Directions = recipeForUpdateDto.Directions;
        RecipeSourceLink = recipeForUpdateDto.RecipeSourceLink;
        Description = recipeForUpdateDto.Description;
        ImageLink = recipeForUpdateDto.ImageLink;
        Visibility = recipeForUpdateDto.Visibility;
        DateOfOrigin = recipeForUpdateDto.DateOfOrigin;

        QueueDomainEvent(new RecipeUpdated(){ Id = Id });
        return this;
    }
    
    protected Recipe() { } // For EF + Mocking
}