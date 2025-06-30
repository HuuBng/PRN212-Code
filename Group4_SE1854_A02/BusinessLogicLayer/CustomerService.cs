using BusinessObjects;
using DataAccessObjects.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore; // Added for EntityState

namespace BusinessLogicLayer
{
    // Service for managing Customer-related business logic
    public class CustomerService
    {
        private UnitOfWork _unitOfWork;

        public CustomerService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Customer AuthenticateCustomer(string email, string password)
        {
            // Note: In a real application, passwords should be securely hashed and compared.
            // For this assignment, we'll follow the direct comparison implied by the provided appsettings.json.
            return _unitOfWork.CustomerRepository.Get(
                c => c.EmailAddress == email && c.Password == password && c.CustomerStatus == 1
            ).FirstOrDefault();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _unitOfWork.CustomerRepository.GetAll();
        }

        public Customer GetCustomerById(int id)
        {
            return _unitOfWork.CustomerRepository.GetById(id);
        }

        public void AddCustomer(Customer customer)
        {
            // Basic validation for Email and Password
            if (string.IsNullOrEmpty(customer.EmailAddress) || string.IsNullOrEmpty(customer.Password))
            {
                throw new ArgumentException("Email and Password are required for a new customer.");
            }

            // Check for existing Email Address
            if (_unitOfWork.CustomerRepository.Get(c => c.EmailAddress == customer.EmailAddress).Any())
            {
                throw new ArgumentException("Customer with this email already exists.");
            }

            // Check for existing Telephone Number
            if (!string.IsNullOrEmpty(customer.Telephone) &&
                _unitOfWork.CustomerRepository.Get(c => c.Telephone == customer.Telephone).Any())
            {
                throw new ArgumentException("Customer with this phone number already exists.");
            }

            customer.CustomerStatus = 1; // Default status for new customers
            _unitOfWork.CustomerRepository.Add(customer);
            _unitOfWork.Save(); // Save changes after adding
        }

        public void UpdateCustomer(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.EmailAddress))
            {
                throw new ArgumentException("Email Address cannot be empty.");
            }

            // Check for email uniqueness if email is changed (excluding current customer)
            var existingCustomerWithEmail = _unitOfWork.CustomerRepository
                                                        .Get(c => c.EmailAddress == customer.EmailAddress && c.CustomerID != customer.CustomerID)
                                                        .FirstOrDefault();
            if (existingCustomerWithEmail != null)
            {
                throw new ArgumentException("Another customer already uses this email address.");
            }

            // Check for telephone number uniqueness if phone is changed (excluding current customer)
            // Only perform this check if the telephone number is provided (not null or empty)
            if (!string.IsNullOrEmpty(customer.Telephone))
            {
                var existingCustomerWithPhone = _unitOfWork.CustomerRepository
                                                            .Get(c => c.Telephone == customer.Telephone && c.CustomerID != customer.CustomerID)
                                                            .FirstOrDefault();
                if (existingCustomerWithPhone != null)
                {
                    throw new ArgumentException("Another customer already uses this phone number.");
                }
            }

            // Fetch the existing entity from the database. This entity is now tracked by the DbContext.
            var existingCustomer = _unitOfWork.CustomerRepository.GetById(customer.CustomerID);
            if (existingCustomer == null)
            {
                throw new ArgumentException($"Customer with ID {customer.CustomerID} not found.");
            }

            // Update modifiable properties on the *tracked* existingCustomer entity
            existingCustomer.CustomerFullName = customer.CustomerFullName;
            existingCustomer.Telephone = customer.Telephone;
            existingCustomer.EmailAddress = customer.EmailAddress; // Now explicitly updating email
            existingCustomer.CustomerBirthday = customer.CustomerBirthday;
            existingCustomer.CustomerStatus = customer.CustomerStatus;
            // Password is handled by a separate method (UpdateCustomerPassword)

            // The GenericRepository.Update method should handle marking the entity state as Modified.
            _unitOfWork.CustomerRepository.Update(existingCustomer);

            _unitOfWork.Save(); // Save changes after updating
        }

        // Dedicated method to update customer password
        public void UpdateCustomerPassword(int customerId, string newPassword)
        {
            var customer = _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} not found.");
            }

            customer.Password = newPassword; // Update the password (in a real app, hash this!)

            // The GenericRepository.Update method should handle marking the entity state as Modified.
            _unitOfWork.CustomerRepository.Update(customer);

            _unitOfWork.Save(); // Save changes
        }

        public void DeleteCustomer(int id)
        {
            // Before deleting, check if the customer has any active booking reservations
            // Business rule: A customer with active bookings cannot be deleted directly
            if (_unitOfWork.BookingReservationRepository.Get(br => br.CustomerID == id && br.BookingStatus == 1).Any()) // Assuming BookingStatus 1 is active
            {
                throw new InvalidOperationException("Cannot delete customer with active booking reservations. Please cancel their bookings first.");
            }

            _unitOfWork.CustomerRepository.Delete(id);
            _unitOfWork.Save(); // Save changes after deleting
        }

        public IEnumerable<Customer> SearchCustomers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllCustomers(); // Return all if search term is empty
            }
            searchTerm = searchTerm.ToLower();
            return _unitOfWork.CustomerRepository.Get(c =>
                (c.CustomerFullName != null && c.CustomerFullName.ToLower().Contains(searchTerm)) ||
                (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(searchTerm)) ||
                (c.Telephone != null && c.Telephone.ToLower().Contains(searchTerm))
            );
        }
    }
}
