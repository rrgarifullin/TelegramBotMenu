using System;

namespace TelegramBot.Core.Exceptions
{
    internal class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
            : base($"Длина задачи {taskLength} превышает максимально допустимое значение {taskLengthLimit}") { }
    }
}
