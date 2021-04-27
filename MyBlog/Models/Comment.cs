using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyBlog.Models
{
    public class Comment
    {
        public int id { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Comment Text")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public string PublishDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Publish Time")]
        public string PublishTime { get; set; }

        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
