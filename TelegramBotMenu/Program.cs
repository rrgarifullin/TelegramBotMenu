namespace TelegramBotMenu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var commandsString = "/start, /help, /info, /exit";
            Console.WriteLine($"Привет! Список доступных команд: {commandsString}");

            bool isExit = false;
            string? userName = null;
            while (!isExit)
            {
                var command = ReadString();
                if (!commandsString.Contains(command))
                {
                    string message = $"введи команду из списка: {commandsString}";
                    PrintMessage(userName, message);
                    continue;
                }

                if (command == "/start")
                {
                    string message = "введи имя: ";
                    PrintMessage(userName, message);
                    userName = ReadString();
                    if (!commandsString.Contains("/echo"))
                        commandsString += ", /echo";
                }
                else if (command == "/help")
                {
                    string message = "информация о программе";
                    PrintMessage(userName, message);
                }
                else if (command == "/info")
                {
                    string message = "версия программы - v1.0, дата создания - 02.03.2025";
                    PrintMessage(userName, message);
                }
                else if (!string.IsNullOrEmpty(userName) && command == "/echo")
                {
                    string echo = ReadString();
                    PrintMessage(userName, echo);
                }
                else if (command == "/exit")
                    isExit = true;
            }
        }

        static void PrintMessage(string userName, string message)
        {
            Console.WriteLine($"{(userName != null ? userName + ", " : "")}{message}");
        }

        static string ReadString()
        {   
            bool isCorrectInput = false;
            string? input = null;
            while(!isCorrectInput)
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Введена пустая строка, повтори ввод");
                    continue;
                }
                isCorrectInput = true;
            }
            return input;
        }
    }
}
