using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();


// 
///<summary>
/// Use the MapGroup API 
/// Para Agrupa el Patch de todoItems y la URI final seria https://localhost:7209/{id}
/// </summary>
var todoItems = app.MapGroup("/todoitems");

//app.MapGet("/", () => "Hello World!");



todoItems.MapGet("/", getAllTodos);
todoItems.MapGet("/complete", getCompleteTodos);
todoItems.MapGet("/{id}", getTodo);
todoItems.MapPost("/",createTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", deleteTodo);



app.Run();

static async Task<IResult> getAllTodos(TodoDb db)
{

    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());

    //return TypedResults.Ok(await db.Todos.ToArrayAsync());

}

static async Task<IResult> getCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos
                        .Where(t => t.IsComplete)
                        .Select(x => new TodoItemDTO(x))
                        .ToArrayAsync()); 

    //return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
}

static async Task<IResult> getTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
            is Todo todo
                ? Results.Ok(new TodoItemDTO(todo))
                : Results.NotFound();

    //return await db.Todos.FindAsync(id)
    //       is Todo todo
    //           ? Results.Ok(todo)
    //           : Results.NotFound();
}


//static async Task<IResult> createTodo(Todo todo, TodoDb db)
static async Task<IResult> createTodo(TodoItemDTO todoItemDTO, TodoDb db)
{

    var todoItem = new Todo
    {
        IsComplete= todoItemDTO.IsComplete,
        Name= todoItemDTO.Name
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    todoItemDTO = new TodoItemDTO(todoItem);

    return Results.Created($"/todoitems/{todoItem.Id}", todoItemDTO);

    //db.Todos.Add(todo);
    //await db.SaveChangesAsync();
    //return Results.Created($"/todoitems/{todo.Id}", todo);

}



//static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
//{

//    var todo = await db.Todos.FindAsync(id);
//    if (todo is null) return Results.NotFound();

//    todo.Name = inputTodo.Name;
//    todo.IsComplete = inputTodo.IsComplete;

//    await db.SaveChangesAsync();
//    return Results.Ok(todo);

//}
static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
{

    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(todo);

}

static async Task<IResult> deleteTodo(int id, TodoDb db)
{

    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();

}




//todoItems.MapGet("/", async (TodoDb db) => await db.Todos.ToListAsync());

//todoItems.MapGet("/complete", async (TodoDb db) => await db.Todos.Where( t=> t.IsComplete).ToListAsync());



//todoItems.MapGet("/{id}", async (int id, TodoDb db) =>
//    await db.Todos.FindAsync(id)
//    is Todo todo
//        ? Results.Ok(todo)
//        : Results.NotFound());

//todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
//{
//    db.Todos.Add(todo);
//    await db.SaveChangesAsync();
//    return Results.Created($"/todoitems/{todo.Id}", todo);
//});

//todoItems.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
//{

//    var todo = await db.Todos.FindAsync(id);
//    if (todo is null) return Results.NotFound();

//    todo.Name = inputTodo.Name;
//    todo.IsComplete = inputTodo.IsComplete;

//    await db.SaveChangesAsync();
//    return Results.Ok(todo);
//});

//todoItems.MapDelete("/{id}", async (int id, TodoDb db) =>
//{
//    if (await db.Todos.FindAsync(id) is Todo todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return Results.NoContent();
//    }

//    return Results.NotFound();

//});




