using System;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Entities;
using TelegramBot.Core.Exceptions;
using TelegramBot.Core.Services.Interface;

namespace TelegramBot.Core.Services.Service
{
    internal class ToDoService : IToDoService
    {
        private readonly int taskCountLimit = 5;
        private readonly int taskLengthLimit = 20;
        IToDoRepository _repository;

        public ToDoService(IToDoRepository toDoRepository)
        {
            _repository = toDoRepository;
        }

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
            _repository.Add(toDoItem);

            return toDoItem;
        }

        public void Delete(Guid id)
        {
            var task = _repository.Get(id);

            if (task != null)
                _repository.Delete(id);
        }

        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            var filteredTasks = _repository.Find(user.UserId, x => x.Name.StartsWith(namePrefix));
            return filteredTasks.ToList();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            var activeTasks = _repository.GetActiveByUserId(userId);
            return activeTasks;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            var tasks = _repository.GetAllByUserId(userId);
            return tasks;
        }

        public void MarkCompleted(Guid id)
        {
            ToDoItem? item = _repository.Get(id);

            if (item != null && item.State != ToDoItemState.Completed)
            {
                _repository.Update(item);
            }
        }
    }
}
