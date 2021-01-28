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
            log.LogInformation("Get Url function initiated.");
            string name = req.Query["name"];
            
            var db = new MongoStuff();
            var collection = db.Database.GetCollection<BsonDocument>("bar");

            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var document = collection.Find(filter).FirstOrDefault();

            if(document == null || document["Url"] == null)
                return  new RedirectResult(System.Environment.GetEnvironmentVariable(Constants.ConfigKeys.AddUrlPage, EnvironmentVariableTarget.Process));
            return new RedirectResult(document["Url"].ToString());
        }
    }
}