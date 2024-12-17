using AutoTrack.Config;
using AutoTrack.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrack.DataBase
{
  /// <summary>
  /// Репозиторий для управления данными клиентов в базе данных.
  /// </summary>
  internal class ClientRepository
  {
    /// <summary>
    /// Ищет клиентов по заданному свойству и тексту.
    /// </summary>
    /// <param name="property">Свойство для поиска.</param>
    /// <param name="searchText">Текст для поиска.</param>
    /// <returns>Список найденных клиентов.</returns>
    public List<Client> SearchClient(string property, string searchText)
    {
      return ApplicationData.DbContext.Clients
          .Where(u => EF.Functions.Like(EF.Property<string>(u, property), $"%{searchText}%"))
          .ToList();
    }

    /// <summary>
    /// Добавляет клиента в базу данных.
    /// </summary>
    /// <param name="client">Модель клиента.</param>
    public void AddCleint(Client client)
    {
      ApplicationData.DbContext.Clients.Add(client);
      ApplicationData.DbContext.SaveChanges();
    }

    /// <summary>
    /// Удаляет клиента из базы данных по идентификатору.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента для удаления.</param>
    public void DeleteClient(int clientId)
    {
      var client = ApplicationData.DbContext.Clients.Find(clientId);
      if (client != null)
      {
        ApplicationData.DbContext.Clients.Remove(client);
        ApplicationData.DbContext.SaveChanges();
      }
    }

    public Client GetClientById(int clientId)
    {
      return ApplicationData.DbContext.Clients.Find(clientId);
    }

    public List<Client> GetAllClients()
    {
      return ApplicationData.DbContext.Clients.ToList();
    }

    /// <summary>
    /// Проверяет, существует ли имя клиента в базе данных.
    /// </summary>
    /// <param name="name">Имя клиента для проверки.</param>
    /// <returns>True, если имя не найдено; иначе False.</returns>
    public bool ValidateClientName(string name)
    {
      var normalizedInputName = NormalizeName(name);
      return !ApplicationData.DbContext.Clients
          .AsEnumerable()
          .Any(c => NormalizeName(c.Name) == normalizedInputName);
    }

    /// <summary>
    /// Нормализует имя, приводя его к нижнему регистру и сортируя части имени.
    /// </summary>
    /// <param name="name">Имя для нормализации.</param>
    /// <returns>Нормализованное имя.</returns>
    private string NormalizeName(string name)
    {
      return string.Join(" ", name
          .Trim()
          .ToLowerInvariant()
          .Split(' ', StringSplitOptions.RemoveEmptyEntries)
          .OrderBy(part => part));
    }
  }
}