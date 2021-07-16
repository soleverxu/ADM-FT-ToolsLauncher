using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ReportConverter.Sqlite.DB.Attributes;

namespace ReportConverter.Sqlite.DB.Builders
{
    class TableCreateCommandBuilder
    {
        public TableCreateCommandBuilder(SqliteCommand command)
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

            string commandText = BuildSqlCommandText(tableSchemaType, true);
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return false;
            }

            Command.CommandText = commandText;
            OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_CreateTableCommandText, commandText);
            return true;
        }

        public virtual bool ExecuteCommand()
        {
            Command.ExecuteNonQuery();
            return true;
        }

        protected virtual string BuildSqlCommandText(Type tableSchemaType, bool pretty = false)
        {
            var tableAttr = tableSchemaType.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null || string.IsNullOrWhiteSpace(tableAttr.TableName))
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            // CREATE TABLE
            sb.Append(@"CREATE TABLE");

            // IF NOT EXISTS?
            if (tableAttr.AllowTableExists)
            {
                sb.Append(@" IF NOT EXISTS");
            }

            // table name
            sb.Append($" [{tableAttr.TableName}]");

            if (pretty) sb.AppendLine();
            sb.Append(@"(");

            // columns
            PropertyInfo[] props = tableSchemaType.GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo pi = props[i];

                var tableColumnAttr = pi.GetCustomAttribute<TableColumnAttribute>();
                if (tableColumnAttr == null || string.IsNullOrWhiteSpace(tableColumnAttr.ColumnName))
                {
                    continue;
                }

                if (i > 0)
                {
                    sb.Append(@", ");
                }

                if (pretty)
                {
                    sb.AppendLine();
                    sb.Append("    ");
                }

                // column name
                sb.Append($"[{tableColumnAttr.ColumnName}]");

                // column data type
                sb.Append($" {tableColumnAttr.ColumnDataType.GetDataTypeString()}");

                // column constraint
                var colConstraintAttr = pi.GetCustomAttribute<TableColumnConstraintAttribute>();
                if (colConstraintAttr != null)
                {
                    // PRIMARY KEY
                    if (colConstraintAttr.PrimaryKeyConstraint)
                    {
                        sb.Append(@" PRIMARY KEY");

                        if (colConstraintAttr.PrimaryKeyAutoIncrement)
                        {
                            sb.Append(@" AUTOINCREMENT");
                        }
                    }

                    // NOT NULL
                    if (colConstraintAttr.NotNullConstraint)
                    {
                        sb.Append(@" NOT NULL");
                    }

                    // UNIQUE
                    if (colConstraintAttr.UniqueConstraint)
                    {
                        sb.Append(@" UNIQUE");
                    }

                    // CHECK (...)
                    if (colConstraintAttr.CheckConstraint && !string.IsNullOrWhiteSpace(colConstraintAttr.CheckExpression))
                    {
                        sb.Append($" CHECK ({colConstraintAttr.CheckExpression})");
                    }

                    // DEFAULT <VALUE>
                    if (colConstraintAttr.DefaultConstraint)
                    {
                        if (tableColumnAttr.ColumnDataType == TableColumnDataType.Integer || 
                            tableColumnAttr.ColumnDataType == TableColumnDataType.Numeric ||
                            tableColumnAttr.ColumnDataType == TableColumnDataType.Real)
                        {
                            sb.Append($" DEFAULT {colConstraintAttr.DefaultValue}");
                        }
                        else
                        {
                            sb.Append($" DEFAULT '{colConstraintAttr.DefaultValue}'");
                        }
                    }

                    // FOREIGN KEY
                    if (colConstraintAttr.ForeignKeyConstraint && 
                        !string.IsNullOrWhiteSpace(colConstraintAttr.ForeignKeyRefTableName) &&
                        !string.IsNullOrWhiteSpace(colConstraintAttr.ForeignKeyRefTableColumnName))
                    {
                        // REFERENCES [ref-table] ([ref-col])
                        sb.Append($" REFERENCES [{colConstraintAttr.ForeignKeyRefTableName}] ([{colConstraintAttr.ForeignKeyRefTableColumnName}])");
                    }
                }
            }

            if (pretty) sb.AppendLine();
            sb.Append(@")");

            // WITHOUT ROWID?
            if (tableAttr.WithoutROWID)
            {
                sb.Append(@" WITHOUT ROWID");
            }

            return sb.ToString();
        }
    }
}
