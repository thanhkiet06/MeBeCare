using System;
using System.Collections.Generic;

namespace MeBeCare.Models
{
    public class Family
    {
        public int FamilyID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public int PrimaryUserID { get; set; }
        public User? PrimaryUser { get; set; }
        public List<Child>? Children { get; set; }
    }
}