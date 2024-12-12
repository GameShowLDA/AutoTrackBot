using AutoTrack.Config;
using AutoTrack.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Telegram.Function
{
  /// <summary>
  /// Класс для управления процессом смены администратора.
  /// </summary>
  internal static class ChangeAdmin
  {
    private static long? pendingAdminId = null;

    /// <summary>
    /// Инициирует процесс смены администратора, отправляя сообщение с кнопками текущему администратору.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="currentAdminId">Идентификатор текущего администратора.</param>
    /// <param name="newAdminId">Идентификатор нового администратора.</param>
    /// <param name="newAdminName">Имя нового администратора.</param>
    public static async Task InitiateChangeAdminAsync(ITelegramBotClient botClient, long currentAdminId, long newAdminId, string newAdminName)
    {
      pendingAdminId = newAdminId;

      var callbackData = new List<CallbackModel>
      {
        new CallbackModel("Принять", "accept_change"),
        new CallbackModel("Отменить", "cancel_change"),
      };

      var inlineKeyboard = TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackData);

      await TelegramBotHandler.SendMessageAsync(botClient, currentAdminId, $"Вы хотите назначить {newAdminName} новым администратором?", inlineKeyboard);
      await TelegramBotHandler.SendMessageAsync(botClient, newAdminId, $"Запрос на назначение администратора отправлен пользователю.");
    }

    /// <summary>
    /// Обрабатывает нажатия на кнопки "Принять" и "Отменить" для подтверждения или отмены смены администратора.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="callbackQuery">Данные callback-запроса.</param>
    public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      if (pendingAdminId == null)
      {
        return;
      }

      var chatId = callbackQuery.Message.Chat.Id;
      var callbackData = callbackQuery.Data;

      if (callbackData == "accept_change")
      {
        ApplicationData.ConfigApp.AdminId = pendingAdminId.Value;
        SaveConfigToFile(ApplicationData.ConfigApp, "config.yaml");

        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Администратор успешно изменён.");
        await TelegramBotHandler.SendMessageAsync(botClient, pendingAdminId.Value, "Вы назначены на роль администратора. Чтобы начать работу, нажмите /start.");
        pendingAdminId = null;
      }
      else if (callbackData == "cancel_change")
      {
        pendingAdminId = null;
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Смена администратора отменена.");
      }
    }

    /// <summary>
    /// Сохраняет конфигурацию в YAML файл.
    /// </summary>
    /// <param name="config">Конфигурация для сохранения.</param>
    /// <param name="filePath">Путь к файлу конфигурации.</param>
    private static void SaveConfigToFile(Config.Config config, string filePath)
    {
      var serializer = new YamlDotNet.Serialization.SerializerBuilder()
          .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
          .Build();

      using var writer = new StreamWriter(filePath);
      serializer.Serialize(writer, config);
    }
  }
}
