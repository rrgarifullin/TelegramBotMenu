using System;
using System.Text;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using TelegramBot.Core.Entities;
using TelegramBot.Core.Exceptions;
using TelegramBot.Core.Services.Interface;

namespace TelegramBot.Bot
{
    public delegate void MessageEventHandler(string message);

    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _reportService;

        public event MessageEventHandler? OnHandleUpdateStarted;
        public event MessageEventHandler? OnHandleUpdateCompleted;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            var chat = update.Message.Chat;
            string command = update.Message.Text;
            string content = "";

            OnHandleUpdateStarted?.Invoke(command);

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
                        await botClient.SendMessage(chat, $"Привет, {user.TelegramUserName}", ct);
                        break;
                    }
                case "/help":
                    {
                        await ProcessCommandHelp(botClient, chat, ct);
                        break;
                    }
                case "/info":
                    {
                        await ProcessCommandInfo(botClient, chat, ct);
                        break;
                    }
                case "/addtask":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        try
                        {
                            _toDoService.Add(user, content);
                        }
                        catch (TaskCountLimitException ex)
                        {
                            await HandleErrorAsync(botClient, ex, ct);
                        }
                        catch (TaskLengthLimitException ex)
                        {
                            await HandleErrorAsync(botClient, ex, ct);
                        }
                        catch (DuplicateTaskException ex)
                        {
                            await HandleErrorAsync(botClient, ex, ct);
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
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        var userActiveTasks = _toDoService.GetActiveByUserId(user.UserId);
                        await PrintTasks(botClient, chat, userActiveTasks, ct);
                        break;
                    }
                case "/removetask":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        if (Guid.TryParse(content, out Guid taskId))
                            _toDoService.Delete(taskId);
                        else
                            await botClient.SendMessage(chat, "Введен неверный Id задачи", ct);

                        break;
                    }
                case "/completetask":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        if (Guid.TryParse(content, out Guid taskId))
                            _toDoService.MarkCompleted(taskId);
                        else
                            await botClient.SendMessage(chat, "Введен неверный Id задачи", ct);

                        break;
                    }
                case "/showalltasks":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        var userTasks = _toDoService.GetAllByUserId(user.UserId);
                        await PrintTasks(botClient, chat, userTasks, ct);

                        break;
                    }
                case "/report":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        (int total, int completed, int active, DateTime generatedAt) = _reportService.GetUserStats(user.UserId);
                        var report = $"Статистика по задачам на {generatedAt}. Всего: {total}; Завершенных: {completed}; Активных: {active};";
                        await botClient.SendMessage(chat, report, ct);
                        break;
                    }
                case "/find":
                    {
                        var user = _userService.GetUser(update.Message.From.Id);
                        if (user == null)
                        {
                            await botClient.SendMessage(chat, "Пользователь не зарегистрирован, необходимо вызвать команду /start", ct);
                            return;
                        }

                        var filteredTasks = _toDoService.Find(user, content);

                        if (filteredTasks.Count == 0)
                            await botClient.SendMessage(chat, "Задачи не найдены", ct);
                        else
                            await PrintTasks(botClient, chat, filteredTasks, ct);

                        break;
                    }
                default:
                    {
                        await botClient.SendMessage(chat, "Введена неизвестная команда", ct);
                        break;
                    }
            }

            OnHandleUpdateCompleted?.Invoke(update.Message.Text);
        }

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _reportService = toDoReportService;
        }

        private async Task PrintTasks(ITelegramBotClient botClient, Chat chat, IReadOnlyList<ToDoItem> tasksList, CancellationToken cancellationToken)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tasksList.Count; i++)
            {
                var task = tasksList[i];
                sb.Append($"{i + 1}. ({task.State}) {task.Name} - {task.CreatedAt} - {task.Id}\n");
            }

            await botClient.SendMessage(chat, sb.ToString(), cancellationToken);
        }

        private ToDoUser ProcessCommandStart(Update update)
        {
            var user = _userService.GetUser(update.Message.From.Id);
            if (user == null)
                user = _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
            return user;
        }

        private async Task ProcessCommandHelp(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
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
            await botClient.SendMessage(chat, helpDescription, cancellationToken);
        }

        private async Task ProcessCommandInfo(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            string text = "Версия программы - v1.4, дата создания - 03.05.2025";
            await botClient.SendMessage(chat, text, cancellationToken);
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await Task.Run(() => Console.WriteLine(exception.Message));
        }
    }
}
