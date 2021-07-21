using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0
{
    static class TestTypeExtensions
    {
        public static string GetTestType(this XmlReport.TestReportBase testReport)
        {
            if (testReport is XmlReport.GUITest.TestReport)
            {
                return TestType.UFTGUITest;
            }

            if (testReport is XmlReport.APITest.TestReport)
            {
                return TestType.UFTAPITest;
            }

            if (testReport is XmlReport.BPT.TestReport)
            {
                return TestType.UFTBPTTest;
            }

            return null;
        }
    }

    static class TestParameterDirectionExtensions
    {
        public static string ToDirectionString(this TestParameterDirection direction)
        {
            switch (direction)
            {
                case TestParameterDirection.Input:
                    return TestParameterDirectionValue.Input;

                case TestParameterDirection.Output:
                    return TestParameterDirectionValue.Output;

                default:
                    return TestParameterDirectionValue.Input;
            }
        }
    }

    static class XmlReportStatusExtensions
    {
        public static string GetStatusString(this XmlReport.ReportStatus status)
        {
            switch (status)
            {
                case XmlReport.ReportStatus.Failed:
                    return "FAILED";

                case XmlReport.ReportStatus.Warning:
                    return "WARNING";

                case XmlReport.ReportStatus.Information:
                    return "INFORMATION";

                case XmlReport.ReportStatus.Passed:
                    return "PASSED";

                case XmlReport.ReportStatus.Done:
                    return "DONE";

                default:
                    return null;
            }
        }
    }
}
