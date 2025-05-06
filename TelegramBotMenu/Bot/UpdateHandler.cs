using System;
using System.Text;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using TelegramBot.Core.Entities;
using TelegramBot.Core.Exceptions;
using TelegramBot.Core.Services.Interface;

namespace TelegramBot.Bot
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _reportService;

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            var chat = update.Message.Chat;

            string command = update.Message.Text;
            string content = "";
            if (command.StartsWith("/addtask") || command.StartsWith("/removetask")
                || command.StartsWith("/completetask") || command.StartsWith("/find"))
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
                        var user = ProcessCommandStart(update);
                        botClient.SendMessage(chat, $"Привет, {user.TelegramUserName}");
                        break;
                    }
                case "/help":
                    {
                        ProcessCommandHelp(botClient, chat);
                        break;
                    }
                case "/info":
                    {
                        ProcessCommandInfo(botClient, chat);
                        break;
                    }
                case "/addtask":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        try
                        {
                            _toDoService.Add(user, content);
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
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        var userActiveTasks = _toDoService.GetActiveByUserId(user.UserId);
                        PrintTasks(botClient, chat, userActiveTasks);
                        break;
                    }
                case "/removetask":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        if (Guid.TryParse(content, out Guid taskId))
                            _toDoService.Delete(taskId);
                        else
                            botClient.SendMessage(chat, "Введен неверный Id задачи");

                        break;
                    }
                case "/completetask":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        if (Guid.TryParse(content, out Guid taskId))
                            _toDoService.MarkCompleted(taskId);
                        else
                            botClient.SendMessage(chat, "Введен неверный Id задачи");

                        break;
                    }
                case "/showalltasks":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        var userTasks = _toDoService.GetAllByUserId(user.UserId);
                        PrintTasks(botClient, chat, userTasks);

                        break;
                    }
                case "/report":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        (int total, int completed, int active, DateTime generatedAt) = _reportService.GetUserStats(user.UserId);
                        var report = $"Статистика по задачам на {generatedAt}. Всего: {total}; Завершенных: {completed}; Активных: {active};";
                        botClient.SendMessage(chat, report);
                        break;
                    }
                case "/find":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start");
                            return;
                        }

                        var filteredTasks = _toDoService.Find(user, content);

                        if (filteredTasks.Count == 0)
                            botClient.SendMessage(chat, "Задачи не найдены");
                        else
                            PrintTasks(botClient, chat, filteredTasks);

                        break;
                    }
                default:
                    {
                        botClient.SendMessage(chat, "Введена неизвестная команда");
                        break;
                    }
            }
        }

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _reportService = toDoReportService;
        }

        public void PrintTasks(ITelegramBotClient botClient, Chat chat, IReadOnlyList<ToDoItem> tasksList)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tasksList.Count; i++)
            {
                var task = tasksList[i];
                sb.Append($"{i + 1}. ({task.State}) {task.Name} - {task.CreatedAt} - {task.Id}\n");
            }

            botClient.SendMessage(chat, sb.ToString());
        }

        private ToDoUser ProcessCommandStart(Update update)
        {
            var user = _userService.GetUser(update.Message.From.Id);
            if (user == null)
                user = _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
            return user;
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
                                       7. /removetask - удалить задачу из списка дел
                                       8. /report - вывести статистику по задачам
                                       9. /find - найти задачу по началу наименования";
            botClient.SendMessage(chat, helpDescription);
        }

        private void ProcessCommandInfo(ITelegramBotClient botClient, Chat chat)
        {
            string text = "Версия программы - v1.4, дата создания - 03.05.2025";
            botClient.SendMessage(chat, text);
        }
    }
}
