using DynamicSQLFetcher;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
SQLExecutor.Initialize(builder.Configuration);

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

app.MapGet("newTest", () =>
{
    return Results.Ok("newTest");
})
.WithName("newTest");
app.MapGet("/getTest", async () =>
{
    string connectionString = builder.Configuration.GetConnectionString("structure");
    Console.WriteLine(connectionString);
    SQLExecutor executor = new SQLExecutor(connectionString);
    Query insideQuery = Query.fromQueryString(QueryType.SELECT, "SELECT name, id FROM Colonnes WHERE id_table = @tableID");
    List<DynamicMapper> mappers = new List<DynamicMapper>();
    mappers.Add(new DynamicMapper("colonnes", insideQuery, false, "name", "id"));
    mappers[0].addLinkParams("tableID", "id");
    Query query = Query.fromQueryString(QueryType.SELECT, "SELECT name, id FROM Tables");
    return Results.Ok(await executor.DetailedSelectQuery(query, mappers, new List<string>() { "name", "id" }, true));
})
.WithName("getTest");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}