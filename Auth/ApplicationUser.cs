using Microsoft.AspNetCore.Identity;

namespace InventoryManager.Model;

public class ApplicationUser:IdentityUser
{
    public ApplicationUser(string userName, string email) : base(userName)
    {
        base.Email = email;
    }
}