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

    public static class CreateLink
    {
        [FunctionName(nameof(CreateLink))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
            {
                log.LogInformation($"{nameof(CreateLink)} function initiated.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Link linkToBeCreated = JsonConvert.DeserializeObject<Link>(requestBody);

                if(null == linkToBeCreated)
                {
                    string msg = $"The request body received: {requestBody} was unable to be deserialized into a {nameof(Link)} object.";
                    log.LogWarning(msg);
                    return new BadRequestObjectResult(msg);
                }

                if(string.IsNullOrEmpty(linkToBeCreated?.Key))
                {
                    string msg = $"{nameof(Link.Key)} was null.";
                    log.LogWarning(msg);
                    return new BadRequestObjectResult(msg);
                }

                if(string.IsNullOrEmpty(linkToBeCreated?.Url))
                {
                    string msg = $"{nameof(Link.Url)} was null.";
                    log.LogWarning(msg);
                    return new BadRequestObjectResult(msg);
                }

                log.LogInformation($"Creating {nameof(Link)} with {nameof(Link.Key)}: {linkToBeCreated?.Key} and {nameof(Link.Url)}: {linkToBeCreated?.Url}.");

                try{
                    MongoStuff db = new MongoStuff();
                    var collection = db.Database.GetCollection<BsonDocument>("bar");

                    var document = new BsonDocument
                    {
                        { $"{nameof(Link.Key)}", linkToBeCreated?.Key},
                        { $"{nameof(Link.Url)}", linkToBeCreated?.Url}
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