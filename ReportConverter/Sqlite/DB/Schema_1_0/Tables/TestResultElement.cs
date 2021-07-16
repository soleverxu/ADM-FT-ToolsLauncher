using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(TestResultElement))]
    partial class TestResultElement
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("result_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true, 
            ForeignKeyRefTableName = nameof(TestResult), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultID { get; set; }

        [TableColumn("type", TableColumnDataType.Text)]
        [TableColumnConstraint(NotNullConstraint = true)]
        public string Type { get; set; }

        [TableColumn("name", TableColumnDataType.Text)]
        public string Name { get; set; }

        [TableColumn("desc", TableColumnDataType.Text)]
        public string Description { get; set; }

        [TableColumn("status", TableColumnDataType.Text)]
        public string Status { get; set; }

        [TableColumn("start_time", TableColumnDataType.Text)]
        public string StartTime { get; set; }

        [TableColumn("start_unix_time", TableColumnDataType.Integer)]
        public long? StartUnixTime { get; set; }

        [TableColumn("duration_seconds", TableColumnDataType.Numeric)]
        [TableColumnConstraint(DefaultConstraint = true, DefaultValue = "0")]
        public double? DurationSeconds { get; set; }

        [TableColumn("error_text", TableColumnDataType.Text)]
        public string ErrorText { get; set; }

        [TableColumn("error_code", TableColumnDataType.Numeric)]
        public long? ErrorCode { get; set; }

        [TableColumn("bottom_file", TableColumnDataType.Text)]
        public string BottomFile { get; set; }

        [TableColumn("html_bottom_file", TableColumnDataType.Text)]
        public string HTMLBottomFile { get; set; }
    }

    partial class TestResultElement
    {
        public static TestResultElement CreateDataObject(TestResult testResultDataObject, XmlReport.GeneralReportNode reportNode, string elementType)
        {
            if (testResultDataObject == null || reportNode == null || string.IsNullOrWhiteSpace(elementType))
            {
                return null;
            }

            return new TestResultElement
            {
                TestResultID = testResultDataObject.ID,
                Type = elementType.ToUpper(),
                Name = reportNode.Name,
                Description = reportNode.Description,
                Status = reportNode.Status.GetStatusString(),
                StartTime = Utils.GetISO8601DateTimeString(reportNode.StartTime, testResultDataObject.TimeZone),
                StartUnixTime = Utils.GetDateTimeOffsetWithTimeZone(reportNode.StartTime, testResultDataObject.TimeZone).ToUnixTimeSeconds(),
                DurationSeconds = (double)reportNode.DurationSeconds,
                ErrorText = reportNode.ErrorText,
                ErrorCode = reportNode.ErrorCode,
                BottomFile = reportNode.BottomFile,
                HTMLBottomFile = reportNode.HTMLBottomFile
            };
        }
    }
}
