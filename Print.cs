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

            var sql = @$"
                SELECT {Tas.id.ToString()}, {Tas.name.ToString()} FROM {Tas.categories.ToString()} ";

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

            Console.WriteLine($"{Tas.categories.ToString().ToUpper()}");
            Console.WriteLine(tps.ToStringTable(
               new[] { Tas.id.ToString().ToUpper(), Tas.name.ToString().ToUpper() },
               a => a.Item1, a => a.Item2));
        }

        public void PrintProducts()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @$"
                SELECT {Tas.id.ToString()}, {Tas.category_id.ToString()}, {Tas.name.ToString()}, {Tas.description.ToString()}, 
                {Tas.price.ToString()} FROM {Tas.products.ToString()} ";

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

            Console.WriteLine(Tas.products.ToString().ToUpper());
            Console.WriteLine(tps.ToStringTable(
               new[] { Tas.id.ToString().ToUpper(), Tas.category_id.ToString().ToUpper(), Tas.name.ToString().ToUpper(), 
                Tas.description.ToString().ToUpper(), Tas.price.ToString().ToUpper() },
               a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4, a => a.Item5));
        }

        public void PrintOrders()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @$"
                SELECT {Tas.id.ToString()}, {Tas.user_id.ToString()}, {Tas.created.ToString()} FROM {Tas.orders.ToString()} ";

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



            Console.WriteLine(Tas.orders.ToString().ToUpper());
            Console.WriteLine(tps.ToStringTable(
               new[] { Tas.id.ToString().ToUpper(), Tas.user_id.ToString().ToUpper(), Tas.created.ToString().ToUpper() },
               a => a.Item1, a => a.Item2, a => a.Item3));
        }


        public void PrintOrdersProducts()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            var sql = @$"
                SELECT {Tas.order_id.ToString()}, {Tas.product_id.ToString()} FROM {Tas.order_product.ToString()} ";

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


            Console.WriteLine(Tas.order_product.ToString().ToUpper());
            Console.WriteLine(tps.ToStringTable(
               new[] { Tas.order_id.ToString().ToUpper(), Tas.product_id.ToString().ToUpper() },
               a => a.Item1, a => a.Item2));
        }

    }
}