using Otus.ToDoList.ConsoleBot;
using TelegramBot.Bot;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Services.Interface;
using TelegramBot.Core.Services.Service;
using TelegramBot.Infrastructure.DataAccess;
using System.Threading;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IUserRepository userRepository = new InMemoryUserRepository();
            IUserService userService = new UserService(userRepository);

            IToDoRepository toDoRepository = new InMemoryToDoRepository();
            IToDoService toDoService = new ToDoService(toDoRepository);

            IToDoReportService reportService = new ToDoReportService(toDoRepository);

            var handler = new UpdateHandler(userService, toDoService, reportService);
            var botClient = new ConsoleBotClient();

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            try
            {
                handler.OnHandleUpdateStarted += HandleUpdateStarted;
                handler.OnHandleUpdateCompleted += HandleUpdateCompleted;

                botClient.StartReceiving(handler, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла непредвиденная ошибка: ");
                Console.WriteLine(ex.Message, ex.StackTrace, ex.InnerException);
            }
            finally
            {
                handler.OnHandleUpdateCompleted -= HandleUpdateCompleted;
                handler.OnHandleUpdateStarted -= HandleUpdateStarted;
            }
        }

        private static void HandleUpdateStarted(string message)
        {
            Console.WriteLine($"Началась обработка сообщения {message}");
        }

        private static void HandleUpdateCompleted(string message)
        {
            Console.WriteLine($"Закончилась обработка сообщения {message}");
        }
    }
}
