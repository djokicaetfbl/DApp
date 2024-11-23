namespace API.SignalR
{
    public class PresenceTracker // da vidimo ko je trenutno logovan
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>(); // username, connectionIds (sa razlicitih uredjaja telefon, laptop itd itd)

        //Dictionary nije thread safe tip objekta
        public Task<bool> UserConnected(string username, string connectionId)
        {
            //Ovo zaključavanje osigurava da se pristup kolekciji OnlineUsers vrši na siguran način kada više niti
            //pristupa ovoj metodi istovremeno

            //orištenje lock u C# je mehanizam za sinhronizaciju koji osigurava da samo jedna nit (thread) istovremeno
            //može pristupiti određenom dijelu koda, što je ključno kada više niti pokušavaju istovremeno izvršiti
            //operacije nad zajedničkim resursima

            //Ključna ideja: Kada jedna nit (thread) uđe u kod koji je okružen lock-om, druge niti moraju čekati dok
            //prva nit ne završi, odnosno dok se lock ne oslobodi. Ovo sprječava probleme kao što su race conditions
            //(trke niti), gdje više niti istovremeno pokušava promijeniti podatke, što može dovesti do nepredvidljivog
            //ponašanja ili oštećenih podataka.

            //Kada jedna nit zaključa OnlineUsers, druge niti moraju čekati dok se prva nit ne završi s radom u bloku lock-a.

            // unutar loka mogu biti samo referentni tipovi

            bool isOnline = false;

            lock (OnlineUsers) 
            {
                if(OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string>() { connectionId });
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId) 
        {
            bool isOffline = false;

            lock (OnlineUsers) 
            { 
                if(!OnlineUsers.ContainsKey(username))
                    return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers) 
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key)
                    .Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;

            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}
