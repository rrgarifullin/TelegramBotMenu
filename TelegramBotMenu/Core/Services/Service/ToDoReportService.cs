using System;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Services.Interface;

namespace TelegramBot.Core.Services.Service
{
    internal class ToDoReportService : IToDoReportService
    {
        private IToDoRepository _repository;

        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            var allTasks = _repository.GetAllByUserId(userId);
            int total = allTasks.Count;
            int active = _repository.CountActive(userId);
            int completed = total - active;
            DateTime generatedAt = DateTime.Now;

            return (total, completed, active, generatedAt);
        }

        public ToDoReportService(IToDoRepository toDoRepository)
        {
            _repository = toDoRepository;
        }
    }
}
