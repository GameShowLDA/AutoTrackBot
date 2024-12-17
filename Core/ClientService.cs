using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Collections.Generic;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  internal static class ClientService
  {
    private static readonly ClientRepository _clientRepository = new ClientRepository();

    public static List<Client> SearchClient(string property, string searchText)
    {
      var users = _clientRepository.SearchClient(property, searchText);
      foreach (var user in users)
      {
        LogInfo($"Имя клиента: {user.Name}, Дата создания: {user.Created}");
      }
      return users;
    }

    /// <summary>
    /// Добавляет клиента в базу данных.
    /// </summary>
    public static void AddClient(Client client)
    {
      _clientRepository.AddCleint(client);
      LogInfo($"Клиент: {client.Name} успешно добавлен!");
    }

    public static List<Client> GetAllClients()
    {
      var clients = _clientRepository.GetAllClients();
      foreach (var client in clients)
      {
        LogInfo($"Клиент: {client.Name}, Дата создания: {client.Created}");
      }
      return clients;
    }

    public static Client GetClientById(int clientId)
    {
      var client = _clientRepository.GetClientById(clientId);
      if (client != null)
      {
        LogInfo($"Клиент найден: {client.Name}, Дата создания: {client.Created}");
      }
      else
      {
        LogError($"Клиент с ID: {clientId} не найден.");
      }
      return client;
    }

    public static bool ValidateClientName(string name)
    {
      var isValid = _clientRepository.ValidateClientName(name);
      if (!isValid)
      {
        LogError($"Клиент {name} уже существует в БД!");
      }
      else
      {
        LogInfo($"Имя клиента {name} доступно для добавления.");
      }

      return isValid;
    }

    /// <summary>
    /// Удаляет клиента из системы по идентификатору.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента для удаления.</param>
    public static void DeleteClient(int Id)
    {
      var client = _clientRepository.GetClientById(Id);
      if (client != null)
      {
        _clientRepository.DeleteClient(Id);
        LogInfo($"Клиент с ID: {Id} успешно удален.");
      }
      else
      {
        LogError($"Клиент с ID: {Id} не найден в БД.");
      }
    }
  }
}