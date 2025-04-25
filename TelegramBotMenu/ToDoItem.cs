using System;

namespace TelegramBotMenu
{
    internal class ToDoItem
    {
        public Guid Id { get;}
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }

        public ToDoItem(ToDoUser user, string name) 
        {
            this.Id = Guid.NewGuid();
            this.User = user;
            this.Name = name;
            this.CreatedAt = DateTime.Now;
            this.State = ToDoItemState.Active;
            this.StateChangedAt = null;
        }
    }
}
