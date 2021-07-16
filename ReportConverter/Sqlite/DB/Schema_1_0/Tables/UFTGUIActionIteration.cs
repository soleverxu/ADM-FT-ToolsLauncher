using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUIActionIteration))]
    partial class UFTGUIActionIteration
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("result_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResult), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultID { get; set; }

        [TableColumn("iteration_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIIteration), ForeignKeyRefTableColumnName = "id")]
        public long? IterationID { get; set; }

        [TableColumn("action_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIAction), ForeignKeyRefTableColumnName = "id")]
        public long? ActionID { get; set; }

        [TableColumn("elem_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResultElement), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultElementID { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }
    }

    partial class UFTGUIActionIteration
    {
        public static UFTGUIActionIteration CreateDataObject(
            XmlReport.GUITest.ActionIterationReport actionIterationReportNode,
            TestResult testResultDataObject,
            UFTGUIIteration iterationDataObject,
            UFTGUIAction actionDataObject,
            TestResultElement testResultElementDataObject
            )
        {
            if (actionIterationReportNode == null || testResultDataObject == null || testResultElementDataObject == null)
            {
                return null;
            }

            return new UFTGUIActionIteration
            {
                TestResultID = testResultDataObject.ID,
                IterationID = iterationDataObject?.ID,
                ActionID = actionDataObject?.ID,
                TestResultElementID = testResultElementDataObject.ID,
                Index = actionIterationReportNode.Index
            };
        }
    }
}
