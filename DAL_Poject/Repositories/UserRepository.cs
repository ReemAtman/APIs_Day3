
using DAL_Poject.Data.Context;
using DAL_Poject.Data.Models;

namespace DAL_Poject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProfileDBContext context;

        public UserRepository(ProfileDBContext context)
        {
            this.context = context;
        }
                
    }
}
