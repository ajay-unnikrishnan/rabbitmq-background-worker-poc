using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace RabbitmqBackgroundWorkerPoc.Worker
{
    public static class ColumnOptionsFactory
    {
        public static ColumnOptions Create()
        {
            return new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn("ProcessId", SqlDbType.NVarChar, dataLength: 100)
                },
                Store = new Collection<StandardColumn>
                {
                    StandardColumn.Message,
                    StandardColumn.MessageTemplate,
                    StandardColumn.Level,
                    StandardColumn.TimeStamp,
                    StandardColumn.Exception,
                    StandardColumn.Properties,
                    StandardColumn.LogEvent
                }
            };
        }
    }
}
