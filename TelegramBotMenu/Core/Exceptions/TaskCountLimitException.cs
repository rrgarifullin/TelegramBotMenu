using System;

namespace TelegramBot.Core.Exceptions
{
    internal class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit)
            : base($"Превышено максимальное количество задач, равное {taskCountLimit}") { }
    }
}
