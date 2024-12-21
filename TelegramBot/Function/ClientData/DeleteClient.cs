using AutoTrack.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using AutoTrack.Model;

namespace AutoTrack.TelegramBot.Function.ClientData
{
  internal class DeleteClient
  {
    /// <summary>
    /// Отображает список всех клиентов с кнопками для удаления.
    /// </summary>
    public static async Task DisplayClientsAsync(ITelegramBotClient botClient, long chatId)
    {
      var clients = ClientService.GetAllClients();
      var buttons = clients.Select(client => new InlineKeyboardButton
      {
        Text = client.Name,
        CallbackData = $"/deleteClient_id{client.Id}"
      }).ToList();

      var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(b => new[] { b }));
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Выберите клиента для удаления:", inlineKeyboard);
    }

    /// <summary>
    /// Обрабатывает выбор клиента для удаления и запрашивает подтверждение.
    /// </summary>
    public static async Task RequestDeleteConfirmationAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var clientIdString = callbackQuery.Data.Replace("/deleteClient_id", "");
      if (int.TryParse(clientIdString, out int clientId))
      {
        var client = ClientService.GetClientById(clientId);
        if (client != null)
        {
          var confirmationButtons = new List<InlineKeyboardButton>
          {
              InlineKeyboardButton.WithCallbackData("Подтвердить", $"/confirmDeleteClient_id{clientId}"),
              InlineKeyboardButton.WithCallbackData("Отменить", "/cancelDeleteClient")
          };

          var inlineKeyboard = new InlineKeyboardMarkup(confirmationButtons.Select(b => new[] { b }));
          await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"Вы уверены, что хотите удалить клиента {client.Name}?", inlineKeyboard, callbackQuery.Message.Id);
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
    /// Обрабатывает подтверждение или отмену удаления клиента.
    /// </summary>
    public static async Task HandleDeleteConfirmationAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };

      if (callbackQuery.Data.StartsWith("/confirmDeleteClient_id"))
      {
        var clientIdString = callbackQuery.Data.Replace("/confirmDeleteClient_id", "");
        if (int.TryParse(clientIdString, out int clientId))
        {
          var cars = CarService.GetCarsByUserId(clientId);
          var carIds = cars.Select(c => c.Id).ToList();

          WorkService.DeleteWorksByCarIds(carIds);
          CarService.DeleteCarsByClientId(clientId);
          ClientService.DeleteClient(clientId);

          await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"Клиент и все связанные данные успешно удалены.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
        }
        else
        {
          await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Ошибка: неверный идентификатор клиента.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
        }
      }
      else if (callbackQuery.Data == "/cancelDeleteClient")
      {
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Удаление клиента отменено.", TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels), callbackQuery.Message.Id);
      }
    }
  }
}
