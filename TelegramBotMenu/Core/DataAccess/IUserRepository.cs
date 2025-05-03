using System;
using TelegramBot.Core.Entities;

namespace TelegramBot.Core.DataAccess
{
    internal interface IUserRepository
    {
        ToDoUser? GetUser(Guid userId);
        ToDoUser? GetUserByTelegramUserId(long telegramUserId);
        void Add(ToDoUser user);
    }
}
