namespace AutoTrack.Model
{
  /// <summary>
  /// Представляет выполненную работу по автомобилю.
  /// </summary>
  public class Work
  {
    /// <summary>
    /// Уникальный идентификатор работы.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор автомобиля.
    /// </summary>
    public int CarId { get; set; }

    /// <summary>
    /// Описание выполненной работы.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Сумма за выполненную работу.
    /// </summary>
    public string Summ { get; set; }

    /// <summary>
    /// Дата выполнения работы.
    /// </summary>
    public DateTime Date { get; set; }
  }
}