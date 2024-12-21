using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Collections.Concurrent;
using AutoTrack.Core;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoTrack.TelegramBot.Function.CarData
{
  /// <summary>
  /// Класс для управления процессом добавления нового автомобиля.
  /// </summary>
  static internal class AddCar
  {
    /// <summary>
    /// Перечисление для шагов регистрации автомобиля.
    /// </summary>
    public enum RegistrationStep
    {
      Start,
      EnterModel,
      EnterNumber,
      EnterVin,
      EnterMileage,
      UserInput,
      Complete
    }

    static Car newCar = new Car();
    static RegistrationStep step = RegistrationStep.Start;

    static public async Task RegisterNewCar(ITelegramBotClient botClient, Message message)
    {
      long chatId = message.Chat.Id;
      MessageProcessing.addCar = true;

      switch (step)
      {
        case RegistrationStep.Start:
          await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Начало регистрации нового автомобиля. Введите марку автомобиля:");
          step = RegistrationStep.EnterModel;
          return;
        case RegistrationStep.EnterModel:
          await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите модель автомобиля:");
          newCar.Brand = message.Text;
          step = RegistrationStep.EnterNumber;
          return;
        case RegistrationStep.EnterNumber:
          await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите номер автомобиля:");
          newCar.Model = message.Text;
          step = RegistrationStep.EnterVin;
          return;
        case RegistrationStep.EnterVin:
          await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите VIN автомобиля:");
          newCar.Number = message.Text;
          step = RegistrationStep.EnterMileage;
          return;
        case RegistrationStep.EnterMileage:
          newCar.Vin = message.Text;
          await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите пробег автомобиля:");
          step = RegistrationStep.UserInput;
          return;
        case RegistrationStep.UserInput:
          if (long.TryParse(message.Text, out long mileage))
          {
            await DisplayClientsAsync(botClient, chatId);
            step = RegistrationStep.Complete;
          }
          else
          {
            await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите корректно значение пробега:");
          }
          return;
      }
    }

    static public async Task RegisterNewCar(ITelegramBotClient botClient, CallbackQuery message)
    {
      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };


      var clientIdString = message.Data.Replace("/addCar_clienId", "");
      if (int.TryParse(clientIdString, out int clientId))
      {
        newCar.UserId = clientId;
        CarService.AddCar(newCar);
        await TelegramBotHandler.SendMessageAsync(botClient, message.From.Id, "Добавление нового автомобиля завершено.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
        MessageProcessing.addCar = false;
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, message.From.Id, "Системная ошибка добавления автомобиля.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
      }

      return;
    }

    /// <summary>
    /// Отображает список всех клиентов с кнопками для удаления.
    /// </summary>
    private static async Task DisplayClientsAsync(ITelegramBotClient botClient, long chatId)
    {
      var clients = ClientService.GetAllClients();
      var buttons = clients.Select(client => new InlineKeyboardButton
      {
        Text = client.Name,
        CallbackData = $"/addCar_clienId{client.Id}"
      }).ToList();

      var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(b => new[] { b }));
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Выберите клиента для удаления:", inlineKeyboard);
    }
  }
}
