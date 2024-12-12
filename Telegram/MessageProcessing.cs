using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using AutoTrack.Config;
using AutoTrack.Utils;
using AutoTrack.Telegram.Function;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Telegram
{
  static internal class MessageProcessing
  {
    private static bool isWaitingForNewAdmin = false;
    internal static bool searchData = false;

    static internal async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
      var chatId = message.From.Id;
      var messageText = message.Text;
      LogInfo($"Сообщение от {message.From.LastName} {message.From.FirstName} - {message.Text}");

      if (isWaitingForNewAdmin && chatId != ApplicationData.ConfigApp.AdminId)
      {
        await ChangeAdmin.InitiateChangeAdminAsync(botClient, ApplicationData.ConfigApp.AdminId, chatId, message.From.FirstName);
        isWaitingForNewAdmin = false;
        return;
      }

      if (searchData)
      {
        await EntityPropertyViewer.SearchDataAsync(botClient, chatId, messageText);
      }

      if (chatId != ApplicationData.ConfigApp.AdminId)
      {
        LogError($"У пользователя {message.From.LastName} {message.From.FirstName} нет прав для управления.");
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "У вас нет доступа к управлению данными");
      }
      else
      {
        if (messageText.ToLower() == "/start" || messageText.ToLower() == "привет")
        {
         await TelegramBotHandler.StartMessageAsync(botClient, chatId);
        }
        else if (messageText.ToLower() == "/change")
        {
          isWaitingForNewAdmin = true;
          await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Отправьте сообщение от пользователя, которого хотите сделать администратором.");
        }
        else if (messageText.ToLower() == "/select")
        {
          await UserSelector.DisplayUsersAsync(botClient, chatId, message);
        }
        else if (messageText.ToLower() == "/search")
        {
          await EntityPropertyViewer.DisplayPropertyButtonsAsync(botClient, chatId);
        }
      }
    }
  }
}
