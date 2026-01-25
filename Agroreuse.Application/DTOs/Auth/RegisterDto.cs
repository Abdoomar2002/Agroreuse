using Agroreuse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agroreuse.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string? Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; }
        public UserType Type { get; set; }
    }

    /// <summary>
    /// Registration DTO for Farmers - requires phone number, email is optional
    /// </summary>
    public class FarmerRegisterDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }

    /// <summary>
    /// Registration DTO for Factories - requires email, phone number is optional
    /// </summary>
    public class FactoryRegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
