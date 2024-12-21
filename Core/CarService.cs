using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Collections.Generic;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  /// <summary>
  /// Сервис для управления данными автомобилей.
  /// </summary>
  internal static class CarService
  {
    private static readonly CarRepository _carRepository = new CarRepository();

    /// <summary>
    /// Ищет автомобили по заданному свойству и тексту.
    /// </summary>
    /// <param name="property">Свойство для поиска.</param>
    /// <param name="searchText">Текст для поиска.</param>
    /// <returns>Список найденных автомобилей.</returns>
    public static List<Car> SearchCars(string property, string searchText)
    {
      var cars = _carRepository.SearchCars(property, searchText);
      foreach (var car in cars)
      {
        LogInfo($"Брэнд машины: {car.Brand}, Модель: {car.Model}, Номер: {car.Number}, VIN: {car.Vin}, Пробег: {car.Mileage}");
      }

      return cars;
    }

    /// <summary>
    /// Получает автомобили по идентификатору пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список автомобилей.</returns>
    public static List<Car> GetCarsByUserId(int userId)
    {
      var cars = _carRepository.GetCarsByUserId(userId);
      foreach (var car in cars)
      {
        LogInfo($"Брэнд машины: {car.Brand}, Модель: {car.Model}, Номер: {car.Number}, VIN: {car.Vin}, Пробег: {car.Mileage}");
      }

      return cars;
    }


    /// <summary>
    /// Получает автомобиль по идентификатору автомобиля.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список автомобилей.</returns>
    public static Car GetCarsByCarId(int carId)
    {
      var car = _carRepository.GetCarByCarId(carId);
      LogInfo($"Брэнд машины: {car.Brand}, Модель: {car.Model}, Номер: {car.Number}, VIN: {car.Vin}, Пробег: {car.Mileage}");

      return car;
    }

    /// <summary>
    /// Добовляет новый автомобиль в систему.
    /// </summary>
    /// <param name="car">Данные о авто.</param>ы
    public static void AddCar(Car car)
    {
      _carRepository.AddCar(car);
      LogInfo($"Добавлен новый автомобиль: Марка - {car.Brand}, Модель - {car.Brand}, Номер - {car.Number}, VIN - {car.Vin}");
    }

    public static void DeleteCarsByClientId(int clientId)
    {
      _carRepository.DeleteCarsByClientId(clientId);
      LogInfo($"Автомобили клиента с ID: {clientId} успешно удалены.");
    }

    public static void DeleteCarByCarId(int carId)
    {
      _carRepository.DeleteCarByCarId(carId);
      LogInfo($"Автомобили клиента с ID: {carId} успешно удалены.");
    }

    public static void UpdateCarProperty(int carId, string property, string newValue)
    {
      _carRepository.UpdateCarProperty(carId, property, newValue);
      LogInfo($"Обновлено свойство {property} для автомобиля с ID: {carId}");
    }
  }
}