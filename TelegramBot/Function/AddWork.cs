using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoTrack.Core;
using AutoTrack.Model;
using Telegram.Bot;
using Telegram.Bot.Types;
using static AutoTrack.Config.Logger;

namespace AutoTrack.TelegramBot.Function
{
  /// <summary>
  /// Класс для управления процессом добавления новой работы.
  /// </summary>
  static internal class AddWork
  {
    private static int _currentCarId;
    private static string _description;
    private static string _summ;

    private enum AddWorkState
    {
      None,
      WaitingForDescription,
      WaitingForSumm
    }

    private static AddWorkState _currentState = AddWorkState.None;

    /// <summary>
    /// Обрабатывает callback-запрос для начала процесса добавления работы.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="callbackQuery">Данные callback-запроса.</param>
    public static async Task ProcessAddWorkAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      var carIdString = callbackQuery.Data.Replace("/addWork_", "");
      if (int.TryParse(carIdString, out _currentCarId))
      {
        _currentState = AddWorkState.WaitingForDescription;
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.From.Id, "Введите описание работы:", null, callbackQuery.Message.MessageId);
      }
      else
      {
        LogError("Ошибка парсинга carId");
        await TelegramBotHandler.SendMessageAsync(botClient, callbackQuery.From.Id, "Ошибка: неверный идентификатор автомобиля.", null, callbackQuery.Message.MessageId);
      }
    }

    /// <summary>
    /// Обрабатывает сообщения для добавления описания и суммы работы.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="message">Сообщение пользователя.</param>
    public static async Task ProcessAddWorkAsync(ITelegramBotClient botClient, long chatId, Message message)
    {
      switch (_currentState)
      {
        case AddWorkState.None:
          await StartAddWorkProcessAsync(botClient, chatId, message);
          break;
        case AddWorkState.WaitingForDescription:
          await HandleDescriptionInputAsync(botClient, chatId, message);
          break;
        case AddWorkState.WaitingForSumm:
          await HandleSummInputAsync(botClient, message);
          break;
      }
    }

    /// <summary>
    /// Начинает процесс добавления работы, запрашивая описание.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="message">Сообщение пользователя.</param>
    private static async Task StartAddWorkProcessAsync(ITelegramBotClient botClient, long chatId, Message message)
    {
      var carIdString = message.Text.Replace("/addWork_", "");
      if (int.TryParse(carIdString, out _currentCarId))
      {
        _currentState = AddWorkState.WaitingForDescription;
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите описание работы:");
      }
      else
      {
        LogError("Ошибка парсинга carId");
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Ошибка: неверный идентификатор автомобиля.");
      }
    }

    /// <summary>
    /// Обрабатывает ввод описания работы.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="message">Сообщение пользователя.</param>
    private static async Task HandleDescriptionInputAsync(ITelegramBotClient botClient, long chatId, Message message)
    {
      _description = message.Text;
      _currentState = AddWorkState.WaitingForSumm;
      await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Введите сумму за работу:");
    }

    /// <summary>
    /// Обрабатывает ввод суммы за работу.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="message">Сообщение пользователя.</param>
    private static async Task HandleSummInputAsync(ITelegramBotClient botClient, Message message)
    {
      _summ = message.Text;
      await SaveWorkAsync(botClient, message.Chat.Id);
      _currentState = AddWorkState.None;
    }

    /// <summary>
    /// Сохраняет новую работу в системе и отправляет подтверждение пользователю.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    private static async Task SaveWorkAsync(ITelegramBotClient botClient, long chatId)
    {
      var work = new Work
      {
        CarId = _currentCarId,
        Description = _description,
        Summ = _summ,
        Date = DateTime.Now
      };

      WorkService.AddWork(work);

      StringBuilder sb = new StringBuilder();

      sb.AppendLine($"Работа: {work.Description}");
      sb.AppendLine($"Сумма: {work.Summ}");
      sb.AppendLine($"Дата: {work.Date}");
      sb.AppendLine(string.Empty);
      sb.AppendLine("Работа успешно добавлена.");

      var callbackModels = new List<CallbackModel>
        {
            new CallbackModel("На главную", "/start"),
        };

      await TelegramBotHandler.SendMessageAsync(botClient, chatId, sb.ToString(), TelegramBotHandler.GetInlineKeyboardMarkupAsync(callbackModels));
      MessageProcessing.addWork = false;
    }
  }
}
