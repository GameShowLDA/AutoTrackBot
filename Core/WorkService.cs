using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Collections.Generic;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  internal static class WorkService
  {
    private static readonly WorkRepository _workRepository = new WorkRepository();

    public static List<Work> SearchWorks(string property, string searchText)
    {
      var works = _workRepository.SearchWorks(property, searchText);
      foreach (var work in works)
      {
        LogInfo($"Описание работы: {work.Description}, Сумма: {work.Summ}, Дата: {work.Date}");
      }

      return works;
    }

    public static List<Work> GetWorksByCarId(int carId)
    {
      var works = _workRepository.GetWorksByCarId(carId);
      foreach (var work in works)
      {
        LogInfo($"Работа: {work.Description}, Сумма: {work.Summ}, Дата: {work.Date}");
      }

      return works;
    }
  }
}