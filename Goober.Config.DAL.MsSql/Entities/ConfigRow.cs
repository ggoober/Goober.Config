using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goober.Config.DAL.Entities
{
    [Table("ConfigRows")]
    public class ConfigRow
    {
        public int Id { get; set; }

        public string Environment { get; set; }

        public string Application { get; set; }

        public string Key { get; set; }

        public string ParentKey { get; set; }

        public string Value { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? RowCreatedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? RowChangedDate { get; set; }

        public string ChangedUserName { get; set; }
    }
}
