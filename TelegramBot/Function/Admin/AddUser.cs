using AutoTrack.Config;
using AutoTrack.Core;
using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoTrack.TelegramBot.Function.Admin
{
  /// <summary>
  /// Класс для управления процессом добавления нового пользователя.
  /// </summary>
  internal class AddUser
  {
    private static long? newUserChatId = null;

    /// <summary>
    /// Инициирует процесс смены администратора, отправляя сообщение с кнопками текущему администратору.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="currentAdminId">Идентификатор текущего администратора.</param>
    /// <param name="newAdminId">Идентификатор нового администратора.</param>
    /// <param name="newAdminName">Имя нового администратора.</param>
    public static async Task InitiateChangeAdminAsync(ITelegramBotClient botClient, long currentAdminId, long newAdminId, string newAdminName)
    {
      newUserChatId = newAdminId;

      var callbackData = new List<CallbackModel>
      {
        new CallbackModel("Принять", "accept_addUser"),
        new CallbackModel("Отменить", "cancel_addUser"),
      };

      var inlineKeyboard = TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackData);

      await TelegramBotHandler.SendMessageAsync(botClient, currentAdminId, $"Вы хотите добавить {newAdminName} в систему?", inlineKeyboard);
      await TelegramBotHandler.SendMessageAsync(botClient, newAdminId, $"Запрос на регестрацию отправлен администратору.");
    }

    /// <summary>
    /// Обрабатывает нажатия на кнопки "Принять" и "Отменить" для подтверждения или отмены смены администратора.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="callbackQuery">Данные callback-запроса.</param>  
    public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      if (newUserChatId == null)
      {
        return;
      }

      var chatId = callbackQuery.Message.Chat.Id;
      var callbackData = callbackQuery.Data;

      if (callbackData == "accept_addUser")
      {
        UserService.AddUser(newUserChatId.Value);
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Пользователь успешно добавлен.");
        await TelegramBotHandler.SendMessageAsync(botClient, newUserChatId.Value, "Администратор одобрил заявку.");
      }
      else if (callbackData == "cancel_addUser")
      {
        await TelegramBotHandler.SendMessageAsync(botClient, newUserChatId.Value, "Администратор отменил вашу заявку.");
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Добавление пользователя отменено.");
      }

      newUserChatId = null;
    }
  }
}
