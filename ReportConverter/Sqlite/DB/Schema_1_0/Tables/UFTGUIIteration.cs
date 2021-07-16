using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUIIteration))]
    partial class UFTGUIIteration
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("result_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResult), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultID { get; set; }

        [TableColumn("elem_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResultElement), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultElementID { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }
    }

    partial class UFTGUIIteration
    {
        public static UFTGUIIteration CreateDataObject(
            TestResult testResultDataObject, 
            TestResultElement testResultElementDataObject, 
            XmlReport.GUITest.IterationReport iterationReportNode)
        {
            if (testResultDataObject == null || testResultElementDataObject == null || iterationReportNode == null)
            {
                return null;
            }

            return new UFTGUIIteration
            {
                TestResultID = testResultDataObject.ID,
                TestResultElementID = testResultElementDataObject.ID,
                Index = iterationReportNode.Index
            };
        }
    }
}
