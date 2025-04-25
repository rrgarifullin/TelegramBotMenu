using Otus.ToDoList.ConsoleBot;
using System.Data;

namespace TelegramBotMenu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IUserService userService = new UserService();
                IToDoService toDoService = new ToDoService(); 
                var handler = new UpdateHandler(userService, toDoService);
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
