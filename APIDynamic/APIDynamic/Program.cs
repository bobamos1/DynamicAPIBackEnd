using APIDynamic;
using DynamicSQLFetcher;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
SQLExecutor.Initialize(builder.Configuration);
string connectionString = SQLExecutor.GetConnectionString("structure");
Console.WriteLine(connectionString);
SQLExecutor executorStructure = new SQLExecutor(connectionString);

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

Dictionary<string, DynamicController> controllers = await DynamicController.initControllers(executorStructure, app);
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
    Query query = Query.fromQueryString(QueryTypes.CBO, "SELECT name, id FROM Tables");
    return Results.Ok(await executorStructure.SelectDictionary(query));
})
.WithName("newTest");
app.MapGet("/newTestArray", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryTypes.ARRAY, "SELECT name FROM Tables");
    return Results.Ok(await executorStructure.SelectArray(query));
})
.WithName("newTestArray");
app.MapGet("/newTestValue", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryTypes.VALUE, "SELECT name FROM Tables ORDER BY name");
    return Results.Ok(await executorStructure.SelectValue(query));
})
.WithName("newTestValue");
app.MapGet("/newTestValueSpecific", async () =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryTypes.SELECT, "SELECT name AS [key], id AS [value] FROM Tables ORDER BY name");
    return Results.Ok(await executorStructure.SelectDictionary(query, "key", "value"));
})
.WithName("newTestValueSpecific");
app.MapGet("/getTest", async () =>
{
    Query insideQuery = Query.fromQueryString(QueryTypes.SELECT, "SELECT name, id FROM Colonnes WHERE id_table = @tableID");
    List<DynamicMapper> mappers = new List<DynamicMapper>();
    mappers.Add(new DynamicMapper("colonnes", insideQuery, "name", "id"));
    mappers[0].addLinkParams("tableID", "id");
    Query query = Query.fromQueryString(QueryTypes.SELECT, "SELECT name, id FROM Tables");
    return Results.Ok(await executorStructure.DetailedSelectQuery(query, mappers, "name", "id"));
})
.WithName("getTest");
app.MapGet("/test", 
    async () => Results.Ok(await executorStructure.SelectDictionary(DynamicController.getRoutes.setParam("controllerID", 1))))
.WithName("test");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}