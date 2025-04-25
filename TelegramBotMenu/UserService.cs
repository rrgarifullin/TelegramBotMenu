using System;

namespace TelegramBotMenu
{
    internal class UserService : IUserService
    {
        private List<ToDoUser> toDoUsers = new List<ToDoUser>();

        public ToDoUser? GetUser(long telegramUserId)
        {
            foreach(var user in toDoUsers)
            {
                if (user.TelegramUserId == telegramUserId)
                    return user;
            }
            return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserId, telegramUserName);
            toDoUsers.Add(user);
            return user;
        }
    }
}
