using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IntCube
{
    public static class IntCube
    {
        [FunctionName("IntCube")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string n = req.Query["n"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            n = n ?? data?.n;

            return n != null
                ? (ActionResult)new OkObjectResult(Cube(log, n))
                : new BadRequestObjectResult("Please pass an integer n on the query string or in the request body");
        }


        private static JObject Cube(ILogger log, string input)
        {
            dynamic returnObject = new JObject();
            returnObject.input = input;
            returnObject.operation = "cube";
            int i;
            log.LogInformation($"Input: {input}");
            log.LogInformation($"Operation: cube");
            try
            {
                i = Int32.Parse(input);
            }
            catch (FormatException e)
            {
                returnObject.error = e.Message;
                log.LogError(e.Message);
                return returnObject;
            }
            long result = i * i * i;
            returnObject.result = result;
            log.LogInformation($"Result: {result}");
            return returnObject;
        }
    }
}
