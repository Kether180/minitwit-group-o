using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minitwit7.data;
using Minitwit7.Models;


namespace Minitwit7.Controllers
{
    [Route("api/[controller]")]
    public class MinitwitController : ControllerBase
    {
        private readonly DataContext _context;

        public MinitwitController(DataContext context) // Connect directly to the database 
        {
            _context = context;
        }


        [HttpPost]
        [Route("/RegisterUser")]
        public async Task<ActionResult<List<User>>> RegisterUser(User user) // registration endpoint - check user's registration errors on models
        {                                                               //  we need to use  BCrypt.Net.BCrypt.HashPassword
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(_context.Users.ToList());
        }

        [HttpPost]
        [Route("/AddUser")]
        public async Task<ActionResult<List<User>>> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(_context.Users.ToList());
        }
    }
}