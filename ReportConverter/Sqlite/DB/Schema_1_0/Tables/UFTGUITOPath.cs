using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUITOPath))]
    partial class UFTGUITOPath
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("step_hierarchy_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIStepHierarchy), ForeignKeyRefTableColumnName = "id")]
        public long? StepHierarchyID { get; set; }

        [TableColumn("name", TableColumnDataType.Text)]
        public string Name { get; set; }

        [TableColumn("type", TableColumnDataType.Text)]
        public string Type { get; set; }

        [TableColumn("index", TableColumnDataType.Integer)]
        public long? Index { get; set; }
    }

    partial class UFTGUITOPath
    {
        public static UFTGUITOPath CreateDataObject(
            XmlReport.TestObjectPathObjectExtType toPathObj,
            UFTGUIStepHierarchy stepHierarchyDataObject,
            long index
            )
        {
            if (toPathObj == null || stepHierarchyDataObject == null)
            {
                return null;
            }

            return new UFTGUITOPath
            {
                StepHierarchyID = stepHierarchyDataObject.ID,
                Name = toPathObj.Name,
                Type = toPathObj.Type,
                Index = index
            };
        }
    }
}
