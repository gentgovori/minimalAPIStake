using FluentValidation;
using FluentValidation.Results;
using gambling.Data;
using gambling.Helpers;
using gambling.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite("DataSource=db.db"));


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


app.MapIdentityApi<MyUser>();




app.MapPost("/gambleyourlifeaway", async (ClaimsPrincipal claims, UserManager<MyUser> user, [FromBody]Stake stake) =>
{
    try
    {
        //get current authenticated user
        var userDetails = await user.GetUserAsync(claims);


        //validate incoming request
        var validator = new StakeValidator(userDetails!.Balance);
        ValidationResult validation = await validator.ValidateAsync(stake);

        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());


        //generate random thread-safe number
        var value = Random.Shared.Next(1, 9);

        //check if the the user has lost the stake
        if (stake.number != value)
        {
            userDetails!.Balance = userDetails.Balance - stake.points;
            await user.UpdateAsync(userDetails);
            return Results.Ok(new ApiResult { account = userDetails!.Balance, points = $"-{stake.points}", status = Status.LOST });
        }


        //user won the stake 
        userDetails!.Balance = userDetails.Balance + (stake.points * 9);
        await user.UpdateAsync(userDetails);
        return Results.Ok(new ApiResult { account = userDetails!.Balance, points = $"+{stake.points}", status = Status.WON });
    }
    catch (BadHttpRequestException ex)
    {
        //something went wrong
        return Results.BadRequest(ex.Message);
    }


})
.WithOpenApi()
.WithName("GambleYourLifeAway")
.RequireAuthorization();

app.Run();





