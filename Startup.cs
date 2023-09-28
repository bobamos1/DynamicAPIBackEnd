using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

StripeConfiguration.ApiKey = "sk_test_51NuEUwHpVTFwinL2tkroWNqpnovIvpzw2r2bKPcjxvV9evuFETB9oJREpIxnvyxUVFgBBJj7qATARoyOhb1PMbue00zlB3fR0o";

var app = builder.Build();

// Configure endpoints


/*
app.MapPost("/checkout", async (HttpContext context) =>
{
    try
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "T-shirt"
                        },
                        UnitAmount = 2000
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = "http://localhost:4242/success.html",
            CancelUrl = "http://localhost:4242/cancel.html"
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(session.ToJson());
    }
    catch (Exception ex)
    {
        // Handle the error
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(ex.Message);
    }
});
*/
// Enable CORS
app.UseCors();

app.Run();



using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class StripeMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path == "/stripe") // Adjust the path as needed
        {
            try
            {
                // Your Stripe API key
                string stripeApiKey = "sk_test_51NuEUwHpVTFwinL2tkroWNqpnovIvpzw2r2bKPcjxvV9evuFETB9oJREpIxnvyxUVFgBBJj7qATARoyOhb1PMbue00zlB3fR0o";

                // Create an HttpClient instance
                using (HttpClient httpClient = new HttpClient())
                {
                    // Set the Stripe API key in the request headers
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {stripeApiKey}");

                    // Create your Stripe request payload here
                    var requestContent = new StringContent("Your request payload goes here");

                    // Send the asynchronous POST request to Stripe
                    HttpResponseMessage response = await httpClient.PostAsync("Stripe API endpoint URL", requestContent);

                    // Check if the request was successful (HTTP status code 2xx)
                    if (response.IsSuccessStatusCode)
                    {
                        // Handle the successful response from Stripe
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Process the response as needed
                        // You can return a response to the client or perform further actions here

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(responseContent);
                        return;
                    }
                    else
                    {
                        // Handle the error from Stripe (e.g., non-2xx status code)
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync("Stripe API request failed.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the request
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex.Message);
                return;
            }
        }

        // If the request doesn't match the Stripe path, continue to the next middleware
        await next(context);
    }
}
