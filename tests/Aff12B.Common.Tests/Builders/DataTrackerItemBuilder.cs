using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating DataTrackerItem instances in tests
    /// </summary>
    public class DataTrackerItemBuilder
    {
        private DataTrackerItem _dataTrackerItem;

        public DataTrackerItemBuilder()
        {
            _dataTrackerItem = new DataTrackerItem
            {
                PrefixSiteName = "TestSite",
                PushTrackerId = 12345,
                StatusId = 1,
                PushDataType = 1,
                PushDataInfo = "Test data tracking information"
            };
        }

        public DataTrackerItemBuilder WithPrefixSiteName(string prefixSiteName)
        {
            _dataTrackerItem.PrefixSiteName = prefixSiteName;
            return this;
        }

        public DataTrackerItemBuilder WithPushTrackerId(long pushTrackerId)
        {
            _dataTrackerItem.PushTrackerId = pushTrackerId;
            return this;
        }

        public DataTrackerItemBuilder WithStatusId(int statusId)
        {
            _dataTrackerItem.StatusId = statusId;
            return this;
        }

        public DataTrackerItemBuilder WithPushDataType(int pushDataType)
        {
            _dataTrackerItem.PushDataType = pushDataType;
            return this;
        }

        public DataTrackerItemBuilder WithPushDataInfo(string pushDataInfo)
        {
            _dataTrackerItem.PushDataInfo = pushDataInfo;
            return this;
        }

        public DataTrackerItemBuilder AsPending()
        {
            _dataTrackerItem.StatusId = 0;
            return this;
        }

        public DataTrackerItemBuilder AsProcessing()
        {
            _dataTrackerItem.StatusId = 1;
            return this;
        }

        public DataTrackerItemBuilder AsCompleted()
        {
            _dataTrackerItem.StatusId = 2;
            return this;
        }

        public DataTrackerItemBuilder AsFailed()
        {
            _dataTrackerItem.StatusId = -1;
            return this;
        }

        public DataTrackerItemBuilder WithCustomerDataType()
        {
            _dataTrackerItem.PushDataType = 1;
            _dataTrackerItem.PushDataInfo = "Customer data tracking";
            return this;
        }

        public DataTrackerItemBuilder WithBetDataType()
        {
            _dataTrackerItem.PushDataType = 2;
            _dataTrackerItem.PushDataInfo = "Bet record data tracking";
            return this;
        }

        public DataTrackerItemBuilder WithDepositDataType()
        {
            _dataTrackerItem.PushDataType = 3;
            _dataTrackerItem.PushDataInfo = "Deposit data tracking";
            return this;
        }

        public DataTrackerItem Build()
        {
            return _dataTrackerItem;
        }
    }
}
