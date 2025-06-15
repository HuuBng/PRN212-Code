using Data.DTO;

public class UserRepository
{
    private static UserRepository _instance;
    private static readonly object _lock = new object();

    private List<Customer> _customers = new List<Customer>();

    private UserRepository()
    {
        _customers.Add(new Customer
        {
            CustomerID = 1,
            CustomerFullName = "John Doe",
            EmailAddress = "johndoe@gmail.com",
            Password = "123456",
            Telephone = "0123456789",
            CustomerBirthday = new DateTime(1990, 1, 1),
            CustomerStatus = 1
        });

        _customers.Add(new Customer
        {
            CustomerID = 2,
            CustomerFullName = "Walter White",
            EmailAddress = "WalterWhite@gmail.com",
            Password = "123456",
            Telephone = "0987654321",
            CustomerBirthday = new DateTime(1995, 5, 15),
            CustomerStatus = 1
        });

        _customers.Add(new Customer
        {
            CustomerID = 3,
            CustomerFullName = "Whihihaha",
            EmailAddress = "test@gmail.com",
            Password = "1",
            Telephone = "0987654321",
            CustomerBirthday = new DateTime(1995, 5, 15),
            CustomerStatus = 1
        });
    }

    public static UserRepository Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new UserRepository();
                }
            }
            return _instance;
        }
    }

    public Customer GetCustomerByEmailAndPassword(string email, string password)
    {
        return _customers.FirstOrDefault(c =>
            c.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase)
            && c.Password == password
            && c.CustomerStatus == 1);
    }

    public List<Customer> GetAllCustomers() => _customers;
}
