using GoodsExchangeAtFUManagement.Models;
using System.Collections.Concurrent;

namespace GoodsExchangeAtFUManagement.DataService
{
    public class SharedDB
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connections = new();
        public ConcurrentDictionary<string, UserConnection> connections => _connections;
    }
}
