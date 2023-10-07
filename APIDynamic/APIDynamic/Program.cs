using APIDynamic;
using DynamicStructureObjects;
using DynamicSQLFetcher;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

Dictionary<string, string> connectionStrings = RoutesInit.LoadConnectionStrings(builder.Configuration);
DynamicConnection.setEmailSender(builder.Configuration["Email:EmailHost"], builder.Configuration["Email:UsernameHost"], builder.Configuration["Email:PasswordHost"], "smtp.gmail.com", 587);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "The API Key to access the API",
        Type = SecuritySchemeType.ApiKey,
        Name = "JWT",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    };
    options.AddSecurityRequirement(requirement);
    //options.OperationFilter<SecurityRequirementsOperationFilter>();
});
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
builder.Services.AddAuthorization(/*options =>
{
    //DynamicController.addPolicies(controllers, options);
}*/);
builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }));
var app = builder.Build();
SQLExecutor executorStructure = new SQLExecutor(connectionStrings["structure"]);
await BDInit.InitDB(executorStructure, true);//keep it commit
Dictionary<string, DynamicController> controllers = await DynamicController.initControllers(executorStructure, builder.Configuration["JwtSettings:Key"]); //
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("NgOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

RoutesInit.InitRoutes(controllers, app, connectionStrings);
app.Run();


/*
 "ConnectionStrings": {
    "structure": "Data Source=...",
    "data": "Data Source=..."
  },
  "JwtSettings": {
    "Issuer": "https://localhost:7247",
    "Audience": "https://localhost:7247",
    "Key": "NotDoingWhatIAmSupposeToDoPisFautRajouterDuStockPourQueLaLigneSoisAssezLngue"
  },
  "Email": {
    "EmailHost": "testcourriel415@gmail.com",
    "UsernameHost": "testcourriel415",
    "PasswordHost": "cbnyrpctgpgcahki"
  },
 */











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