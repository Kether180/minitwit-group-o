using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minitwit7.data;
using Minitwit7.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Minitwit7.Controllers
{
    [Route("api/[controller]")]
    public class MinitwitController : ControllerBase
    {
        private readonly DataContext _context;

        public MinitwitController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/AddUser")]
        public async Task<ActionResult<List<User>>> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(_context.Users.ToList());
        }

        [HttpPost]
        [Route("/AddMsg")]
        public async Task<ActionResult<List<Message>>> AddUMsg(Message msg)
        {
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();
            return Ok(_context.Messages.ToList());
        }

        [HttpGet]
        [Route("/GetMsgs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<Message>>> GetMsgs()
        {
            var msgs = _context.Messages.ToList();
            if(msgs == null){return NoContent();}
            else{return Ok(msgs);}
        }
    }
}