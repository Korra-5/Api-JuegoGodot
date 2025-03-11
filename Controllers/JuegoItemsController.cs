using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JuegoApi.Models;

namespace JuegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuegoItemsController : ControllerBase
    {
        private readonly JuegoContext _context;

        public JuegoItemsController(JuegoContext context)
        {
            _context = context;
        }

        // GET: api/JuegoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JuegoItem>>> GetJuegoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/JuegoItems/username
        [HttpGet("{name}")]
        public async Task<ActionResult<JuegoItem>> GetJuegoItem(string name)
        {
            var juegoItem = await _context.TodoItems.FindAsync(name);

            if (juegoItem == null)
            {
                return NotFound();
            }

            return juegoItem;
        }

        // PUT: api/JuegoItems/username
        [HttpPut("{name}")]
        public async Task<IActionResult> PutJuegoItem(string name, JuegoItem juegoItem)
        {
            if (name != juegoItem.Name)
            {
                return BadRequest();
            }

            _context.Entry(juegoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JuegoItemExists(name))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/JuegoItems
        [HttpPost]
        public async Task<ActionResult<JuegoItem>> PostJuegoItem(JuegoItem juegoItem)
        {
            // Check if user already exists
            var existingItem = await _context.TodoItems.FindAsync(juegoItem.Name);
            
            if (existingItem != null)
            {
                // Update time only if new time is better (lower)
                if (juegoItem.Time < existingItem.Time)
                {
                    existingItem.Time = juegoItem.Time;
                    _context.Entry(existingItem).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                
                return CreatedAtAction(nameof(GetJuegoItem), new { name = juegoItem.Name }, existingItem);
            }
            else
            {
                // Add new user with their time
                _context.TodoItems.Add(juegoItem);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetJuegoItem), new { name = juegoItem.Name }, juegoItem);
            }
        }

        // DELETE: api/JuegoItems/username
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteJuegoItem(string name)
        {
            var juegoItem = await _context.TodoItems.FindAsync(name);
            if (juegoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(juegoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JuegoItemExists(string name)
        {
            return _context.TodoItems.Any(e => e.Name == name);
        }
    }
}