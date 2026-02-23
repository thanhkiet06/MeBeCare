using System;

namespace MeBeCare.Models
{
    public class Vaccination
    {
        public int VaccinationID { get; set; }
        public int ChildID { get; set; }
        public string? VaccineName { get; set; }
        public DateTime VaccinationDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }

        public Child? Child { get; set; }
    }
}