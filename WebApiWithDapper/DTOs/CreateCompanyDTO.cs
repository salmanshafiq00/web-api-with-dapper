﻿using System.ComponentModel.DataAnnotations;

namespace WebApiWithDapper.DTOs
{
    public record CreateCompanyDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
