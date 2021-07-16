using Microsoft.Data.Sqlite;
using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(Metadata))]
    partial class Metadata
    {
        [TableColumn("code", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true)]
        public long? Code { get; set; }

        [TableColumn("name", TableColumnDataType.Text)]
        [TableColumnConstraint(NotNullConstraint = true)]
        public string Name { get; set; }

        [TableColumn("value", TableColumnDataType.Blob)]
        public object Value { get; set; }
    }

    partial class Metadata
    {
        public static Metadata CreateDataObject(long code, string name, object value)
        {
            return new Metadata
            {
                Code = code,
                Name = name,
                Value = value
            };
        }
    }
}
