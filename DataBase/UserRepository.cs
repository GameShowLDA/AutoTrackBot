using AutoTrack.Config;
using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrack.DataBase
{
  /// <summary>
  /// Репозиторий для управления данными пользователей в базе данных.
  /// </summary>
  internal class UserRepository
  {
    /// <summary>
    /// Добавляет нового пользователя в базу данных.
    /// </summary>
    /// <param name="user">Объект пользователя для добавления.</param>
    public void AddUser(User user)
    {
      ApplicationData.DbContext.Users.Add(user);
      ApplicationData.DbContext.SaveChanges();
    }

    /// <summary>
    /// Вовзвращает результат проверки пользователя в базе данных.
    /// </summary>
    /// <param name="chatId">ID телеграмм чата.</param>
    /// <returns>Результат проверки пользователя в базе данных.</returns>
    public bool GetResultVerificationUser(long chatId)
    {
      return ApplicationData.DbContext.Users.Where(x => x.ChatId == chatId).Count() != 0;
    }
  }
}
