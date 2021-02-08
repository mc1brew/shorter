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

    public static class AddUrl
    {
        [FunctionName("AddUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
            {
                log.LogInformation("Add Url function initiated.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string name = data?.Name;
                string url = data?.Url;

                if(string.IsNullOrEmpty(name))
                {
                    log.LogError("Name was null.");
                    throw new ArgumentNullException("Name was null.");
                }

                if(string.IsNullOrEmpty(url))
                {
                    log.LogError("Url was null.");
                    throw new ArgumentNullException("Url was null.");
                }

                log.LogInformation($"Creating link with Name: {name} and Url: {url}.");

                try{
                    MongoStuff db = new MongoStuff();
                    var collection = db.Database.GetCollection<BsonDocument>("bar");

                    var document = new BsonDocument
                    {
                        { "Name", name},
                        { "Url", url}
                    };

                    collection.InsertOne(document);
                }
                catch(Exception ex)
                {
                    log.LogError(ex, ex.Message.ToString());
                    throw ex;
                }

                return new OkResult();
            }
    }

}