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
using AutoTrack.TelegramBot.Function;
using static AutoTrack.Config.Logger;
using AutoTrack.TelegramBot.Function.Admin;

namespace AutoTrack.TelegramBot
{
  /// <summary>
  /// Класс для обработки входящих сообщений от пользователей.
  /// </summary>
  static internal class MessageProcessing
  {
    private static bool isWaitingForNewAdmin = false;
    internal static bool isAddNewUser = false;
    internal static bool searchData = false;
    internal static bool addWork = false;
    internal static bool addClient = false;

    /// <summary>
    /// Обрабатывает входящие сообщения от пользователей.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="message">Сообщение от пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    internal static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
      var chatId = message.From.Id;
      var messageText = message.Text.ToLower();
      LogInfo($"Сообщение от {message.From.LastName} {message.From.FirstName} - {message.Text}");

      if (isWaitingForNewAdmin && chatId != ApplicationData.ConfigApp.AdminId)
      {
        await ChangeAdmin.InitiateChangeAdminAsync(botClient, ApplicationData.ConfigApp.AdminId, chatId, message.From.FirstName);
        isWaitingForNewAdmin = false;
        return;
      }

      if (isAddNewUser)
      {
        await AddUser.InitiateChangeAdminAsync(botClient, ApplicationData.ConfigApp.AdminId, chatId, message.From.FirstName);
        isAddNewUser = false;
        return;
      }

      if (searchData)
      {
        await EntityPropertyViewer.SearchDataAsync(botClient, chatId, messageText);
        return;
      }

      if (addWork)
      {
        await AddWork.ProcessAddWorkAsync(botClient, chatId, message);
        return;
      }

      if (addClient)
      { 
        await AddClient.HandleClientNameInputAsync(botClient, message);
        return;
      }

      var commandHandlers = new Dictionary<string, Func<Task>>
      {
        { "/start", async () => await TelegramBotHandler.StartMessageAsync(botClient, chatId) },
        { "привет", async () => await TelegramBotHandler.StartMessageAsync(botClient, chatId) },
        { "/select", async () => await UserSelector.DisplayUsersAsync(botClient, chatId, message) },
        { "/search", async () => await EntityPropertyViewer.DisplayPropertyButtonsAsync(botClient, chatId) },
        { "/help", async () => { await TelegramBotHandler.StartMessageAsync(botClient, chatId); }},
        { "/addclient", async () => { addClient = true; await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите ФИО нового клиента:"); } },
        { "/deleteclient", async () => { await DeleteClient.DisplayClientsAsync(botClient, chatId); }},

        { "/change", async () => { if (chatId != ApplicationData.ConfigApp.AdminId) { await SendAccessDeniedMessageAsync(botClient, chatId); return; } isWaitingForNewAdmin = true; await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Отправьте сообщение от пользователя, которого хотите сделать администратором."); } },
        { "/adduser", async () => { if (chatId != ApplicationData.ConfigApp.AdminId) { await SendAccessDeniedMessageAsync(botClient, chatId); return; } isAddNewUser = true; await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Отправьте сообщение от пользователя, которого хотите добавить в систему."); } },
        { "/log", async () => { if (chatId != ApplicationData.ConfigApp.AdminId) { await SendAccessDeniedMessageAsync(botClient, chatId); return; } await LogViewer.DisplayLogFilesAsync(botClient, chatId); }},
      };

      if (commandHandlers.TryGetValue(messageText, out var handler))
      {
        await handler();
      }
    }

    private static async Task SendAccessDeniedMessageAsync(ITelegramBotClient botClient, long chatId)
    {
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "У вас не достаточно прав для данной команды. Введите /help для просмотора доступынх вам команд.");
    }
  }
}
