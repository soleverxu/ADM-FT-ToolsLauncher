using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DBSchema_1_0 = ReportConverter.Sqlite.DB.Schema_1_0;

namespace ReportConverter.Sqlite
{
    static class Converter
    {
        private const string SCHEMA_VERSION = "1.0";
        private static readonly Type[] _tableSchemaTypes = new Type[]
        {
            typeof(DBSchema_1_0.Tables.Metadata),
            typeof(DBSchema_1_0.Tables.TestResult),
            typeof(DBSchema_1_0.Tables.TestResultElement),
            typeof(DBSchema_1_0.Tables.TestParameter),
            typeof(DBSchema_1_0.Tables.TestAUT),
            typeof(DBSchema_1_0.Tables.TestAUTAddition),
            typeof(DBSchema_1_0.Tables.UFTGUIIteration),
            typeof(DBSchema_1_0.Tables.UFTGUIAction),
            typeof(DBSchema_1_0.Tables.UFTGUIActionIteration),
            typeof(DBSchema_1_0.Tables.UFTGUIStepHierarchy),
            typeof(DBSchema_1_0.Tables.UFTGUITOPath),
            typeof(DBSchema_1_0.Tables.UFTGUISIDProperty),
            typeof(DBSchema_1_0.Tables.UFTGUICheckpoint),
            typeof(DBSchema_1_0.Tables.UFTGUICheckpointProperty),
        };

        public static bool ConvertAndSave(CommandArguments args, IEnumerable<XmlReport.TestReportBase> input)
        {
            try
            {
                OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.VerbMsg_Sqlite_PreConvert);

                // initialize database
                if (!DB.Database.Initialize(args.SqliteDBFile))
                {
                    return false;
                }

                // create tables with predefined table schema types
                if (!DB.Database.CreateTable(_tableSchemaTypes))
                {
                    return false;
                }

                // fill metadata table
                if (!FillMetadataTable())
                {
                    return false;
                }

                // fill data to tables for each test report
                foreach (XmlReport.TestReportBase testReport in input)
                {
                    // the output Sqlite file path must be NOT same as the input file
                    FileInfo fiInput = new FileInfo(testReport.ReportFile);
                    FileInfo fiOutput = new FileInfo(args.SqliteDBFile);
                    if (fiInput.FullName == fiOutput.FullName)
                    {
                        OutputWriter.WriteLine(Properties.Resources.ErrMsg_Sqlite_OutputSameAsInput);
                        ProgramExit.Exit(ExitCode.InvalidArgument);
                        return false;
                    }

                    // fill data to tables for general test report info
                    DBSchema_1_0.Tables.TestResult testResultDataObject;
                    if (!FillTablesForGeneralTestReportInfo(testReport, out testResultDataObject))
                    {
                        return false;
                    }

                    // convert for specific test type
                    if (testReport is XmlReport.GUITest.TestReport guiTestReport)
                    {
                        if (!ConvertGUITestReport(guiTestReport, testResultDataObject))
                        {
                            return false;
                        }
                    }

                    OutputWriter.WriteVerboseLine(OutputVerboseLevel.Verbose, Properties.Resources.VerbMsg_Sqlite_PostProcTestResult, testReport.TestAndReportName);
                }

                return true;
            }
            finally
            {
                DB.Database.Close();
            }
        }

        #region Convert - GUI Test report
        private static bool ConvertGUITestReport(
            XmlReport.GUITest.TestReport guiTestReport,             // raw data
            DBSchema_1_0.Tables.TestResult testResultDataObject     // related test result data object
            )
        {
            if (guiTestReport == null || testResultDataObject == null)
            {
                return false;
            }

            // iterations
            foreach (var iterationReportNode in guiTestReport.Iterations)
            {
                // fill data to tables for iteration report node
                DBSchema_1_0.Tables.UFTGUIIteration iterationDataObject;
                if (!FillTablesForGUIIteration(iterationReportNode, testResultDataObject, out iterationDataObject))
                {
                    return false;
                }

                // Actions
                long actionIndex = 0;
                foreach (var actionReportNode in iterationReportNode.Actions)
                {
                    actionIndex++;
                    if (!ConvertGUITestActionReport(actionReportNode, testResultDataObject, iterationDataObject, actionIndex))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ConvertGUITestActionReport(
            XmlReport.GUITest.ActionReport actionReport,                    // raw data
            DBSchema_1_0.Tables.TestResult testResultDataObject,            // related test result data object
            DBSchema_1_0.Tables.UFTGUIIteration iterationDataObject,        // related iteration data object
            long index,                                                     // index of actions that all have same ancestors (starts with 1)
            DBSchema_1_0.Tables.UFTGUIAction parentActionDataObject = null  // parent action data object
            )
        {
            if (actionReport == null || testResultDataObject == null)
            {
                return false;
            }

            // fill data to tables for action report node
            DBSchema_1_0.Tables.UFTGUIAction actionDataObject;
            if (!FillTablesForGUIAction(actionReport, testResultDataObject, iterationDataObject, parentActionDataObject, index, out actionDataObject))
            {
                return false;
            }

            // sub Actions
            long subActionIndex = 0;
            foreach (var subActionReportNode in actionReport.SubActions)
            {
                subActionIndex++;
                if (!ConvertGUITestActionReport(subActionReportNode, testResultDataObject, iterationDataObject, subActionIndex, actionDataObject))
                {
                    return false;
                }
            }

            // Action Iterations
            foreach (var actionIterationReportNode in actionReport.ActionIterations)
            {
                if (!ConvertGUITestActionIterationReport(actionIterationReportNode, testResultDataObject, iterationDataObject, actionDataObject))
                {
                    return false;
                }
            }

            // Contexts
            foreach (var contextReportNode in actionReport.Contexts)
            {
                if (!ConvertGUITestContextReport(contextReportNode, testResultDataObject, iterationDataObject, actionDataObject))
                {
                    return false;
                }
            }

            // Steps
            foreach (var stepReportNode in actionReport.Steps)
            {
                if (!ConvertGUITestStepReport(stepReportNode, testResultDataObject, iterationDataObject, actionDataObject))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ConvertGUITestActionIterationReport(
            XmlReport.GUITest.ActionIterationReport actionIterationReport,  // raw data
            DBSchema_1_0.Tables.TestResult testResultDataObject,            // related test result data object
            DBSchema_1_0.Tables.UFTGUIIteration iterationDataObject,        // related iteration data object
            DBSchema_1_0.Tables.UFTGUIAction actionDataObject               // related action data object
            )
        {
            if (actionIterationReport == null || testResultDataObject == null)
            {
                return false;
            }

            // fill data to tables for action iteration report node
            DBSchema_1_0.Tables.UFTGUIActionIteration actionIterationDataObject;
            if (!FillTablesForGUIActionIteration(actionIterationReport, testResultDataObject, iterationDataObject, actionDataObject, out actionIterationDataObject))
            {
                return false;
            }

            // Contexts
            foreach (var contextReportNode in actionIterationReport.Contexts)
            {
                if (!ConvertGUITestContextReport(contextReportNode, testResultDataObject, iterationDataObject, actionDataObject, actionIterationDataObject))
                {
                    return false;
                }
            }

            // Steps
            foreach (var stepReportNode in actionIterationReport.Steps)
            {
                if (!ConvertGUITestStepReport(stepReportNode, testResultDataObject, iterationDataObject, actionDataObject, actionIterationDataObject))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ConvertGUITestContextReport(
            XmlReport.GUITest.ContextReport contextReport,                              // raw data
            DBSchema_1_0.Tables.TestResult testResultDataObject,                        // related test result data object
            DBSchema_1_0.Tables.UFTGUIIteration iterationDataObject,                    // related iteration data object
            DBSchema_1_0.Tables.UFTGUIAction actionDataObject,                          // related action data object
            DBSchema_1_0.Tables.UFTGUIActionIteration actionIterationDataObject = null, // related action iteration data object
            DBSchema_1_0.Tables.UFTGUIStepHierarchy parentDataObject = null             // parent step hierarchy data object
            )
        {
            if (contextReport == null || testResultDataObject == null)
            {
                return false;
            }

            // fill data to tables for context report node
            DBSchema_1_0.Tables.UFTGUIStepHierarchy shrDataObject;
            if (!FillTablesForGUIContext(contextReport, testResultDataObject, iterationDataObject, actionDataObject,
                actionIterationDataObject, parentDataObject, out shrDataObject))
            {
                return false;
            }

            // sub Contexts
            foreach (var subContextReportNode in contextReport.SubContexts)
            {
                if (!ConvertGUITestContextReport(subContextReportNode, testResultDataObject, iterationDataObject, 
                    actionDataObject, actionIterationDataObject, shrDataObject))
                {
                    return false;
                }
            }

            // Steps
            foreach (var stepReportNode in contextReport.Steps)
            {
                if (!ConvertGUITestStepReport(stepReportNode, testResultDataObject, iterationDataObject,
                    actionDataObject, actionIterationDataObject, shrDataObject))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ConvertGUITestStepReport(
            XmlReport.GUITest.StepReport stepReport,                                    // raw data
            DBSchema_1_0.Tables.TestResult testResultDataObject,                        // related test result data object
            DBSchema_1_0.Tables.UFTGUIIteration iterationDataObject,                    // related iteration data object
            DBSchema_1_0.Tables.UFTGUIAction actionDataObject,                          // related action data object
            DBSchema_1_0.Tables.UFTGUIActionIteration actionIterationDataObject = null, // related action iteration data object
            DBSchema_1_0.Tables.UFTGUIStepHierarchy parentDataObject = null             // parent step hierarchy data object
            )
        {
            if (stepReport == null || testResultDataObject == null)
            {
                return false;
            }

            // fill data to tables for step report node
            DBSchema_1_0.Tables.UFTGUIStepHierarchy shrDataObject;
            if (!FillTablesForGUIStep(stepReport, testResultDataObject, iterationDataObject, actionDataObject,
                actionIterationDataObject, parentDataObject, out shrDataObject))
            {
                return false;
            }

            // sub Steps
            foreach (var subStepReportNode in stepReport.SubSteps)
            {
                if (!ConvertGUITestStepReport(subStepReportNode, testResultDataObject, iterationDataObject,
                    actionDataObject, actionIterationDataObject, shrDataObject))
                {
                    return false;
                }
            }

            // Contexts
            foreach (var contextReportNode in stepReport.Contexts)
            {
                if (!ConvertGUITestContextReport(contextReportNode, testResultDataObject, iterationDataObject,
                    actionDataObject, actionIterationDataObject, shrDataObject))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Fill tables
        private static bool FillMetadataTable()
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();

            // insert data to Metadata table
            return DB.Database.InsertTable(
                DBSchema_1_0.Tables.Metadata.CreateDataObject(1, "schema_version", SCHEMA_VERSION),
                DBSchema_1_0.Tables.Metadata.CreateDataObject(101, "tool_name", assemblyName.Name),
                DBSchema_1_0.Tables.Metadata.CreateDataObject(102, "tool_version", assemblyName.Version.ToString()),
                DBSchema_1_0.Tables.Metadata.CreateDataObject(103, "tool_vendor", Properties.Resources.CompanyName),
                DBSchema_1_0.Tables.Metadata.CreateDataObject(201, "create_unixtime", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                );
        }

        private static bool FillTablesForGeneralTestReportInfo(
            XmlReport.TestReportBase testReport,                        // raw data
            out DBSchema_1_0.Tables.TestResult testResultDataObject     // [out] data object after inserted to tables
            )
        {
            if (testReport == null)
            {
                throw new ArgumentNullException(nameof(testReport));
            }

            // TestResult data object, do insert first to fetch the auto-increment Id
            testResultDataObject = DBSchema_1_0.Tables.TestResult.CreateDataObject(testReport);
            if (!DB.Database.InsertTable(testResultDataObject))
            {
                return false;
            }

            // TestParameter data objects
            List<object> testParamDataObjects = new List<object>();
            if (testReport.TestInputParameters != null)
            {
                foreach (var parameter in testReport.TestInputParameters)
                {
                    testParamDataObjects.Add(DBSchema_1_0.Tables.TestParameter.CreateDataObject(testResultDataObject, DBSchema_1_0.TestParameterDirection.Input, parameter));
                }
            }
            if (testReport.TestOutputParameters != null)
            {
                foreach (var parameter in testReport.TestOutputParameters)
                {
                    testParamDataObjects.Add(DBSchema_1_0.Tables.TestParameter.CreateDataObject(testResultDataObject, DBSchema_1_0.TestParameterDirection.Output, parameter));
                }
            }
            if (!DB.Database.InsertTable(testParamDataObjects.ToArray()))
            {
                return false;
            }

            // TestAUT data objects
            if (testReport.TestAUTs != null)
            {
                foreach (var aut in testReport.TestAUTs)
                {
                    // create data object and do insert since we need to process the additional info of AUT
                    var autDataObject = DBSchema_1_0.Tables.TestAUT.CreateDataObject(testResultDataObject, aut);
                    if (!DB.Database.InsertTable(autDataObject))
                    {
                        return false;
                    }

                    // TestAUTAddition data objects
                    if (aut.AdditionalInfos != null && aut.AdditionalInfos.Length > 0)
                    {
                        List<object> autAdditionDataObjects = new List<object>(aut.AdditionalInfos.Length);
                        for (int i = 0; i < aut.AdditionalInfos.Length; i++)
                        {
                            autAdditionDataObjects.Add(DBSchema_1_0.Tables.TestAUTAddition.CreateDataObject(autDataObject, aut.AdditionalInfos[i], i + 1));
                        }

                        if (!DB.Database.InsertTable(autAdditionDataObjects.ToArray()))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool FillTablesForGeneralReportNodeInfo(
            XmlReport.GeneralReportNode reportNode,                                     // raw data
            string testResultElementType,                                               // element type of the test result element
            DBSchema_1_0.Tables.TestResult ownerTestResultDataObject,                   // referenced owner - test result data object
            out DBSchema_1_0.Tables.TestResultElement testResultElementDataObject       // [out] data object after inserted to tables
            )
        {
            if (reportNode == null)
            {
                throw new ArgumentNullException(nameof(reportNode));
            }
            if (ownerTestResultDataObject == null)
            {
                throw new ArgumentNullException(nameof(ownerTestResultDataObject));
            }
            if (string.IsNullOrWhiteSpace(testResultElementType))
            {
                throw new ArgumentException("Invalid testResultElementType");
            }

            // TestResultElement data object, do insert first to fetch the auto-increment Id
            testResultElementDataObject = DBSchema_1_0.Tables.TestResultElement.CreateDataObject(ownerTestResultDataObject, reportNode, testResultElementType);
            if (!DB.Database.InsertTable(testResultElementDataObject))
            {
                return false;
            }

            // TestParameter data objects
            List<object> testParamDataObjects = new List<object>();
            if (reportNode.InputParameters != null)
            {
                foreach (var parameter in reportNode.InputParameters)
                {
                    testParamDataObjects.Add(DBSchema_1_0.Tables.TestParameter.CreateDataObject(
                        ownerTestResultDataObject, 
                        testResultElementDataObject, 
                        DBSchema_1_0.TestParameterDirection.Input, 
                        parameter));
                }
            }
            if (reportNode.OutputParameters != null)
            {
                foreach (var parameter in reportNode.OutputParameters)
                {
                    testParamDataObjects.Add(DBSchema_1_0.Tables.TestParameter.CreateDataObject(
                        ownerTestResultDataObject,
                        testResultElementDataObject,
                        DBSchema_1_0.TestParameterDirection.Output,
                        parameter));
                }
            }
            if (!DB.Database.InsertTable(testParamDataObjects.ToArray()))
            {
                return false;
            }

            // TestAUT data objects
            if (reportNode.AUTs != null)
            {
                foreach (var aut in reportNode.AUTs)
                {
                    // create data object and do insert since we need to process the additional info of AUT
                    var autDataObject = DBSchema_1_0.Tables.TestAUT.CreateDataObject(ownerTestResultDataObject, testResultElementDataObject, aut);
                    if (!DB.Database.InsertTable(autDataObject))
                    {
                        return false;
                    }

                    // TestAUTAddition data objects
                    if (aut.AdditionalInfos != null && aut.AdditionalInfos.Length > 0)
                    {
                        List<object> autAdditionDataObjects = new List<object>(aut.AdditionalInfos.Length);
                        for (int i = 0; i < aut.AdditionalInfos.Length; i++)
                        {
                            autAdditionDataObjects.Add(DBSchema_1_0.Tables.TestAUTAddition.CreateDataObject(autDataObject, aut.AdditionalInfos[i], i + 1));
                        }

                        if (!DB.Database.InsertTable(autAdditionDataObjects.ToArray()))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool FillTablesForGUIIteration(
            XmlReport.GUITest.IterationReport iterationReportNode,      // raw data
            DBSchema_1_0.Tables.TestResult ownerTestResultDataObject,   // referenced owner - test result data object
            out DBSchema_1_0.Tables.UFTGUIIteration iterationDataObject // [out] data object after inserted to tables
            )
        {
            iterationDataObject = null;

            // insert data to general tables for iteration report node
            DBSchema_1_0.Tables.TestResultElement elemDataObject;
            if (!FillTablesForGeneralReportNodeInfo(
                iterationReportNode,
                DBSchema_1_0.TestResultElementType.UFT_GUI_Iteration,
                ownerTestResultDataObject,
                out elemDataObject))
            {
                return false;
            }

            // insert data to UFTGUIIteration table
            iterationDataObject = DBSchema_1_0.Tables.UFTGUIIteration.CreateDataObject(
                ownerTestResultDataObject,
                elemDataObject,
                iterationReportNode);
            if (!DB.Database.InsertTable(iterationDataObject))
            {
                return false;
            }

            return true;
        }

        private static bool FillTablesForGUIAction(
            XmlReport.GUITest.ActionReport actionReportNode,                // raw data
            DBSchema_1_0.Tables.TestResult ownerTestResultDataObject,       // ref owner - test result data object
            DBSchema_1_0.Tables.UFTGUIIteration ownerIterationDataObject,   // ref owner - gui iteration data object
            DBSchema_1_0.Tables.UFTGUIAction ownerActionDataObject,         // ref owner - gui action data object
            long index,                                                     // index of actions with same owner iteration and test result
            out DBSchema_1_0.Tables.UFTGUIAction actionDataObject           // [out] data object after inserted to tables
            )
        {
            actionDataObject = null;

            // insert data to general tables for action report node
            DBSchema_1_0.Tables.TestResultElement elemDataObject;
            if (!FillTablesForGeneralReportNodeInfo(
                actionReportNode,
                DBSchema_1_0.TestResultElementType.UFT_GUI_Action,
                ownerTestResultDataObject,
                out elemDataObject))
            {
                return false;
            }

            // insert data to UFTGUIAction table
            actionDataObject = DBSchema_1_0.Tables.UFTGUIAction.CreateDataObject(
                ownerTestResultDataObject,
                ownerIterationDataObject,
                elemDataObject,
                index,
                ownerActionDataObject
                );
            if (!DB.Database.InsertTable(actionDataObject))
            {
                return false;
            }

            return true;
        }

        private static bool FillTablesForGUIActionIteration(
            XmlReport.GUITest.ActionIterationReport actionIterationReportNode,      // raw data
            DBSchema_1_0.Tables.TestResult ownerTestResultDataObject,               // ref owner - test result data object
            DBSchema_1_0.Tables.UFTGUIIteration ownerIterationDataObject,           // ref owner - gui iteration data object
            DBSchema_1_0.Tables.UFTGUIAction ownerActionDataObject,                 // ref owner - gui action data object
            out DBSchema_1_0.Tables.UFTGUIActionIteration actionIterationDataObject // [out] data object after inserted to tables
            )
        {
            actionIterationDataObject = null;

            // insert data to general tables for action iteration report node
            DBSchema_1_0.Tables.TestResultElement elemDataObject;
            if (!FillTablesForGeneralReportNodeInfo(
                actionIterationReportNode,
                DBSchema_1_0.TestResultElementType.UFT_GUI_Action_Iteration,
                ownerTestResultDataObject,
                out elemDataObject))
            {
                return false;
            }

            // insert data to UFTGUIActionIteration table
            actionIterationDataObject = DBSchema_1_0.Tables.UFTGUIActionIteration.CreateDataObject(
                actionIterationReportNode,
                ownerTestResultDataObject,
                ownerIterationDataObject,
                ownerActionDataObject,
                elemDataObject
                );
            if (!DB.Database.InsertTable(actionIterationDataObject))
            {
                return false;
            }

            return true;
        }

        private static bool FillTablesForGUIContext(
            XmlReport.GUITest.ContextReport contextReportNode,                          // raw data
            DBSchema_1_0.Tables.TestResult ownerTestResultDataObject,                   // ref owner - test result data object
            DBSchema_1_0.Tables.UFTGUIIteration ownerIterationDataObject,               // ref owner - gui iteration data object
            DBSchema_1_0.Tables.UFTGUIAction ownerActionDataObject,                     // ref owner - gui action data object
            DBSchema_1_0.Tables.UFTGUIActionIteration ownerActionIterationDataObject,   // ref owner - gui action iteration data object
            DBSchema_1_0.Tables.UFTGUIStepHierarchy parentDataObject,                   // ref owner - parent hierarchy data object
            out DBSchema_1_0.Tables.UFTGUIStepHierarchy shrDataObject                   // [out] data object after inserted to tables
            )
        {
            shrDataObject = null;

            // insert data to general tables for context report node
            DBSchema_1_0.Tables.TestResultElement elemDataObject;
            if (!FillTablesForGeneralReportNodeInfo(
                contextReportNode,
                DBSchema_1_0.TestResultElementType.UFT_GUI_Context,
                ownerTestResultDataObject,
                out elemDataObject))
            {
                return false;
            }

            // insert data to UFTGUIStepHierarchy table
            shrDataObject = DBSchema_1_0.Tables.UFTGUIStepHierarchy.CreateDataObject(
                ownerTestResultDataObject,
                ownerIterationDataObject,
                ownerActionDataObject,
                ownerActionIterationDataObject,
                elemDataObject,
                parentDataObject
                );
            if (!DB.Database.InsertTable(shrDataObject))
            {
                return false;
            }

            return true;
        }

        private static bool FillTablesForGUIStep(
            XmlReport.GUITest.StepReport stepReportNode,                                // raw data
            DBSchema_1_0.Tables.TestResult ownerTestResultDataObject,                   // ref owner - test result data object
            DBSchema_1_0.Tables.UFTGUIIteration ownerIterationDataObject,               // ref owner - gui iteration data object
            DBSchema_1_0.Tables.UFTGUIAction ownerActionDataObject,                     // ref owner - gui action data object
            DBSchema_1_0.Tables.UFTGUIActionIteration ownerActionIterationDataObject,   // ref owner - gui action iteration data object
            DBSchema_1_0.Tables.UFTGUIStepHierarchy parentDataObject,                   // ref owner - parent hierarchy data object
            out DBSchema_1_0.Tables.UFTGUIStepHierarchy shrDataObject                   // [out] data object after inserted to tables
            )
        {
            shrDataObject = null;

            var checkpoint = XmlReport.GUITest.CheckpointReport.FromStepReport(stepReportNode);

            // element type, step or checkpoint
            string elemType = DBSchema_1_0.TestResultElementType.UFT_GUI_Step;
            if (checkpoint != null)
            {
                elemType = DBSchema_1_0.TestResultElementType.UFT_GUI_Checkpoint;
            }

            // insert data to general tables for step report node
            DBSchema_1_0.Tables.TestResultElement elemDataObject;
            if (!FillTablesForGeneralReportNodeInfo(
                stepReportNode,
                elemType,
                ownerTestResultDataObject,
                out elemDataObject))
            {
                return false;
            }

            // insert data to UFTGUIStepHierarchy table
            shrDataObject = DBSchema_1_0.Tables.UFTGUIStepHierarchy.CreateDataObject(
                stepReportNode,
                ownerTestResultDataObject,
                ownerIterationDataObject,
                ownerActionDataObject,
                ownerActionIterationDataObject,
                elemDataObject,
                parentDataObject
                );
            if (!DB.Database.InsertTable(shrDataObject))
            {
                return false;
            }

            // insert data to UFTGUITOPath table
            if (stepReportNode.TestObjectPathObjects != null)
            {
                List<object> dataObjectList = new List<object>();
                long index = 0;
                foreach (var toPathObj in stepReportNode.TestObjectPathObjects)
                {
                    index++;
                    dataObjectList.Add(DBSchema_1_0.Tables.UFTGUITOPath.CreateDataObject(toPathObj, shrDataObject, index));
                }

                if (!DB.Database.InsertTable(dataObjectList.ToArray()))
                {
                    return false;
                }
            }

            // insert data to UFTGUISIDProperty table
            List<object> sidPropsDataObjectList = new List<object>();
            if (stepReportNode.SmartIdentification?.SIDBasicProperties?.Properties != null)
            {
                foreach (var basicSID in stepReportNode.SmartIdentification.SIDBasicProperties.Properties)
                {
                    sidPropsDataObjectList.Add(DBSchema_1_0.Tables.UFTGUISIDProperty.CreateDataObject(basicSID, shrDataObject));
                }
            }
            if (stepReportNode.SmartIdentification?.SIDOptionalProperties != null)
            {
                foreach (var optSID in stepReportNode.SmartIdentification.SIDOptionalProperties)
                {
                    sidPropsDataObjectList.Add(DBSchema_1_0.Tables.UFTGUISIDProperty.CreateDataObject(optSID, shrDataObject));
                }
            }
            if (!DB.Database.InsertTable(sidPropsDataObjectList.ToArray()))
            {
                return false;
            }

            // insert data to tables if the step is a checkpoint
            if (checkpoint != null)
            {
                // insert data to UFTGUICheckpoint table
                var checkpointDataObject = DBSchema_1_0.Tables.UFTGUICheckpoint.CreateDataObject(checkpoint, shrDataObject);
                if (!DB.Database.InsertTable(checkpointDataObject))
                {
                    return false;
                }

                // insert data to UFTGUICheckpointProperty table
                if (checkpoint.Detail?.Properties != null)
                {
                    long index = 0;
                    List<object> cpPropList = new List<object>();
                    foreach (var prop in checkpoint.Detail.Properties)
                    {
                        index++;
                        cpPropList.Add(DBSchema_1_0.Tables.UFTGUICheckpointProperty.CreateDataObject(prop, checkpointDataObject, index));
                    }

                    if (!DB.Database.InsertTable(cpPropList.ToArray()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
