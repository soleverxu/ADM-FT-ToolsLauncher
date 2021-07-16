using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(TestResult))]
    partial class TestResult
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("input_file", TableColumnDataType.Text)]
        [TableColumnConstraint(NotNullConstraint = true)]
        public string InputFile { get; set; }

        [TableColumn("test_type", TableColumnDataType.Text)]
        [TableColumnConstraint(NotNullConstraint = true)]
        public string TestType { get; set; }

        [TableColumn("test_name", TableColumnDataType.Text)]
        public string TestName { get; set; }

        [TableColumn("result_name", TableColumnDataType.Text)]
        public string ResultName { get; set; }

        [TableColumn("testing_tool_name", TableColumnDataType.Text)]
        public string TestingToolName { get; set; }

        [TableColumn("testing_tool_version", TableColumnDataType.Text)]
        public string TestingToolVersion { get; set; }

        [TableColumn("start_time", TableColumnDataType.Text)]
        public string StartTime { get; set; }

        [TableColumn("start_unix_time", TableColumnDataType.Integer)]
        public long? StartUnixTime { get; set; }

        [TableColumn("duration_seconds", TableColumnDataType.Numeric)]
        [TableColumnConstraint(DefaultConstraint = true, DefaultValue = "0")]
        public double? DurationSeconds { get; set; }

        [TableColumn("env_hostname", TableColumnDataType.Text)]
        public string HostName { get; set; }

        [TableColumn("env_locale", TableColumnDataType.Text)]
        public string Locale { get; set; }

        [TableColumn("env_time_zone", TableColumnDataType.Text)]
        public string TimeZone { get; set; }

        [TableColumn("env_os", TableColumnDataType.Text)]
        public string OSInfo { get; set; }

        [TableColumn("env_cpu", TableColumnDataType.Text)]
        public string CPUInfo { get; set; }

        [TableColumn("env_cpu_cores", TableColumnDataType.Numeric)]
        public double? CPUCores { get; set; }

        [TableColumn("env_memory", TableColumnDataType.Text)]
        public string TotalMemory { get; set; }

        [TableColumn("env_login_user", TableColumnDataType.Text)]
        public string LoginUser { get; set; }
    }

    partial class TestResult
    {
        public static TestResult CreateDataObject(XmlReport.TestReportBase testReport)
        {
            if (testReport == null)
            {
                return null;
            }

            return new TestResult
            {
                InputFile = testReport.ReportFile,
                TestType = testReport.GetTestType(),
                TestName = testReport.TestName,
                ResultName = testReport.ReportName,
                TestingToolName = testReport.TestingToolName,
                TestingToolVersion = testReport.TestingToolVersion,
                StartTime = Utils.GetISO8601DateTimeString(testReport.TestRunStartTime, testReport.TimeZone),
                StartUnixTime = Utils.GetDateTimeOffsetWithTimeZone(testReport.TestRunStartTime, testReport.TimeZone).ToUnixTimeSeconds(),
                DurationSeconds = (double)testReport.TestDurationSeconds,
                HostName = testReport.HostName,
                Locale = testReport.Locale,
                TimeZone = testReport.TimeZone,
                OSInfo = testReport.OSInfo,
                CPUInfo = testReport.CPUInfo,
                CPUCores = testReport.CPUCores,
                TotalMemory = testReport.TotalMemory,
                LoginUser = testReport.LoginUser
            };
        }
    }
}
