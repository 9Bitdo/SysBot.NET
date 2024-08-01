using SysBot.Base;

namespace SysBot.Pokemon.Discord;

public class DCRequests : MyRequests
{
    public DCRequests(string Authorization) : base()
    {
        // Set request to Header
        this.client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        // Setup an Authorization of your own Discord account which is equivalent to the account password.
        this.client.DefaultRequestHeaders.Add("Authorization", Authorization);
    }
}
