using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(TestAUT))]
    partial class TestAUT
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

        [TableColumn("runtime_engine", TableColumnDataType.Text)]
        public string RuntimEngine { get; set; }

        [TableColumn("name", TableColumnDataType.Text)]
        public string Name { get; set; }

        [TableColumn("path", TableColumnDataType.Text)]
        public string Path { get; set; }

        [TableColumn("version", TableColumnDataType.Text)]
        public string Version { get; set; }

        [TableColumn("technology", TableColumnDataType.Text)]
        public string Technology { get; set; }

        [TableColumn("reserved", TableColumnDataType.Blob)]
        public string Reserved { get; set; }
    }

    partial class TestAUT
    {
        public static TestAUT CreateDataObject(TestResult testResultDataObject, XmlReport.TestedApplicationType aut)
        {
            if (testResultDataObject == null || aut == null)
            {
                return null;
            }

            return new TestAUT
            {
                TestResultID = testResultDataObject.ID,
                TestResultElementID = null,
                RuntimEngine = aut.RuntimeEngineInfo,
                Name = aut.Name,
                Path = aut.Path,
                Version = aut.Version,
                Technology = aut.Technology,
                Reserved = aut.Reserved
            };
        }

        public static TestAUT CreateDataObject(TestResult testResultDataObject, TestResultElement testResultElementDataObject, XmlReport.TestedApplicationType aut)
        {
            if (testResultDataObject == null || testResultElementDataObject == null || aut == null)
            {
                return null;
            }

            TestAUT instance = CreateDataObject(testResultDataObject, aut);
            instance.TestResultElementID = testResultElementDataObject.ID;
            return instance;
        }
    }
}
