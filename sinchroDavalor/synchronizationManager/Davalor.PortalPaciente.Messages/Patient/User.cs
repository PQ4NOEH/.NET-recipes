using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.PortalPaciente.Messages.Patient
{
    public class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Patient = new HashSet<PatientAggregate>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        public string Hash { get; set; }

        [StringLength(200)]
        public string TokenHash { get; set; }

        public int RetryCount { get; set; }

        public int Active { get; set; }

        public int Locked { get; set; }

        public int RecordDeleted { get; set; }

        public DateTimeOffset? RecordDeletedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string NewEmail { get; set; }

        [StringLength(128)]
        public string NewEmailCode { get; set; }

        public DateTimeOffset? NewEmailRequest { get; set; }

        [StringLength(128)]
        public string ForgotPasswordCode { get; set; }

        public DateTimeOffset? ForgotPasswordRequest { get; set; }

        public DateTimeOffset LastChangePassword { get; set; }

        public DateTimeOffset? LastLogon { get; set; }

        [StringLength(100)]
        public string RegistrationCode { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public Guid? LanguageId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientAggregate> Patient { get; set; }
    }
}
