using AutoTrack.Core;
using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Runtime.ConstrainedExecution;
using System.Collections.Concurrent;

namespace AutoTrack.TelegramBot.Function.CarData
{
  /// <summary>
  /// Класс для изменений данных автомобилей.
  /// </summary>
  internal class EditCar
  {
    static int clientId;
    static int carId;
    private static readonly ConcurrentDictionary<long, string> _editingProperties = new();

    /// <summary>
    /// Отображает список всех клиентов с кнопками для выбора.
    /// </summary>
    public static async Task DisplayClientsAsync(ITelegramBotClient botClient, long chatId)
    {
      var clients = ClientService.GetAllClients();
      var buttons = clients.Select(client => new InlineKeyboardButton
      {
        Text = client.Name,
        CallbackData = $"/editCar_Client_id{client.Id}"
      }).ToList();

      var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(b => new[] { b }));
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Выберите клиента для отображения автомобилей:", inlineKeyboard);
    }

    /// <summary>
    /// Отображает список всех аавтомобилей с кнопками для выбора.
    /// </summary>
    public static async Task DisplayCarAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var clientIdString = callbackQuery.Data.Replace("/editCar_Client_id", "");
      if (int.TryParse(clientIdString, out clientId))
      {
        var cars = CarService.GetCarsByUserId(clientId).ToList();
        var buttons = cars.Select(car => new InlineKeyboardButton
        {
          Text = car.Number,
          CallbackData = $"/editCar_id{car.Id}"
        }).ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Выберите автомобиль для редактирования:");
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
    /// Отображает список всех аавтомобилей с кнопками для выбора.
    /// </summary>
    public static async Task DisplayPropertyCarAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var carIdString = callbackQuery.Data.Replace("/editCar_id", "");
      if (int.TryParse(carIdString, out carId))
      {
        var car = CarService.GetCarsByCarId(carId);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Выберите парметр для редактирования:");

        stringBuilder.AppendLine($"Марка: {car.Brand}");
        stringBuilder.AppendLine($"Модель: {car.Model}");
        stringBuilder.AppendLine($"Номер: {car.Number}");
        stringBuilder.AppendLine($"VIN: {car.Vin}");
        stringBuilder.AppendLine($"Пробег: {car.Mileage} км");

        List<CallbackModel> list = new List<CallbackModel>();
        list.Add(new CallbackModel(car.Brand, "/editCarProperty_Brand"));
        list.Add(new CallbackModel(car.Model, "/editCarProperty_Model"));
        list.Add(new CallbackModel(car.Number, "/editCarProperty_Number"));
        list.Add(new CallbackModel(car.Vin, "/editCarProperty_Vin"));
        list.Add(new CallbackModel(car.Mileage.ToString(), "/editCarProperty_Mileage"));

        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, stringBuilder.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(list), callbackQuery.Message.Id);
        MessageProcessing.editCar = true;
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор клиента.", null, callbackQuery.Message.Id);
      }
    }

    public static async Task HandlePropertySelectionAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var property = callbackQuery.Data.Replace("/editCarProperty_", "");
      _editingProperties[callbackQuery.Message.Chat.Id] = property;

      await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"Введите новое значение для {property}:");
    }

    public static async Task UpdateCarPropertyAsync(ITelegramBotClient botClient, Message message)
    {
      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };

      if (_editingProperties.TryGetValue(message.Chat.Id, out var property))
      {
        var newValue = message.Text;

        // Обновление в базе данных
        CarService.UpdateCarProperty(carId, property, newValue);

        await TelegramBotHandler.SendMessageAsync(botClient, message.Chat.Id, $"{property} успешно обновлено.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
        _editingProperties.TryRemove(message.Chat.Id, out _);
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, message.Chat.Id, "Ошибка: не выбрано свойство для редактирования.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
      }
      MessageProcessing.editCar = false;
    }

  }
}
