using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(TestParameter))]
    partial class TestParameter
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

        [TableColumn("direction", TableColumnDataType.Text)]
        [TableColumnConstraint(NotNullConstraint = true)]
        public string Direction { get; set; }

        [TableColumn("name", TableColumnDataType.Text)]
        public string Name { get; set; }

        [TableColumn("value", TableColumnDataType.Blob)]
        public object Value { get; set; }

        [TableColumn("type", TableColumnDataType.Text)]
        public string Type { get; set; }
    }

    partial class TestParameter
    {
        public static TestParameter CreateDataObject(TestResult testResultDataObject, TestParameterDirection direction, XmlReport.ParameterType parameter)
        {
            if (testResultDataObject == null || parameter == null)
            {
                return null;
            }

            return new TestParameter
            {
                TestResultID = testResultDataObject.ID,
                TestResultElementID = null,
                Direction = direction.ToDirectionString(),
                Name = parameter.name,
                Value = parameter.value,
                Type = parameter.type
            };
        }

        public static TestParameter CreateDataObject(TestResult testResultDataObject, TestResultElement testResultElementDataObject, TestParameterDirection direction, XmlReport.ParameterType parameter)
        {
            if (testResultDataObject == null || testResultElementDataObject == null || parameter == null)
            {
                return null;
            }

            TestParameter instance = CreateDataObject(testResultDataObject, direction, parameter);
            instance.TestResultElementID = testResultElementDataObject.ID;
            return instance;
        }
    }
}
