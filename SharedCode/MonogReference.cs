using System;

using MongoDB.Driver;
using MongoDB.Bson;

namespace Kvin.Shorter
{
    public class MongoStuff
    {
        private readonly string _connectionString;
        private readonly string _databaseName = "short";
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        public MongoStuff()
        {
            _connectionString = System.Environment.GetEnvironmentVariable(Constants.ConfigKeys.MongoConnectionString, EnvironmentVariableTarget.Process);
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));
            settings.SslSettings = new SslSettings() {EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12};
            this._mongoClient = new MongoClient(settings);
            _mongoDatabase = _mongoClient.GetDatabase(_databaseName);

        }

        public IMongoDatabase Database {get { return _mongoDatabase;}}
    }

}
