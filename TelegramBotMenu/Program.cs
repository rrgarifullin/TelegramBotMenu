namespace TelegramBotMenu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> commandsList = new List<string>() { "/start", "/help", "/info", "/exit", "/addtask", "/showtasks", "/removetask" };
            Console.WriteLine($"Привет! Список доступных команд: {string.Join(", ", commandsList)}");

            CommandsProcessing(commandsList);
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

        static int ReadInt()
        {
            bool isNumber = false;
            int result = 0;

            while (!isNumber)
            {
                var input = ReadString();
                if (int.TryParse(input, out result))
                    isNumber = true;
                else
                    Console.Write("Введено не число, повтори ввод: ");
            }
            return result;
        }

        static void PrintTasksList(List<string> tasks)
        {
            for (int i = 0; i < tasks.Count; i++)
                Console.WriteLine($"{i + 1}. {tasks[i]}");
        }

        static void AddTask(List<string> tasksList)
        {
            Console.WriteLine("Введи описание задачи: ");
            string task = ReadString();
            tasksList.Add(task);
            Console.WriteLine($"Задача \"{task}\" добавлена");
        }

        static void ShowTasks(List<string> tasksList)
        {
            if (tasksList.Count > 0)
                PrintTasksList(tasksList);
            else
                Console.WriteLine("Список задач пуст");
        }

        static void RemoveTask(List<string> tasksList)
        {
            if (tasksList.Count > 0)
            {
                Console.WriteLine("Твой список задач: ");
                PrintTasksList(tasksList);
                Console.Write("Введи номер задачи для удаления: ");
                int taskNumber = ReadInt();
                if (taskNumber > 0 && taskNumber <= tasksList.Count)
                {
                    var task = tasksList[taskNumber - 1];
                    tasksList.RemoveAt(taskNumber - 1);
                    Console.WriteLine($"Задача \"{task}\" удалена");
                }
                else
                    Console.WriteLine("Задачи с таким номером нет");
            }
            else
            {
                Console.WriteLine("Список задач пуст, удаление невозможно");
            }
        }

        static void CommandsProcessing(List<string> commandsList)
        {
            bool isExit = false;
            string? userName = null;

            List<string> toDoList = new List<string>();

            while (!isExit)
            {
                var command = ReadString();
                if (!commandsList.Contains(command))
                {
                    Console.WriteLine($"Введи команду из списка: {string.Join(", ", commandsList)}");
                    continue;
                }

                switch (command)
                {
                    case "/start":
                        {
                            Console.WriteLine("Введи имя: ");
                            userName = ReadString();
                            if (!commandsList.Contains("/echo"))
                                commandsList.Add("/echo");
                            Console.WriteLine($"{userName}, чем могу помочь?");
                            break;
                        }
                    case "/help":
                        {
                            string helpDescription = @"Описание команд:
                                                       1. /info - вывести информацию о программе
                                                       2. /echo - вывести введенный текст
                                                       3. /addtask - добавить задачу в список дел
                                                       4. /showtasks - вывести список дел
                                                       5. /removetask - удалить задачу из списка дел";
                            Console.WriteLine(helpDescription);
                            break;
                        }
                    case "/info":
                        {
                            Console.WriteLine("Версия программы - v1.1, дата создания - 08.03.2025");
                            break;
                        }
                    case "/echo":
                        {
                            if (!string.IsNullOrEmpty(userName))
                            {
                                string echo = ReadString();
                                Console.WriteLine(echo);
                            }
                            break;
                        }
                    case "/addtask":
                        {
                            AddTask(toDoList);
                            break;
                        }
                    case "/showtasks":
                        {
                            ShowTasks(toDoList);
                            break;
                        }
                    case "/removetask":
                        {
                            RemoveTask(toDoList);
                            break;
                        }
                    case "/exit":
                        {
                            isExit = true;
                            break;
                        }
                }    
            }
        }
    }
}
