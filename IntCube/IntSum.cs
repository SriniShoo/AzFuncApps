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
    public static class IntSum
    {
        [FunctionName("IntSum")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string input1 = req.Query["a"];
            string input2 = req.Query["b"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            input1 ??= data?.a;
            input2 = input2 ?? data?.b;

            return input1 != null && input2 != null
                ? (ActionResult)new OkObjectResult(Sum(log, input1, input2))
                : new BadRequestObjectResult($"a: {input1} and b: {input2}. Please pass integers 'a' & 'b' on the query string or in the request body");
        }

        private static JObject Sum(ILogger log, string a, string b)
        {
            dynamic returnObject = new JObject();
            returnObject.a = a;
            returnObject.b = b;
            returnObject.operation = "sum";
            int i, j;
            log.LogInformation($"Inputs: {a} & {b}");
            log.LogInformation($"Operation: sum");
            try
            {
                i = Int32.Parse(a);
                j = Int32.Parse(b);
            }
            catch (FormatException e)
            {
                returnObject.error = e.Message;
                log.LogError(e.Message);
                return returnObject;
            }
            long result = i + j;
            returnObject.result = result;
            log.LogInformation($"Result: {result}");
            return returnObject;
        }
    }
}
