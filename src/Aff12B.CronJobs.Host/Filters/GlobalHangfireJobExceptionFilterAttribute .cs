using Aff12B.Common.Infrastructure.Helpers;
using Hangfire.Common;
using Hangfire.States;
using System.Text;

namespace Aff12B.CronJobs.Host.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class GlobalHangfireJobExceptionFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private readonly ILogger Logger;

        public GlobalHangfireJobExceptionFilterAttribute(ILogger<GlobalHangfireJobExceptionFilterAttribute> logger)
        {
            Logger = logger;
        }

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState is not null)
            {
                var exception = failedState.Exception;
                Logger.LogError(GenerateJobException(context), exception);
            }
        }

        private static string GenerateJobException(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Job Name: {context.BackgroundJob.Job.Type.Name}");
            stringBuilder.AppendLine($"Job Id: {context.BackgroundJob.Id}");
            stringBuilder.AppendLine($"Created Time: {context.BackgroundJob.CreatedAt}");
            stringBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(failedState.Exception));
            return stringBuilder.ToString();
        }
    }

}
