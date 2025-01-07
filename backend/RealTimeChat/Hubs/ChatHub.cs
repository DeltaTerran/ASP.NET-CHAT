using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using RealTimeChat.Models;

namespace RealTimeChat.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(string userName, string message);
}

public class ChatHub : Hub<IChatClient>
{
    //private readonly IDistributedCache _cache;

    //public ChatHub(IDistributedCache cache)
    //{
    //    _cache = cache;
    //}
    private readonly IMemoryCache _memoryCache;

    public ChatHub(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public async Task JoinChat(UserConnection connection)
    {

        var cacheKey = Context.ConnectionId;
        _memoryCache.Set(cacheKey, connection, TimeSpan.FromMinutes(30));
        #region Old Code
        //var stringConnection = JsonSerializer.Serialize(connection);
        //var stringConnection = "{\u0022username\u0022:\u0022" + connection.UserName + "\u0022, \u0022ChatRoom\u0022:\u0022" + connection.ChatRoom + "\u0022}";
        //if (string.IsNullOrEmpty(connection.UserName) || string.IsNullOrEmpty(connection.ChatRoom))
        //{
        //    throw new ArgumentException("UserName or ChatRoom is null or empty");
        //}
        //await _cache.SetStringAsync(Context.ConnectionId, stringConnection);
        #endregion

        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        await Clients
            .Group(connection.ChatRoom)
            .ReceiveMessage("Admin", $"{connection.UserName} присоединился к чату");


        var cachedConnection = _memoryCache.Get<UserConnection>(cacheKey);
        if (cachedConnection != null)
        {
        }

    }

    public async Task SendMessage(string message)
    {
        //var stringConnection = await _cache.GetAsync(Context.ConnectionId);
        var cacheKey = Context.ConnectionId;
        var connection = _memoryCache.Get<UserConnection>(cacheKey);

        //var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

        if (connection is not null)
        {
            await Clients
                .Group(connection.ChatRoom)
                .ReceiveMessage(connection.UserName, message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var cacheKey = Context.ConnectionId;
        _memoryCache.Remove(cacheKey);
        //var stringConnection = await _cache.GetAsync(Context.ConnectionId);
        //var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

        //if (connection is not null)
        //{
        //    await _cache.RemoveAsync(Context.ConnectionId);
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);

        //    await Clients
        //        .Group(connection.ChatRoom)
        //        .ReceiveMessage("Admin", $"{connection.UserName} покинул чат");
        //}
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SomeChatRoom");
        await base.OnDisconnectedAsync(exception);
    }
}