namespace LI4.Hubs;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub {
    public static readonly string URL = "/api/ws/chat";

    public async Task SendMessage(string user, string message) {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
