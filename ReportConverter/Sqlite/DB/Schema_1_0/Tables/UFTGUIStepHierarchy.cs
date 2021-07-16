using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUIStepHierarchy))]
    partial class UFTGUIStepHierarchy
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

        [TableColumn("action_iteration_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIActionIteration), ForeignKeyRefTableColumnName = "id")]
        public long? ActionIterationID { get; set; }

        [TableColumn("elem_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestResultElement), ForeignKeyRefTableColumnName = "id")]
        public long? TestResultElementID { get; set; }

        [TableColumn("elem_type", TableColumnDataType.Text)]
        public string TestResultElementType { get; set; }

        [TableColumn("parent_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIStepHierarchy), ForeignKeyRefTableColumnName = "id")]
        public long? ParentID { get; set; }

        [TableColumn("test_obj_path", TableColumnDataType.Text)]
        public string TestObjectPath { get; set; }

        [TableColumn("test_obj_op", TableColumnDataType.Text)]
        public string TestObjectOperation { get; set; }

        [TableColumn("test_obj_op_data", TableColumnDataType.Blob)]
        public object TestObjectOperationData { get; set; }

        [TableColumn("is_sid", TableColumnDataType.Integer)]
        public int? IsSIDValue { get; set; }

        [TableColumn("sid_basic_match", TableColumnDataType.Integer)]
        public int? SIDBasicMatchCount { get; set; }

        public bool IsSIDEnabled
        {
            get { return IsSIDValue > 0; }
            set { IsSIDValue = value == true ? 1 : 0; }
        }
    }

    partial class UFTGUIStepHierarchy
    {
        public static UFTGUIStepHierarchy CreateDataObject(
            TestResult testResultDataObject,
            UFTGUIIteration iterationDataObject,
            UFTGUIAction actionDataObject,
            UFTGUIActionIteration actionIterationDataObject,
            TestResultElement testResultElementDataObject,
            UFTGUIStepHierarchy parentDataObject = null
            )
        {
            if (testResultDataObject == null || testResultElementDataObject == null)
            {
                return null;
            }

            return new UFTGUIStepHierarchy
            {
                TestResultID = testResultDataObject.ID,
                IterationID = iterationDataObject?.ID,
                ActionID = actionDataObject?.ID,
                ActionIterationID = actionIterationDataObject?.ID,
                TestResultElementID = testResultElementDataObject.ID,
                TestResultElementType = testResultElementDataObject.Type,
                ParentID = parentDataObject?.ID
            };
        }

        public static UFTGUIStepHierarchy CreateDataObject(
            XmlReport.GUITest.StepReport stepReportNode,
            TestResult testResultDataObject,
            UFTGUIIteration iterationDataObject,
            UFTGUIAction actionDataObject,
            UFTGUIActionIteration actionIterationDataObject,
            TestResultElement testResultElementDataObject,
            UFTGUIStepHierarchy parentDataObject = null
            )
        {
            if (stepReportNode == null)
            {
                return null;
            }

            UFTGUIStepHierarchy instance = CreateDataObject(testResultDataObject, iterationDataObject, actionDataObject,
                actionIterationDataObject, testResultElementDataObject, parentDataObject);

            instance.TestObjectPath = stepReportNode.TestObjectPath;
            instance.TestObjectOperation = stepReportNode.TestObjectOperation;
            instance.TestObjectOperationData = stepReportNode.TestObjectOperationData;
            instance.IsSIDEnabled = stepReportNode.SmartIdentification != null;
            instance.SIDBasicMatchCount = stepReportNode.SmartIdentification?.SIDBasicProperties?.BasicMatch;

            return instance;
        }
    }
}
