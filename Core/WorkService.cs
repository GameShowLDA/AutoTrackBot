using AutoTrack.Config;
using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Collections.Generic;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  /// <summary>
  /// Сервис для управления данными о работах.
  /// </summary>
  internal static class WorkService
  {
    private static readonly WorkRepository _workRepository = new WorkRepository();

    /// <summary>
    /// Ищет работы по заданному свойству и тексту.
    /// </summary>
    /// <param name="property">Свойство для поиска.</param>
    /// <param name="searchText">Текст для поиска.</param>
    /// <returns>Список найденных работ.</returns>
    public static List<Work> SearchWorks(string property, string searchText)
    {
      var works = _workRepository.SearchWorks(property, searchText);
      foreach (var work in works)
      {
        LogInfo($"Описание работы: {work.Description}, Сумма: {work.Summ}, Дата: {work.Date}");
      }

      return works;
    }

    /// <summary>
    /// Получает работы по идентификатору автомобиля.
    /// </summary>
    /// <param name="carId">Идентификатор автомобиля.</param>
    /// <returns>Список работ.</returns>
    public static List<Work> GetWorksByCarId(int carId)
    {
      var works = _workRepository.GetWorksByCarId(carId);
      foreach (var work in works)
      {
        LogInfo($"Работа: {work.Description}, Сумма: {work.Summ}, Дата: {work.Date}");
      }

      return works;
    }

    /// <summary>
    /// Добавляет новую работу в систему.
    /// </summary>
    /// <param name="work">Данные о работе.</param>
    public static void AddWork(Work work)
    {
      _workRepository.AddWork(work);
      LogInfo($"Добавлена новая работа: Описание - {work.Description}, Сумма - {work.Summ}, CarId - {work.CarId}");
    }

    public static void DeleteWorksByCarIds(List<int> carIds)
    {
      _workRepository.DeleteWorksByCarIds(carIds);
      LogInfo($"Работы для автомобилей с ID: {string.Join(", ", carIds)} успешно удалены.");
    }
  }
}