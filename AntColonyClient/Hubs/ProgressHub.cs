using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AntColonyClient.Hubs
{
    public class ProgressHub : Hub
    {
        public async Task Send(string data,string widthProgress, string BanerMessage)
        {
            await Clients.All.SendAsync("Receive", widthProgress, BanerMessage);
        }
    }
}
