using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using AutoTrack.Config;
using AutoTrack.TelegramBot.Function;
using AutoTrack.TelegramBot.Function.Admin;

namespace AutoTrack.TelegramBot
{
  internal class CallbackProcessing
  {
    /// <summary>
    /// Обрабатывает входящие callback-запросы от пользователей.
    /// </summary>
    /// <param name="callbackQuery">Данные callback-запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    static internal async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
      var chatId = callbackQuery.From.Id;
      var handlers = new Dictionary<string, Func<ITelegramBotClient, CallbackQuery, Task>>
        {
          { "/start", async (client, query) => await TelegramBotHandler.StartMessageAsync(client, query.Message.Chat.Id, query.Message.MessageId) },

          { "accept_change", ChangeAdmin.HandleCallbackQueryAsync },
          { "cancel_change", ChangeAdmin.HandleCallbackQueryAsync },

          { "accept_addUser", AddUser.HandleCallbackQueryAsync },
          { "cancel_addUser", AddUser.HandleCallbackQueryAsync },

          { "confirm_addClient", AddClient.HandleConfirmationAsync },
          { "cancel_addClient", AddClient.HandleConfirmationAsync },

          { "/viewLog", LogViewer.SendErrorLogsAsync },
          { "/selectCar_", UserSelector.HandleSelectCarAsync },
          { "/selectUser_", UserSelector.HandleSelectUserAsync },
          { "/search_", EntityPropertyViewer.HandlePropertySelectionAsync },
          { "/checkWork_", EntityPropertyViewer.HandleCheckWorkAsync },
          {
            "/addWork_", async (client, query) =>
            {
              MessageProcessing.addWork = true;
              await AddWork.ProcessAddWorkAsync(client, query);
            }
          },
          { "/deleteClient", DeleteClient.RequestDeleteConfirmationAsync},
          { "/confirmDeleteClient_id", DeleteClient.HandleDeleteConfirmationAsync},
          { "/cancelDeleteClient", DeleteClient.HandleDeleteConfirmationAsync},
        };

      foreach (var handler in handlers)
      {
        if (callbackQuery.Data.StartsWith(handler.Key))
        {
          await handler.Value(botClient, callbackQuery);
          return;
        }
      }

      await HandleOtherCallbacksAsync(botClient, callbackQuery, cancellationToken);
    }

    private static async Task HandleOtherCallbacksAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
      await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Обработка других действий.");
    }
  }
}
