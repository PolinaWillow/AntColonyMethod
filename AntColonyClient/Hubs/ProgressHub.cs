using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AntColonyClient.Hubs
{
    public class ProgressHub : Hub
    {
        public async Task Send(string widthProgress, string message)
        {
            await Clients.All.SendAsync("Receive", widthProgress, message);
        }
    }
}
