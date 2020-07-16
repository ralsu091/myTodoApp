using Microsoft.EntityFrameworkCore;
using todo_api.Models;
namespace todo_api.Contexts
{
    public class TodoContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>(builder =>
            {
                builder.HasKey("Id");
            });
        }


    }
}