using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using AutoTrack.Core;

namespace AutoTrack.TelegramBot.Function
{
  static internal class AddClient
  {

    private static string _clientName;

    /// <summary>
    /// Обрабатывает ввод имени клиента.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="message">Сообщение пользователя.</param>
    public static async Task HandleClientNameInputAsync(ITelegramBotClient botClient, Message message)
    {
      _clientName = message.Text;
      if (!ClientService.ValidateClientName(_clientName))
      {
        await TelegramBotHandler.SendMessageAsync(botClient, message.Chat.Id, "Имя клиента уже есть в списке.");
        MessageProcessing.addClient = false;
        return;
      }

      var confirmationMessage = $"Вы ввели имя: {_clientName}. Подтвердите добавление клиента.";
      var callbackData = new List<CallbackModel>
            {
                new CallbackModel("Подтвердить", "confirm_addClient"),
                new CallbackModel("Отменить", "cancel_addClient")
            };

      var inlineKeyboard = TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackData);
      await TelegramBotHandler.SendMessageAsync(botClient, message.Chat.Id, confirmationMessage, inlineKeyboard);
    }

    /// <summary>
    /// Обрабатывает подтверждение или отмену добавления клиента.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="callbackQuery">Данные callback-запроса.</param>
    public static async Task HandleConfirmationAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };
      MessageProcessing.addClient = false;

      if (callbackQuery.Data == "confirm_addClient")
      {

        var client = new Client
        {
          Name = _clientName,
          Created = DateTime.Now
        };

        ClientService.AddClient(client);

        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Клиент успешно добавлен.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
      }
      else if (callbackQuery.Data == "cancel_addClient")
      {
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Добавление клиента отменено.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
      }
    }
  }
}
