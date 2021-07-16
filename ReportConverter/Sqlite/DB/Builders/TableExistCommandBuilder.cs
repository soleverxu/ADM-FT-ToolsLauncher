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
    class TableExistCommandBuilder
    {
        public TableExistCommandBuilder(SqliteCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            Command = command;
        }

        public SqliteCommand Command { get; private set; }

        public virtual bool BuildCommand(Type tableSchemaType)
        {
            if (tableSchemaType == null)
            {
                throw new ArgumentNullException(nameof(tableSchemaType));
            }

            string commandText = BuildSqlCommandText(tableSchemaType);
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return false;
            }

            Command.CommandText = commandText;
            OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_CheckTableExistCommandText, commandText);
            return true;
        }

        public virtual bool ExecuteCommand()
        {
            object result = Command.ExecuteScalar();
            if (result == null || Convert.ToInt64(result) != 1)
            {
                return false;
            }
            return true;
        }

        protected virtual string BuildSqlCommandText(Type tableSchemaType)
        {
            var tableAttr = tableSchemaType.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null || string.IsNullOrWhiteSpace(tableAttr.TableName))
            {
                return null;
            }

            return $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableAttr.TableName}'";
        }
    }
}
