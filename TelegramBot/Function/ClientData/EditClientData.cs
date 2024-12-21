using AutoTrack.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Microsoft.Data.SqlClient.DataClassification;
using Telegram.Bot.Types;
using AutoTrack.Model;

namespace AutoTrack.TelegramBot.Function.ClientData
{
  static internal class EditClientData
  {

    private static int clientDataId;

    /// <summary>
    /// Отображает список всех клиентов с кнопками для удаления.
    /// </summary>
    public static async Task DisplayClientsAsync(ITelegramBotClient botClient, long chatId)
    {
      var clients = ClientService.GetAllClients();
      var buttons = clients.Select(client => new InlineKeyboardButton
      {
        Text = client.Name,
        CallbackData = $"/editClient_id{client.Id}"
      }).ToList();

      var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(b => new[] { b }));
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Выберите клиента для изменения данных:", inlineKeyboard);
    }

    /// <summary>
    /// Обрабатывает выбор клиента для редактирования имени и запрашивает новое имя.
    /// </summary>
    public static async Task RequestNewClientNameAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var clientIdString = callbackQuery.Data.Replace("/editClient_id", "");
      if (int.TryParse(clientIdString, out int clientId))
      {
        var client = ClientService.GetClientById(clientId);
        if (client != null)
        {
          clientDataId = client.Id;
          MessageProcessing.editClient = true;
          await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"Введите новое имя для клиента {client.Name}:", null, callbackQuery.Message.Id);
        }
        else
        {
          await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: клиент не найден.", null, callbackQuery.Message.Id);
        }
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор клиента.", null, callbackQuery.Message.Id);
      }
    }

    /// <summary>
    /// Обрабатывает новое имя клиента и подтверждает изменение.
    /// </summary>
    public static async Task HandleNewClientNameAsync(ITelegramBotClient botClient, Message message)
    {
      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };

      var client = ClientService.GetClientById(clientDataId);
      if (client != null)
      {
        // Проверка уникальности имени
        if (ClientService.ValidateClientName(message.Text))
        {
          ClientService.UpdateClientName(clientDataId, message.Text);
          await TelegramBotHandler.SendMessageAsync(botClient, message.From.Id, $"Имя клиента успешно обновлено на {message.Text}.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
          MessageProcessing.editClient = false;
        }
        else
        {
          await TelegramBotHandler.SendMessageAsync(botClient, message.From.Id, "Ошибка: имя клиента уже существует. Пожалуйста, выберите другое имя.");
        }
      }
      else
      {
        await TelegramBotHandler.SendMessageAsync(botClient, message.From.Id, "Ошибка: клиент не найден.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
        MessageProcessing.editClient = false;
      }
    }
  }
}
