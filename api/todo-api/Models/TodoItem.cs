using System;

namespace todo_api.Models
{
    /// <summary>
    /// A Todo item is the main model that hold eveything about a task
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Unique Id of TodoItem.
        /// </summary>
        /// <value></value>
        /// <remarks>Guid is used to (almost) gurantee uniqueness</remarks>
        public Guid Id { get; set; }

        /// <summary>
        /// Descripe what you are doing/accomplishing
        /// </summary>
        /// <value></value>
        public string Description { get; set; }

        /// <summary>
        /// Date and Time of the item. Stored in UTC
        /// </summary>
        /// <value></value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Flag to indicate if this item is completed (true) or not (false)
        /// </summary>
        /// <value></value>
        public bool Completed { get; set; }

    }
}