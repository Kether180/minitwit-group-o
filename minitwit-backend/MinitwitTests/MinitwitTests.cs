
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Xunit;
using Minitwit7.Controllers;
using Minitwit7.Models;
using Minitwit7.data;


public class DatabaseFixture : IDisposable
{

    private readonly SQLiteConnection connection;
    private readonly SimulatorController simCon;
    public DatabaseFixture(SimulatorController simCon)
    {
        this.simCon = simCon;
        // Setup
        this.connection = new SQLiteConnection("Data Source =:memory:");
        this.connection.Open();
    }

    public void Dispose()
    {
        // Teardown
        this.connection.Dispose();
    }

    public DataContext CreateContext()
    {
        var result = new DataContext(new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(this.connection)
            .Options);
        result.Database.EnsureDeleted();
        result.Database.EnsureCreated();
        return result;
    }
}

public class MinitwitTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;
    private readonly Auxiliary auxiliary;
    private readonly SimulatorController simCon;

    public MinitwitTests(DatabaseFixture fixture, SimulatorController simCon)
    {
        this.fixture = fixture;
        this.simCon = simCon;
        auxiliary = new Auxiliary(fixture, simCon);
    }

    private static T GetObjectResultContent<T>(ActionResult<T> result)
    {
        return (T) ((ObjectResult) result.Result).Value;
    }


    [Theory]
    [InlineData("user1", "default", null, null, "You were successfully registered and can login now", true)]
    [InlineData("user1", "default", null, null, "The username is already taken", false)]
    [InlineData("", "default", null, null, "You have to enter a username", false)]
    [InlineData("meh", "", null, null, "You have to enter a password", false)]
    [InlineData("meh", "foo", null, "broken", "You have to enter a valid email address", false)]
    public async Task TestRegister(String username, String password, String email, String expected, bool success)
    {
        // Arrange
        var auxiliary = new Auxiliary(this.fixture, this.simCon);
        var context = fixture.CreateContext();

        // Act
        var response = await auxiliary.Register(username, password, email);


        // Assert
        if (success) {
            Assert.Equal(expected, result);
        } else {
            Assert.IsType<BadRequestObjectResult>(response.Result);

        }
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

    /*[Fact]
    public void TestTimeline()
    {
        // Arrange
        var db = fixture.Db;
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
        Assert.Contains<String>("You are no longer following &#34;user1&#34;", Minitwit.get("/user1/unfollow") );
        Assert.DoesNotContain<String>("test message by user1",Minitwit.get("/"));
        Assert.Contains<String>("test message by user2",Minitwit.get("/"));

    }*/
}