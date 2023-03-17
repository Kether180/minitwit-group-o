using Xunit;
using Minitwit7.Controllers;
using Minitwit7.Models;
using Microsoft.AspNetCore.Mvc;



public class Auxiliary {

    private readonly DatabaseFixture fixture;
    private readonly SimulatorController simCon;

    public Auxiliary(DatabaseFixture fixture, SimulatorController simCon)
    {
        this.fixture = fixture;
        this.simCon = simCon;
    }



    public async Task<ActionResult<List<User>>> Register(string username, string password, string? password2 = null, string? email = null)
    {
        if (password2 == null)
        {
            password2 = password;
        }
        if (email == null)
        {
            email = username + "@example.com";
        }

        var user = new CreateUser()
        {
            username = username,
            email = email,
            pwd = password
        };

        var response = await simCon.RegisterUser(user);
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