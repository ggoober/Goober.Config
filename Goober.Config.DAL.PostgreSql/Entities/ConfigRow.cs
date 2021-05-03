using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goober.Config.DAL.PostgreSql.Entities
{
    [Table("ConfigRows", Schema = "public")]
    public class ConfigRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
