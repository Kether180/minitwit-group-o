
using System.Data;
using System.Data.Common;
using System;
using System.Data.SQLite;
using Xunit; // unsure if this is needed
//using Minitwit;
namespace MinitwitTests;



public class DatabaseFixture : IDisposable
{

    public DatabaseFixture()
    {
        // Setup
        Db = new SQLiteConnection("Data Source = database.db;Version=3;New=True;Compress=True;");
    }

    public void Dispose()
    {
        // Teardown
        Db.Dispose();
    }

    public SQLiteConnection Db { get; private set; }
}

public class MinitwitTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture _fixture;

    public MinitwitTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }



    [Theory]
    [InlineData("user1", "default", "default", "email@example.com")]
    [InlineData("user1", "default", "default", "email@example.com")]
    [InlineData("", "default", "default", "email@example.com")]
    [InlineData("meh", "", "default", "email@example.com")]
    [InlineData("meh", "x", "y", "email@example.com")]
    [InlineData("meh", "foo", "foo", "broken")]
    public void TestRegister(String username, String password, String password2, String email)
    {
        // Arrange
        var db = _fixture.Db;

        // Act
        var result = Minitwit.Register(username, password, password2, email);

        // Assert
        Assert.Equal("You were successfully registered and can login now", result);
        Assert.Equal("The username is already taken", result);
        Assert.Equal("You have to enter a username", result);
        Assert.Equal("You have to enter a password", result);
        Assert.Equal("The two passwords do not match", result);
        Assert.Equal("You have to enter a valid email address", result);
    }

    [Theory]
    [InlineData("user1", "default")]
    [InlineData("user1", "wrongpassword")]
    [InlineData("user2", "wrongpassword")]
    public void TestLogin(String username, String password)
    {
        // Arrange
        var db = _fixture.Db;
        Minitwit.Register("user1", "default", "default", "email@example.com");

        // Act
        var result = Minitwit.Login(username, password);

        // Assert
        Assert.Equal("You were logged in", result);
        Assert.Equal("Invalid password", result);
        Assert.Equal("Invalid username", result);
    }

    [Fact]
    public void TestLogout()
    {
        // Arrange
        var db = _fixture.Db;
        Minitwit.Register("user1", "default", "default", "email@example.com");

        // Act
        var result = Minitwit.Logout();

        // Assert
        Assert.Equal("You were logged out", result);
    }

    [Theory]
    [InlineData("test message 1")]
    [InlineData("<test message 2>")]
    public void TestAddMessage(String message)
    {
        // Arrange
        var db = _fixture.Db;
        Minitwit.Register("user1", "default", "default", "email@example.com");
        Minitwit.Login("user1", "default");

        // Act
        Minitwit.AddMessage(message);
        var result = Minitwit.get("/"); // pretty much stolen from python tests, most likely not correct

        // Assert
        Assert.Equal("test message 1", result);
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