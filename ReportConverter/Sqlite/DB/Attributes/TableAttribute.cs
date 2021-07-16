using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            TableName = name;
        }

        public string TableName { get; private set; }
        public bool WithoutROWID { get; set; }
        public bool AllowTableExists { get; set; }
    }
}
