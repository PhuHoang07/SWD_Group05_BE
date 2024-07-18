using GoodsExchangeAtFUManagement.DataService;
using GoodsExchangeAtFUManagement.Models;
using Microsoft.AspNetCore.SignalR;

namespace GoodsExchangeAtFUManagement.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDB _sharedDB;

        public ChatHub(SharedDB sharedDB)
        {
            _sharedDB = sharedDB;
        }

        public async Task JoinChat(UserConnection connection)
        {
            await Clients.All
                .SendAsync("ReceiveMessage", "admin", $"{connection.UserName} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
            _sharedDB.connections[Context.ConnectionId] = connection;
            await Clients.Group(connection.ChatRoom).SendAsync("ReceiveMessage", "admin", $"{connection.UserName} has joined {connection.ChatRoom}");
        }

        public async Task SendMessage(string msg)
        {
            if (_sharedDB.connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveSpecificMessage", userConnection.UserName, msg);
            }
        }
    }
}
