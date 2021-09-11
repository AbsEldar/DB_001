using System;
using System.Collections.Generic;
using Npgsql;

namespace DB_001
{
    public class Print
    {
        private readonly NpgsqlConnection _connection;
        public Print(NpgsqlConnection connection)
        {
            _connection = connection;

        }
        public void PrintCategories()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @"
                SELECT id, name FROM categories ";

            // using var cmd = new NpgsqlCommand(sql, connection);

            var tps = new List<Tuple<int, string>>();
            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);

                        var item = Tuple.Create(id, name);
                        tps.Add(item);
                    }
                }



            }

            Console.WriteLine("CATEGORIES");
            Console.WriteLine(tps.ToStringTable(
               new[] { "Id", "Name" },
               a => a.Item1, a => a.Item2));
        }

        public void PrintProducts()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @"
                SELECT id, category_id, name, description, price FROM products ";

            // using var cmd = new NpgsqlCommand(sql, connection);

            var tps = new List<Tuple<long, long, string, string, double>>();

            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt64(0);
                        var categoryId = reader.GetInt64(1);
                        var name = reader.GetString(2);
                        var description = reader.GetString(3);
                        var price = reader.GetDouble(4);

                        var item = Tuple.Create(id, categoryId, name, description, price);
                        tps.Add(item);
                    }
                }


            }

            Console.WriteLine("PRODUCTS");
            Console.WriteLine(tps.ToStringTable(
               new[] { "Id", "CategoryId", "Name", "Description", "Price" },
               a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4, a => a.Item5));
        }

        public void PrintOrders()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @"
                SELECT id, user_id, created FROM orders ";

            using var cmd = new NpgsqlCommand(sql, _connection);


            var tps = new List<Tuple<long, long, DateTime>>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt64(0);
                    var userId = reader.GetInt64(1);
                    var created = reader.GetDateTime(2);


                    var item = Tuple.Create(id, userId, created);
                    tps.Add(item);
                }
            }



            Console.WriteLine("ORDERS");
            Console.WriteLine(tps.ToStringTable(
               new[] { "Id", "UserId", "Created" },
               a => a.Item1, a => a.Item2, a => a.Item3));
        }


        public void PrintOrdersProducts()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @"
                SELECT order_id, product_id FROM order_product ";

            using var cmd = new NpgsqlCommand(sql, _connection);


            var tps = new List<Tuple<long, long>>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var orderId = reader.GetInt64(0);
                    var productId = reader.GetInt64(1);


                    var item = Tuple.Create(orderId, productId);
                    tps.Add(item);
                }
            }


            Console.WriteLine("ORDERS_PRODUCTS");
            Console.WriteLine(tps.ToStringTable(
               new[] { "OrderId", "ProductId" },
               a => a.Item1, a => a.Item2));
        }

    }
}