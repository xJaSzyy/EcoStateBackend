using EcoState.Domain;
using Microsoft.AspNetCore.Authorization;

namespace EcoState.Extensions;

public class EnumAuthorizeAttribute : AuthorizeAttribute
{
    public EnumAuthorizeAttribute(Role role)
    {
        Roles = role.ToString().Replace(" ", string.Empty);
    }   
}