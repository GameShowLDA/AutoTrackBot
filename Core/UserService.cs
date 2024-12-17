using AutoTrack.DataBase;
using AutoTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  /// <summary>
  /// Сервис для управления данными пользователей.
  /// </summary>
  internal static class UserService
  {
    private static readonly UserRepository _userRepository = new UserRepository();

    /// <summary>
    /// Добавляет нового пользователя в систему.
    /// </summary>
    /// <param name="chatId">Идентификатор чата пользователя.</param>
    public static void AddUser(long chatId)
    {
      var user = new User
      {
        ChatId = chatId,
      };

      _userRepository.AddUser(user);
      LogInfo($"Добавлен новый пользователь: ChatId - {user.ChatId}");
    }

    /// <summary>
    /// Вовзвращет результат проверки пользователя в базе данных по идентификатору чата.
    /// </summary>
    /// <param name="chatId">Id чата телеграмма.</param>
    /// <returns>Результат проверки пользователя.</returns>
    public static bool GetResultVerificationUser(long chatId)
    {
      if (!_userRepository.GetResultVerificationUser(chatId))
      {
        LogError("Пользователь не найден!");
        return false;
      }
      else
      {
        return true;
      }
    }
  }
}
