
using Microsoft.AspNetCore.Identity;

namespace DAL_Poject.Data.Models
{
    public class Student : IdentityUser
    {
        public int Age { get; set; }
    }
}
