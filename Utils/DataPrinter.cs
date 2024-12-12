using AutoTrack.DataBase;
using ConsoleTables;
using System.Linq;

namespace AutoTrack.Utils
{
  /// <summary>
  /// Класс для вывода данных из базы данных в виде таблиц.
  /// </summary>
  public class DataPrinter
  {
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DataPrinter"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public DataPrinter(ApplicationDbContext context)
    {
      _context = context;
    }

    /// <summary>
    /// Выводит данные из всех таблиц.
    /// </summary>
    public void PrintAllTables()
    {
      PrintUsers();
      PrintCars();
      PrintWorks();
    }

    private void PrintUsers()
    {
      var users = _context.Users.ToList();
      var table = new ConsoleTable("ID", "Name", "Created");

      foreach (var user in users)
      {
        table.AddRow(user.Id, user.Name, user.Created);
      }

      table.Write();
    }

    private void PrintCars()
    {
      var cars = _context.Cars.ToList();
      var table = new ConsoleTable("ID", "UserId", "Brand", "Model", "Number", "VIN", "Mileage");

      foreach (var car in cars)
      {
        table.AddRow(car.Id, car.UserId, car.Brand, car.Model, car.Number, car.Vin, car.Mileage);
      }

      table.Write();
    }

    private void PrintWorks()
    {
      var works = _context.Works.ToList();
      var table = new ConsoleTable("ID", "CarId", "Description", "Summ", "Date");

      foreach (var work in works)
      {
        table.AddRow(work.Id, work.CarId, work.Description, work.Summ, work.Date);
      }

      table.Write();
    }
  }
}