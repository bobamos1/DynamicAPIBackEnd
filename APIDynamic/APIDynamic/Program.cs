using DynamicSQLFetcher;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
SQLExecutor.Initialize(builder.Configuration);
string connectionString = SQLExecutor.GetConnectionString("structure");
Console.WriteLine(connectionString);
SQLExecutor executor = new SQLExecutor(connectionString);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/newTest", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryType.CBO, "SELECT name, id FROM Tables");
    return Results.Ok(await executor.SelectDictionary(query));
})
.WithName("newTest");
app.MapGet("/newTestArray", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryType.ARRAY, "SELECT name FROM Tables");
    return Results.Ok(await executor.SelectArray(query));
})
.WithName("newTestArray");
app.MapGet("/newTestValue", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryType.VALUE, "SELECT name FROM Tables ORDER BY name");
    return Results.Ok(await executor.SelectValue(query));
})
.WithName("newTestValue");
app.MapGet("/newTestValueSpecific", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryType.SELECT, "SELECT name AS [key], id AS [value] FROM Tables ORDER BY name");
    return Results.Ok(await executor.SelectDictionary(query, "key", "value"));
})
.WithName("newTestValueSpecific");
app.MapGet("/getTest", async () =>
{
    Query insideQuery = Query.fromQueryString(QueryType.SELECT, "SELECT name, id FROM Colonnes WHERE id_table = @tableID");
    List<DynamicMapper> mappers = new List<DynamicMapper>();
    mappers.Add(new DynamicMapper("colonnes", insideQuery, "name", "id"));
    mappers[0].addLinkParams("tableID", "id");
    Query query = Query.fromQueryString(QueryType.SELECT, "SELECT name, id FROM Tables");
    return Results.Ok(await executor.DetailedSelectQuery(query, mappers, "name", "id"));
})
.WithName("getTest");
app.Map("/test", async (HttpContext context, CancellationToken ct)
    => await Results.Json(new { test = context.Request.Method }).ExecuteAsync(context))
    .WithName("test");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}