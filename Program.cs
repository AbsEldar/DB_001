using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;

namespace DB_001
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        const string connectionString = "Host=localhost;Username=otus4;Password=otus4;Database=otus4";
        static void Main(string[] args)
        {
            // using var connection = new NpgsqlConnection(connectionString);
            

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open(); 


                var seedData = new SeedData(connection);
                seedData.Seed();
                
                var print = new Print(connection);
                print.PrintCategories();
                print.PrintProducts();
                print.PrintOrders();
                print.PrintOrdersProducts();


                System.Console.WriteLine("Введите имя новой категории:");
                var catName = Console.ReadLine();
                seedData.InsertCategory(catName);

                System.Console.WriteLine("Обновлённая таблица категорий:");
                print.PrintCategories();

            }

            




            Console.ReadKey();
        }

        

    }
}
