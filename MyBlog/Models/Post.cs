using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class Post
    {
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name ="Post Title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Post Description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(1024)]
        [Display(Name = "Post Content")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public string PublishDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Publish Time")]
        public string PublishTime { get; set; }

        [MaxLength(100)]
        [Display(Name = "Post Image")]
        public string ImagePath { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual List<Comment> Comments { get; set; }
    }
}
