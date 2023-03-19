
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore.Sqlite;
using Xunit;
using Minitwit7.Controllers;
using Minitwit7.data;
using Minitwit7.Models;

public class MinitwitTests : IDisposable
{
    private readonly DataContext context;
    private readonly SimulatorController simCon;

    public MinitwitTests()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseInMemoryDatabase(databaseName: "MiniTwitDatabase");

            var dbContextOptions = builder.Options;
            context = new TestDataContext(dbContextOptions);
            // Delete existing db before creating a new one
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            simCon = new SimulatorController(context);
    }

    public void Dispose()
    {
        // Teardown
        context.Dispose();
    }
    private static T GetObjectResultContent<T>(ActionResult<T> result)
    {
        return (T) ((ObjectResult) result.Result).Value;
    }




    [Fact]
    public async Task test_create_user_successful(){
        // Arrange
        var user = new CreateUser {
            username = "testUser",
            email = "testuser@email.com",
            pwd = "testpass"
        };

        // Act
        var result = await simCon.RegisterUser(user, 1);
        var id = Helpers.GetUserIdByUsername(context, "testUser");

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(4,id);
    }

    [Theory]
    [InlineData("","mail@mail.com","12345")]
    [InlineData("username","mailmail.com","12345")]
    [InlineData("username","mail@mail.com","")]
    [InlineData("TestUser1","TestUser1@test.com","12345")]
    public async Task test_create_user_unsuccessful(String username, string email, String pass){
        // Arrange
        var user = new CreateUser {
            username = username,
            email = email,
            pwd = pass
        };

        // Act
        var result = await simCon.RegisterUser(user, 1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task test_follow_successful(){
        // Arrange
        var followReq = new FollowRequest {
            follow = "TestUser2"
        };
        var expectedFollowerList = new List<User>();


        // Act
        var result = await simCon.AddFollower("TestUser1", followReq);
        var followers = await simCon.GetUserFollowers("TestUser2");
        expectedFollowerList.Add(context.Users.Where(u => u.UserId == 1).First());

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
       // Assert.Equal(expectedFollowerList,followers.Value);
    }

}