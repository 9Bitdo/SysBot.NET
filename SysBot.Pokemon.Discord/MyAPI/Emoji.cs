using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Drawing;
using System.IO;

using Newtonsoft.Json.Linq;
using SysBot.Base;

namespace SysBot.Pokemon.Discord;

public class DiscordEmoji 
{
    // Use object to obtain the information required
    public string channel; // channel's address
    public string Authorization; // personal authorization code
    public string name; // emoji name, recommended to be consistent with PKHeX
    public string id; // EmojiCode
    public string url => $"https://cdn.discordapp.com/emojis/{this.id}.webp?size=128&quality=lossless"; // corresponding URL address
    private DCRequests Requests; // Requests，to simplify code and eliminate multiple instantiations 
    private JArray emojiArray = null; // all the emojiArray


    // Import the emoji name followed by the emoji class
    // Implement Constructor
    // Execute within the constructor InitializeAsync
    // Implement wait method to achieve asynchronous effect because constructor cannot be async Task
    // Arrow functions are used since there is only single line of code
    public DiscordEmoji(string name, string channel = "", string Authorization = "") => InitializeAsync(name, channel, Authorization).Wait();

    // Initialization method add attributes that will be used globally in the future to the object
    // Object is a term, the entire class is an object after 'new'. For example, Requests = new DCRequests(Authorization). Requests is an object.
    // Attribute is a term. The variables contained in the object. For example, this.channel in DiscordEmoji is an attribute.
    // Method is a term, the function contained in the object, such as public async Task<JArray> AllEmojis(), this AllEmojis is the method in DiscordEmoji
    public async Task InitializeAsync(string name="", string channel="", string Authorization="")
    {
        this.Authorization = Authorization == "" ? this.Authorization : Authorization;            
        this.Requests = new DCRequests(this.Authorization); // Instantiate Requests in advance to avoid multiple instantiations and simplify the code
        this.channel = channel == "" ? this.channel : channel;
        this.name = name == "" ? this.name : name;
        this.emojiArray = null; // emojiArray must be updated to reflect the current state of the data whenever an upload, deletion, or modification take place
        this.id = await FindID();
        
    }

    // Obtain uploaded emoji's information
    public async Task<JArray> AllEmojis()
    {
        if (emojiArray != null)
        {
            // Optimize the request timing and avoid repeated requests.
            EchoUtil.Echo("emojiArray is present, skip fetching ");
            return emojiArray;
        }

        // Send a GET request to obtain data (consecutive lines of code in Sysbot.Base Requests written in advance, and simplify into a single of code to implement our function)
        Console.WriteLine($"https://discord.com/api/v9/guilds/{this.channel}/emojis");
        var response = await this.Requests.Get($"https://discord.com/api/v9/guilds/{this.channel}/emojis");

        // Parse data: (ToJson is the method being implement earlier, and it should return json type data, because JArray.Parse parses it)
        try
        {
            emojiArray = JArray.Parse(await response.ToJson());
        }
        catch
        {
            emojiArray = null;
        }

        return emojiArray;
    }

    // Search for the ID corresponding to the emoji
    public async Task<string> FindID()
    {
        // Obtain all emoji information
        JArray emojiArray = await AllEmojis();
        
        // Return null if no information is obtained
        if (emojiArray == null)
            return "";

        // Use Linq syntax to find the corresponding ID
        var ids = emojiArray.Where(x => x["name"].ToString() == this.name).Select(x => x["id"]).ToArray();
        if (ids.Count() == 0)
            return "";
        else
            return ids.LastOrDefault().ToString();
    }

    // Modify the emoji name
    public async Task Update(string newName, string id="")
    {
        // Determine whether there is a specified ID and use a default ID if not specified
        id = id == "" ? this.id : id; 
        
        // Build data
        Dictionary<string, string> data = new()
        {
            {"name", newName},
        };

        // Send PATCH request
        var response = await this.Requests.Patch($"https://discord.com/api/v9/guilds/{this.channel}/emojis/{id}", data);

        // Return success or fail's operation information
        Console.WriteLine( await response.ToJson() );
        // Generate a response on a successive or fail deletion
        Console.WriteLine($"id{this.id},name:{this.name},newName:{newName}");

        // Reinitialize
        await this.InitializeAsync();
        return ;
    }

    // Upload emoji using Base64 format
    public async Task UploadBase64(string base64String)
    {
        // Build image's information
        // base64 = default format for Discord's upload and internet data transmission
        string imageJson = "data:image/jpeg;base64," + base64String;

        // Build data
        Dictionary<string, string> data = new()
        {
            {"image", imageJson},
            {"name", this.name},
        };

        // Instantiate requests
        var response = await this.Requests.Post($"https://discord.com/api/v9/guilds/{this.channel}/emojis", data);

        // Determine whether upload is successful
        // Console.WriteLine( await response.ToJson() );
        // Generate a response on successive upload
        Console.WriteLine( await response.ToJson() );
        // Reinitialize
        await this.InitializeAsync();
        return ;
    }

    // Upload emoji using URL
    public async Task Upload(string picUrl)
    {
        // Download image
        var response = await this.Requests.Get(picUrl);
        var responseBytes = await response.ToBytes();
        // Convert bytes to base64 format
        string base64String = Convert.ToBase64String(responseBytes);
        // Upload image
        await UploadBase64(base64String);

        return ;
    }
    
    // Upload emoji using local image
    public async Task Upload(Bitmap bitmap)
    {
        using ( MemoryStream memoryStream = new MemoryStream() )
        {
            // Save Bitmap object as byte array bytes
            // Bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteArr = memoryStream.ToArray();

            // Convert byte array to Base64 string
            string base64String = Convert.ToBase64String(byteArr);

            await UploadBase64(base64String);
        };

        return ;
    }

    // Delete emoji
    public async Task Delete(string id = "")
    {
        // Default id will be used if no id is specified
        id = id == "" ? this.id : id;

        // Send DELETE request to obtain data
        var response = await this.Requests.Delete($"https://discord.com/api/v9/guilds/{this.channel}/emojis/{id}");

        // Return success or fail's operation information
        Console.WriteLine( await response.ToJson() );

        // Generate a response on a successive or fail deletion

        // Reinitialize
        await this.InitializeAsync();
        return ;
    }

    // Delete all emoji (use with caution)
    public async Task DeleteAll()
    {
        // Obtain all emoji information
        JArray emojiArray = await AllEmojis();

        foreach (var obj in emojiArray)
        {
            string emojiName = obj["name"].ToString();
            string emojiCode = obj["id"].ToString();

            // Skip the process if specific content is included
            if ( emojiName.Contains("type") )
                continue;

            // Delete the corresponding emoji
            await Delete(emojiCode);
            // Set the deletion action to 0.5 seconds, (Discord set a restriction on the timing)
            await Task.Delay(5_00);
        }

        return ;
    }
}
