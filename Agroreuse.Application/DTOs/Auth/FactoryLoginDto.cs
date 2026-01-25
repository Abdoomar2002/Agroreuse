using System;
using System.Collections.Generic;
using System.Text;

namespace Agroreuse.Application.DTOs.Auth
{
    public class FactoryLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class FarmerLoginDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
