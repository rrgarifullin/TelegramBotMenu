using System.Text;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace TelegramBotMenu
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService userService;
        private ToDoUser? user;
        private readonly IToDoService toDoService;

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            string command = update.Message.Text;
            string content = "";
            if (command.StartsWith("/addtask") || command.StartsWith("/removetask") || command.StartsWith("/completetask"))
            {
                var indexOfSpace = update.Message.Text.IndexOf(" ");
                if (indexOfSpace != -1)
                {
                    command = update.Message.Text.Substring(0, indexOfSpace);
                    content = update.Message.Text.Substring(indexOfSpace + 1);
                }
            }

            switch (command)
            {
                case "/start":
                    {
                        ProcessCommandStart(update);
                        botClient.SendMessage(update.Message.Chat, $"Привет, {user.TelegramUserName}");
                        break;
                    }
                case "/help":
                    {
                        ProcessCommandHelp(botClient, update.Message.Chat);
                        break;
                    }
                case "/info":
                    {
                        ProcessCommandInfo(botClient, update.Message.Chat);
                        break;
                    }
                case "/addtask":
                    {
                        if (userService.GetUser(update.Message.From.Id) == null) return;

                        try
                        {
                            toDoService.Add(user, content);
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
                        if (userService.GetUser(update.Message.From.Id) == null) return;

                        var userActiveTasks = toDoService.GetActiveByUserId(user.UserId);
                        PrintTasks(botClient, update.Message.Chat, userActiveTasks);
                        break;
                    }
                case "/removetask":
                    {
                        if (userService.GetUser(update.Message.From.Id) == null) return;

                        if (Guid.TryParse(content, out Guid taskId))
                            toDoService.Delete(taskId);
                        else
                            botClient.SendMessage(update.Message.Chat, "Введен неверный Id задачи");

                        break;
                    }
                case "/completetask":
                    {
                        if (userService.GetUser(update.Message.From.Id) == null) return;

                        if (Guid.TryParse(content, out Guid taskId))
                            toDoService.MarkCompleted(taskId);
                        else
                            botClient.SendMessage(update.Message.Chat, "Введен неверный Id задачи");

                        break;
                    }
                case "/showalltasks":
                    {
                        if (userService.GetUser(update.Message.From.Id) == null) return;

                        var userTasks = toDoService.GetAllByUserId(user.UserId);
                        PrintTasks(botClient, update.Message.Chat, userTasks);

                        break;
                    }
                default:
                    {
                        botClient.SendMessage(update.Message.Chat, "Введена неизвестная команда");
                        break;
                    }
            }

            //if (command == "/start")
            //{
            //    ProcessCommandStart(update);
            //    botClient.SendMessage(update.Message.Chat, $"Привет, {user.TelegramUserName}");
            //}
            //else if (command == "/help")
            //{
            //    ProcessCommandHelp(botClient, update.Message.Chat);
            //}
            //else if (command == "/info")
            //{
            //    ProcessCommandInfo(botClient, update.Message.Chat);
            //}
            //else if (command == "/addtask" && userService.GetUser(update.Message.From.Id) != null)
            //{
            //    try
            //    {
            //        toDoService.Add(user, content);
            //    }
            //    catch (TaskCountLimitException ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //    catch (TaskLengthLimitException ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //    catch (DuplicateTaskException ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //}
            //else if (command == "/showtasks" && userService.GetUser(update.Message.From.Id) != null)
            //{
            //    var userActiveTasks = toDoService.GetActiveByUserId(user.UserId);
            //    PrintActiveTasks(botClient, update.Message.Chat, userActiveTasks);
            //}
            //else if (command == "/removetask" && userService.GetUser(update.Message.From.Id) != null)
            //{
            //    if (Guid.TryParse(content, out Guid taskId))
            //        toDoService.Delete(taskId);
            //    else
            //        botClient.SendMessage(update.Message.Chat, "Введен неверный Id задачи");
            //}
            //else if (command == "/completetask" && userService.GetUser(update.Message.From.Id) != null)
            //{
            //    if (Guid.TryParse(content, out Guid taskId))
            //        toDoService.MarkCompleted(taskId);
            //    else
            //        botClient.SendMessage(update.Message.Chat, "Введен неверный Id задачи");
            //}
            //else if (command == "/showalltasks" && userService.GetUser(update.Message.From.Id) != null)
            //{
            //    var userTasks = toDoService.GetAllByUserId(user.UserId);
            //    PrintAllTasks(botClient, update.Message.Chat, userTasks);
            //}
            //else
            //{
            //    botClient.SendMessage(update.Message.Chat, "Введена неизвестная команда");
            //}
        }

        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            this.userService = userService;
            this.toDoService = toDoService;
        }

        public void PrintTasks(ITelegramBotClient botClient, Chat chat, IReadOnlyList<ToDoItem> tasksList)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tasksList.Count; i++)
            {
                var task = tasksList[i];
                sb.Append($"{i + 1}. {(task.State)} {task.Name} - {task.CreatedAt} - {task.Id}\n");
            }

            botClient.SendMessage(chat, sb.ToString());
        }

        private void ProcessCommandStart(Update update)
        {
            user = userService.GetUser(update.Message.From.Id);
            if (user == null)
                user = userService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
        }

        private void ProcessCommandHelp(ITelegramBotClient botClient, Chat chat)
        {
            string helpDescription = @"Описание команд:
                                       1. /start - начать общение с ботом
                                       2. /info - вывести информацию о программе
                                       3. /addtask - добавить задачу в список дел
                                       4. /showtasks - вывести список активых дел
                                       5. /showalltasks - вывести все задачи
                                       6. /completetask - выполнить задачу
                                       5. /removetask - удалить задачу из списка дел";
            botClient.SendMessage(chat, helpDescription);
        }

        private void ProcessCommandInfo(ITelegramBotClient botClient, Chat chat)
        {
            string text = "Версия программы - v1.3, дата создания - 25.04.2025";
            botClient.SendMessage(chat, text);
        }
    }
}
