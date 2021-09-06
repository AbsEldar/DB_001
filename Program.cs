using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;

namespace DB_001
{
    class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        const string connectionString = "Host=localhost;Username=otus4;Password=otus4;Database=otus4";
        static void Main(string[] args)
        {


            isCategoryTableExists();
            isProductTableExists();

            if (!IsCategoriesExists() && !IsProductsExists())
            {
                InsertCategoriesAndProducts();
            }

            isOrderTableExists();

            if(!IsOrdersExists())
            {
                InsertOrders();
            }

            isOrderProductTableExists();

            if(!IsProductsOrdersExists())
            {
                InsertOrdersProducts();
            }


            PrintCategories();
            PrintProducts();
            PrintOrders();
            PrintOrdersProducts();




            Console.ReadKey();
        }

        static void PrintCategories()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, name FROM categories ";

            using var cmd = new NpgsqlCommand(sql, connection);


            var tps = new List<Tuple<int, string>>();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                
                var item = Tuple.Create(id, name);
                tps.Add(item);
            }

            Console.WriteLine("CATEGORIES");
            Console.WriteLine(tps.ToStringTable(
               new[] { "Id", "Name" },
               a => a.Item1, a => a.Item2));
        }

        static void PrintProducts()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, category_id, name, description, price FROM products ";

            using var cmd = new NpgsqlCommand(sql, connection);


            var tps = new List<Tuple<long, long, string, string, double>>();

            var reader = cmd.ExecuteReader();
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

            Console.WriteLine("PRODUCTS");
            Console.WriteLine(tps.ToStringTable(
               new[] { "Id", "CategoryId", "Name", "Description", "Price" },
               a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4, a => a.Item5 ));
        }

        static void PrintOrders()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, user_id, created FROM orders ";

            using var cmd = new NpgsqlCommand(sql, connection);


            var tps = new List<Tuple<long, long, DateTime>>();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt64(0);
                var userId = reader.GetInt64(1);
                var created = reader.GetDateTime(2);
               

                var item = Tuple.Create(id, userId, created);
                tps.Add(item);
            }

            Console.WriteLine("ORDERS");
            Console.WriteLine(tps.ToStringTable(
               new[] { "Id", "UserId", "Created" },
               a => a.Item1, a => a.Item2, a => a.Item3));
        }


        static void PrintOrdersProducts()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT order_id, product_id FROM order_product ";

            using var cmd = new NpgsqlCommand(sql, connection);


            var tps = new List<Tuple<long, long>>();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var orderId = reader.GetInt64(0);
                var productId = reader.GetInt64(1);


                var item = Tuple.Create(orderId, productId);
                tps.Add(item);
            }

            Console.WriteLine("ORDERS_PRODUCTS");
            Console.WriteLine(tps.ToStringTable(
               new[] { "OrderId", "ProductId" },
               a => a.Item1, a => a.Item2));
        }

        static void isCategoryTableExists()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sqlCategoryExists = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='categories'";
            using var cmdCatExists = new NpgsqlCommand(sqlCategoryExists, connection);
            var isCatTableExists = (long)cmdCatExists.ExecuteScalar();

            if(isCatTableExists == 0)
            {
                CreateCategoriesTable();
            }
        }

        static void isProductTableExists()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sqlProducts = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='products'";
            using var cmdProdExists = new NpgsqlCommand(sqlProducts, connection);
            var isProductsTableExists = (long)cmdProdExists.ExecuteScalar();

            if (isProductsTableExists == 0)
            {
                CreateProductsTable();
            }
        }


        static void isOrderTableExists()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sqlProducts = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='orders'";
            using var cmdProdExists = new NpgsqlCommand(sqlProducts, connection);
            var isProductsTableExists = (long)cmdProdExists.ExecuteScalar();

            if (isProductsTableExists == 0)
            {
                CreateOrdersTable();
            }
        }


        static void isOrderProductTableExists()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sqlProducts = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='order_product'";
            using var cmdProdExists = new NpgsqlCommand(sqlProducts, connection);
            var isProductOrderTableExists = (long)cmdProdExists.ExecuteScalar();

            if (isProductOrderTableExists == 0)
            {
                CreateOrder_ProductTable();
            }
        }




        static bool IsCategoriesExists()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT count(*) FROM public.categories;";
            using var cmd1 = new NpgsqlCommand(sql, connection);
            var countCats = (long)cmd1.ExecuteScalar();

            if (countCats > 0) return true;

            return false;
        }

        static bool IsProductsExists()
        {
            if (GetCountProducts() > 0) return true;

            return false;
        }

        static long GetCountProducts()
        {

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT count(*) FROM public.products;";
            using var cmd1 = new NpgsqlCommand(sql, connection);
            var countProducts = (long)cmd1.ExecuteScalar();
            return countProducts;
        }

        static bool IsOrdersExists()
        {
          if (GetCountOrders() > 0) return true;

            return false;
        }

        static long GetCountOrders()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT count(*) FROM public.orders;";
            using var cmd1 = new NpgsqlCommand(sql, connection);
            var countOrders = (long)cmd1.ExecuteScalar();
            return countOrders;
        }


        static bool IsProductsOrdersExists()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"SELECT count(*) FROM public.order_product;";
            using var cmd1 = new NpgsqlCommand(sql, connection);
            var countCats = (long)cmd1.ExecuteScalar();

            if (countCats > 0) return true;

            return false;
        }




        static void CreateProductsTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                CREATE SEQUENCE products_id_seq;

                CREATE TABLE products
                (
                    id BIGINT  NOT NULL    DEFAULT NEXTVAL('products_id_seq'),
                    category_id BIGINT     NOT NULL,                             
                    name      CHARACTER VARYING(255)      NOT NULL,
                    description       CHARACTER VARYING(255),
                    price    decimal NOT NULL,
  
                    CONSTRAINT products_pkey PRIMARY KEY (id),
                    CONSTRAINT products_fk_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
                );

                CREATE INDEX name_idx ON products(name);
                ";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created Products table. Affected rows count: {affectedRowsCount}");
        }

        static void CreateOrdersTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                CREATE SEQUENCE orders_id_seq;

                CREATE TABLE orders
                (
                    id BIGINT  NOT NULL    DEFAULT NEXTVAL('orders_id_seq'),
                    user_id BIGINT     NOT NULL,                             
                    created date NOT NULL,
                    CONSTRAINT orders_pkey PRIMARY KEY (id)
                );

                ";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created Orders table. Affected rows count: {affectedRowsCount}");
        }

        static void CreateOrder_ProductTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
               CREATE TABLE order_product
                (
                    order_id BIGINT REFERENCES orders (id) ON UPDATE CASCADE ON DELETE CASCADE,
                    product_id BIGINT REFERENCES products (id) ON UPDATE CASCADE,                             
                    CONSTRAINT order_product_pkey PRIMARY KEY (order_id, product_id)
                );

                ";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created Order_Product table. Affected rows count: {affectedRowsCount}");
        }


        static void InsertProducts(Category category)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var price = 55.55;

            for (int i = 0; i < 5; i++)
            {
                var sql = @"INSERT INTO products(name, category_id, description, price)
                    VALUES(@name, @category_id, @description, @price)";

                using var cmd = new NpgsqlCommand(sql, connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("category_id", category.Id));
                parameters.Add(new NpgsqlParameter("name", $"Product_00{i} for categoryId={category.Name}"));
                parameters.Add(new NpgsqlParameter("description", $" Description Product_00{i} for categoryId={category.Name}"));
                parameters.Add(new NpgsqlParameter("price", price = price +7.88));
               

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine($"Добавление прошло успешно");


        }


        static void InsertOrders()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            

            for (int i = 0; i < 20; i++)
            {
                var sql = @"INSERT INTO orders(user_id, created)
                    VALUES(@user_id, @created)";

                using var cmd = new NpgsqlCommand(sql, connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("user_id", i+1));
                parameters.Add(new NpgsqlParameter("created", DateTime.Now));
               
                cmd.ExecuteNonQuery();
            }

            //Console.WriteLine($"Добавление прошло успешно");


        }


        static void InsertOrdersProducts()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();



            for (int i = 0; i < 10; i++)
            {

                Random rnd = new Random();
                var orderId = i+1;
                var productId = rnd.Next(1, (int)GetCountProducts());

                var sql = @"INSERT INTO order_product(order_id, product_id)
                    VALUES(@order_id, @product_id)";

                using var cmd = new NpgsqlCommand(sql, connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("order_id", orderId));
                parameters.Add(new NpgsqlParameter("product_id", productId));

                cmd.ExecuteNonQuery();
            }

            //Console.WriteLine($"Добавление прошло успешно");


        }


        static void CreateCategoriesTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                CREATE SEQUENCE categories_id_seq;

                CREATE TABLE categories
                (
                    id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('categories_id_seq'),
                    name      CHARACTER VARYING(255)      NOT NULL,
  
                    CONSTRAINT categories_pkey PRIMARY KEY (id)
                );

                CREATE INDEX name_category_idx ON categories(name);
                ";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created Categories table. Affected rows count: {affectedRowsCount}");
        }

        static void InsertCategoriesAndProducts()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            

            for (int i = 0; i < 5; i++)
            {
                var sql = @"INSERT INTO categories(name)
                    VALUES(@name)
                    RETURNING id;";

                using var cmdCategory = new NpgsqlCommand(sql, connection);
                var parameters = cmdCategory.Parameters;
                parameters.Add(new NpgsqlParameter("name", $"Category_00{i}"));


                var categoryId = (long)cmdCategory.ExecuteScalar();

                InsertProducts(new Category { Id = categoryId, Name = $"Category_00{i}" });

            }

            //Console.WriteLine($"Добавление прошло успешно");
        }


    }
}
