namespace AutoTrack.Model
{
  /// <summary>
  /// Представляет автомобиль пользователя.
  /// </summary>
  public class Car
  {
    /// <summary>
    /// Уникальный идентификатор автомобиля.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Марка автомобиля.
    /// </summary>
    public string Brand { get; set; }

    /// <summary>
    /// Модель автомобиля.
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Номер автомобиля.
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// VIN автомобиля.
    /// </summary>
    public string Vin { get; set; }

    /// <summary>
    /// Пробег автомобиля.
    /// </summary>
    public int Mileage { get; set; }
  }
}
