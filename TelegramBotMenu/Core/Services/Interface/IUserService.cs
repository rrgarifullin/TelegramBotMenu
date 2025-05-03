using System;
using TelegramBot.Core.Entities;

namespace TelegramBot.Core.Services.Interface
{
    internal interface IUserService
    {
        ToDoUser RegisterUser(long telegramUserId, string telegramUserName);
        ToDoUser? GetUser(long telegramUserId);
    }
}
