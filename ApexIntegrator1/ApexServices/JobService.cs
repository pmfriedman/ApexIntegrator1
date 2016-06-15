using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using ApexServices.Apex;

namespace ApexServices
{
    public class JobService
    {
        public async Task WaitForJob(long jobEntityKey, Type jobInfoType, TimeSpan maxTimeToWait)
        {
            var connection = await Connection.GetConnectionAsync();

            DateTime start = DateTime.Now;
            while (start + maxTimeToWait > DateTime.Now)
            {
                var jobResult = await connection.Query.RetrieveAsync(
                    connection.Session,
                    connection.RegionContext,
                    new Apex.RetrievalOptions
                    {
                        Expression = new EqualToExpression
                        {
                            Left = new PropertyExpression { Name = "EntityKey" },
                            Right = new ValueExpression { Value = jobEntityKey }
                        },
                        PropertyInclusionMode = PropertyInclusionMode.All,
                        Type = jobInfoType.Name
                    });

                if (jobResult.RetrieveResult.Items.OfType<JobInfo>().Single().JobState_State == JobState.Completed.ToString())
                {
                    return;
                }

                if (jobResult.RetrieveResult.Items.OfType<JobInfo>().Single().JobState_State == JobState.Error.ToString())
                {
                    //throw new Exception(jobResult.RetrieveResult.Items.OfType<JobInfo>().Single())
                }
                    await Task.Delay(1000);
            }
            throw new Exception($"Exceeded maxWaitTime for job {jobEntityKey}");
        }
    }
}
