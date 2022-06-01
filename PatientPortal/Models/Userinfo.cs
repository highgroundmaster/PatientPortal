using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public partial class Userinfo
    {
        public Userinfo()
        {
            Patients = new HashSet<Patient>();
        }

        public ulong UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailId { get; set; } = null!;

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Phone Number (WhatsApp)")]
        public string PhoneNumber { get; set; } = null!;

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
