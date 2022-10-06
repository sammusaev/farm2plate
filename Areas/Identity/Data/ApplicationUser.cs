using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using farm2plate.Models;
using Microsoft.AspNetCore.Identity;

namespace farm2plate.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }
    }

    // Used for dropdown list during registration
    public enum UserType
    {
        Vendor, 
        Customer
    }
}
