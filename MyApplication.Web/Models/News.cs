using System;
using System.ComponentModel.DataAnnotations;

namespace MyApplication.Web.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime PublishedDate { get; set; }

        public string? ImagePath { get; set; }
    }
} 