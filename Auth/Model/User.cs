﻿using Microsoft.AspNetCore.Identity;

namespace Bakalauras.Auth.Model
{
    public class BookieUser : IdentityUser
    {
        public bool isBlocked { get; set; }
    }
}
