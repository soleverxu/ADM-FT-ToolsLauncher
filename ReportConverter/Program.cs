using ReportConverter.XmlReport;
using GUITestReport = ReportConverter.XmlReport.GUITest.TestReport;
using APITestReport = ReportConverter.XmlReport.APITest.TestReport;
using BPTReport = ReportConverter.XmlReport.BPT.TestReport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    class Program
    {
        private const string XMLReport_File = "run_results.xml";
        private const string XMLReport_SubDir_Report = "Report";

        private static ExitCode.ExitCodeData _lastReadInputErrorCode = ExitCode.Success;

        static void Main(string[] args)
        {
            string[] parseErrors;
            CommandArguments arguments = ArgHelper.Instance.ParseCommandArguments(args, out parseErrors);

            // set output verbose level
            if (arguments.ExtraVerbose)
            {
                OutputWriter.SetVerboseLevel((int)OutputVerboseLevel.ExtraVerbose);
            }
            else if (arguments.Verbose)
            {
                OutputWriter.SetVerboseLevel((int)OutputVerboseLevel.Verbose);
            }

            bool isTitlePrinted = false;
            // errors when parsing arguments
            if (parseErrors != null && parseErrors.Length > 0)
            {
                OutputWriter.WriteTitle();
                OutputWriter.WriteLines(parseErrors);
                isTitlePrinted = true;
            }

            // show help?
            if (args.Length == 0 || arguments.ShowHelp)
            {
                OutputWriter.WriteTitle();
                OutputWriter.WriteCommandUsage();
                ProgramExit.Exit(ExitCode.Success);
                return;
            }

            // show version?
            if (arguments.ShowVersion)
            {
                OutputWriter.WriteVersion();
                ProgramExit.Exit(ExitCode.Success);
                return;
            }

            if (!isTitlePrinted)
            {
                OutputWriter.WriteTitle();
            }
            Convert(arguments);
        }

        static void Convert(CommandArguments args)
        {
            // input
            IEnumerable<TestReportBase> testReports = ReadInput(args);

            // output - none
            if (args.OutputFormats == OutputFormats.None)
            {
                ProgramExit.Exit(ExitCode.UnknownOutputFormat, true);
                return;
            }

            // output - junit
            if ((args.OutputFormats & OutputFormats.JUnit) == OutputFormats.JUnit)
            {
                ConvertToJunit(args, testReports);
            }

            // output - nunit 3
            if ((args.OutputFormats & OutputFormats.NUnit3) == OutputFormats.NUnit3)
            {
            }

            // output - sqlite
            if ((args.OutputFormats & OutputFormats.Sqlite) == OutputFormats.Sqlite)
            {
                ConvertToSqlite(args, testReports);
            }
        }

        static IEnumerable<TestReportBase> ReadInput(CommandArguments args)
        {
            if (string.IsNullOrWhiteSpace(args.InputPath))
            {
                ProgramExit.Exit(ExitCode.InvalidInput, true);
                yield break;
            }

            foreach (string path in args.AllPositionalArgs)
            {
                TestReportBase testReport = ReadInputInternal(path);
                if (testReport != null)
                {
                    // the path contains a valid report, do not use any of the subdirectories in this path
                    // to do recusive search
                    OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.VerbMsg_RawReportPathFound, path);
                    yield return testReport;
                }
                else if (args.RecursiveSearch)
                {
                    // recursive search is enabled, search recurvisely as this path is not a valid report path
                    var testReports = ReadInputRecursively(args, path, 1);
                    foreach (TestReportBase tReport in testReports)
                    {
                        yield return tReport;
                    }
                }
                else
                {
                    OutputWriter.WriteLine(Properties.Resources.ErrMsg_Input_InvalidFirstReportNode + " " + path);
                    _lastReadInputErrorCode = ExitCode.InvalidInput;
                }
            }

            if (_lastReadInputErrorCode.Code != ExitCode.Success.Code && args.AllPositionalArgs.Count == 1 && !args.RecursiveSearch)
            {
                // only one test report and it failed to read, exit
                ProgramExit.Exit(_lastReadInputErrorCode);
                yield break;
            }
        }

        static IEnumerable<TestReportBase> ReadInputRecursively(CommandArguments args, string path, int depth)
        {
            if (depth > args.RecursiveSearchDepth)
            {
                yield break;
            }

            OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.VerbMsg_RecusiveSearchingPath, path);

            string[] subPathList = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            if (subPathList != null)
            {
                foreach (string subPath in subPathList)
                {
                    TestReportBase testReport = ReadInputInternal(subPath);
                    if (testReport != null)
                    {
                        // the path contains a valid report, do not use any of the subdirectories in this path
                        // to do recusive search
                        OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.VerbMsg_RawReportPathFound, subPath);
                        yield return testReport;
                    }
                    else
                    {
                        // this path is not a valid report, search recurvisely
                        var testReports = ReadInputRecursively(args, subPath, depth + 1);
                        foreach (TestReportBase tReport in testReports)
                        {
                            yield return tReport;
                        }
                    }
                }
            }
        }

        static TestReportBase ReadInputInternal(string path)
        {
            string xmlReportFile = path;
            if (!File.Exists(xmlReportFile))
            {
                // the input path could be a folder, try to detect it
                string dir = xmlReportFile;
                xmlReportFile = Path.Combine(dir, XMLReport_File);
                if (!File.Exists(xmlReportFile))
                {
                    // still not find, may be under "Report" sub folder? try it
                    xmlReportFile = Path.Combine(dir, XMLReport_SubDir_Report, XMLReport_File);
                    if (!File.Exists(xmlReportFile))
                    {
                        OutputWriter.WriteVerboseLine(OutputVerboseLevel.ExtraVerbose, Properties.Resources.WarnMsg_CannotFindXmlReportFile, path);
                        _lastReadInputErrorCode = ExitCode.FileNotFound;
                        return null;
                    }
                }
            }

            // load XML from file with the specified XML schema
            ResultsType root = XmlReportUtilities.LoadXmlFileBySchemaType<ResultsType>(xmlReportFile);
            if (root == null)
            {
                _lastReadInputErrorCode = ExitCode.CannotReadFile;
                return null;
            }

            // try to load the XML data as a GUI test report
            GUITestReport guiReport = new GUITestReport(root, xmlReportFile);
            if (guiReport.TryParse())
            {
                return guiReport;
            }

            // try to load as API test report
            APITestReport apiReport = new APITestReport(root, xmlReportFile);
            if (apiReport.TryParse())
            {
                return apiReport;
            }

            // try to load as BP test report
            BPTReport bptReport = new BPTReport(root, xmlReportFile);
            if (bptReport.TryParse())
            {
                return bptReport;
            }

            return null;
        }

        static void ConvertToJunit(CommandArguments args, IEnumerable<TestReportBase> inputTestReports)
        {
            // the output JUnit path must be NOT an exist directory
            if (Directory.Exists(args.JUnitXmlFile))
            {
                OutputWriter.WriteLine(Properties.Resources.ErrMsg_JUnit_OutputCannotDir);
                ProgramExit.Exit(ExitCode.InvalidArgument);
                return;
            }

            // if not an aggregation report output, then only convert for the first report
            if (!args.Aggregation)
            {
                TestReportBase testReport = inputTestReports.First();
                if (testReport == null)
                {
                    ProgramExit.Exit(ExitCode.CannotReadFile);
                    return;
                }

                // the output JUnit file path must be NOT same as the input file
                FileInfo fiInput = new FileInfo(testReport.ReportFile);
                FileInfo fiOutput = new FileInfo(args.JUnitXmlFile);
                if (fiInput.FullName == fiOutput.FullName)
                {
                    OutputWriter.WriteLine(Properties.Resources.ErrMsg_JUnit_OutputSameAsInput);
                    ProgramExit.Exit(ExitCode.InvalidArgument);
                    return;
                }

                // convert
                if (!JUnit.Converter.ConvertAndSave(args, testReport))
                {
                    ProgramExit.Exit(ExitCode.GeneralError);
                }
                else
                {
                    OutputWriter.WriteLine(Properties.Resources.InfoMsg_JUnit_OutputGenerated, fiOutput.FullName);
                }
            }
            else
            {
                // an aggregation report output
                if (!JUnit.Converter.ConvertAndSaveAggregation(args, inputTestReports))
                {
                    ProgramExit.Exit(ExitCode.GeneralError);
                }
                else
                {
                    FileInfo fiOutput = new FileInfo(args.JUnitXmlFile);
                    OutputWriter.WriteLine(Properties.Resources.InfoMsg_JUnit_OutputGenerated, fiOutput.FullName);
                }
            }
        }

        static void ConvertToSqlite(CommandArguments args, IEnumerable<TestReportBase> inputTestReports)
        {
            // the output Sqlite DB file path must be NOT an exist directory
            if (Directory.Exists(args.SqliteDBFile))
            {
                OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_OutputCannotDir);
                ProgramExit.Exit(ExitCode.InvalidArgument);
                return;
            }

            if (!Sqlite.Converter.ConvertAndSave(args, inputTestReports))
            {
                ProgramExit.Exit(ExitCode.GeneralError);
            }
            else
            {
                FileInfo fiOutput = new FileInfo(args.SqliteDBFile);
                OutputWriter.WriteLine(Properties.Resources.InfoMsg_Sqlite_OutputGenerated, fiOutput.FullName);
            }
        }
    }
}
