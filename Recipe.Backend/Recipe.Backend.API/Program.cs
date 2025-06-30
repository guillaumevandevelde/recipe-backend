using Microsoft.EntityFrameworkCore;
using Recipe.Backend.Data;
using Recipe.Backend.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddDbContext<RecipeContext>(options =>
    options.UseInMemoryDatabase("RecipeDb"));
builder.Services.AddScalarApiExplorer();

var app = builder.Build();

app.UseServiceDefaults();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/recipes", async (RecipeContext db) =>
    await db.Recipes.ToListAsync());

app.MapGet("/recipes/{id}", async (RecipeContext db, int id) =>
    await db.Recipes.FindAsync(id)
        is Recipe.Backend.Data.Recipe recipe
            ? Results.Ok(recipe)
            : Results.NotFound());

app.MapPost("/recipes", async (RecipeContext db, Recipe.Backend.Data.Recipe recipe) =>
{
    db.Recipes.Add(recipe);
    await db.SaveChangesAsync();

    return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapPut("/recipes/{id}", async (RecipeContext db, int id, Recipe.Backend.Data.Recipe inputRecipe) =>
{
    var recipe = await db.Recipes.FindAsync(id);

    if (recipe is null) return Results.NotFound();

    recipe.Name = inputRecipe.Name;
    recipe.Description = inputRecipe.Description;
    recipe.Ingredients = inputRecipe.Ingredients;
    recipe.Instructions = inputRecipe.Instructions;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/recipes/{id}", async (RecipeContext db, int id) =>
{
    if (await db.Recipes.FindAsync(id) is Recipe.Backend.Data.Recipe recipe)
    {
        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();
        return Results.Ok(recipe);
    }

    return Results.NotFound();
});

app.Run();
