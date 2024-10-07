namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public static class FlowExecutionReportExtensions
    {
        public static void FailIfOneOfScenariosFailed(this IEnumerable<FlowExecutionReport> reports)
        {
            foreach (var flowExecutionReport in reports)
            {
                if (flowExecutionReport.Success == false)
                {
                    throw new Exception($"Failed\n{flowExecutionReport.FlowDescription}");
                }
            }
        }

        public static void WriteToOutput(this IEnumerable<FlowExecutionReport> reports, Action<string> write)
        {
            foreach (var flowExecutionReport in reports)
            {
                flowExecutionReport.WriteToOutput(write);
                write("\n");
            }
        }

        public static void WriteToOutput(this FlowExecutionReport report, Action<string> write)
        {
            if (report.Executed == false)
            {
                write("Not executed\n");

                write(report.FlowDescription.ToString() + "\n");

                return;
            }

            write("Description:\n");
            write(report.FlowDescription.ToString() + "\n");
            write("\n");
            write("\n");

            foreach (var log in report.Logs)
            {
                write(log + "\n");
            }

            write("\n");

            if (report.Success)
            {
                write("Succeeded!" + "\n");
            }
            else
            {
                write("Failed!" + "\n");

                if (report.Exception is null)
                {
                    write("No exception is presented!\n");
                }
                else
                {
                    write(report.Exception.ToString());
                }

                foreach (var log in report.Suggestions)
                {
                    write(log + "\n");
                }
            }

            write("\n");
        }
    }
}