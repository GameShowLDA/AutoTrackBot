namespace AutoTrack.Model
{
  /// <summary>
  /// Представляет клиентов автосервиса.
  /// </summary>
  public class Client
  {
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Дата создания пользователя.
    /// </summary>
    public DateTime Created { get; set; }
  }
}