using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Collections.Generic;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  internal static class UserService
  {
    private static readonly UserRepository _userRepository = new UserRepository();

    public static List<User> SearchUsers(string property, string searchText)
    {
      var users = _userRepository.SearchUsers(property, searchText);
      foreach (var user in users)
      {
        LogInfo($"Имя клиента: {user.Name}, Дата создания: {user.Created}");
      }
      return users;
    }
  }
}