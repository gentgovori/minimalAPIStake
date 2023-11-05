using FluentValidation;
using FluentValidation.Results;
using gambling.Data;
using gambling.Helpers;
using gambling.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<AppDbContext>(x => x.UseInMemoryDatabase("AppDb"));

builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

builder.Services.AddIdentityCore<MyUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//map new .NET 8 identity endpoints
app.MapIdentityApi<MyUser>();



//The method to play the game 
app.MapPost("/gambleyourlifeaway", async (ClaimsPrincipal claims, UserManager<MyUser> user, [FromBody] Stake stake) =>
{
    try
    {
        if (stake is not null)
        {
            //get current authenticated user
            var userDetails = await user.GetUserAsync(claims);

            //validate incoming request
            var validator = new StakeValidator(userDetails!.Balance);
            ValidationResult validation = await validator.ValidateAsync(stake);

            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }


            //generate random thread-safe number
            var randomNumber = Random.Shared.Next(1, 9);


            //check if the the user has lost the stake
            if (stake.number != randomNumber)
            {
                userDetails!.Balance = userDetails.Balance - stake.points;
                await user.UpdateAsync(userDetails);
                return Results.Ok(new ApiResult { account = userDetails!.Balance, points = $"-{stake.points}", status = Status.LOST });
            }


            //user won the stake 
            userDetails!.Balance = userDetails.Balance + (stake.points * 9);
            await user.UpdateAsync(userDetails);
            return Results.Ok(new ApiResult { account = userDetails!.Balance, points = $"+{stake.points * 9}", status = Status.WON });
        }

        return Results.NoContent();

    }
    catch (BadHttpRequestException)
    {
        //something went wrong
        //usually log the error and throw
        throw new ArgumentNullException(nameof(stake));
    }


})
.WithOpenApi()
.WithName("GambleYourLifeAway")
.RequireAuthorization();

app.Run();





