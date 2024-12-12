using AutoTrack.Core;
using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Telegram.Function
{
  /// <summary>
  /// Класс для отображения существующих пользователей в базе данных.
  /// </summary>
  internal static class UserSelector
  {
    /// <summary>
    /// Отображает список всех пользователей в базе данных.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата, куда будет отправлен список пользователей.</param>
    public static async Task DisplayUsersAsync(ITelegramBotClient botClient, long chatId, Message message)
    {
      using var context = new ApplicationDbContext();
      var users = context.Users.ToList();

      if (!users.Any())
      {
        await botClient.SendMessage(chatId, "В базе данных нет пользователей.");
        return;
      }

      var messageBuilder = new StringBuilder();
      messageBuilder.AppendLine("Список пользователей:");
      List<CallbackModel> callbackModels = new List<CallbackModel>();

      foreach (var user in users)
      {
        messageBuilder.AppendLine($"ID: {user.Id}, Имя: {user.Name}, Дата создания: {user.Created}");
        callbackModels.Add(new CallbackModel(user.Name, $"/selectUser_id{user.Id}"));
      }

      await TelegramBotHandler.SendMessageAsync(botClient, chatId, messageBuilder.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
    }

    public static async Task HandleSelectUserAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var userIdString = callbackQuery.Data.Replace("/selectUser_id", "");
      if (int.TryParse(userIdString, out int userId))
      {
        var cars = CarService.GetCarsByUserId(userId);
        StringBuilder sb = new StringBuilder();
        List<CallbackModel> callbackModels = new List<CallbackModel>();
        foreach (var car in cars)
        {
          sb.AppendLine($"Брэнд машины: {car.Brand}");
          sb.AppendLine($"Модель: {car.Model}");
          sb.AppendLine($"Номер машины: {car.Number}");
          sb.AppendLine($"VIN машины: {car.Vin}");
          sb.AppendLine($"Пробег: {car.Mileage}");
          sb.AppendLine(string.Empty);

          callbackModels.Add(new CallbackModel(car.Number, $"/selectCar_id{car.Id}"));
        }

        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, sb.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.MessageId);
      }
      else
      {
        LogError("Ошибка парсинга userId");
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор пользователя.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(new CallbackModel("На главную", "/start")), callbackQuery.Message.MessageId);
      }
    }

    public static async Task HandleSelectCarAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var carIdString = callbackQuery.Data.Replace("/selectCar_id", "");
      if (int.TryParse(carIdString, out int carId))
      {
        var works = WorkService.GetWorksByCarId(carId);
        StringBuilder sb = new StringBuilder();
        foreach (var work in works)
        {
          sb.AppendLine($"Работа: {work.Description}");
          sb.AppendLine($"Сумма: {work.Summ}");
          sb.AppendLine($"Дата: {work.Date}");
          sb.AppendLine(string.Empty);
        }

        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, sb.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(new CallbackModel("На главную", "/start")), callbackQuery.Message.MessageId);
      }
      else
      {
        LogError("Ошибка парсинга carId");
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор автомобиля.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(new CallbackModel("На главную", "/start")), callbackQuery.Message.MessageId);
      }
    }
  }
}