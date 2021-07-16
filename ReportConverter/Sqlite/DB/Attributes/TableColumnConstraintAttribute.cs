using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class TableColumnConstraintAttribute : Attribute
    {
        public TableColumnConstraintAttribute()
        {

        }

        public bool PrimaryKeyConstraint { get; set; }
        public bool PrimaryKeyAutoIncrement { get; set; }
        public bool NotNullConstraint { get; set; }
        public bool UniqueConstraint { get; set; }
        public bool CheckConstraint { get; set; }
        public string CheckExpression { get; set; }
        public bool DefaultConstraint { get; set; }
        public string DefaultValue { get; set; }
        public bool ForeignKeyConstraint { get; set; }
        public string ForeignKeyRefTableName { get; set; }
        public string ForeignKeyRefTableColumnName { get; set; }
    }
}
