using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public partial class Patient
    {
        public Patient()
        {
            Donors = new HashSet<Donor>();
            Swaps = new HashSet<Swap>();
        }

        public ulong PatientId { get; set; }

        [StringLength(200)]
        public string Name { get; set; } = null!;

        [PatientSexValidation]
        public string Sex { get; set; } = null!;

        [Range(0, 150, ErrorMessage = "The Age should be between 0 and 150 years")]
        public int Age { get; set; }

        [PatientBloodTypeValidation]
        public string BloodType { get; set; } = null!;

        [StringLength(300)]
        public string? PastHistory { get; set; }


        [StringLength(100)]
        public string City { get; set; } = null!;

        [StringLength(100)]
        public string State { get; set; } = null!;
        public byte[]? Reports { get; set; }
        public ulong? PatientUserId { get; set; }

        public virtual Userinfo PatientUser { get; set; } = null!;
        public virtual ICollection<Donor> Donors { get; set; }
        public virtual ICollection<Swap> Swaps { get; set; }
    }

    public class PatientBloodTypeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var patient = (Patient)validationContext.ObjectInstance;
            if (patient.BloodType == null)
                return new ValidationResult("Blood Type is required.");

            string[] bloodTypes = new string[] { "A", "B", "AB", "O" };

            return bloodTypes.Contains(patient.BloodType) ? ValidationResult.Success : new ValidationResult($"Given Blood Type {patient.BloodType} is not Valid");
        }
    }

    public class PatientSexValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var patient = (Patient)validationContext.ObjectInstance;
            if (patient.Sex == null)
                return new ValidationResult("Sex is required.");

            string[] sexes = new string[] { "male", "female", "others" };
            return sexes.Contains(patient.Sex) ? ValidationResult.Success : new ValidationResult($"Given sex {patient.Sex} is not Valid");
        }
    }
}

