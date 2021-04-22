using System;

namespace Goober.Config.DAL.Entities
{
    public class ConfigRowResult
    {
        public int Id { get; set; }

        public string Environment { get; set; }

        public string Application { get; set; }

        public string Key { get; set; }

        public string ParentKey { get; set; }

        public string Value { get; set; }

        public DateTime? RowCreatedDate { get; set; }

        public DateTime? RowChangedDate { get; set; }

        public string ChangedUserName { get; set; }
    }
}
