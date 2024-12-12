using AutoTrack.Config;
using AutoTrack.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrack.DataBase
{
  internal class UserRepository
  {
    public List<User> SearchUsers(string property, string searchText)
    {
      return ApplicationData.DbContext.Users
          .Where(u => EF.Functions.Like(EF.Property<string>(u, property), $"%{searchText}%"))
          .ToList();
    }
  }
}