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

        public SimulatorController(DataContext context) // Connect directly to the database 
        {
            _context = context;
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(Error))]
        [Route("/register")]
        public async Task<ActionResult<List<User>>> RegisterUser(CreateUser user, int latest = -1)
        {
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

            // Hashing the users password is done as stated in this post: https://stackoverflow.com/questions/4181198/how-to-hash-a-password

            // Creating the salt for the password hash
            byte[] salt = new byte[16];
            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(salt);
            }

            // Hash the password with the salt
            var pbkdf2 = new Rfc2898DeriveBytes(user.password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            // combine the salt and password into one variable, with the salt in the first 16 bytes,
            // and the password in the last 20.
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // store the salt + hashed password in a string
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            newUser.PwHash = savedPasswordHash;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(_context.Users.ToList());
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("/getAllUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = _context.Users.ToList();

            await Task.CompletedTask;

            return Ok(users);
        }


        [HttpGet]
        [Route("/msgs")]
        public async Task<ActionResult<List<MessageRes>>> GetMsgs(int no = 100, int latest = -1)
        {
            helpers.updateLatest(latest);

            List<Message> msgs = _context.Messages.OrderByDescending(m => m.PubDate).Take(no).ToList();

            List<MessageRes> res = new List<MessageRes>();
            foreach (Message msg in msgs)
            {
                string username = _context.Users.Where(u => u.UserId == msg.AuthorId).First().Username;
                res.Add(new MessageRes()
                {
                    content = msg.text,
                    pub_date = msg.PubDate,
                    username = username
                });
            }

            await Task.CompletedTask;

            return Ok(res);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(Error))]
        [Route("/msgs/{username}")]
        public async Task<ActionResult<List<Message>>> AddUMsg(string username, CreateMessage msg, int latest = -1)
        {
            helpers.updateLatest(latest);

            int userId = helpers.getUserIdByUsername(_context, username);
            if (userId == -1)
                return BadRequest(new Error("Username does not match a user"));

            Message newMsg = new Message()
            {
                AuthorId = userId,
                text = msg.content,
                PubDate = DateTime.UtcNow,
                Flagged = 0
            };

            _context.Messages.Add(newMsg);
            await _context.SaveChangesAsync();

            return Ok(_context.Messages.ToList());
        }



        [HttpGet]
        [Route("/msgs/{username}")]
        public async Task<ActionResult<List<Message>>> GetMsgsByUser(string username, int latest = -1)
        {
            helpers.updateLatest(latest);
            var user = _context.Users.Where(x => x.Username == username).FirstOrDefault();
            if(user == null) { return NoContent();}
            var msgs = _context.Messages.Where(x => x.AuthorId == user.UserId).ToList();

            await Task.CompletedTask;

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

    public class MessageRes
    {
        public string content { get; set; }
        public DateTime pub_date { get; set; }
        public string username { get; set; }
    }

    public class CreateMessage
    {
        public string content { get; set;}
    }
}