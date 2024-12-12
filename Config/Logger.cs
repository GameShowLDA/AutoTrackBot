namespace AutoTrack.Config
{
  using NLog;

  /// <summary>
  /// Класс для ведения логов с использованием NLog.
  /// </summary>
  static internal class Logger
  {
    private static readonly NLog.Logger LoggerValue = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Логирует информационное сообщение.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    static public void LogInfo(string message)
    {
      LoggerValue.Info(message);
    }

    /// <summary>
    /// Логирует сообщение об ошибке.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    static public void LogError(string message)
    {
      LoggerValue.Error(message);
    }

    /// <summary>
    /// Логирует сообщение об исключении.
    /// </summary>
    /// <param name="ex">Исключение для логирования.</param>
    static public void LogException(Exception ex)
    {
      LoggerValue.Error(ex);
    }
  }
}
