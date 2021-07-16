using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUIAction))]
    partial class UFTGUIAction
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

        [TableColumn("elem_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResultElement), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultElementID { get; set; }

        [TableColumn("parent_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIAction), ForeignKeyRefTableColumnName = "id")]
        public long? ParentActionID { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }
    }

    partial class UFTGUIAction
    {
        public static UFTGUIAction CreateDataObject(
            TestResult testResultDataObject, 
            UFTGUIIteration iterationDataObject,
            TestResultElement testResultElementDataObject,
            long? index,
            UFTGUIAction parentActionDataObject = null)
        {
            if (testResultDataObject == null || testResultElementDataObject == null)
            {
                return null;
            }

            return new UFTGUIAction
            {
                TestResultID = testResultDataObject.ID,
                IterationID = iterationDataObject?.ID,
                TestResultElementID = testResultElementDataObject.ID,
                ParentActionID = parentActionDataObject?.ID,
                Index = index
            };
        }
    }
}
