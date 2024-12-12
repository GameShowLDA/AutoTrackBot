namespace AutoTrack
{
  using AutoTrack.Config;
  using AutoTrack.DataBase;
  using AutoTrack.Telegram;
  using AutoTrack.Tests;
  using AutoTrack.Utils;
  using static Config.Logger;

  internal class Program
  {
    static async Task Main(string[] args)
    {
      LogInfo("Отладка: Программа запущена");

      try
      {
        var config = ApplicationData.ConfigApp;
        LogInfo($"Строка подключения к базе данных: {config.DataPath}");
        LogInfo($"Токен бота: {config.BotToken}");
        LogInfo($"ID администратора: {config.AdminId}");

        using var dbContext = new ApplicationDbContext();
        ApplicationData.DbContext = dbContext;

        // dbContext.DeleteDatabase();
        dbContext.CheckDatabaseAndTables();

        // var seeder = new DataSeeder(dbContext);
        // seeder.SeedData();
        // LogInfo("База данных заполнена тестовыми данными.");

        var printer = new DataPrinter(dbContext);
        printer.PrintAllTables();

        var botHandler = new TelegramBotHandler(config.BotToken);
        await botHandler.StartBotAsync();
        await Task.Delay(-1);
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }
  }
}
