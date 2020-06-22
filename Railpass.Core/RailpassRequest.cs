using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Railpass.Core
{
    public class RailpassRequest {
        public string Insz { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
    }

    public class RailpassRequestEntity : ITableEntity
    {
        public string Insz { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            
            Insz = RowKey;
            Email = properties["Email"].StringValue;
            Firstname = properties["Firstname"].StringValue;
            Lastname = properties["Lastname"].StringValue;
            if (properties.ContainsKey("Address"))
            {
                Address = properties["Address"].StringValue;
            }
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return new Dictionary<string, EntityProperty>
            {
                { "Address", new EntityProperty(Address) },
                { "Insz", new EntityProperty(Insz) },
                { "Email", new EntityProperty(Email) },
                { "Firstname", new EntityProperty(Firstname) },
                { "Lastname", new EntityProperty(Lastname) },
            };
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
        public string Address { get; set; }
    }
}
