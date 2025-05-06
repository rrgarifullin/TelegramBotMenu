using System;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Entities;
using TelegramBot.Core.Services.Interface;

namespace TelegramBot.Core.Services.Service
{
    internal class UserService : IUserService
    {
        private IUserRepository _repository;

        public UserService(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            var user = _repository.GetUserByTelegramUserId(telegramUserId);
            return user;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserId, telegramUserName);
            _repository.Add(user);
            return user;
        }
    }
}
