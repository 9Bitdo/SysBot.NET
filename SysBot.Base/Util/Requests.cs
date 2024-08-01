
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;


namespace SysBot.Base;

public class MyRequests
{
    // Initiate properties
    public HttpResponseMessage response = null; // This part will be used in some scenarios that require custom operations.
    public HttpClient client = new(); // This part will be used in some scenarios that require custom operations.

    // Conventional built-in method is not used, ie: post needs to be converted into json instead of directly passing in data.
    // Writing it separately make the code concise and convenient, and support the function of detecting running results.
    public async Task<MyRequests> Get(string url) => await RequestBase(url, "get");
    public async Task<MyRequests> Post(string url, Dictionary<string, string> data)  => await RequestBase(url, "postJson", data);
    public async Task<MyRequests> Delete(string url)  => await RequestBase(url, "delete");
    public async Task<MyRequests> Patch(string url, Dictionary<string, string> data)  => await RequestBase(url, "patch", data);

    // A general method for requests and common content, such as successful detection
    // Return 'this' is used for obtaining different types of properties and calling methods multiple times.
    public async Task<MyRequests> RequestBase(string url, string requestsType, Dictionary<string, string> data = null)
    {
        try
        {
            // Send GET request
            if (requestsType == "get")
            {
                this.response = await this.client.GetAsync(url);
            }
            // Send POST request
            else if (requestsType == "postJson")
            {
                string jsonData = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                this.response = await this.client.PostAsync(url, content);
            }
            // Send DELETE request
            else if (requestsType == "delete")
            {
                this.response = await this.client.DeleteAsync(url);
            }
            // Send PATCH request
            else if (requestsType == "patch")
            {
                string jsonData = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                this.response = await this.client.PatchAsync(url, content);
            }
            
            // Verify if the response is successful
            if ( !response.IsSuccessStatusCode )
            {   
                Console.WriteLine("Request failed with status code: " + this.response.StatusCode);
                this.response = null;
            }
            return this;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Request exception: " + e.Message);
            this.response = null;
            return this;
        }            
    }

    // The type of return result chosen depend on the content of the code being developed
    // Return json result
    public async Task<string> ToJson() => this.response != null ? await this.response.Content.ReadAsStringAsync() : "";
    // Return bytes result
    public async Task<byte[]> ToBytes() => this.response != null ? await this.response.Content.ReadAsByteArrayAsync() : new byte[]{};
}
