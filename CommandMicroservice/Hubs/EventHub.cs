
namespace CommandMicroservice.Hubs
{
    public class EventHub : Hub
    {
        public async Task SendEvent(EventInfo eventInfo)
        {
            await Clients.All.SendAsync("ReceiveEvent", eventInfo);
        }
    }
}
