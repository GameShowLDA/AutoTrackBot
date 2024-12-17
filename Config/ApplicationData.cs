namespace AutoTrack.Config
{
  using AutoTrack.DataBase;
  using System;
  using System.IO;
  using YamlDotNet.Serialization;
  using YamlDotNet.Serialization.NamingConventions;
  using static Logger;

  /// <summary>
  /// Класс для хранения глобальных данных приложения.
  /// </summary>
  internal class ApplicationData
  {
    /// <summary>
    /// Получает или устанавливает конфигурацию приложения.
    /// </summary>
    public static Config ConfigApp { get; private set; }

    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    public static ApplicationDbContext DbContext { get; set; }

    /// <summary>
    /// Статический конструктор для инициализации конфигурации приложения.
    /// </summary>
    static ApplicationData()
    {
      const string configFilePath = "config.yaml";

      if (!File.Exists(configFilePath))
      {
        LogInfo("Файл конфигурации не найден. Введите данные вручную.");

        Console.Write("Введите строку подключения к базе данных: ");
        string databaseConnectionString = Console.ReadLine();

        Console.Write("Введите токен бота Telegram: ");
        string botToken = Console.ReadLine();

        Console.Write("Введите идентификатор администратора бота: ");
        if (!long.TryParse(Console.ReadLine(), out long adminId))
        {
          LogError("Неверный формат идентификатора администратора.");
          return;
        }

        // Создаём новый экземпляр Config с введёнными данными
        ConfigApp = new Config
        {
          DataPath = databaseConnectionString,
          BotToken = botToken,
          AdminId = adminId,
        };

        // Сохраняем конфигурацию в файл
        SaveConfigToFile(ConfigApp, configFilePath);
        LogInfo("Конфигурация успешно создана и сохранена в файл.");
      }
      else
      {
        ConfigApp = new Config(configFilePath);
        LogInfo("Конфигурация загружена из файла.");
      }
    }

    /// <summary>
    /// Сохраняет конфигурацию в YAML файл.
    /// </summary>
    /// <param name="config">Конфигурация для сохранения.</param>
    /// <param name="filePath">Путь к файлу конфигурации.</param>
    private static void SaveConfigToFile(Config config, string filePath)
    {
      var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

      using var writer = new StreamWriter(filePath);
      serializer.Serialize(writer, config);
    }
  }
}
