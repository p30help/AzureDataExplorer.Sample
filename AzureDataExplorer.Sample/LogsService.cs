using Kusto.Cloud.Platform.Data;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuickStart;

namespace AzureDataExplorer.Sample
{
    public interface ILogsService
    {
        Task CreateTableAsync();

        Task DropTableAsync();

        Task IngestInlineAsync(string type, string message);

        Task IngestListAsync(List<LogEntity> entities);

        Task DeleteRecordsAsync(Guid logId);

        Task<int> CountAsync(string type);

        Task ClearAllRecords();

        Task<List<LogEntity>> GetRecords();
        
    }

    public class LogsService : ILogsService
    {

        private AppConfig _appConfig;

        public LogsService(IOptionsMonitor<AppConfig> appConfig)
        {
            _appConfig = appConfig.CurrentValue;
        }

        const string TableName = "logs";

        public async Task<List<LogEntity>> GetRecords()
        {
            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(createKustoConnectionString())) // For regular querying
            {
                var query = $"{TableName}";

                // It is strongly recommended that each request has its own unique
                // request identifier. This is mandatory for some scenarios (such as cancelling queries)
                // and will make troubleshooting easier in others.
                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                using (var reader = await queryProvider.ExecuteQueryAsync(_appConfig.DatabaseName, query, clientRequestProperties))
                {
                    var json = JsonConvert.SerializeObject(reader.ToDataSet().Tables[0]);

                    return JsonConvert.DeserializeObject<List<LogEntity>>(json);
                }
            }
        }

        public Task CreateTableAsync()
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(createKustoConnectionString())) // For control commands
            {
                //var command = $".create table {TableName} (Id:string, RecordDate:datetime, Type:string, Message:string)";

                var command =
                    CslCommandGenerator.GenerateTableCreateCommand(
                        TableName,
                        new[]
                        {
                            Tuple.Create("Id", "System.String"),
                            Tuple.Create("RecordDate", "System.DateTime"),
                            Tuple.Create("Type", "System.String"),
                            Tuple.Create("Message", "System.string")
                        });

                return adminClient.ExecuteControlCommandAsync(_appConfig.DatabaseName, command);
            }
        }

        public Task DropTableAsync()
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(createKustoConnectionString())) // For control commands
            {
                var command = $".drop table {TableName}";

                return adminClient.ExecuteControlCommandAsync(_appConfig.DatabaseName, command);
            }
        }

        public Task IngestInlineAsync(string type, string message)
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(createKustoConnectionString())) // For control commands
            {
                var cmd = $".ingest inline into table {TableName} <|{Guid.NewGuid()},{DateTime.Now},{type},{message}";

                return adminClient.ExecuteControlCommandAsync(_appConfig.DatabaseName, cmd);
            }
        }

        public Task IngestListAsync(List<LogEntity> entities)
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(createIngestConnectionString())) // For control commands
            {
                // must be completed
            }

            return Task.CompletedTask;
        }

        public Task DeleteRecordsAsync(Guid logId)
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(createKustoConnectionString())) // For control commands
            {
                var command = $".delete table {TableName} records  <| {TableName} | where Id == '{logId}'";

                return adminClient.ExecuteControlCommandAsync(_appConfig.DatabaseName, command);
            }
        }

        public async Task<int> CountAsync(string type)
        {
            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(createKustoConnectionString())) // For regular querying
            {
                var query = $"{TableName} | where Type == '{type}' | count";

                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                using (var reader = await queryProvider.ExecuteQueryAsync(_appConfig.DatabaseName, query, clientRequestProperties))
                {
                    var result = reader.ToJObjects().ToArray();
                    return result.First().Value<int>("Count");
                }
            }
        }

        public Task ClearAllRecords()
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(createKustoConnectionString())) // For control commands
            {
                var command = $".clear table {TableName} data";

                return adminClient.ExecuteControlCommandAsync(_appConfig.DatabaseName, command);
            }
        }

        private KustoConnectionStringBuilder createKustoConnectionString()
        {
            var kustoConnectionString = Utils.Authentication.GenerateConnectionString(_appConfig.KustoUri,
                AuthenticationModeOptions.AppKey, _appConfig.AppClientId, _appConfig.AppClintSecret, _appConfig.AppTenantId);

            return kustoConnectionString;
        }

        private KustoConnectionStringBuilder createIngestConnectionString()
        {
            var ingestConnectionString = Utils.Authentication.GenerateConnectionString(_appConfig.IngestUri,
                AuthenticationModeOptions.AppKey, _appConfig.AppClientId, _appConfig.AppClintSecret, _appConfig.AppTenantId);

            return ingestConnectionString;
        }


    }
}
