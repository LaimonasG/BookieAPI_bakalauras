﻿using System.ComponentModel.DataAnnotations;

namespace Bakalauras.data.entities
{
    public class Chapter
    {
        public int Id { get; set; }
        [Required]
        public int BookId { get; set; }
        
        public string Name { get; set; }

        public string Content { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
    }
}