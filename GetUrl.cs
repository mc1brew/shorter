using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using MongoDB.Driver;
using MongoDB.Bson;

namespace Kvin.Shorter
{
    public static class GetUrl
    {
        [FunctionName("GetUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];
            
            var db = new MongoStuff();
            var collection = db.Database.GetCollection<BsonDocument>("bar");

            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var document = collection.Find(filter).FirstOrDefault();

            if(document == null || document["Url"] == null)
                return  new RedirectResult("http://www.k.vin");
            return new RedirectResult(document["Url"].ToString());
        }
    }

    public class MongoStuff
    {
        private readonly string _connectionString;
        private readonly string _databaseName = "short";
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        public MongoStuff()
        {
            _connectionString = System.Environment.GetEnvironmentVariable("MongoConnectionString", EnvironmentVariableTarget.Process);
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));
            settings.SslSettings = new SslSettings() {EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12};
            this._mongoClient = new MongoClient(settings);
            _mongoDatabase = _mongoClient.GetDatabase(_databaseName);

        }

        public IMongoDatabase Database {get { return _mongoDatabase;}}
    }
}
