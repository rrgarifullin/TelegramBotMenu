using System.Data;

namespace TelegramBotMenu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int maxNumberOfTasks = 0;
            int maxTaskLength = 0;
            try
            {
                Console.WriteLine("Введи максимально допустимое количество задач: ");
                maxNumberOfTasks = ParseAndValidateInt(1, 100);

                Console.WriteLine("Введи максимально допустимую длину задачи");
                maxTaskLength = ParseAndValidateInt(1, 100);

                List<string> commandsList = new List<string>() { "/start", "/help", "/info", "/exit", "/addtask", "/showtasks", "/removetask" };
                Console.WriteLine($"Привет! Список доступных команд: {string.Join(", ", commandsList)}");

                CommandsProcessing(commandsList, maxNumberOfTasks, maxTaskLength);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла непредвиденная ошибка: ");
                Console.WriteLine(ex.Message, ex.StackTrace, ex.InnerException);
            }
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

        static int ParseAndValidateInt(int min, int max)
        {
            int inputInt = ReadInt();
            if (inputInt < min || inputInt > max)
                throw new ArgumentException($"Число должно быть в диапазоне от {min} до {max}");
            return inputInt;
        }

        static void PrintTasksList(List<string> tasks)
        {
            for (int i = 0; i < tasks.Count; i++)
                Console.WriteLine($"{i + 1}. {tasks[i]}");
        }

        static void AddTask(List<string> tasksList, int taskCountLimit, int taskLengthLimit)
        {
            if (tasksList.Count >= taskCountLimit)
                throw new TaskCountLimitException(taskCountLimit);

            Console.WriteLine("Введи описание задачи: ");
            string task = ReadString();

            if (task.Length > taskLengthLimit)
                throw new TaskLengthLimitException(task.Length, taskLengthLimit);
            if (tasksList.Contains(task))
                throw new DuplicateTaskException(task);

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

        static string ProcessCommandStart(List<string> commandsList)
        {
            string userName;
            Console.WriteLine("Введи имя: ");
            userName = ReadString();
            if (!commandsList.Contains("/echo"))
                commandsList.Add("/echo");
            Console.WriteLine($"{userName}, чем могу помочь?");

            return userName;
        }

        static void ProcessCommandHelp()
        {
            string helpDescription = @"Описание команд:
                                                       1. /info - вывести информацию о программе
                                                       2. /echo - вывести введенный текст
                                                       3. /addtask - добавить задачу в список дел
                                                       4. /showtasks - вывести список дел
                                                       5. /removetask - удалить задачу из списка дел";
            Console.WriteLine(helpDescription);
        }

        static void ProcessCommandInfo()
        {
            Console.WriteLine("Версия программы - v1.2, дата создания - 16.03.2025");
        }

        static void ProcessCommandEcho(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                string echo = ReadString();
                Console.WriteLine(echo);
            }
        }

        static void CommandsProcessing(List<string> commandsList, int maxNumberOfTasks, int maxTaskLength)
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
                            userName = ProcessCommandStart(toDoList);
                            break;
                        }
                    case "/help":
                        {
                            ProcessCommandHelp();
                            break;
                        }
                    case "/info":
                        {
                            ProcessCommandInfo();
                            break;
                        }
                    case "/echo":
                        {
                            ProcessCommandEcho(userName);
                            break;
                        }
                    case "/addtask":
                        {
                            try
                            {
                                AddTask(toDoList, maxNumberOfTasks, maxTaskLength);
                            }
                            catch (TaskCountLimitException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (TaskLengthLimitException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (DuplicateTaskException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch
                            {
                                throw;
                            }
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
