using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUICheckpointProperty))]
    partial class UFTGUICheckpointProperty
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("checkpoint_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUICheckpoint), ForeignKeyRefTableColumnName = "id")]
        public long? CheckpointID { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }

        [TableColumn("std_prop_name", TableColumnDataType.Text)]
        public string StdPropName { get; set; }

        [TableColumn("std_prop_expected_value", TableColumnDataType.Blob)]
        public string StdPropValueExpected { get; set; }

        [TableColumn("std_prop_actual_value", TableColumnDataType.Blob)]
        public string StdPropValueActual { get; set; }

        [TableColumn("stdimg_prop_name", TableColumnDataType.Text)]
        public string StdImagePropName { get; set; }

        [TableColumn("stdimg_prop_expected_value", TableColumnDataType.Blob)]
        public string StdImagePropValueExpected { get; set; }

        [TableColumn("stdimg_prop_actual_value", TableColumnDataType.Blob)]
        public string StdImagePropValueActual { get; set; }

        [TableColumn("stdimg_is_check_pass", TableColumnDataType.Integer)]
        public int? StdImageIsCheckPassValue { get; set; }

        [TableColumn("stdimg_is_regex", TableColumnDataType.Integer)]
        public int? StdImageIsRegexValue { get; set; }

        [TableColumn("stdimg_is_use_formula", TableColumnDataType.Integer)]
        public int? StdImageIsUseFormulaValue { get; set; }

        public bool? StdImageIsCheckPass
        {
            get { return StdImageIsCheckPassValue > 0; }
            set
            {
                if (value == null)
                {
                    StdImageIsCheckPassValue = null;
                }
                else
                {
                    StdImageIsCheckPassValue = value == true ? 1 : 0;
                }
            }
        }

        public bool? StdImageIsRegex
        {
            get { return StdImageIsRegexValue > 0; }
            set
            {
                if (value == null)
                {
                    StdImageIsRegexValue = null;
                }
                else
                {
                    StdImageIsRegexValue = value == true ? 1 : 0;
                }
            }
        }

        public bool? StdImageIsUseFormula
        {
            get { return StdImageIsUseFormulaValue > 0; }
            set
            {
                if (value == null)
                {
                    StdImageIsUseFormulaValue = null;
                }
                else
                {
                    StdImageIsUseFormulaValue = value == true ? 1 : 0;
                }
            }
        }
    }

    partial class UFTGUICheckpointProperty
    {
        public static UFTGUICheckpointProperty CreateDataObject(
            XmlReport.CheckpointPropertyExtType checkpointProperty,
            UFTGUICheckpoint checkpointDataObject,
            long index
            )
        {
            if (checkpointProperty == null || checkpointDataObject == null)
            {
                return null;
            }

            return new UFTGUICheckpointProperty
            {
                CheckpointID = checkpointDataObject.ID,
                Index = index,
                StdPropName = checkpointProperty.StdChkPointPropertyName,
                StdPropValueExpected = checkpointProperty.StdChkPointPropertyExpectedValue,
                StdPropValueActual = checkpointProperty.StdChkPointPropertyActualValue,
                StdImagePropName = checkpointProperty.ImageChkPointPropertyName,
                StdImagePropValueExpected = checkpointProperty.ImageChkPointPropertyExpectedValue,
                StdImagePropValueActual = checkpointProperty.ImageChkPointPropertyActualValue,
                StdImageIsCheckPass = checkpointProperty.ImageChkPointPropertyCheckPass,
                StdImageIsRegex = checkpointProperty.ImageChkPointPropertyIsRegExp,
                StdImageIsUseFormula = checkpointProperty.ImageChkPointPropertyIsUseFormula
            };
        }
    }
}
