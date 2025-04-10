using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreelanceManager.IO.Clients
{
    public class ClientModel
    {
        public Guid? Id { get; set; }
        [Required, MaxLength(10)]
        public string Code { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(20)]
        public string FiscalNumber { get; set; }
        [Required, MaxLength(200)]
        public string Street { get; set; }
        [Required, MaxLength(200)]
        public string Address { get; set; }
        [Required, MaxLength(100)]
        public string ZipCode { get; set; }
        [Required, MaxLength(100)]
        public string City { get; set; }
        [Required, MaxLength(100)]
        public string Country { get; set; }
        [MaxLength(100)]
        public string Website { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
        public bool IsActive { get; set; }

    }
}