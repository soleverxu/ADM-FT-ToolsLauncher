using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUICheckpoint))]
    partial class UFTGUICheckpoint
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("hierarchy_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIHierarchy), ForeignKeyRefTableColumnName = "id")]
        public long? HierarchyID { get; set; }

        [TableColumn("elem_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResultElement), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultElementID { get; set; }

        [TableColumn("fail_desc", TableColumnDataType.Text)]
        public string FailDescription { get; set; }

        [TableColumn("type", TableColumnDataType.Text)]
        public string Type { get; set; }

        [TableColumn("subtype", TableColumnDataType.Text)]
        public string SubType { get; set; }

        [TableColumn("short_desc", TableColumnDataType.Text)]
        public string ShortDescription { get; set; }

        [TableColumn("timeout", TableColumnDataType.Numeric)]
        public double? Timeout { get; set; }

        [TableColumn("used_time", TableColumnDataType.Numeric)]
        public double? UsedTime { get; set; }

        [TableColumn("bmp_expected_file", TableColumnDataType.Text)]
        public string BmpExpectedFile { get; set; }

        [TableColumn("bmp_actual_file", TableColumnDataType.Text)]
        public string BmpActualFile { get; set; }

        [TableColumn("bmp_diff_file", TableColumnDataType.Text)]
        public string BmpDiffFile { get; set; }

        [TableColumn("acc_alt_result_xml_file", TableColumnDataType.Text)]
        public string AltAccResultXmlFile { get; set; }

        [TableColumn("acc_alt_result_xsl_file", TableColumnDataType.Text)]
        public string AltAccResultXslFile { get; set; }

        [TableColumn("text_expected", TableColumnDataType.Text)]
        public string TextExpected { get; set; }

        [TableColumn("text_captured", TableColumnDataType.Text)]
        public string TextCaptured { get; set; }

        [TableColumn("text_before", TableColumnDataType.Text)]
        public string TextBefore { get; set; }

        [TableColumn("text_after", TableColumnDataType.Text)]
        public string TextAfter { get; set; }

        [TableColumn("text_is_regex", TableColumnDataType.Integer)]
        public int? TextIsRegexValue { get; set; }

        [TableColumn("text_match_case", TableColumnDataType.Integer)]
        public int? TextMatchCaseValue { get; set; }

        [TableColumn("text_exact_match", TableColumnDataType.Integer)]
        public int? TextExactMatchValue { get; set; }

        [TableColumn("text_ignore_space", TableColumnDataType.Integer)]
        public int? TextIgnoreSpaceValue { get; set; }

        public bool? TextIsRegex
        {
            get { return TextIsRegexValue > 0; }
            set
            {
                if (value == null)
                {
                    TextIsRegexValue = null;
                }
                else
                {
                    TextIsRegexValue = value == true ? 1 : 0;
                }
            }
        }

        public bool? TextMatchCase
        {
            get { return TextMatchCaseValue > 0; }
            set
            {
                if (value == null)
                {
                    TextMatchCaseValue = null;
                }
                else
                {
                    TextMatchCaseValue = value == true ? 1 : 0;
                }
            }
        }

        public bool? TextExactMatch
        {
            get { return TextExactMatchValue > 0; }
            set
            {
                if (value == null)
                {
                    TextExactMatchValue = null;
                }
                else
                {
                    TextExactMatchValue = value == true ? 1 : 0;
                }
            }
        }

        public bool? TextIgnoreSpace
        {
            get { return TextIgnoreSpaceValue > 0; }
            set
            {
                if (value == null)
                {
                    TextIgnoreSpaceValue = null;
                }
                else
                {
                    TextIgnoreSpaceValue = value == true ? 1 : 0;
                }
            }
        }
    }

    partial class UFTGUICheckpoint
    {
        public static UFTGUICheckpoint CreateDataObject(XmlReport.GUITest.CheckpointReport checkpointReportNode, UFTGUIHierarchy hierarchyDataObject)
        {
            if (checkpointReportNode == null || hierarchyDataObject == null)
            {
                return null;
            }

            return new UFTGUICheckpoint
            {
                HierarchyID = hierarchyDataObject.ID,
                TestResultElementID = hierarchyDataObject.TestResultElementID,
                FailDescription = checkpointReportNode.FailedDescription,
                Type = checkpointReportNode.Detail?.Type,
                SubType = checkpointReportNode.Detail?.CheckpointSubType,
                ShortDescription = checkpointReportNode.Detail?.ShortDescription,
                Timeout = checkpointReportNode.Detail?.MaxTimeout,
                UsedTime = checkpointReportNode.Detail?.UsedTimeout,
                BmpExpectedFile = checkpointReportNode.Detail?.bmpChkPointFileExpected,
                BmpActualFile = checkpointReportNode.Detail?.bmpChkPointFileActual,
                BmpDiffFile = checkpointReportNode.Detail?.bmpChkPointFileDifferent,
                AltAccResultXmlFile = checkpointReportNode.Detail?.resultxml,
                AltAccResultXslFile = checkpointReportNode.Detail?.resultxsl,
                TextExpected = checkpointReportNode.Detail?.Expected,
                TextCaptured = checkpointReportNode.Detail?.Captured,
                TextBefore = checkpointReportNode.Detail?.TextBefore,
                TextAfter = checkpointReportNode.Detail?.TextAfter,
                TextIsRegex = checkpointReportNode.Detail?.Regex,
                TextMatchCase = checkpointReportNode.Detail?.MatchCase,
                TextExactMatch = checkpointReportNode.Detail?.ExactMatch,
                TextIgnoreSpace = checkpointReportNode.Detail?.IgnoreSpaces
            };
        }
    }
}
