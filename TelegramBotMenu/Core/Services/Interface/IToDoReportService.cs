using System;

namespace TelegramBot.Core.Services.Interface
{
    internal interface IToDoReportService
    {
        (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId);
    }
}
