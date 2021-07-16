using Microsoft.Data.Sqlite;
using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Builders
{
    class TableInsertCommandBuilder
    {
        public TableInsertCommandBuilder(SqliteCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            Command = command;
        }

        public SqliteCommand Command { get; private set; }

        public virtual bool BuildCommand(object dataObject)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException(nameof(dataObject));
            }

            Dictionary<string, object> dbParameterDict = new Dictionary<string, object>();
            string commandText = BuildSqlCommandTextAndParameters(dataObject, dbParameterDict, true);
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return false;
            }

            Command.CommandText = commandText;

            Command.Parameters.Clear();
            foreach (var dbParam in dbParameterDict)
            {
                Command.Parameters.AddWithValue(dbParam.Key, dbParam.Value);
            }

            OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_InsertTableCommandText, commandText);
            return true;
        }

        public virtual bool ExecuteCommand()
        {
            int n = Command.ExecuteNonQuery();
            if (n != 1)
            {
                return false;
            }
            return true;
        }

        protected virtual string BuildSqlCommandTextAndParameters(object dataObject, Dictionary<string, object> paramDict, bool pretty = false)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException(nameof(dataObject));
            }
            if (paramDict == null)
            {
                throw new ArgumentNullException(nameof(paramDict));
            }

            // table name
            Type tableSchemaType = dataObject.GetType();
            var tableAttr = tableSchemaType.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null || string.IsNullOrWhiteSpace(tableAttr.TableName))
            {
                return null;
            }

            // column names
            PropertyInfo[] props = tableSchemaType.GetProperties();
            if (props.Length == 0)
            {
                return null;
            }

            // create column name list and parameter dictionary
            List<string> columnNameList = new List<string>(props.Length);
            List<string> paramNameList = new List<string>(props.Length);
            paramDict.Clear();
            foreach(PropertyInfo pi in props)
            {
                var tableColumnAttr = pi.GetCustomAttribute<TableColumnAttribute>();
                if (tableColumnAttr == null || string.IsNullOrWhiteSpace(tableColumnAttr.ColumnName))
                {
                    continue;
                }

                var colConstraintAttr = pi.GetCustomAttribute<TableColumnConstraintAttribute>();
                if (colConstraintAttr != null && colConstraintAttr.PrimaryKeyConstraint && colConstraintAttr.PrimaryKeyAutoIncrement)
                {
                    // skip the auto-increment column in the insert command
                    continue;
                }

                // column name
                string colName = tableColumnAttr.ColumnName;
                columnNameList.Add(colName);

                // database parameter name and value
                string paramName = colName.Replace(' ', '_');
                object value = pi.GetValue(dataObject);

                // check if value NULL is acceptable in case the NOT NULL constraint is set on the column
                if (value == null && colConstraintAttr != null && colConstraintAttr.NotNullConstraint)
                {
                    // NOT NULL constraint is found on the column and the value is NULL
                    // it is not allowed
                    OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_NullValueNotAllowed, colName, tableAttr.TableName);
                    return null;
                }

                paramNameList.Add(paramName);
                paramDict.Add(paramName, Utils.GetDBValue(value));
            }

            // build SQL command text
            StringBuilder sb = new StringBuilder();

            // INSERT INTO
            sb.Append($"INSERT INTO [{tableAttr.TableName}]");

            sb.Append(@" (");

            // column names
            for (int i = 0; i < columnNameList.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(@", ");
                }

                if (pretty)
                {
                    sb.AppendLine();
                    sb.Append("    ");
                }

                sb.Append($"[{columnNameList[i]}]");
            }

            if (pretty) sb.AppendLine();
            sb.Append(@") VALUES (");

            // parameter names
            for (int i = 0; i < paramNameList.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(@", ");
                }

                if (pretty)
                {
                    sb.AppendLine();
                    sb.Append("    ");
                }

                sb.Append($"${paramNameList[i]}");
            }

            if (pretty) sb.AppendLine();
            sb.Append(@")");

            return sb.ToString();
        }
    }
}
