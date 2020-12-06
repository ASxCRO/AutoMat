using AutoMat.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutoMat.Core.Services
{
    public class AutoMatDataStore : IDataStore<Oglas>
    {
        public Task<bool> AddItemAsync(Oglas item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Oglas> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Oglas>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Oglas item)
        {
            throw new NotImplementedException();
        }
    }
}
