using Aff12B.Common.DataDomain.Models;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating TagModel instances in tests
    /// </summary>
    public class TagModelBuilder
    {
        private TagModel _tagModel;

        public TagModelBuilder()
        {
            _tagModel = new TagModel
            {
                Id = 1,
                TagName = "test-tag",
                Description = "Test tag description",
                Status = 1,
                UpdateTime = DateTime.UtcNow,
                IsAbnormal = 0
            };
        }

        public TagModelBuilder WithId(int id)
        {
            _tagModel.Id = id;
            return this;
        }

        public TagModelBuilder WithTagName(string tagName)
        {
            _tagModel.TagName = tagName;
            return this;
        }

        public TagModelBuilder WithDescription(string description)
        {
            _tagModel.Description = description;
            return this;
        }

        public TagModelBuilder WithStatus(int status)
        {
            _tagModel.Status = status;
            return this;
        }

        public TagModelBuilder WithUpdateTime(DateTime? updateTime)
        {
            _tagModel.UpdateTime = updateTime;
            return this;
        }

        public TagModelBuilder WithIsAbnormal(byte isAbnormal)
        {
            _tagModel.IsAbnormal = isAbnormal;
            return this;
        }

        public TagModelBuilder AsActive()
        {
            _tagModel.Status = 1;
            return this;
        }

        public TagModelBuilder AsInactive()
        {
            _tagModel.Status = 0;
            return this;
        }

        public TagModelBuilder AsNormalTag()
        {
            _tagModel.IsAbnormal = 0;
            return this;
        }

        public TagModelBuilder AsAbnormalTag()
        {
            _tagModel.IsAbnormal = 1;
            return this;
        }

        public TagModelBuilder UpdatedHoursAgo(int hours)
        {
            _tagModel.UpdateTime = DateTime.UtcNow.AddHours(-hours);
            return this;
        }

        public TagModelBuilder WithoutUpdateTime()
        {
            _tagModel.UpdateTime = null;
            return this;
        }

        public TagModel Build()
        {
            return _tagModel;
        }
    }
}
