using System;

namespace TelegramBotMenu
{
    internal class ToDoUser
    {
        public Guid UserId { get; }
        public long TelegramUserId { get; }
        public string? TelegramUserName { get; }
        public DateTime RegisteredAt { get; }

        public ToDoUser(long telegramUserId, string telegramUserName) 
        {
            TelegramUserId = telegramUserId;
            TelegramUserName = telegramUserName;
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.Now;
        }
    }
}
