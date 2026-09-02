using Microsoft.AspNetCore.SignalR;

namespace QuebecInsider.Api.Hubs;

public sealed class TrendingHub : Hub
{
    public Task JoinTrendingRoom() => Groups.AddToGroupAsync(Context.ConnectionId, "trending");
}
