
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Minitwit7.Controllers;
using Minitwit7.data;

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


        // Act
        var result = await simCon.AddFollower("TestUser1", followReq);
        var followerCount = context.Followers.Where(u => u.UserId == 1 && u.FollowsId == 2).Count();

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(1, followerCount);
    }

    [Fact]
    public async Task test_follow_unsuccessful(){
        // Arrange
        var followReq = new FollowRequest {
            follow = "TestUser1337"
        };

        // Act
        var result = await simCon.AddFollower("TestUser1", followReq);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task test_follow_unfollow_successful(){
        // Arrange
        var followReq = new FollowRequest {
            follow = "TestUser2"
        };

        var unfollowReq = new FollowRequest {
            unfollow = "TestUser2"
        };

        // Act
        await simCon.AddFollower("TestUser1", followReq);
        var result = await simCon.AddFollower("TestUser1", unfollowReq);
        var followerCount = context.Followers.Where(u => u.UserId == 1 && u.FollowsId == 2).Count();

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(0, followerCount);
    }

    [Fact]
    public async Task test_unfollow_while_not_following(){
        // Arrange
        var unfollowReq = new FollowRequest {
            unfollow = "TestUser2"
        };

        // Act
        var result = await simCon.AddFollower("TestUser1", unfollowReq);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task test_post_message_successful(){
        // Arrange
        var msg1 = new CreateMessage{
            content = "Test message"
        };

        // Act
        var result = await simCon.AddUMsg("TestUser1", msg1);
        var msgCount = context.Messages.Where(a => a.AuthorId == 1).Count();

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(1, msgCount);
    }

    [Fact]
    public async Task test_post_message_unsuccessful(){
        // Arrange
        var msg1 = new CreateMessage{
            content = "Test message"
        };

        // Act
        var result = await simCon.AddUMsg("TestUser1337", msg1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
