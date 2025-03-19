using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationSystem.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100, ErrorMessage = "Role name cannot be longer than 100 characters")]
        public string Name { get; set; }

        public int? ParentRoleId { get; set; }
        
        // Navigation property for parent role
        public virtual Role ParentRole { get; set; }
        
        // Navigation property for child roles
        public virtual ICollection<Role> Children { get; set; } = new List<Role>();

        // Additional properties for role management
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string Description { get; set; }
    }
}