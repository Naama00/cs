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

        private static void displayMenu()
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Sales Management");
            Console.WriteLine("2. Products Management");
            Console.WriteLine("3. Customers Management");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();


            switch (input)
            {
                case "1": displaySubMenu("Sales", s_dalSale); break;
                case "2": displaySubMenu("Products", s_dalProduct); break;
                case "3": displaySubMenu("Customers", s_dalCustomer); break;
                case "4": return;
                default:
                    Console.WriteLine("Invalid choice.");
                    displayMenu();
                    break;
            }
        }

        private static void displaySubMenu<T>(string entityName, ICrud<T> dal) where T : class
        {
            bool backToMain = false;

            while (!backToMain)
            {
                Console.WriteLine($"\n--- {entityName} Management ---\n");
                Console.WriteLine("1. Display all items");
                Console.WriteLine("2. Display item by ID");
                Console.WriteLine("3. Add new item");
                Console.WriteLine("4. Update item");
                Console.WriteLine("5. Delete item");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("Select an action: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // הצגת כולם
                        Console.WriteLine($"\nListing all {entityName}:");
                        var allItems = dal.ReadAll();
                        foreach (var item in allItems)
                        {
                            Console.WriteLine(item);
                        }
                        break;

                    case "2": // הצגה לפי ID
                        Console.Write($"Enter {entityName} ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            T? item = dal.Read(id);
                            if (item != null)
                                Console.WriteLine($"Found: {item}");
                            else
                                Console.WriteLine($"{entityName} with ID {id} was not found.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }
                        break;

                    case "3": // הוספה
                        {
                            T? newItem = null;

                            if (typeof(T) == typeof(Customer))
                            {
                                Console.WriteLine("\n--- Adding New Customer ---\n");
   
                                Console.Write("Enter Name: ");
                                string name = Console.ReadLine() ?? "";
                                Console.Write("Enter Address: ");
                                string address = Console.ReadLine() ?? "";
                                Console.Write("Enter Phone: ");
                                string phone = Console.ReadLine() ?? "";

                                // אנחנו שולחים 0 בתור ה-ID. ה-DAL יעדכן את זה למספר הרץ הנכון.
                                newItem = new Customer(0, name, address, phone) as T;
                            }
                            else if (typeof(T) == typeof(Product))
                            {
                                Console.WriteLine("\n--- Adding New Product ---\n");
                                Console.Write("Enter Product Name: ");
                                string name = Console.ReadLine() ?? "";

                                // כנ"ל לגבי מוצר
                                newItem = new Product(0, name) as T;
                            }

                            if (newItem != null)
                            {
                                // הפעולה Create מחזירה את ה-ID האמיתי שנוצר במערכת
                                int realId = dal.Create(newItem);
                                Console.WriteLine($"\nSuccessfully added! The new ID is: {realId}");
                            }
                        }
                        break;

                    case "6": // חזרה
                        backToMain = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }
    }