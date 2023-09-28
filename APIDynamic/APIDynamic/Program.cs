using APIDynamic;
using DynamicStructureObjects;
using DynamicSQLFetcher;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Dictionary<string, string> connectionStrings = RoutesInit.LoadConnectionStrings(builder.Configuration);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
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
app.UseAuthentication();
app.UseAuthorization();
Dictionary<string, DynamicController> controllers = await DynamicController.initControllers(new SQLExecutor(connectionStrings["structure"]));
//await BDInit.InitDB(controllers);//keep it commit
RoutesInit.InitRoutes(controllers, app, connectionStrings);


app.Run();














/*
SQLExecutor executorStructure = new SQLExecutor(builder.Configuration.GetConnectionString("structure"));
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

app.MapGet("/newTest", (HttpContext context) =>
{
    //return Results.Ok("ok");
    Query query = Query.fromQueryString(QueryTypes.CBO, "SELECT name, id FROM Tables");
    //return Results.Ok(await executorStructure.SelectDictionary(query));
    return Results.Ok("allo");
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
    async () => Results.Ok(controllers.ToDictionary(item => item.Key, item => item.Value.Name))
)
.WithName("test");

*/
/*
internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/