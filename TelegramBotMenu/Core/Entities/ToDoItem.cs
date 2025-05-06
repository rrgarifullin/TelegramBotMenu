using System;

namespace TelegramBot.Core.Entities
{
    internal class ToDoItem
    {
        public Guid Id { get; }
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }

        public ToDoItem(ToDoUser user, string name)
        {
            Id = Guid.NewGuid();
            User = user;
            Name = name;
            CreatedAt = DateTime.Now;
            State = ToDoItemState.Active;
            StateChangedAt = null;
        }
    }
}
