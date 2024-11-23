using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker) // Presence Tracker -> (prati) korisnike koji su online
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            var currentUsers = await _tracker.GetOnlineUsers();
           
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers); // dakle klijentima koji su logovani u apliakciju,
                                                                         //na ovaj nacin im se omogucava da update-uju listu aktivnih korisnika!

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId); // ConnectionId se dobija iz WebSocket-a
            if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            
            await base.OnDisconnectedAsync(exception);
        }

        //WebSocket je protokol koji omogućava dvosmjernu komunikaciju između klijenta (npr. web preglednik) i servera preko jedne TCP veze.
        //Za razliku od standardnih HTTP zahtjeva, koji su ograničeni na komunikaciju klijent-server (gdje klijent mora
        //prvo poslati zahtjev da bi dobio odgovor), WebSocket omogućava obostranu razmjenu podataka u stvarnom vremenu,
        //bez potrebe za konstantnim ponovnim uspostavljanjem veze.
    }
}
