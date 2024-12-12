using AutoTrack.Config;
using AutoTrack.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrack.DataBase
{
  internal class CarRepository
  {
    public List<Car> SearchCars(string property, string searchText)
    {
      return ApplicationData.DbContext.Cars
          .Where(c => EF.Functions.Like(EF.Property<string>(c, property), $"%{searchText}%"))
          .ToList();
    }

    public List<Car> GetCarsByUserId(int userId)
    {
      return ApplicationData.DbContext.Cars
          .Where(c => c.UserId == userId)
          .ToList();
    }
  }
}