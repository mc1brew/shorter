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
    public static class GetLink
    {
        [FunctionName(nameof(GetLink))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(GetLink)} function initiated.");
            string key = req.Query[$"{nameof(Link.Key)}"];
            
            var db = new MongoStuff();
            var collection = db.Database.GetCollection<BsonDocument>("bar");

            var filter = Builders<BsonDocument>.Filter.Eq($"{nameof(Link.Key)}", key);
            var document = collection.Find(filter).FirstOrDefault();

            if(document == null)
                return new NotFoundResult();

            Link linkResult = new Link {TargetUrl = document[$"{nameof(Link.TargetUrl)}"].ToString()};

            return new OkObjectResult(linkResult);
        }
    }
}