namespace CommandMicroservice.Models
{
    public class PondData
    {
        [Key]
        public int EntryId { get; set; }

        [Required]
        public int PondId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public double Temperature_C { get; set; }

        [Required]
        public double pH { get; set; }

        [Required]
        public double DissolvedOxygen_g_mL { get; set; }

        [Required]
        public int Turbidity_ntu { get; set; }

        [Required]
        public double Ammonia_g_mL { get; set; }

        [Required]
        public double Nitrite_g_mL { get; set; }

        [Required]
        public int Population { get; set; }

        [Required]
        public double FishLength_cm { get; set; }

        [Required]
        public double FishWeight_g { get; set; }
    }
}
