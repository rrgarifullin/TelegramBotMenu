using System;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private List<ToDoUser> _toDoUsers = new List<ToDoUser>();

        public void Add(ToDoUser user)
        {
            _toDoUsers.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            var user = _toDoUsers.FirstOrDefault(x => x.UserId == userId);
            return user;
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            if (_toDoUsers == null) return null;

            var user = _toDoUsers.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
            return user;
        }
    }
}
