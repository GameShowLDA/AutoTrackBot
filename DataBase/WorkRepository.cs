using AutoTrack.Config;
using AutoTrack.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrack.DataBase
{
  /// <summary>
  /// Репозиторий для управления данными о работах в базе данных.
  /// </summary>
  internal class WorkRepository
  {
    /// <summary>
    /// Ищет работы по заданному свойству и тексту.
    /// </summary>
    /// <param name="property">Свойство для поиска.</param>
    /// <param name="searchText">Текст для поиска.</param>
    /// <returns>Список найденных работ.</returns>
    public List<Work> SearchWorks(string property, string searchText)
    {
      return ApplicationData.DbContext.Works
          .Where(w => EF.Functions.Like(EF.Property<string>(w, property), $"%{searchText}%"))
          .ToList();
    }

    /// <summary>
    /// Получает работы по идентификатору автомобиля.
    /// </summary>
    /// <param name="carId">Идентификатор автомобиля.</param>
    /// <returns>Список работ.</returns>
    public List<Work> GetWorksByCarId(int carId)
    {
      return ApplicationData.DbContext.Works
          .Where(w => w.CarId == carId)
          .ToList();
    }

    /// <summary>
    /// Добавляет новую работу в базу данных.
    /// </summary>
    /// <param name="work">Объект работы для добавления.</param>
    public void AddWork(Work work)
    {
      ApplicationData.DbContext.Works.Add(work);
      ApplicationData.DbContext.SaveChanges();
    }
    public void DeleteWorksByCarIds(List<int> carIds)
    {
      var works = ApplicationData.DbContext.Works.Where(w => carIds.Contains(w.CarId)).ToList();
      ApplicationData.DbContext.Works.RemoveRange(works);
      ApplicationData.DbContext.SaveChanges();
    }
  }
}