using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class ApplicationUser: IdentityUser
    {
        public virtual List<Comment> Comments { get; set; }
    }
}
