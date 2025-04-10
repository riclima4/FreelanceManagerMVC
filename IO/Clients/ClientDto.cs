using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.Entities;
using FreelanceManager.IO._shared;

namespace FreelanceManager.IO.Clients
{
    public class ClientDto : BaseDto<Client>
    {
        public ClientDto()
        {
        }
        public ClientDto(Client entity) : base(entity)
        {
            Id = entity.Id;
            Code = entity.Code;
            Name = entity.Name;
            FiscalNumber = entity.FiscalNumber;
            Street = entity.Street;
            Address = entity.Address;
            ZipCode = entity.ZipCode;
            City = entity.City;
            Country = entity.Country;
            Website = entity.Website;
            Email = entity.Email;
            Phone = entity.Phone;
            Notes = entity.Notes;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string FiscalNumber { get; set; }
        public string Street { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
    }
}