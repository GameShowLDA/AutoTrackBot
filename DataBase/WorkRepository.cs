using AutoTrack.Config;
using AutoTrack.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrack.DataBase
{
  internal class WorkRepository
  {
    public List<Work> SearchWorks(string property, string searchText)
    {
      return ApplicationData.DbContext.Works
          .Where(w => EF.Functions.Like(EF.Property<string>(w, property), $"%{searchText}%"))
          .ToList();
    }

    public List<Work> GetWorksByCarId(int carId)
    {
      return ApplicationData.DbContext.Works
          .Where(w => w.CarId == carId)
          .ToList();
    }
  }
}