
using System.Data;
using System.Data.Common;
using System;
using Microsoft.EntityFrameworkCore;
using Xunit; // unsure if this is needed
namespace MinitwitTests;



public class DatabaseFixture : IDisposable
{

    private readonly SqliteConnection connection;
    public DatabaseFixture()
    {
        // Setup
        this.connection = new SqliteConnection("Data Source =:memory:");
        this.connection.Open();
    }

    public void Dispose()
    {
        // Teardown
        this.connection.Dispose();
    }

    public MiniTwitContext CreateContext()
    {
        var result = new MiniTwitContext(new DbContextOptionsBuilder<MiniTwitContext>()
            .UseSqlite(this.connection)
            .Options);
        result.Database.EnsureCreated();
        return result;
    }
}

public class MinitwitTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;
    private readonly Auxiliary auxiliary;

    public MinitwitTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        auxiliary = new Auxiliary(fixture);
    }



    [Theory]
    [InlineData("user1", "default", null, null, "You were successfully registered and can login now")]
    [InlineData("user1", "default", null, null, "The username is already taken")]
    [InlineData("", "default", null, null, "You have to enter a username")]
    [InlineData("meh", "", null, null, "You have to enter a password")]
    [InlineData("meh", "x", "y", null, "The two passwords do not match")]
    [InlineData("meh", "foo", null, "broken", "You have to enter a valid email address")]
    public async Task TestRegister(String username, String password, String password2, String email, String expected)
    {
        // Arrange
        var auxiliary = new Auxiliary(this.fixture);
        var context = fixture.CreateContext();

        // Act
        var response = await auxiliary.Register(username, password, password2, email);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("user1", "default", "You were logged in")]
    [InlineData("user1", "wrongpassword", "Invalid password")]
    [InlineData("user2", "wrongpassword", "Invalid username")]
    public async Task TestLogin(String username, String password, String expected)
    {
        // Arrange
        var auxiliary = new Auxiliary(this.fixture);
        var context = fixture.CreateContext();
        await auxiliary.Register(username, password);

        // Act
        var response = await auxiliary.Login(username, password);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(expected, result);

    }

    [Fact]
    public async Task TestLogout()
    {
        // Arrange
        var auxiliary = new Auxiliary(this.fixture);
        var context = fixture.CreateContext();
        await auxiliary.RegisterAndLogin("user1", "default");

        // Act
        var response = await auxiliary.Logout();
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("You were logged out", result);
    }

    [Theory]
    [InlineData("test message 1")]
    [InlineData("<test message 2>")]
    public async Task TestAddMessage(String message)
    {
        // Arrange
        var auxiliary = new Auxiliary(fixture);
        var context = fixture.CreateContext();
        await auxiliary.RegisterAndLogin("foo", "default");

        // Act
        await auxiliary.AddMessage(message);
        var response = await context.httpClient.GetAsync("/");
        var result = response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains(message, result);
    }

    [Fact]
    public void TestTimeline()
    {
        // Arrange
        var db = _fixture.Db;
        Minitwit.Register("user1", "default", "default", "email@example.com");
        Minitwit.Login("user1", "default");


        // Act
        Minitwit.AddMessage("test message by user1");
        Minitwit.Logout();

        // Arrange
        Minitwit.Register("user2", "default", "default", "email@example.com");
        Minitwit.Login("user2", "default");

        // Act
        Minitwit.AddMessage("test message by user2");

        // Assert both messages should be in the public timeline
        Assert.Contains<String>("test message by user1",Minitwit.get("/public"));
        Assert.Contains<String>("test message by user2",Minitwit.get("/public"));

        // Assert user2 timeline should not have user1 message
        Assert.DoesNotContain<String>("test message by user1",Minitwit.get("/"));
        Assert.Contains<String>("test message by user2", Minitwit.get("/"));

        // Act
        Minitwit.FollowUser("user1");

        // Assert user2 is following user1 and can see user1 messages
        Assert.Contains<String>("You are now following &#34;user1&#34;",Minitwit.get("/user1/follow"));
        Assert.Contains<String>("test message by user1",Minitwit.get("/"));
        Assert.Contains<String>("test message by user2",Minitwit.get("/"));

        // Assert user specific timelines only show own users messages
        Assert.Contains<String>("test message by user1",Minitwit.get("/user1"));
        Assert.DoesNotContain<String>("test message by user2",Minitwit.get("/user1"));

        Assert.Contains<String>("test message by user2",Minitwit.get("/user2"));
        Assert.DoesNotContain<String>("test message by user1",Minitwit.get("/user2"));

        // Act
        Minitwit.UnfollowUser("user1");

        // Assert user2 is not following user1 and can not see user1 messages
        Assert.Contains<String>("You are no longer following &#34;user1&#34;", Minitwit.get("/user1/unfollow") )
        Assert.DoesNotContain<String>("test message by user1",Minitwit.get("/"));
        Assert.Contains<String>("test message by user2",Minitwit.get("/"));

    }
}