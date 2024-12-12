namespace AutoTrack.DataBase
{
  using AutoTrack.Config;
  using AutoTrack.Model;
  using Microsoft.EntityFrameworkCore;
  using static Config.Logger;
  using System.IO;

  /// <summary>
  /// Контекст базы данных для приложения AutoTrack.
  /// </summary>
  public class ApplicationDbContext : DbContext
  {
    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Таблица автомобилей.
    /// </summary>
    public DbSet<Car> Cars { get; set; }

    /// <summary>
    /// Таблица выполненных работ.
    /// </summary>
    public DbSet<Work> Works { get; set; }

    /// <summary>
    /// Настраивает подключение к базе данных.
    /// </summary>
    /// <param name="optionsBuilder">Параметры конфигурации.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (string.IsNullOrEmpty(ApplicationData.ConfigApp.DataPath))
      {
        LogError("Путь к базе данных не задан в конфигурации.");
        throw new ArgumentNullException(nameof(ApplicationData.ConfigApp.DataPath), "Путь к базе данных не может быть null.");
      }

      var dbPath = Path.Combine(Directory.GetCurrentDirectory(), ApplicationData.ConfigApp.DataPath);
      var connectionString = $"Data Source={dbPath}";
      LogInfo($"Используется строка подключения: {connectionString}");
      optionsBuilder.UseSqlite(connectionString);
    }

    /// <summary>
    /// Проверяет существование базы данных и всех таблиц.
    /// </summary>
    public void CheckDatabaseAndTables()
    {
      LogInfo("Проверка существования базы данных и таблиц начата.");

      try
      {
        if (Database.EnsureCreated())
        {
          LogInfo("База данных была создана.");
        }
        else
        {
          LogInfo("База данных уже существует.");
        }

        if (Database.CanConnect())
        {
          LogInfo("Подключение к базе данных успешно.");

          var tablesExist = Users.Any() || Cars.Any() || Works.Any();
          if (tablesExist)
          {
            LogInfo("Все таблицы существуют и доступны.");
          }
          else
          {
            LogError("Не удалось найти данные в одной или нескольких таблицах.");
          }
        }
        else
        {
          LogError("Не удалось подключиться к базе данных.");
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }

    /// <summary>
    /// Удаляет базу данных, если она существует.
    /// </summary>
    public void DeleteDatabase()
    {
      LogInfo("Попытка удаления базы данных.");
      try
      {
        if (Database.EnsureDeleted())
        {
          LogInfo("База данных успешно удалена.");
        }
        else
        {
          LogInfo("База данных не была удалена, так как она не существует.");
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }
  }
}