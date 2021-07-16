using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB
{
    static class Database
    {
        private static SqliteConnection _conn;

        public static bool Initialize(string file)
        {
            // try to truncate the file if it already exists
            if (File.Exists(file))
            {
                try
                {
                    using (var fs = new FileStream(file, FileMode.Create))
                    {
                        fs.SetLength(0);
                    }
                }
                catch (Exception ex)
                {
                    OutputWriter.WriteLine(Properties.Resources.ErrMsg_Prefix + ex.Message);
                    return false;
                }

                OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_TruncateDBFile);
            }

            // create connection string with the file path
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = file,
                ForeignKeys = true
            }.ToString();
            OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_ConnectionString, connectionString);

            // create the sqlite connection with the connection string
            _conn = new SqliteConnection(connectionString);

            // open connection
            _conn.Open();
            OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_ConnectionOpened);

            return true;
        }

        public static void Close()
        {
            if (_conn != null)
            {
                try
                {
                    _conn.Close();
                    OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.VerbMsg_Sqlite_ConnectionClosed);
                }
                catch (Exception ex)
                {
                    OutputWriter.WriteLine(Properties.Resources.ErrMsg_Prefix + ex.Message);
                }
            }
        }

        private static bool CheckConnection()
        {
            if (_conn == null || _conn.State == ConnectionState.Closed || _conn.State == ConnectionState.Broken)
            {
                OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_DBNotConnect);
                return false;
            }
            return true;
        }

        public static bool CreateTable(params Type[] tableSchemaTypes)
        {
            if (tableSchemaTypes.Length == 0)
            {
                return true;
            }

            if (!CheckConnection())
            {
                return false;
            }

            using (var command = _conn.CreateCommand())
            {
                Builders.TableCreateCommandBuilder createCB = new Builders.TableCreateCommandBuilder(command);
                Builders.TableExistCommandBuilder existCB = new Builders.TableExistCommandBuilder(command);

                foreach (Type t in tableSchemaTypes)
                {
                    // build 'create table' command
                    if (!createCB.BuildCommand(t))
                    {
                        OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_ErrorBuildCommand, $"CreateTable - {t.Name}");
                        return false;
                    }
                    // run 'create table' command
                    createCB.ExecuteCommand();

                    // build 'check table exists' command
                    if (!existCB.BuildCommand(t))
                    {
                        OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_ErrorBuildCommand, $"CheckTableExist - {t.Name}");
                        return false;
                    }
                    // run 'check table exists' command
                    if (!existCB.ExecuteCommand())
                    {
                        OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_CreateTableFailed, t.Name);
                        return false;
                    }

                    OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.VerbMsg_Sqlite_TableCreated, t.Name);
                }
            }

            return true;
        }

        public static bool InsertTable(params object[] dataObjects)
        {
            if (dataObjects.Length == 0)
            {
                return true;
            }

            if (!CheckConnection())
            {
                return false;
            }

            using (var command = _conn.CreateCommand())
            {
                Builders.TableInsertCommandBuilder insertCB = new Builders.TableInsertCommandBuilder(command);
                Builders.TableSequenceCommandBuilder seqCB = new Builders.TableSequenceCommandBuilder(command);

                foreach (object dataObj in dataObjects)
                {
                    if (dataObj == null)
                    {
                        continue;
                    }

                    Type tableSchemaType = dataObj.GetType();

                    // build 'insert table' command
                    if (!insertCB.BuildCommand(dataObj))
                    {
                        OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_ErrorBuildCommand, $"InsertTable - {tableSchemaType.Name}");
                        return false;
                    }
                    // run 'insert table' command
                    if (!insertCB.ExecuteCommand())
                    {
                        OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.ErrMsg_Sqlite_InsertOneRecordFailed + $" {tableSchemaType.Name}");
                        return false;
                    }

                    // build 'fetch table sequence' command
                    if (!seqCB.BuildCommand(tableSchemaType))
                    {
                        OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_ErrorBuildCommand, $"FetchTableSequence - {tableSchemaType.Name}");
                        return false;
                    }
                    // run 'fetch table sequence' command
                    long seq = seqCB.ExecuteCommand();
                    // set auto-increment value to data object
                    if (seq > 0)
                    {
                        if (!Utils.SetDataObjectAutoIncrementValue(dataObj, seq))
                        {
                            OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_AutoIncrementPropertyIsMissing + $" {tableSchemaType.Name}");
                        }
                    }
                }
            }

            return true;
        }
    }
}
