using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(TestAUTAddition))]
    partial class TestAUTAddition
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("aut_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(TestAUT), ForeignKeyRefTableColumnName = "id")]
        public long? TestAUTID { get; set; }

        [TableColumn("value", TableColumnDataType.Blob)]
        public object Value { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }
    }

    partial class TestAUTAddition
    {
        public static TestAUTAddition CreateDataObject(TestAUT autDataObject, string autAddition, long? index)
        {
            if (autDataObject == null)
            {
                return null;
            }

            return new TestAUTAddition
            {
                TestAUTID = autDataObject.ID,
                Value = autAddition,
                Index = index
            };
        }
    }
}
