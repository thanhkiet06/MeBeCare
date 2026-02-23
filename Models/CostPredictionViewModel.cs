using System.ComponentModel.DataAnnotations;

namespace MeBeCare.Models
{
    public class CostPredictionViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn loại chi phí")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời gian")]
        [Range(1, 60, ErrorMessage = "Thời gian phải từ 1 đến 60 tháng")]
        public int DurationInMonths { get; set; }

        public decimal PredictedCost { get; set; }

        // Chi tiết từng mục chi phí
        public decimal MedicationCost { get; set; }
        public decimal DoctorFee { get; set; }
        public decimal LabTestCost { get; set; }
        public decimal VaccinationCost { get; set; }
        public decimal OtherCost { get; set; }

        // Chi phí sinh đẻ
        public string? ChildbirthType { get; set; } // Normal, C-section
        public decimal HospitalStayCost { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal AnesthesiaCost { get; set; }
        public decimal PostpartumCareCost { get; set; }

        public int UltrasoundCost { get; set; }        
        public int SupplementCost { get; set; }
        public int AmbulanceCost { get; set; }         
        public int BabyCareCost { get; set; }         

    }
}
