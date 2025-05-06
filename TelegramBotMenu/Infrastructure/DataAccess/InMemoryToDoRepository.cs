using System;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private List<ToDoItem> _toDoItems = new List<ToDoItem>();

        public void Add(ToDoItem item)
        {
            _toDoItems.Add(item);
        }

        public int CountActive(Guid userId)
        {
            var tasks = GetActiveByUserId(userId);
            return tasks.Count;
        }

        public void Delete(Guid id)
        {
            _toDoItems.RemoveAll(x => x.Id == id);
        }

        public bool ExistsByName(Guid userId, string name)
        {
            var task = _toDoItems.FirstOrDefault(x => x.User.UserId == userId && x.Name == name);
            return task == null ? false : true;
        }

        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            var tasks = GetAllByUserId(userId);
            IReadOnlyList<ToDoItem> filteredTasks = tasks.Where(x => predicate(x)).ToList();
            return filteredTasks;
        }

        public ToDoItem? Get(Guid id)
        {
            var task = _toDoItems.FirstOrDefault(x => x.Id == id);
            return task;
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            var tasks = _toDoItems
                            .Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active)
                            .ToList();
            return tasks;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            var tasks = _toDoItems
                            .Where(x => x.User.UserId == userId)
                            .ToList();
            return tasks;
        }

        public void Update(ToDoItem item)
        {
            item.State = ToDoItemState.Completed;
            item.StateChangedAt = DateTime.Now;
        }
    }
}
