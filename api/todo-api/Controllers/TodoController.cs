using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Contexts;
using todo_api.Models;

namespace todo_api.Controllers
{
    [ApiController]
    public class TodoController : ControllerBase
    {
        private TodoContext _context { get; set; }
        public TodoController(TodoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// gets your TODO items
        /// </summary>
        /// <remarks>Get the list of TODO items</remarks>
        /// <param name="date">optional query for a particular day. Not supplying a date will return todays todos (in UTC)</param>
        /// <response code="200">List of TODOs</response>
        /// <response code="400">Bad date supplied</response>
        [HttpGet]
        [Route("/todos")]
        public async Task<TodoItem[]> GetTodos([FromQuery] DateTime? date)
        {
            var queryValue = date ?? DateTime.UtcNow;
            var items = await _context.TodoItems.Where(x => x.Date.Date == queryValue.Date).ToArrayAsync();

            return items;
        }

        /// <summary>
        /// add a TODO item
        /// </summary>
        /// <remarks>Add a TODO item</remarks>
        /// <param name="body">TODO item to add</param>
        /// <response code="201">created</response>
        /// <response code="400">Item supplied is invalid</response>
        [HttpPost]
        [Route("/todos")]
        public async Task<IActionResult> AddTodo([FromBody] TodoItem body)
        {
            // Users are not allowed to set the ID
            if (body.Id == Guid.Empty)
            {
                body.Id = Guid.NewGuid();
                await _context.AddAsync(body);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }

            return BadRequest();
        }

        /// <summary>
        /// Update a TODO item
        /// </summary>
        /// <remarks>Updates a TODO item</remarks>
        /// <param name="id">Unique Id of TODO item</param>
        /// <param name="body">TODO item to update</param>
        /// <response code="200">Update operation succeeded</response>
        /// <response code="400">TODO item Id does not match the one supplied in the path</response>
        /// <response code="404">Unable to find TODO item</response>
        [HttpPut]
        [Route("/todos/{id}")]
        public async Task<IActionResult> UpdateTodo([FromRoute][Required] Guid? id, [FromBody] TodoItem body)
        {
            if (!id.HasValue || id.Value != body.Id)
            {
                return BadRequest();
            }

            var item = await _context.TodoItems.SingleOrDefaultAsync(x => x.Id == id.Value);
            if (item == null)
            {
                return NotFound();
            }


            item.Description = body.Description;
            item.Completed = body.Completed;
            item.Date = body.Date;
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Delete a TODO item
        /// </summary>
        /// <remarks>Deletes a TODO item by id</remarks>
        /// <param name="id">Unique Id of TODO item</param>
        /// <response code="200">Successfully deleted TODO item</response>
        /// <response code="400">Invalid TODO Item Id supplied</response>
        /// <response code="404">Unable to find TODO item</response>
        [HttpDelete]
        [Route("/todos/{id}")]
        public async Task<IActionResult> DeleteTodo([FromRoute][Required] Guid? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var item = await _context.TodoItems.SingleOrDefaultAsync(x => x.Id == id.Value);
            if (item == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}