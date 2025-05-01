using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Linq;

namespace TelegramBotMenu
{
    internal class ToDoService : IToDoService
    {
        private readonly List<ToDoItem> toDoItems = new List<ToDoItem>();
        private readonly int taskCountLimit = 5;
        private readonly int taskLengthLimit = 20;

        public ToDoItem Add(ToDoUser user, string name)
        {
            IReadOnlyList<ToDoItem> userTasks = GetActiveByUserId(user.UserId);

            if (userTasks.Count >= taskCountLimit)
                throw new TaskCountLimitException(taskCountLimit);

            if (name.Length > taskLengthLimit)
                throw new TaskLengthLimitException(name.Length, taskLengthLimit);

            foreach (var item in userTasks)
            {
                if (item.Name == name)
                    throw new DuplicateTaskException(name);
            }

            ToDoItem toDoItem = new ToDoItem(user, name);
            toDoItems.Add(toDoItem);

            return toDoItem;
        }

        public void Delete(Guid id)
        {
            List<ToDoItem> task = toDoItems.Where(x => x.Id == id).ToList();

            if (task.Count > 0)
                toDoItems.RemoveAll(x => x.Id == id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            List<ToDoItem> userActiveTasks = toDoItems
                                                .Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active)
                                                .ToList();
            return userActiveTasks;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem> userTasks = toDoItems.Where(x => x.User.UserId == userId).ToList();
            return userTasks;
        }

        public void MarkCompleted(Guid id)
        {
            ToDoItem? item = toDoItems.FirstOrDefault(x => x.Id == id);

            if (item != null && item.State != ToDoItemState.Completed)
            {
                item.State = ToDoItemState.Completed;
                item.StateChangedAt = DateTime.Now;
            }
        }
    }
}
