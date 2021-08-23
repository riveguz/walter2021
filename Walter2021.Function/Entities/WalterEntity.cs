using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Walter2021.Function.Entities
{
    public class WalterEntity : TableEntity
    {
        public DateTime CreateTime { get; set; }

        public string TaskDescription { get; set; }

        public bool IsCompleted { get; set; }
    }
}
