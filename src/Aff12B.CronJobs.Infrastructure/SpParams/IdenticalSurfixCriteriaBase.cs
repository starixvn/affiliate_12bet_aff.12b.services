using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public abstract class IdenticalSurfixCriteriaBase(string prefixProvider = null) : CriteriaBase
    {
        protected readonly string _prefixProvider = prefixProvider;

        protected abstract string SuffixSettingKey { get; }

        public override string GetSettingKey()
            => _prefixProvider == null || string.IsNullOrWhiteSpace(_prefixProvider)
                ? SuffixSettingKey
                : $"{_prefixProvider}_{SuffixSettingKey}";
    }
}
