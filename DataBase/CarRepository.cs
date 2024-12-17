using AutoTrack.Config;
using AutoTrack.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrack.DataBase
{
  /// <summary>
  /// Репозиторий для управления данными автомобилей в базе данных.
  /// </summary>
  internal class CarRepository
  {
    /// <summary>
    /// Ищет автомобили по заданному свойству и тексту.
    /// </summary>
    /// <param name="property">Свойство для поиска.</param>
    /// <param name="searchText">Текст для поиска.</param>
    /// <returns>Список найденных автомобилей.</returns>
    public List<Car> SearchCars(string property, string searchText)
    {
      return ApplicationData.DbContext.Cars
          .Where(c => EF.Functions.Like(EF.Property<string>(c, property), $"%{searchText}%"))
          .ToList();
    }

    /// <summary>
    /// Получает автомобили по идентификатору пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список автомобилей.</returns>
    public List<Car> GetCarsByUserId(int userId)
    {
      return ApplicationData.DbContext.Cars
          .Where(c => c.UserId == userId)
          .ToList();
    }

    /// <summary>
    /// Добаввляет новый автомобиль в базу данных.
    /// </summary>
    /// <param name="car">Объект автомобиля для добавления.</param>
    public void AddCar(Car car)
    {
      ApplicationData.DbContext.Cars.Add(car);
      ApplicationData.DbContext.SaveChanges();
    }

    public void DeleteCarsByClientId(int clientId)
    {
      var cars = ApplicationData.DbContext.Cars.Where(c => c.UserId == clientId).ToList();
      ApplicationData.DbContext.Cars.RemoveRange(cars);
      ApplicationData.DbContext.SaveChanges();
    }
  }
}