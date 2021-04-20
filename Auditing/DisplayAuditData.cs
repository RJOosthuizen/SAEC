using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DisplayAudit.Function
{
    public static class DisplayAuditData
    {
        [FunctionName("DisplayAuditData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Get request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);


            //check if data has been received
            if (data == null)
            {
                return new OkObjectResult("No Audit Data.");
            }

            //store 1 json record at a time (eg a record has been created and deleted)
            var auditRecords = new List<string>();
            //loop through received data and add to list
            foreach (var value in data)
            {
                //get value of column1 (the audit record)
                auditRecords.Add(Convert.ToString(value["AuditDataRecord"]));
            }

            //final htmlstring
            string htmlString = "";
            //temp htmlstring to capture values
            string htmlTempString = "";
            //loop through list to go through all json records
            foreach (var auditRecord in auditRecords) //eg created json record, updated json record, deleted json record
            //eg {Audit1}, {Audit2} ...
            {
                data = JsonConvert.DeserializeObject(auditRecord); //convert the json record to dynamic type

                //get all the values of the individual json record
                foreach (var value in data)
                {
                    htmlTempString = htmlTempString + "<td style='text-align:left; padding:8px;background-color:#CCCCCC;color:black;'>" + value.Value + "</td>"; //get values from inside the json record (eg SortOrderGroupCode's value of 2)
                }
                htmlTempString = htmlTempString + "</tr><tr>"; //add new row in table

            }
            //get the keys of the json for the last record(all keys of the json records should be the same)
            foreach (var key in data)
            {
                htmlString = htmlString + "<th style='text-align:left;padding:8px;background-color:black;color:white;'>" + key.Name + "</th>"; //get last keys eg SortOrderGroupCode
            }
            //add together
            //htmlString = "<table border=1 cellspacing=0 cellpadding=0><tr>" + htmlString + "</tr><tr>" + htmlTempString + "</tr></table>";
            htmlString = "<div style='overflow:auto;'><table border=1 cellspacing=0 cellpadding=0 style='border-collapse:collapse;width:100%;'><tr>" + htmlString + "</tr><tr>" + htmlTempString + "</tr></table></div>";


            return new OkObjectResult(htmlString);
        }
    }
}
