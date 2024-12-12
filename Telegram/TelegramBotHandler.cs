using AutoTrack.Model;
using AutoTrack.Utils;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Telegram
{
  public partial class TelegramBotHandler
  {
    /// <summary>
    /// Клиент Telegram бота.
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    /// Инициализирует новый экземпляр класса TelegramBotHandler.
    /// </summary>
    /// <param name="dbManager">Менеджер базы данных.</param>
    /// <param name="botToken">Токен Telegram бота.</param>
    public TelegramBotHandler(string botToken)
    {
      _botClient = new TelegramBotClient(botToken);
    }

    /// <summary>
    /// Запускает бота и начинает обработку сообщений.
    /// </summary>
    public async Task StartBotAsync()
    {
      var cts = new CancellationTokenSource();
      var receiverOptions = new ReceiverOptions
      {
        AllowedUpdates = Array.Empty<UpdateType>()
      };

      _botClient.StartReceiving(
          updateHandler: HandleUpdateAsync,
          errorHandler: HandlePollingErrorAsync,
          receiverOptions: receiverOptions,
          cancellationToken: cts.Token
      );

      var me = await _botClient.GetMe(cts.Token);
      LogInfo($"Начала работы с @{me.Username}");
    }


    /// <summary>
    /// Обрабатывает входящие обновления от Telegram.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="update">Объект обновления.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      if (update.Type == UpdateType.Message && update.Message?.Text != null)
      {
        await MessageProcessing.HandleMessageAsync(botClient, update.Message, cancellationToken);
      }
      else if (update.Type == UpdateType.CallbackQuery)
      {
        await CallbackProcessing.HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
      }
    }

    static internal async Task StartMessageAsync(ITelegramBotClient botClient, long chatId, int messageId = -1)
    {
      var greeting = TimeGreeting.GetGreeting();
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"{greeting}. Выберите функцию:");
      sb.AppendLine("/select - Выбор пользователя для просмотра работы.");
      sb.AppendLine("/search - Поиск работы по параметрам.");
      if (messageId != -1)
      {
        await SendMessageAsync(botClient, chatId, sb.ToString(), null, messageId);
      }
      else
      { 
        await SendMessageAsync(botClient, chatId, sb.ToString());
      }
    }

    /// <summary>
    /// Обрабатывает ошибки, возникающие при получении обновлений.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="exception">Возникшее исключение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
      var ErrorMessage = exception switch
      {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
      };

      LogError(ErrorMessage);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Асинхронно отправляет или редактирует сообщение пользователю через Telegram бота.
    /// </summary>
    /// <param name="botClient">Клиент Telegram бота.</param>
    /// <param name="chatId">Идентификатор чата пользователя.</param>
    /// <param name="message">Текст сообщения для отправки или редактирования.</param>
    /// <param name="inlineKeyboardMarkup">Опциональная встроенная клавиатура.</param>
    /// <param name="messageId">Идентификатор сообщения для редактирования (если есть).</param>
    /// <returns>Задача, представляющая асинхронную операцию отправки или редактирования сообщения.</returns>
    internal static async Task SendMessageAsync(ITelegramBotClient botClient, long chatId, string message, InlineKeyboardMarkup inlineKeyboardMarkup = null, int? messageId = null)
    {
      try
      {
        if (inlineKeyboardMarkup == null && messageId == null)
        {
          await botClient.SendMessage(chatId, message);
        }
        else if (inlineKeyboardMarkup == null && messageId.HasValue)
        {
          await botClient.EditMessageText(chatId, messageId.Value, message);
        }
        else if (messageId.HasValue)
        {
          await botClient.EditMessageText(chatId, messageId.Value, message, replyMarkup: inlineKeyboardMarkup);
        }
        else
        {
          await botClient.SendMessage(chatId, message, replyMarkup: inlineKeyboardMarkup);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        await TelegramBotHandler.SendMessageAsync(botClient, chatId, "Произошла системная ошибка! Повторите попытку позже...");
      }
    }

    /// <summary>
    /// Возвращает кнопки сообщения по моделям данных.
    /// </summary>
    /// <param name="data">Модели Callback/</param>
    /// <returns>Кнопки сообщения по моделям данных.</returns>
    internal static InlineKeyboardMarkup GetInlineKeyboardMarkupAsync(List<CallbackModel> data)
    {

      List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

      foreach (var callbackModel in data)
      {
        buttons.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(callbackModel.Name, callbackModel.Command) });
      }

      return new InlineKeyboardMarkup(buttons);
    }

    /// <summary>
    /// Возвращает кнопки сообщения по модели данных.
    /// </summary>
    /// <param name="callbackModel">Модель Callback.</param>
    /// <returns>Кнопка сообщения.</returns>
    internal static InlineKeyboardMarkup GetInlineKeyboardMarkupAsync(CallbackModel callbackModel)
    {
      List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
      buttons.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(callbackModel.Name, callbackModel.Command) });

      return new InlineKeyboardMarkup(buttons);
    }
  }
}
