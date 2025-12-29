using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        private static ISale s_dalSale;
        private static IProduct s_dalProduct;
        private static ICustomer s_dalCustomer;

        static void Main(string[] args)
        {
            try
            {
                s_dalSale = new SaleImplementation();
                s_dalProduct = new ProductImplementation();
                s_dalCustomer = new CustomerImplementation();

                Initialization.Initialize(s_dalSale, s_dalCustomer, s_dalProduct);

                displayMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        #region Helper Methods (Safe Reading)
        private static int ReadInt(string message)
        {
            int result;
            Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Invalid input. Please enter a whole number: ");
            }
            return result;
        }

        private static double ReadDouble(string message)
        {
            double result;
            Console.Write(message);
            while (!double.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Invalid input. Please enter a valid number: ");
            }
            return result;
        }

        private static DateTime ReadDate(string message)
        {
            DateTime result;
            Console.Write(message);
            while (!DateTime.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Invalid format. Please enter date (yyyy-mm-dd): ");
            }
            return result;
        }
        #endregion

        #region Entity Input Logic (DRY Principle)
        private static Customer InputCustomer(int id = 0)
        {
            Console.WriteLine($"\n--- {(id == 0 ? "New" : "Update")} Customer Details ---");
            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Address: ");
            string addr = Console.ReadLine() ?? "";
            Console.Write("Phone: ");
            string phone = Console.ReadLine() ?? "";
            return new Customer(id, name, addr, phone);
        }

        private static Product InputProduct(int id = 0)
        {
            Console.WriteLine($"\n--- {(id == 0 ? "New" : "Update")} Product Details ---");
            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";
            Categories cat = (Categories)ReadInt("Category (0-dogs, 1-fish, 2-cats, 3-parrots, 4-rabbits, 5-hamsters): ");
            double price = ReadDouble("Price: ");
            int qty = ReadInt("Initial Quantity: ");
            return new Product(id, name, cat, price, qty);
        }

        private static Sale InputSale(int id = 0)
        {
            Console.WriteLine($"\n--- {(id == 0 ? "New" : "Update")} Sale Details ---");
            int prodId = ReadInt("Product ID: ");
            int qty = ReadInt("Required Quantity: ");
            double price = ReadDouble("Discounted Price: ");
            Console.Write("Club Member? (y/n): ");
            bool isClub = Console.ReadLine()?.ToLower() == "y";
            DateTime start = ReadDate("Start Date (yyyy-mm-dd): ");
            DateTime end = ReadDate("End Date (yyyy-mm-dd): ");
            return new Sale(id, prodId, qty, price, isClub, start, end);
        }
        #endregion

        #region Menus
        private static void displayMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n======= Main Menu =======");
                Console.WriteLine("1. Sales Management");
                Console.WriteLine("2. Products Management");
                Console.WriteLine("3. Customers Management");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");

                string input = Console.ReadLine() ?? "";
                switch (input)
                {
                    case "1": displaySubMenu("Sales", s_dalSale); break;
                    case "2": displaySubMenu("Products", s_dalProduct); break;
                    case "3": displaySubMenu("Customers", s_dalCustomer); break;
                    case "4": exit = true; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private static void displaySubMenu<T>(string entityName, ICrud<T> dal) where T : class
        {
            bool backToMain = false;
            while (!backToMain)
            {
                Console.WriteLine($"\n--- {entityName} Management ---");
                Console.WriteLine("1. Display All | 2. Find by ID | 3. Add | 4. Update | 5. Delete | 6. Back");
                Console.Write("Select an action: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1": // Read All
                        foreach (var item in dal.ReadAll()) Console.WriteLine(item);
                        break;

                    case "2": // Read by ID
                        int idToFind = ReadInt("Enter ID: ");
                        var found = dal.Read(idToFind);
                        Console.WriteLine(found != null ? found : "Not found.");
                        break;

                    case "3": // ADD (Create)
                    case "4": // UPDATE 
                        {
                            int targetId = (choice == "4") ? ReadInt("Enter ID to update: ") : 0;

                            // בדיקה אם קיים לפני עדכון
                            if (choice == "4" && dal.Read(targetId) == null)
                            {
                                Console.WriteLine("Item not found.");
                                break;
                            }

                            T? item = null;
                            if (typeof(T) == typeof(Customer)) item = InputCustomer(targetId) as T;
                            else if (typeof(T) == typeof(Product)) item = InputProduct(targetId) as T;
                            else if (typeof(T) == typeof(Sale)) item = InputSale(targetId) as T;

                            if (item != null)
                            {
                                try
                                {
                                    if (choice == "3")
                                    {
                                        int newId = dal.Create(item);
                                        Console.WriteLine($"Added successfully! New ID: {newId}");
                                    }
                                    else
                                    {
                                        dal.Update(item);
                                        Console.WriteLine("Updated successfully!");
                                    }
                                }
                                catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
                            }
                        }
                        break;

                    case "5": // Delete
                        try
                        {
                            dal.Delete(ReadInt("Enter ID to delete: "));
                            Console.WriteLine("Deleted successfully.");
                        }
                        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
                        break;

                    case "6": backToMain = true; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }
        #endregion
    }
}