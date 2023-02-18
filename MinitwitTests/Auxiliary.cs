using System;
using MinitwitTests;


public class Auxiliary {

    private readonly DatabaseFixture fixture;

    public Auxiliary(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    public MiniTwitContext CreateContext()
    {
        return fixture.CreateContext();
    }

    public async Task<HttpResponseMessage> Register(string username, string password, string? password2 = null, string? email = null)
    {
        if (password2 == null)
        {
            password2 = password;
        }
        if (email == null)
        {
            email = username + "@example.com";
        }

        var context = CreateContext();

        var user = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password },
            { "password2", password2 },
            { "email", email }
        };

        var content = new FormUrlEncodedContent(user);
        var response = await context.Client.PostAsync("/register", content).Result;
        return response;
    }

    public async Task<HttpResponseMessage> Login(String username, String password)
    {
        var context = CreateContext();

        var user = new Dictionary<String, String>
        {
            { "username", username },
            { "password", password }
        };
        var content = new FormUrlEncodedContent(user);
        var response = await context.httpClient.PostAsync("/login", content).Result;
        return response;
    }


    public async Task RegisterAndLogin(String username, String password)
    {
        await Register(username, password);
        await Login(username, password);
    }

    public async Task<HttpResponseMessage> Logout()
    {
        var context = CreateContext();
        var response = await context.httpClient.GetAsync("/logout").Result;
        return response;
    }

    public async Task<HttpResponseMessage> AddMessage(String text)
    {
        var context = CreateContext();
        var message = new Dictionary<String, String>
        {
            { "text", text }
        };
        var content = new FormUrlEncodedContent(message);
        var response = await context.httpClient.PostAsync("/add_message", content);

        var assertString = await response.Content.ReadAsStringAsync();
        if (text != null)
        {
            Assert.Contains("Your message was recorded", assertString);
        }
        return response;
    }
}