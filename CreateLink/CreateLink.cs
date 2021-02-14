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
                Response createLinkResponse = new Response();

                if(string.IsNullOrEmpty(linkToBeCreated?.Key))
                {
                    string msg = $"{nameof(Link.Key)} was null.";
                    log.LogWarning(msg);
                    createLinkResponse.ErrorMessages.Add(msg);
                }

                if(string.IsNullOrEmpty(linkToBeCreated?.TargetUrl))
                {
                    string msg = $"{nameof(Link.TargetUrl)} was null.";
                    log.LogWarning(msg);
                    createLinkResponse.ErrorMessages.Add(msg);
                }
                
                if(createLinkResponse.ErrorMessages.Count == 0)
                {
                    try{
                        MongoStuff db = new MongoStuff();
                        var collection = db.Database.GetCollection<BsonDocument>("bar");
                        var filter = Builders<BsonDocument>.Filter.Eq($"{nameof(Link.Key)}", linkToBeCreated.Key);
                        var document = collection.Find(filter).FirstOrDefault();
                        //Todo: Learn how the exists function works.
                        if(null != document)
                        {
                            string msg = $"{nameof(Link.Key)}: '{linkToBeCreated.Key}' already exists.";
                            log.LogWarning(msg);
                            createLinkResponse.ErrorMessages.Add(msg);
                        }
                    }
                    catch(Exception ex)
                    {
                        log.LogError(ex, ex.Message.ToString());
                        throw ex;
                    }
                }

                if(createLinkResponse.ErrorMessages.Count == 0)
                {
                    log.LogInformation($"Creating {nameof(Link)} with {nameof(Link.Key)}: {linkToBeCreated?.Key} and {nameof(Link.TargetUrl)}: {linkToBeCreated?.TargetUrl}.");

                    try{
                        MongoStuff db = new MongoStuff();
                        var collection = db.Database.GetCollection<BsonDocument>("bar");

                        var document = new BsonDocument
                        {
                            { $"{nameof(Link.Key)}", linkToBeCreated?.Key},
                            { $"{nameof(Link.TargetUrl)}", linkToBeCreated?.TargetUrl}
                        };

                        collection.InsertOne(document);
                    }
                    catch(Exception ex)
                    {
                        log.LogError(ex, ex.Message.ToString());
                        throw ex;
                    }
                    
                    createLinkResponse.Success = true;
                    createLinkResponse.Payload = Newtonsoft.Json.JsonConvert.SerializeObject(linkToBeCreated);
                    return new OkObjectResult(createLinkResponse);
                }
                createLinkResponse.Success = false;
                return new BadRequestObjectResult(createLinkResponse);
            }
    }

}