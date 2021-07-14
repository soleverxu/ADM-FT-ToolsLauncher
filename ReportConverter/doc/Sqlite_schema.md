# Report Converter - Sqlite Schema
This document describes the Sqlite database schema that is populated by the **Report Converter** when converting one or more test results.

## Sqlite Database
The Sqlite database is populated by [Microsoft.Data.Sqlite][ms-sqlite-provider] which provides a consistent version of SQLite on all platforms.

## Sqlite Tables (Schema 1.0)
The following tables are created and filled in the Sqlite database when the test results are converted by the **Report Converter**.

The table name begins with **Test** generally contains the information for all test results and/or test result elements. 

| Table | Description |
| ---- | ---- |
| [Metadata](#tbl_metadata) | Consists of the properties that describe the metadata of the Sqlite database. |
| [TestResult](#tbl_test_result) | Consists of the information of the test results. |
| [TestResultElement](#tbl_test_result_element) | Consists of the information of all the elements in test results. |
| [TestParameter](#tbl_test_param) | Consists of the parameters of test results and test result elements. |
| [TestAUT](#tbl_test_aut) | Consists of the application under test (AUT) associated with the test results and test result elements. |
| [TestAUTAddition](#tbl_test_aut_addition) | Consists of the additional information of the application under test (AUT) associated with the test results and test result elements. |
| [UFTGUIIteration](#tbl_uft_gui_iteration) | Consists of the information of the **UFT One GUI Test Iteration** test result elements. |
| [UFTGUIAction](#tbl_uft_gui_action) | Consists of the information of the **UFT One GUI Test Action** test result elements. |
| [UFTGUIActionIteration](#tbl_uft_gui_action_iteration) | Consists of the information of the **UFT One GUI Test Action Iteration** test result elements. |
| [UFTGUIContext](#tbl_uft_gui_context) | Consists of the information of the **UFT One GUI Test Context** test result elements. |
| [UFTGUIStep](#tbl_uft_gui_step) | Consists of the information of the **UFT One GUI Test Step** test result elements. |
| [UFTGUITOPath](#tbl_uft_gui_test_obj_path) | Consists of the information of the test object path in **UFT One GUI Test**. |
| [UFTGUISIDProperty](#tbl_uft_gui_sid_prop) | Consists of the smart identification properties associated with the **UFT One GUI Test Step**. |
| [UFTAPIIteration](#tbl_uft_api_iteration) | Consists of the information of the **UFT One API Test Iteration** test result elements. |
| [UFTAPIActivity](#tbl_uft_api_activity) | Consists of the information of the **UFT One API Test Activity** test result elements. |
| [UFTAPIActivityCheckpoint](#tbl_uft_api_activity_checkpoint) | Consists of the information of the **UFT One API Test Activity** checkpoints. |
| [UFTAPIActivityDetail](#tbl_uft_api_activity_detail) | Consists of the detail information of the **UFT One API Test Activity** test result elements. |
| [UFTBPTHierarchy](#tbl_uft_bpt_hierarchy) | Consists of the **UFT One Business Process Test (BPT)** result elements which make up the hierarchy of the BPT tests. |
| [UFTBPTBCStep](#tbl_uft_bpt_bc_step) | Consists of the information of the **UFT One Business Process Test (BPT)** Business Component steps. |
| [UFTBPTBCStepTOPath](#tbl_uft_bpt_bc_step_test_obj_path) | Consists of the information of the test object path in **UFT One Business Process Test (BPT)** Business Component steps. |


### <a name="tbl_metadata"></a>Metadata Table
The **Metadata** table consists of the properties that describe the metadata of the Sqlite database.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| code | INTEGER | **PRIMARY KEY** | The identifier represents the metadata property which is commonly used to fetch the value quickly without involving the property name. |
| name | TEXT | **NOT NULL** | The name of the metadata property. |
| value | TEXT | | The value of the metadata property. |

#### Metadata Properties
The following Metadata properties are populated in the [Metadata table](#tbl_metadata).

| Property Code | Property Name | Sample Value | Remarks |
| ---- | ---- | ---- | ---- |
| 1 | schema_version | `1.0` | The version of the database schema that is filled when populating the Sqlite database. The current version is `1.0`. |
| 101 | tool_name | `ReportConverter` | The name of the tool that populates the Sqlite database. |
| 102 | tool_version | `1.1.2.5` | The version of the tool that populates the Sqlite database. |
| 103 | tool_vendor | `Micro Focus` | The vendor of the tool that populates the Sqlite database. |
| 201 | create_unixtime | `1625802749` | The **Unix time** (the number of seconds since 1970-01-01 00:00:00 UTC) that describes the data and time when the Sqlite database is created. |


### <a name="tbl_test_result"></a>TestResult Table
The **TestResult** table consists of the information of the test results. Each row in the table represents one test result.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test result. |
| input_file | TEXT | **NOT NULL** | The file path from which the test result is read. |
| test_type | TEXT | **NOT NULL** | The type of the test associated with the test result. The available values might be (always in upper case): `UFT_GUI`, `UFT_API`, `UFT_BPT`.<br/><br/>`UFT_GUI` indicates the test is a **Micro Focus UFT One GUI Test**.<br/>`UFT_API` indicates the test is a **Micro Focus UFT One API Test**.<br/>`UFT_BPT` indicates the test is a **Micro Focus UFT One Business Process Test**. |
| test_name | TEXT | **NOT NULL** | The name of the test associated with the test result. |
| result_name | TEXT | **NOT NULL** | The name of the test result. |
| testing_tool_name | TEXT | | The name of the testing tool that populates the test result. |
| testing_tool_version | TEXT | | The version of the testing tool that populates the test result. |
| start_time | INTEGER | **NOT NULL** | The **Unix time** (the number of seconds since 1970-01-01 00:00:00 UTC) that describes the data and time when the test run starts. |
| duration_seconds | REAL | **NOT NULL** | The seconds indicates how long the test is run. |
| env_hostname | TEXT | | The hostname of the machine on which the test is running. |
| env_locale | TEXT | | The locale setting of the machine on which the test is running. |
| env_time_zone | TEXT | | The time zone setting of the machine on which the test is running. |
| env_os | TEXT | | The operating system of the machine on which the test is running. |
| env_cpu | TEXT | | The CPU information of the machine on which the test is running. |
| env_cpu_cores | INTEGER | | The number of the CPU cores of the machine on which the test is running. |
| env_memory | INTEGER | | The total number of the memory of the machine on which the test is running, in MB. |
| env_login_user | TEXT | | The user name that logs in the machine when the test is running on. |


### <a name="tbl_test_result_element"></a>TestResultElement Table
The **TestResultElement** table consists of the information of all the elements in test results.

A **test result element** is a *ReportNode* in the raw test result XML, and together with other elements can construct the entire test result structure. For example, a typical **Micro Focus UFT One GUI Test** result may consist of one or more `TestIteration`, `Action`, and `Step` elements.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test result element. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current test result element. |
| type | TEXT | **NOT NULL** | The type of the test result element which is typically the combination of the test type and the element type, always in upper case. For example, `UFT_GUI_ITERATION`, `UFT_API_ACTIVITY`, `UFT_BPT_FLOW` and so on. |
| name | TEXT | | The name of the test result element. |
| desc | TEXT | | The description of the test result element. |
| status | TEXT | | The run status of the test result element.<br/><br/>The status might be one of the following values (always in lower case): `failed`, `warning`, `information`, `passed`, `done`, *NULL*. |
| start_time | INTEGER | **NOT NULL** | The **Unix time** (the number of seconds since 1970-01-01 00:00:00 UTC) that describes the data and time when the test result element starts. |
| duration_seconds | REAL | **NOT NULL** | The seconds represents the running period of the test result element. |
| error_text | TEXT | | The error text if the **status** is `failed`. |
| error_code | INTEGER | | The error code if the **status** is `failed`. The value `0` means not an error. |


### <a name="tbl_test_param"></a>TestParameter Table
The **TestParameter** table consists of the input and output parameters of test results and test result elements.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test parameter. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current test parameter. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element that owns the current test parameter. If value is *NULL*, the parameter belongs to the test result. |
| direction | TEXT | **NOT NULL** | The direction of the test parameter, either `INPUT` or `OUTPUT`, always in upper case. |
| name | TEXT | | The name of the test parameter. |
| value | TEXT | | The value of the test parameter. |
| type | TEXT | | The value type of the test parameter. The value (always in lower case) might be: `string`, `int` and so on. If *NULL*, the type is `string` by default. |


### <a name="tbl_test_aut"></a>TestAUT Table
The **TestAUT** table consists of the application under test (AUT) associated with the test results and test result elements.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test AUT. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current AUT. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element that owns the current AUT. If value is *NULL*, the AUT belongs to the test result. |
| runtime_engine | TEXT | | The runtime engine information of the AUT. |
| name | TEXT | | The name of the AUT. |
| path | TEXT | | The path of the AUT that is loaded when test is running. |
| version | TEXT | | The version of the AUT. |
| technology | TEXT | | The technology used in the AUT. |
| reserved | TEXT | | The reserved field of the AUT that might be filled by test result generator if necessary. |


### <a name="tbl_test_aut_addition"></a>TestAUTAddition Table
The **TestAUTAddition** table consists of the additional information of the application under test (AUT) associated with the test results and test result elements.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test AUT additional information. |
| aut_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestAUT(id)** | The identifier of the record in the [TestAUT](#tbl_test_aut) table represents the test AUT that owns the current AUT additional information. |
| value | TEXT | | The value of the AUT additional information. |
| index | INTEGER | | The number starts with `0` indicates the index of the AUT additional information for the same test AUT. |


### <a name="tbl_uft_gui_iteration"></a>UFTGUIIteration Table
The **UFTGUIIteration** table consists of the information of the **UFT One GUI Test Iteration** test result elements. The element type is `UFT_GUI_ITERATION`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One GUI Test Iteration**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current iteration. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current iteration. |
| index | INTEGER | **NOT NULL** | The number starts with `1` represents the index of the iteration. |


### <a name="tbl_uft_gui_action"></a>UFTGUIAction Table
The **UFTGUIAction** table consists of the information of the **UFT One GUI Test Action** test result elements. The element type is `UFT_GUI_ACTION`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One GUI Test Action**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current action. |
| iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIIteration(id)** | The identifier of the record in the [UFTGUIIteration](#tbl_uft_gui_iteration) table represents the GUI test iteration that owns the current action. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current action. |
| parent_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIAction(id)** | The identifier of the record in the [UFTGUIAction](#tbl_uft_gui_action) table represents the GUI test action that owns the current action as the parent. |


### <a name="tbl_uft_gui_action_iteration"></a>UFTGUIActionIteration Table
The **UFTGUIActionIteration** table consists of the information of the **UFT One GUI Test Action Iteration** test result elements. The element type is `UFT_GUI_ACTION_ITERATION`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One GUI Test Action Iteration**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current action iteration. |
| iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIIteration(id)** | The identifier of the record in the [UFTGUIIteration](#tbl_uft_gui_iteration) table represents the GUI test iteration that owns the current action iteration. |
| action_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIAction(id)** | The identifier of the record in the [UFTGUIAction](#tbl_uft_gui_action) table represents the GUI test action that owns the current action iteration. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current action iteration. |
| index | INTEGER | **NOT NULL** | The number starts with `1` represents the index of the action iteration. |


### <a name="tbl_uft_gui_context"></a>UFTGUIContext Table
The **UFTGUIContext** table consists of the information of the **UFT One GUI Test Context** test result elements. The element type is `UFT_GUI_CONTEXT`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One GUI Test Context**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current context. |
| iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIIteration(id)** | The identifier of the record in the [UFTGUIIteration](#tbl_uft_gui_iteration) table represents the GUI test iteration that owns the current context. |
| action_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIAction(id)** | The identifier of the record in the [UFTGUIAction](#tbl_uft_gui_action) table represents the GUI test action that owns the current context. |
| action_iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIActionIteration(id)** | The identifier of the record in the [UFTGUIActionIteration](#tbl_uft_gui_action_iteration) table represents the GUI test action iteration that owns the current context. |
| step_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIStep(id)** | The identifier of the record in the [UFTGUIStep](#tbl_uft_gui_step) table represents the GUI test step that owns the current context. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current context. |
| parent_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIContext(id)** | The identifier of the record in the [UFTGUIContext](#tbl_uft_gui_context) table represents the GUI test context that owns the current context as the parent. |


### <a name="tbl_uft_gui_step"></a>UFTGUIStep Table
The **UFTGUIStep** table consists of the information of the **UFT One GUI Test Step** test result elements. The element type is `UFT_GUI_STEP` or `UFT_GUI_CHECKPOINT` (if *is_checkpoint* is `1`).

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One GUI Test Step**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current step. |
| iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIIteration(id)** | The identifier of the record in the [UFTGUIIteration](#tbl_uft_gui_iteration) table represents the GUI test iteration that owns the current step. |
| action_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIAction(id)** | The identifier of the record in the [UFTGUIAction](#tbl_uft_gui_action) table represents the GUI test action that owns the current step. |
| action_iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIActionIteration(id)** | The identifier of the record in the [UFTGUIActionIteration](#tbl_uft_gui_action_iteration) table represents the GUI test action iteration that owns the current step. |
| context_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIContext(id)** | The identifier of the record in the [UFTGUIContext](#tbl_uft_gui_context) table represents the GUI test context that owns the current step. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current step. |
| parent_id | INTEGER | **FOREIGN KEY REFERENCES UFTGUIStep(id)** | The identifier of the record in the [UFTGUIStep](#tbl_uft_gui_step) table represents the GUI test step that owns the current step as the parent. |
| test_obj_path | TEXT | | The full path of the test object associated with the current step. For example, `Window("Notepad").WinMenu("Menu")`. |
| test_obj_oper | TEXT | | The operation of the test object associated with the current step. |
| test_obj_oper_data | TEXT | | The operation data of the test object associated with the current step. |
| is_sid_enabled | INTEGER | | Indicates whether the smart identification is enabled and takes effect. Any number greater than `0` represents **true**. |
| sid_basic_match | INTEGER | | The number of the smart identification properties that match the test object's. |
| is_checkpoint | INTEGER | | Indicates whether the current step represents a checkpoint. The value `1` indicates it is a checkpoint; otherwise, no. |
| checkpoint_fail_desc | TEXT | | The description of the checkpoint if the status is `failed`. |
| checkpoint_type | TEXT | | The type of the checkpoint. |
| checkpoint_subtype | TEXT | | The secondary (sub) type of the checkpoint. |


### <a name="tbl_uft_gui_test_obj_path"></a>UFTGUITOPath Table
The **UFTGUITOPath** table consists of the information of the test object path in **UFT One GUI Test**.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test object path information. |
| step_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES UFTGUIStep(id)** | The identifier of the record in the [UFTGUIStep](#tbl_uft_gui_step) table represents the GUI test step that owns the current test object path. |
| name | TEXT | | The name of the test object path. |
| type | TEXT | | The type of the test object path. For example, `Window` or `Page`. |


### <a name="tbl_uft_gui_sid_prop"></a>UFTGUISIDProperty Table
The **UFTGUISIDProperty** table consists of the smart identification properties associated with the **UFT One GUI Test Step**.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the smart identification property. |
| step_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES UFTGUIStep(id)** | The identifier of the record in the [UFTGUIStep](#tbl_uft_gui_step) table represents the GUI test step that owns the current smart identification property. |
| type | TEXT | **NOT NULL** | The type of the smart identification property, either `BASIC` or `OPTIONAL`, always in upper case. |
| name | TEXT | | The name of the smart identification property. |
| value | TEXT | | The value of the smart identification property. |
| info | TEXT | | The additional information of the smart identification property. |
| matches | INTEGER | | Indicates whether the current smart identification property matches that in the test object. Any number greater than `0` is treated as **true** which means the smart identification property matches. |


### <a name="tbl_uft_api_iteration"></a>UFTAPIIteration Table
The **UFTAPIIteration** table consists of the information of the **UFT One API Test Iteration** test result elements. The element type is `UFT_API_ITERATION`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One API Test Iteration**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current iteration. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current iteration. |
| index | INTEGER | **NOT NULL** | The number starts with `1` represents the index of the iteration. |


### <a name="tbl_uft_api_activity"></a>UFTAPIActivity Table
The **UFTAPIActivity** table consists of the information of the **UFT One API Test Activity** test result elements. The element type is `UFT_API_ACTIVITY`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One API Test Activity**. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current activity. |
| iteration_id | INTEGER | **FOREIGN KEY REFERENCES UFTAPIIteration(id)** | The identifier of the record in the [UFTAPIIteration](#tbl_uft_api_iteration) table represents the iteration that owns the current activity. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current activity. |
| parent_id | INTEGER | **FOREIGN KEY REFERENCES UFTAPIActivity(id)** | The identifier of the record in the [UFTAPIActivity](#tbl_uft_api_activity) table represents the API test activity that owns the current activity as the parent. |
| status | TEXT | | The status of the current activity. The value might be: `failure`, `success`, `done` or *NULL*. |


### <a name="tbl_uft_api_activity_checkpoint"></a>UFTAPIActivityCheckpoint Table
The **UFTAPIActivityCheckpoint** table consists of the information of the **UFT One API Test Activity** checkpoints.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One API Test Activity** checkpoint. |
| activity_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES UFTAPIActivity(id)** | The identifier of the record in the [UFTAPIActivity](#tbl_uft_api_activity) table represents the **UFT One API Test Activity** that owns the current activity checkpoint. |
| status | TEXT | | The status of the current activity checkpoint. The value might be: `failure`, `success`, `done` or *NULL*. |


### <a name="tbl_uft_api_activity_detail"></a>UFTAPIActivityDetail Table
The **UFTAPIActivityDetail** table consists of the detail information of the **UFT One API Test Activity** test result elements.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One API Test Activity** detail item. |
| activity_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES UFTAPIActivity(id)** | The identifier of the record in the [UFTAPIActivity](#tbl_uft_api_activity) table represents the **UFT One API Test Activity** that owns the current detail item. |
| checkpoint_id | INTEGER | **FOREIGN KEY REFERENCES UFTAPIActivityCheckpoint(id)** | The identifier of the record in the [UFTAPIActivityCheckpoint](#tbl_uft_api_activity_checkpoint) table represents the **UFT One API Test Activity** checkpoint that owns the current detail item. |
| name | TEXT | **NOT NULL** | The name of the activity detail item.<br/><br/>The possible names are: `VTD_Type`, `VTD_Name`, `VTD_Message`, `VTD_Status`, `VTD_Details`, `VTD_Operation`, `VTD_Expected`, `VTD_Actual`, `VTD_XPath`, `Alt_Name`, `Comment`. |
| name_code | INTEGER | **NOT NULL** | The number which is the equivalent of the name that is commonly used to fetch the data quickly.<br/><br/>`1` = `VTD_Type`<br/>`2` = `VTD_Name`<br/>`3` = `VTD_Message`<br/>`4` = `VTD_Status`<br/>`5` = `VTD_Details`<br/>`6` = `VTD_Operation`<br/>`7` = `VTD_Expected`<br/>`8` = `VTD_Actual`<br/>`9` = `VTD_XPath`<br/>`10` = `Alt_Name`<br/>`11` = `Comment` |
| alt_name | TEXT | | The alternative name of the activity detail item. |
| code | TEXT | | The code of the activity detail item. |
| value | TEXT | | The value of the activity detail item. |


### <a name="tbl_uft_bpt_hierarchy"></a>UFTBPTHierarchy Table
The **UFTBPTHierarchy** table consists of the **UFT One Business Process Test (BPT)** result elements which make up the hierarchy of the BPT tests. Each row in the table represents a **hierarchy element** which is one of the BPT result elements: `UFT_BPT_ITERATION`, `UFT_BPT_GROUP`, `UFT_BPT_FLOW`, `UFT_BPT_BRANCH`, `UFT_BPT_GENERAL_STEP`, `UFT_BPT_RECOVERY_STEP`, `UFT_BPT_BC`.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One Business Process Test** hierarchy element. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current BPT hierarchy element. |
| elem_id | INTEGER | **FOREIGN KEY REFERENCES TestResultElement(id)** | The identifier of the record in the [TestResultElement](#tbl_test_result_element) table represents the test result element associated with the current BPT hierarchy element. |
| parent_id | INTEGER | **FOREIGN KEY REFERENCES UFTBPTHierarchy(id)** | The identifier of the record in the [UFTBPTHierarchy](#tbl_uft_bpt_hierarchy) table represents the BPT hierarchy element that owns the current BPT hierarchy element as the parent. |
| index | INTEGER | | The number starts with `1` represents the index of the hierarchy items with the same element type under the same parent or *NULL*. |
| branch_case_name | TEXT | | The name of the branch case if the current element type is `UFT_BPT_BRANCH`. |
| branch_case_desc | TEXT | | The description of the branch case if the current element type is `UFT_BPT_BRANCH`. |


### <a name="tbl_uft_bpt_bc_step"></a>UFTBPTBCStep Table
The **UFTBPTBCStep** table consists of the information of the **UFT One Business Process Test (BPT)** Business Component steps.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the **UFT One Business Process Test** Business Component step. |
| result_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES TestResult(id)** | The identifier of the record in the [TestResult](#tbl_test_result) table represents the test result that owns the current BPT Business Component step. |
| bc_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES UFTBPTHierarchy(id)** | The identifier of the record in the [UFTBPTHierarchy](#tbl_uft_bpt_hierarchy) table represents the Business Component that owns the current BPT Business Component step. |
| parent_id | INTEGER | **FOREIGN KEY REFERENCES UFTBPTBCStep(id)** | The identifier of the record in the [UFTBPTBCStep](#tbl_uft_bpt_bc_step) table represents the BPT Business Component step that owns the current BPT Business Component step as the parent. |
| is_context | INTEGER | | Indicates whether the current BPT Business Component step is a **context** step. Any number greater than `0` represents **true**. |
| test_obj_path | TEXT | | The full path of the test object associated with the current BPT Business Component step. For example, `Window("Notepad").WinMenu("Menu")`. |
| test_obj_oper | TEXT | | The operation of the test object associated with the current BPT Business Component step. |
| test_obj_oper_data | TEXT | | The operation data of the test object associated with the current BPT Business Component step. |


### <a name="tbl_uft_bpt_bc_step_test_obj_path"></a>UFTBPTBCStepTOPath Table
The **UFTBPTBCStepTOPath** table consists of the information of the test object path in **UFT One Business Process Test (BPT)** Business Component steps.

| Column | Data Type | Constraints | Description |
| ---- | ---- | ---- | ---- |
| id | INTEGER | **PRIMARY KEY**,<br/>**AUTOINCREMENT** | The identifier of the record in the table represents the test object path information. |
| step_id | INTEGER | **NOT NULL**,<br/>**FOREIGN KEY REFERENCES UFTBPTBCStep(id)** | The identifier of the record in the [UFTBPTBCStep](#tbl_uft_bpt_bc_step) table represents the BPT Business Component step that owns the current test object path. |
| name | TEXT | | The name of the test object path. |
| type | TEXT | | The type of the test object path. For example, `Window` or `Page`. |




[ms-sqlite-provider]: <https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite> 'Microsoft.Data.Sqlite'

