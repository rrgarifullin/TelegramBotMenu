using System;

namespace TelegramBotMenu
{
    internal class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task) 
            : base($"Задача {task} уже существует") { }
    }
}
