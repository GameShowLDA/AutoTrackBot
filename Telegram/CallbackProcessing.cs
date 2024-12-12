using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using AutoTrack.Config;
using AutoTrack.Telegram.Function;

namespace AutoTrack.Telegram
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
      if (chatId != ApplicationData.ConfigApp.AdminId)
      {
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "У вас нет доступа к управлению данными");
      }
      else
      {
        if (callbackQuery.Data == "/start")
        {
          await TelegramBotHandler.StartMessageAsync(botClient, chatId, callbackQuery.Message.MessageId);
        }
        else if (callbackQuery.Data == "accept_change" || callbackQuery.Data == "cancel_change")
        {
          await ChangeAdmin.HandleCallbackQueryAsync(botClient, callbackQuery);
        }
        else if (callbackQuery.Data.Contains("/selectCar_"))
        { 
          await UserSelector.HandleSelectCarAsync(botClient, callbackQuery);
        }
        else if (callbackQuery.Data.Contains("/selectUser_"))
        {
          await UserSelector.HandleSelectUserAsync(botClient, callbackQuery);
        }
        else if (callbackQuery.Data.Contains("/search_"))
        {
          await EntityPropertyViewer.HandlePropertySelectionAsync(botClient, callbackQuery);
        }
        else if (callbackQuery.Data.Contains("/checkWork_"))
        {
          await EntityPropertyViewer.HandleCheckWorkAsync(botClient, callbackQuery);
        }
        else
        {
          await HandleOtherCallbacksAsync(botClient, callbackQuery, cancellationToken);
        }
      }
    }

    private static async Task HandleOtherCallbacksAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
      // Логика обработки других callback-запросов
      await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, "Обработка других действий.");
    }
  }
}
