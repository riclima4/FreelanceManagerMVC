using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Base;
using FreelanceManager.IO.Clients;

namespace FreelanceManager.Data
{
    public class Client : EntityBase
    {
        public Client() { }

        public Client(ClientModel model, int number)
        {
            Code = model.Code;
            Name = model.Name;
            FiscalNumber = model.FiscalNumber;
            Street = model.Street;
            Address = model.Address;
            ZipCode = model.ZipCode;
            City = model.City;
            Country = model.Country;
            Website = model.Website;
            Email = model.Email;
            Phone = model.Phone;
            Notes = model.Notes;
            InternalNumber = number;
            IsActive = model.IsActive;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string FiscalNumber { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Address { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int InternalNumber { get; set; }
    }
}