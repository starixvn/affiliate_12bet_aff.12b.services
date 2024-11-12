using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public abstract class IdenticalSurfixCriteriaBase : CriteriaBase
    {
        protected readonly string _prefixProvider;

        protected IdenticalSurfixCriteriaBase(string prefixProvider = null)
            => _prefixProvider = prefixProvider;

        protected abstract string SuffixSettingKey { get; }

        public override string GetSettingKey()
            => _prefixProvider == null || string.IsNullOrWhiteSpace(_prefixProvider)
                ? SuffixSettingKey
                : $"{_prefixProvider}_{SuffixSettingKey}";
    }
}
