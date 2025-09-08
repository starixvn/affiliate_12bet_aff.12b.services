namespace Aff12B.Common.DataDomain.Models
{
    /// <summary>
    /// Interface for data models that have an UpdateTime property.
    /// This is used to ensure the generic constraint works with DateTime tracking.
    /// </summary>
    public interface IHasUpdateTime
    {
        DateTime? UpdateTime { get; }
    }
}
