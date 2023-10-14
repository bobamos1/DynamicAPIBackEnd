using APIDynamic;
using DynamicStructureObjects;
using DynamicSQLFetcher;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

Dictionary<string, string> connectionStrings = InitializationFile.LoadConnectionStrings(builder.Configuration);
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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await InitializationFile.InitDB(executorStructure, true);//keep it commit
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("NgOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

Dictionary<string, DynamicController> controllers = await DynamicController.initControllers(executorStructure, builder.Configuration["JwtSettings:Key"]); //
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
