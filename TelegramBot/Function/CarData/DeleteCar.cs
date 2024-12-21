using AutoTrack.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using AutoTrack.Model;

namespace AutoTrack.TelegramBot.Function.CarData
{
  /// <summary>
  /// Класс для управления процессом удаления автомобиля.
  /// </summary>
  internal class DeleteCar
  {
    static int clientId;

    /// <summary>
    /// Отображает список всех клиентов с кнопками для выбора.
    /// </summary>
    public static async Task DisplayClientsAsync(ITelegramBotClient botClient, long chatId)
    {
      var clients = ClientService.GetAllClients();
      var buttons = clients.Select(client => new InlineKeyboardButton
      {
        Text = client.Name,
        CallbackData = $"/deleteCar_Client_id{client.Id}"
      }).ToList();

      var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(b => new[] { b }));
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Выберите клиента для отображения автомобилей:", inlineKeyboard);
    }

    /// <summary>
    /// Отображает список всех аавтомобилей с кнопками для выбора.
    /// </summary>
    public static async Task DisplayCarAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var clientIdString = callbackQuery.Data.Replace("/deleteCar_Client_id", "");
      if (int.TryParse(clientIdString, out clientId))
      {
        var cars = CarService.GetCarsByUserId(clientId).ToList();
        var buttons = cars.Select(car => new InlineKeyboardButton
        {
          Text = car.Number,
          CallbackData = $"/deleteCar_id{car.Id}"
        }).ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Выберите автомобиль для удаления:");
        foreach (var item in cars)
        {
          sb.AppendLine($"Марка: {item.Brand}");
          sb.AppendLine($"Модель: {item.Model}");
          sb.AppendLine($"Номер: {item.Number}");
          sb.AppendLine($"VIN: {item.Vin}");
          sb.AppendLine($"{string.Empty}");
        }

        var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(b => new[] { b }));
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, sb.ToString(), inlineKeyboard);
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор клиента.", null, callbackQuery.Message.Id);
      }
    }

    /// <summary>
    /// Обрабатывает подтверждение или отмену удаления клиента.
    /// </summary>
    public static async Task HandleDeleteConfirmationAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };

      var clientIdString = callbackQuery.Data.Replace("/deleteCar_id", "");
      if (int.TryParse(clientIdString, out int carId))
      {
        CarService.DeleteCarsByClientId(carId);
        CarService.DeleteCarByCarId(carId);

        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"Автомобиль успешено удален.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор автомобиля.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
      }

    }
  }
}
