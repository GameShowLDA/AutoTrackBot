using AutoTrack.DataBase;
using AutoTrack.Model;
using System.Collections.Generic;
using static AutoTrack.Config.Logger;

namespace AutoTrack.Core
{
  internal static class CarService
  {
    private static readonly CarRepository _carRepository = new CarRepository();

    public static List<Car> SearchCars(string property, string searchText)
    {
      var cars = _carRepository.SearchCars(property, searchText);
      foreach (var car in cars)
      {
        LogInfo($"Брэнд машины: {car.Brand}, Модель: {car.Model}, Номер: {car.Number}, VIN: {car.Vin}, Пробег: {car.Mileage}");
      }

      return cars;
    }

    public static List<Car> GetCarsByUserId(int userId)
    {
      var cars = _carRepository.GetCarsByUserId(userId);
      foreach (var car in cars)
      {
        LogInfo($"Брэнд машины: {car.Brand}, Модель: {car.Model}, Номер: {car.Number}, VIN: {car.Vin}, Пробег: {car.Mileage}");
      }

      return cars;
    }
  }
}