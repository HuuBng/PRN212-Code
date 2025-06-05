using System.ComponentModel.DataAnnotations;

namespace Core.Objects
{
    public partial class Customer
    {
        public Customer(string? CustomerFullName,
                        string? Telephone,
                        string EmailAddress,
                        DateOnly? CustomerBirthday,
                        byte? CustomerStatus,
                        string Password)
        {
            this.CustomerFullName = CustomerFullName;
            this.Telephone = Telephone;
            this.EmailAddress = EmailAddress;
            this.CustomerBirthday = CustomerBirthday;
            this.CustomerStatus = CustomerStatus;
            this.Password = Password;
        }

        public Customer()
        {
        }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerID { get; set; }

        [StringLength(50, ErrorMessage = "Customer full name cannot exceed 50 characters.")]
        public string? CustomerFullName { get; set; }

        [StringLength(12, ErrorMessage = "Telephone number cannot exceed 12 characters.")]
        public string? Telephone { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string EmailAddress { get; set; }

        public DateOnly? CustomerBirthday { get; set; }

        [Length(1, 1, ErrorMessage = "Customer status must be a single byte value.")]
        public byte? CustomerStatus { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
