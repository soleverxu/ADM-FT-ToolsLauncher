using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class TableColumnAttribute : Attribute
    {
        public TableColumnAttribute(string name, TableColumnDataType dataType)
        {
            ColumnName = name;
            ColumnDataType = dataType;
        }

        public string ColumnName { get; private set; }
        public TableColumnDataType ColumnDataType { get; private set; }
    }

    enum TableColumnDataType
    {
        Text,
        Numeric,
        Integer,
        Real,
        Blob,
    }

    static class TableColumnDataTypeExtensions
    {
        public static string GetDataTypeString(this TableColumnDataType dataType)
        {
            switch (dataType)
            {
                case TableColumnDataType.Text:
                    return "TEXT";

                case TableColumnDataType.Numeric:
                    return "NUMERIC";

                case TableColumnDataType.Integer:
                    return "INTEGER";

                case TableColumnDataType.Real:
                    return "REAL";

                case TableColumnDataType.Blob:
                default:
                    return "BLOB";
            }
        }
    }
}
