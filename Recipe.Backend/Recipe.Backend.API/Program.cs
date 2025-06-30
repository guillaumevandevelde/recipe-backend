using Microsoft.EntityFrameworkCore;
using Recipe.Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RecipeContext>(options =>
    options.UseInMemoryDatabase("RecipeDb"));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
