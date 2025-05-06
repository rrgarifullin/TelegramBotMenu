using System;
using TelegramBot.Core.Entities;

namespace TelegramBot.Core.Services.Interface
{
    internal interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid id);
        void Delete(Guid id);
        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);
    }
}
