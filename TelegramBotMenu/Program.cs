using Otus.ToDoList.ConsoleBot;
using TelegramBot.Bot;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.Services.Interface;
using TelegramBot.Core.Services.Service;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IUserRepository userRepository = new InMemoryUserRepository();
                IUserService userService = new UserService(userRepository);

                IToDoRepository toDoRepository = new InMemoryToDoRepository();
                IToDoService toDoService = new ToDoService(toDoRepository);

                IToDoReportService reportService = new ToDoReportService(toDoRepository);

                var handler = new UpdateHandler(userService, toDoService, reportService);
                var botClient = new ConsoleBotClient();
                botClient.StartReceiving(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла непредвиденная ошибка: ");
                Console.WriteLine(ex.Message, ex.StackTrace, ex.InnerException);
            }
        }     
    }
}
