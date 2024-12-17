using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AutoTrack.Model;
using AutoTrack.DataBase;
using Microsoft.EntityFrameworkCore;
using static AutoTrack.Config.Logger;
using AutoTrack.Core;
using System.Text;

namespace AutoTrack.TelegramBot.Function
{
  /// <summary>
  /// Класс для отображения свойств сущностей в базе данных.
  /// </summary>
  internal static class EntityPropertyViewer
  {
    private static string _selectedProperty;

    /// <summary>
    /// Отображает кнопки для выбора свойств сущностей.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата, куда будут отправлены кнопки.</param>
    public static async Task DisplayPropertyButtonsAsync(ITelegramBotClient botClient, long chatId)
    {
      var properties = GetProperties(typeof(Car))
          .Concat(GetProperties(typeof(Model.Client)))
          .Distinct()
          .ToList();

      var callbackModels = properties.Select(property => new CallbackModel(property, $"/search_{property}")).ToList();
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Выберите свойство для поиска:", GetInlineKeyboardMarkup(callbackModels));
    }

    /// <summary>
    /// Обрабатывает CallbackQuery и выводит сообщение с выбранным свойством.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="callbackQuery">Данные callback-запроса.</param>
    public static async Task HandlePropertySelectionAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      _selectedProperty = callbackQuery.Data.Replace("/search_", "");
      var chatId = callbackQuery.Message.Chat.Id;
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, $"Вы выбрали свойство: {_selectedProperty}. Введите текст для поиска.", null, callbackQuery.Message.MessageId);
      MessageProcessing.searchData = true;
    }

    /// <summary>
    /// Ищет данные в базе данных по выбранному свойству и введённому тексту.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="searchText">Текст для поиска.</param>
    public static async Task SearchDataAsync(ITelegramBotClient botClient, long chatId, string searchText)
    {
      try
      {
        var tableName = DetermineTable(_selectedProperty);
        LogInfo($"Поиск по свойству {_selectedProperty} будет выполнен в таблице: {tableName}");

        switch (tableName)
        {
          case "User":
            await SearchUsersAsync(botClient, chatId, searchText);
            break;
          case "Car":
            await SearchCarsAsync(botClient, chatId, searchText);
            break;
        }

        MessageProcessing.searchData = false;
      }
      catch (ArgumentException ex)
      {
        LogError(ex.Message);
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Ошибка: свойство не найдено.");
      }
    }

    /// <summary>
    /// Ищет пользователей по заданному тексту.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="searchText">Текст для поиска.</param>
    private static async Task SearchUsersAsync(ITelegramBotClient botClient, long chatId, string searchText)
    {
      var users = ClientService.SearchClient(_selectedProperty, searchText);
      StringBuilder sb = new StringBuilder();
      List<CallbackModel> callbackModels = new List<CallbackModel>();
      foreach (var user in users)
      {
        LogInfo($"Имя клиента: {user.Name}, Дата создания: {user.Created}");
        var cars = CarService.SearchCars("UserId", user.Id.ToString());
        foreach (var car in cars)
        {
          sb.AppendLine($"Имя клиента: {user.Name}");
          sb.AppendLine($"Брэнд машины: {car.Brand}");
          sb.AppendLine($"Модель: {car.Model}");
          sb.AppendLine($"Номер машины: {car.Number}");
          sb.AppendLine($"VIN машины: {car.Vin}");
          sb.AppendLine($"Пробег: {car.Mileage}");
          sb.AppendLine(string.Empty);

          callbackModels.Add(new CallbackModel(car.Number, $"/checkWork_id{car.Id}"));
        }
      }

      await TelegramBotHandler.SendMessageAsync(botClient, chatId, sb.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
    }

    /// <summary>
    /// Ищет автомобили по заданному тексту.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="searchText">Текст для поиска.</param>
    private static async Task SearchCarsAsync(ITelegramBotClient botClient, long chatId, string searchText)
    {
      var carsList = CarService.SearchCars(_selectedProperty, searchText);
      StringBuilder sb = new StringBuilder();
      List<CallbackModel> callbackModels = new List<CallbackModel>();
      foreach (var car in carsList)
      {
        LogInfo($"Брэнд машины: {car.Brand}, Модель: {car.Model}");
        var usersList = ClientService.SearchClient("Id", car.UserId.ToString());
        foreach (var user in usersList)
        {
          sb.AppendLine($"Имя клиента: {user.Name}");
          sb.AppendLine($"Брэнд машины: {car.Brand}");
          sb.AppendLine($"Модель: {car.Model}");
          sb.AppendLine($"Номер машины: {car.Number}");
          sb.AppendLine($"VIN машины: {car.Vin}");
          sb.AppendLine($"Пробег: {car.Mileage}");
          sb.AppendLine(string.Empty);

          callbackModels.Add(new CallbackModel(car.Number, $"/checkWork_id{car.Id}"));
        }
      }

      await TelegramBotHandler.SendMessageAsync(botClient, chatId, sb.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
    }

    /// <summary>
    /// Обрабатывает проверку работы по заданному идентификатору и отправляет результаты в чат.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="callbackQuery">Данные callback-запроса, содержащие идентификатор работы.</param>
    public static async Task HandleCheckWorkAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var carIdString = callbackQuery.Data.Replace("/checkWork_id", "");
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

        var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
            new CallbackModel("Добавить работу", $"/addWork_{carIdString}"),
        };

        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, sb.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.MessageId);
      }
      else
      {
        LogError("Ошибка парсинга carId");
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор автомобиля.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(new CallbackModel("На главную", "/start")), callbackQuery.Message.MessageId);
      }
    }

    /// <summary>
    /// Определяет, в какой таблице искать по выбранному свойству.
    /// </summary>
    /// <param name="property">Выбранное свойство.</param>
    /// <returns>Название таблицы для поиска.</returns>
    private static string DetermineTable(string property)
    {
      if (HasProperty(typeof(Model.Client), property))
      {
        return "User";
      }
      else if (HasProperty(typeof(Car), property))
      {
        return "Car";
      }
      else
      {
        throw new ArgumentException("Свойство не найдено в известных таблицах.");
      }
    }

    /// <summary>
    /// Проверяет, содержит ли тип указанное свойство.
    /// </summary>
    /// <param name="type">Тип для проверки.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>True, если свойство существует; иначе False.</returns>
    private static bool HasProperty(Type type, string propertyName)
    {
      return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
    }

    /// <summary>
    /// Извлекает имена свойств из указанного типа, исключая те, которые содержат "Id".
    /// </summary>
    /// <param name="type">Тип, из которого извлекаются свойства.</param>
    /// <returns>Список имён свойств.</returns>
    private static IEnumerable<string> GetProperties(Type type)
    {
      return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(prop => !prop.Name.Contains("Id", StringComparison.OrdinalIgnoreCase))
                 .Select(prop => prop.Name);
    }

    /// <summary>
    /// Создаёт клавиатуру с кнопками из списка моделей обратного вызова.
    /// </summary>
    /// <param name="callbackModels">Список моделей обратного вызова.</param>
    /// <returns>Встроенная клавиатура с кнопками.</returns>
    private static InlineKeyboardMarkup GetInlineKeyboardMarkup(List<CallbackModel> callbackModels)
    {
      var buttons = callbackModels.Select(model => new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(model.Name, model.Command) }).ToList();
      return new InlineKeyboardMarkup(buttons);
    }
  }
}