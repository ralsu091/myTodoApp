using Xunit;
using System;
using System.Linq;
using todo_api.Models;
using todo_api.Contexts;
using todo_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace todo_api.Tests
{
    public class TestDbContext
    {
        [Fact]
        private async void Get_Empty_Db_Returns_EmptyArray()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db1").Options);
            TodoController controller = new TodoController(_dbContext);
            var result = await controller.GetTodos((DateTime?)null);
            Assert.Empty(result);

        }

        [Fact]
        public async void Post_SingleTodo_InvalidId_ShouldReturn400()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db2").Options);
            TodoController controller = new TodoController(_dbContext);
            var result = await controller.AddTodo(new TodoItem { Id = Guid.NewGuid() });
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Post_SingleTodo_Valid_ShouldSucceed()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db3").Options);
            TodoController controller = new TodoController(_dbContext);
            var result = await controller.AddTodo(new TodoItem { Description = "Laundry", Date = DateTime.UtcNow });
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async void Update_SingleTodo_WrongId_In_body_ShoulReturn400()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db4").Options);
            TodoController controller = new TodoController(_dbContext);
            await controller.AddTodo(new TodoItem { Description = "Laundry", Date = DateTime.UtcNow });
            var item = _dbContext.TodoItems.First();
            var result = await controller.UpdateTodo(item.Id, new TodoItem{});
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Update_SingleTodo_ValidId_ValidBody_ShouldSucceed()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db4").Options);
            TodoController controller = new TodoController(_dbContext);
            await controller.AddTodo(new TodoItem { Description = "Laundry", Date = DateTime.UtcNow });
            var item = _dbContext.TodoItems.First();
            var result = await controller.UpdateTodo(item.Id, new TodoItem{Id = item.Id, Description = "Laundry 2", Date = item.Date, Completed = true});
            Assert.IsType<OkResult>(result);
            item = _dbContext.TodoItems.First();
            Assert.Equal("Laundry 2", item.Description);
            Assert.True(item.Completed);
        }

        [Fact]
        public async void Update_NonExistent_Item_ShouldReturnNotFound()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db5").Options);
            TodoController controller = new TodoController(_dbContext);
            var id = Guid.NewGuid();
            var result = await controller.UpdateTodo(id, new TodoItem{Id = id, Description = "Laundry 2", Date = DateTime.UtcNow, Completed = true});
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_NonExistent_Item_ShouldReturnNotFound()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db6").Options);
            TodoController controller = new TodoController(_dbContext);
            var id = Guid.NewGuid();
            var result = await controller.DeleteTodo(id);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_Item_ShouldSucceed()
        {
            var _dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("todo-db7").Options);
            TodoController controller = new TodoController(_dbContext);
            await controller.AddTodo(new TodoItem { Description = "Laundry", Date = DateTime.UtcNow });
            var item = _dbContext.TodoItems.First();
            var result = await controller.DeleteTodo(item.Id);
            Assert.IsType<OkResult>(result);
        }
    }
}