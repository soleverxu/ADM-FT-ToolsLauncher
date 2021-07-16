using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0
{
    enum TestParameterDirection
    {
        Input,
        Output
    }

    class TestParameterDirectionValue
    {
        public const string Input = "INPUT";
        public const string Output = "OUTPUT";
    }
}
