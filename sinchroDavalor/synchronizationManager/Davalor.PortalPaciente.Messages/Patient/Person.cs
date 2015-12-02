
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.PortalPaciente.Messages.Patient
{ 
    public partial class Person
    {
        
        public Person()
        {
            Patient = new HashSet<PatientAggregate>();
            PersonLocation = new HashSet<PersonLocation>();
        }

        public Guid Id { get; set; }

        [StringLength(35)]
        public string Name { get; set; }

        [StringLength(35)]
        public string Surname1 { get; set; }

        [StringLength(35)]
        public string Surname2 { get; set; }

        public int? Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Phone1 { get; set; }

        [StringLength(30)]
        public string Phone2 { get; set; }

        public Guid? DocumentTypeId { get; set; }

        [StringLength(16)]
        public string DocumentIdentifier { get; set; }

        public Guid? NationalityId { get; set; }

        public Guid? TitleId { get; set; }

        public Guid? LanguageId { get; set; }

        public Guid? CurrencyId { get; set; }

        public int Deleted { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<PatientAggregate> Patient { get; set; }

        public virtual ICollection<PersonLocation> PersonLocation { get; set; }

    }


}
