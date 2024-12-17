using AutoTrack.DataBase;
using AutoTrack.Model;
using Bogus;
using System;
using System.Linq;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Tests
{
  /// <summary>
  /// Класс для заполнения базы данных тестовыми данными.
  /// </summary>
  public class DataSeeder
  {
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DataSeeder"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public DataSeeder(ApplicationDbContext context)
    {
      _context = context;
    }

    /// <summary>
    /// Заполняет базу данных тестовыми данными.
    /// </summary>
    public void SeedData()
    {
      LogInfo("Проверка наличия пользователей в базе данных.");
      if (!_context.Clients.Any())
      {
        LogInfo("Пользователи не найдены, добавляем новых пользователей.");
        var userFaker = new Faker<Client>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Created, f => f.Date.Past());

        var users = userFaker.Generate(5);
        _context.Clients.AddRange(users);
        _context.SaveChanges();
        LogInfo("Пользователи успешно добавлены.");
      }
      else
      {
        LogInfo("Пользователи уже существуют в базе данных.");
      }

      var userIds = _context.Clients.Select(u => u.Id).ToList();

      if (!_context.Cars.Any())
      {
        var carFaker = new Faker<Car>()
            .RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(c => c.Model, f => f.Vehicle.Model())
            .RuleFor(c => c.Number, f => f.Random.Replace("???-####"))
            .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
            .RuleFor(c => c.Mileage, f => f.Random.Number(0, 200000))
            .RuleFor(c => c.UserId, f => f.PickRandom(userIds));

        var cars = carFaker.Generate(20);
        _context.Cars.AddRange(cars);
        _context.SaveChanges();
      }

      var carIds = _context.Cars.Select(c => c.Id).ToList();

      if (!_context.Works.Any())
      {
        var workFaker = new Faker<Work>()
            .RuleFor(w => w.Description, f => f.Lorem.Sentence())
            .RuleFor(w => w.Summ, f => f.Commerce.Price())
            .RuleFor(w => w.Date, f => f.Date.Past())
            .RuleFor(w => w.CarId, f => f.PickRandom(carIds));

        var works = workFaker.Generate(30);
        _context.Works.AddRange(works);
        _context.SaveChanges();
      }
    }
  }
}