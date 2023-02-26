using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Minitwit7.data;
using Minitwit7.Models;
using System.Security.Cryptography;

namespace Minitwit7.Controllers
{
    [Route("api/[controller]")]
    public class SimulatorController : ControllerBase
    {
        private readonly DataContext _context;
        private int LATEST;

        public SimulatorController(DataContext context) // Connect directly to the database 
        {
            _context = context;
            LATEST = 0;
        }

        [HttpGet]
        [Route("/latest")]
        public ActionResult<LatestRes> Latest()
        {
            LatestRes res = new LatestRes();
            res.latest = helpers.getLatest();
            return Ok(res);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(Error))]
        [Route("/register")]
        public async Task<ActionResult<List<User>>> RegisterUser(CreateUser user, int latest = -1) // registration endpoint - check user's registration errors on models
        {                                                               //  we need to use  BCrypt.Net.BCrypt.HashPassword
            helpers.updateLatest(latest);

            User newUser = new User();

            if (user.username == null || user.username == "")
                return BadRequest(new Error("You have to enter a username"));
            
            else if (user.email == null || !user.email.Contains("@"))
                return BadRequest(new Error("You have to enter a valid email address"));

            else if (user.password == null || user.password == "")
                return BadRequest(new Error("You have to enter a password"));

            else if (helpers.getUserIdByUsername(_context, user.username) != -1)
                return BadRequest(new Error("The username is already taken"));
                
            
            newUser.Username = user.username;
            newUser.Email = user.email;


            newUser.PwHash = user.password;
            
           

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(_context.Users.ToList());
        }

        [HttpPost]
        [Route("/AddUser")]
        public async Task<ActionResult<List<User>>> AddUser(User user, int latest = - 1)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(_context.Users.ToList());
        }


        [HttpGet]
        [Route("/getAllUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = _context.Users.ToList();

            return Ok(users);
        }

        [HttpPost]
        [Route("/msgs")]
        public async Task<ActionResult<List<Message>>> AddUMsg(Message msg, int latest = -1)
        {
            helpers.updateLatest(latest);
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return Ok(_context.Messages.ToList());
        }

        [HttpGet]
        [Route("/msgs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<Message>>> GetMsgs(int latest = -1)
        {
            helpers.updateLatest(latest);
            var msgs = _context.Messages.ToList();
            if(msgs == null){return NoContent();}
            else{return Ok(msgs);}
        }

        [HttpGet]
        [Route("/msgs/{username}")]
        public async Task<ActionResult<List<Message>>> GetMsgsByUser(string username, int latest = -1)
        {
            helpers.updateLatest(latest);
            var user = _context.Users.Where(x => x.Username == username).FirstOrDefault();
            if(user == null) { return NoContent();}
            var msgs = _context.Messages.Where(x => x.AuthorId == user.UserId).ToList();

            return Ok(msgs);
        }

        [HttpPost]
        [Route("/fllws")]
        public async Task<ActionResult<List<Follower>>> AddFollower(Follower follower, int latest = -1)
        {
            helpers.updateLatest(latest);
            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();

            return Ok(_context.Followers.ToList());
        }

        [HttpGet]
        [Route("/fllws")]
        public async Task<ActionResult<List<User>>> GetUserFollowers(string username, int latest = -1)
        {
            helpers.updateLatest(latest);
            var followers = new List<User>();
            var user = _context.Users.Where(x => x.Username == username).FirstOrDefault();
            var flws = _context.Followers.Where(x => x.WhoId == user.UserId).ToList();
            foreach (var follower in flws)
            {
                var userF = _context.Users.Where(x => x.UserId == follower.WhomId).FirstOrDefault();
                followers.Add(userF);
            }


            return Ok(followers);
        }
    }

    public static class helpers
    {
        private static int LATEST = 0;

        public static int getLatest()
        {
            return LATEST;
        }

        public static void updateLatest(int latest)
        {
            if (latest != -1)
            {
                LATEST = latest;
            }
            else
            {
                LATEST = 0;
            }
        }

        public static int getUserIdByUsername(DataContext _context, string username)
        {
            User? u = _context.Users.Where(u => u.Username == username).FirstOrDefault();
            if (u != null)
                return u.UserId;
            return -1;
            
        }
    }

    public class Error
    {
        public int status { get; set; }
        public string error_msg { get; set; }
        public Error(string _error_msg, int _status = 400)
        {
            error_msg = _error_msg;
            status = _status;
        }
    }

    public class LatestRes
    {
        public int latest { get; set; }
    }

    public class CreateUser
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}