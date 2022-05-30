using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PatientPortal.Models
{
    public partial class Donor
    {
        public Donor()
        {
            Swaps = new HashSet<Swap>();
        }

        public ulong DonorId { get; set; }
		[StringLength(200)]
		public string Name { get; set; } = null!;

		[DonorSexValidation]
		public string Sex { get; set; } = null!;

		[Range(0, 100, ErrorMessage = "The Age should be between 0 and 100 years")]
		public int Age { get; set; }

		[DonorBloodTypeValidation]
		[Display(Name = "Blood Type")]
		public string BloodType { get; set; } = null!;

		[StringLength(300)]
		[Display(Name = "Past History")]
		public string? PastHistory { get; set; }

		[StringLength(100)]
		public string? City { get; set; }

		[StringLength(100)]
		public string State { get; set; } = null!;

		[DonorRelationshipValidation]
		[Display(Name = "Relationship with Patient")]
		public string PatientRelation { get; set; } = null!;
		public ulong? FamilyPatientId { get; set; }

        public virtual Patient? FamilyPatient { get; set; }
        public virtual ICollection<Swap> Swaps { get; set; }
    }

	public class DonorBloodTypeValidation : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var donor = (Donor)validationContext.ObjectInstance;
			if (donor.BloodType == null)
				return new ValidationResult("Blood Type is required.");

			string[] bloodTypes = new string[] { "A", "B", "AB", "O" };

			return bloodTypes.Contains(donor.BloodType) ? ValidationResult.Success : new ValidationResult($"Given Blood Type {donor.BloodType} is not Valid");
		}
	}

	public class DonorRelationshipValidation : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var donor = (Donor)validationContext.ObjectInstance;
			if (donor.PatientRelation == null)
				return new ValidationResult("Relationship to Patient is required.");

			string[] relationships = new string[] { "father", "mother", "husband", "wife", "brother", "sister" };

			return relationships.Contains(donor.PatientRelation) ? ValidationResult.Success : new ValidationResult($"Given Relationship {donor.PatientRelation} is not Valid");
		}
	}
	public class DonorSexValidation : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var donor = (Donor)validationContext.ObjectInstance;
			if (donor.Sex == null)
				return new ValidationResult("Sex is required.");

			string[] sexes = new string[] { "male", "female", "others" };
			return sexes.Contains(donor.Sex) ? ValidationResult.Success : new ValidationResult($"Given sex {donor.Sex} is not Valid");
		}
	}
}
