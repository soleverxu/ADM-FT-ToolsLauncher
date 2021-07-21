using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB.Schema_1_0.Tables
{
    [Table(nameof(UFTGUISIDProperty))]
    partial class UFTGUISIDProperty
    {
        [TableColumn("id", TableColumnDataType.Integer)]
        [TableColumnConstraint(PrimaryKeyConstraint = true, PrimaryKeyAutoIncrement = true)]
        public long ID { get; set; }

        [TableColumn("hierarchy_id", TableColumnDataType.Integer)]
        [TableColumnConstraint(NotNullConstraint = true, ForeignKeyConstraint = true,
            ForeignKeyRefTableName = nameof(UFTGUIHierarchy), ForeignKeyRefTableColumnName = "id")]
        public long? HierarchyID { get; set; }

        [TableColumn("category", TableColumnDataType.Text)]
        public string Category { get; set; }

        [TableColumn("name", TableColumnDataType.Text)]
        public string Name { get; set; }

        [TableColumn("value", TableColumnDataType.Blob)]
        public string Value { get; set; }

        [TableColumn("info", TableColumnDataType.Blob)]
        public string Info { get; set; }

        [TableColumn("matches", TableColumnDataType.Integer)]
        public int? Matches { get; set; }
    }

    partial class UFTGUISIDProperty
    {
        public static UFTGUISIDProperty CreateDataObject(XmlReport.SIDBasicPropertyExtType basicSID, UFTGUIHierarchy hierarchyDataObject)
        {
            if (basicSID == null || hierarchyDataObject == null)
            {
                return null;
            }

            return new UFTGUISIDProperty
            {
                HierarchyID = hierarchyDataObject.ID,
                Category = UFTGUISIDCategory.BasicSID,
                Name = basicSID.Name,
                Value = basicSID.Value,
                Info = null,
                Matches = null
            };
        }

        public static UFTGUISIDProperty CreateDataObject(XmlReport.SIDOptionalPropertyExtType optSID, UFTGUIHierarchy hierarchyDataObject)
        {
            if (optSID == null || hierarchyDataObject == null)
            {
                return null;
            }

            return new UFTGUISIDProperty
            {
                HierarchyID = hierarchyDataObject.ID,
                Category = UFTGUISIDCategory.OptionalSID,
                Name = optSID.Name,
                Value = optSID.Value,
                Info = optSID.Info,
                Matches = optSID.Matches
            };
        }
    }
}
