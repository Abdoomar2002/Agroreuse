using Agroreuse.Domain.Enums;
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
    public class AdminLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class GeneralLoginDto
    {
        public string? Email { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }
}
