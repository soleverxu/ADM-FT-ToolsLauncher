using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUIHierarchy))]
    partial class UFTGUIHierarchy
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

        [TableColumn("elem_type", TableColumnDataType.Text)]
        public string TestResultElementType { get; set; }

        [TableColumn("parent_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIHierarchy), ForeignKeyRefTableColumnName = "id")]
        public long? ParentID { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }

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

    partial class UFTGUIHierarchy
    {
        public static UFTGUIHierarchy CreateDataObject(
            TestResult testResultDataObject,
            TestResultElement testResultElementDataObject,
            long? index,
            UFTGUIHierarchy parentDataObject = null
            )
        {
            if (testResultDataObject == null || testResultElementDataObject == null)
            {
                return null;
            }

            return new UFTGUIHierarchy
            {
                TestResultID = testResultDataObject.ID,
                TestResultElementID = testResultElementDataObject.ID,
                TestResultElementType = testResultElementDataObject.Type,
                ParentID = parentDataObject?.ID,
                Index = index
            };
        }

        public static UFTGUIHierarchy CreateDataObject(
            XmlReport.GUITest.StepReport stepReportNode,
            TestResult testResultDataObject,
            TestResultElement testResultElementDataObject,
            long? index,
            UFTGUIHierarchy parentDataObject = null
            )
        {
            if (stepReportNode == null)
            {
                return null;
            }

            UFTGUIHierarchy instance = CreateDataObject(testResultDataObject, testResultElementDataObject, index, parentDataObject);

            instance.TestObjectPath = stepReportNode.TestObjectPath;
            instance.TestObjectOperation = stepReportNode.TestObjectOperation;
            instance.TestObjectOperationData = stepReportNode.TestObjectOperationData;
            instance.IsSIDEnabled = stepReportNode.SmartIdentification != null;
            instance.SIDBasicMatchCount = stepReportNode.SmartIdentification?.SIDBasicProperties?.BasicMatch;

            return instance;
        }
    }
}
