using System;

namespace TelegramBot.Core.Exceptions
{
    internal class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task)
            : base($"Задача {task} уже существует") { }
    }
}
