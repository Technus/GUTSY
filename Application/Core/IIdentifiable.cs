namespace GeneralUnifiedTestSystemYard.Core;

public interface IIdentifiable
{
    /// <summary>
    ///     Gets command unique identifier
    /// </summary>
    /// <returns></returns>
    public string Identifier { get; }
}