using System;
using Npgsql;

namespace DB_001
{
    public class SeedData
    {
        private readonly NpgsqlConnection _connection;
        public SeedData(NpgsqlConnection connection)
        {
            _connection = connection;

        }

        public void Seed()
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

        }


        
        public void isCategoryTableExists()
        {
            

            var sqlCategoryExists = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='categories'";
            using var cmdCatExists = new NpgsqlCommand(sqlCategoryExists, _connection);
            var isCatTableExists = (long)cmdCatExists.ExecuteScalar();

            if(isCatTableExists == 0)
            {
                CreateCategoriesTable();
            }
        }

        public void isProductTableExists()
        {
           

            var sqlProducts = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='products'";
            using var cmdProdExists = new NpgsqlCommand(sqlProducts, _connection);
            var isProductsTableExists = (long)cmdProdExists.ExecuteScalar();

            if (isProductsTableExists == 0)
            {
                CreateProductsTable();
            }
        }


        public void isOrderTableExists()
        {
            

            var sqlProducts = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='orders'";
            using var cmdProdExists = new NpgsqlCommand(sqlProducts, _connection);
            var isProductsTableExists = (long)cmdProdExists.ExecuteScalar();

            if (isProductsTableExists == 0)
            {
                CreateOrdersTable();
            }
        }


        public void isOrderProductTableExists()
        {
           

            var sqlProducts = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='order_product'";
            using var cmdProdExists = new NpgsqlCommand(sqlProducts, _connection);
            var isProductOrderTableExists = (long)cmdProdExists.ExecuteScalar();

            if (isProductOrderTableExists == 0)
            {
                CreateOrder_ProductTable();
            }
        }




        public bool IsCategoriesExists()
        {
            

            var sql = @"SELECT count(*) FROM public.categories;";
            using var cmd1 = new NpgsqlCommand(sql, _connection);
            var countCats = (long)cmd1.ExecuteScalar();

            if (countCats > 0) return true;

            return false;
        }

        public bool IsProductsExists()
        {
            if (GetCountProducts() > 0) return true;

            return false;
        }

        public long GetCountProducts()
        {


            var sql = @"SELECT count(*) FROM public.products;";
            using var cmd1 = new NpgsqlCommand(sql, _connection);
            var countProducts = (long)cmd1.ExecuteScalar();
            return countProducts;
        }

        public bool IsOrdersExists()
        {
          if (GetCountOrders() > 0) return true;

            return false;
        }

        public long GetCountOrders()
        {
            var sql = @"SELECT count(*) FROM public.orders;";
            using var cmd1 = new NpgsqlCommand(sql, _connection);
            var countOrders = (long)cmd1.ExecuteScalar();
            return countOrders;
        }


        public bool IsProductsOrdersExists()
        {

            var sql = @"SELECT count(*) FROM public.order_product;";
            using var cmd1 = new NpgsqlCommand(sql, _connection);
            var countCats = (long)cmd1.ExecuteScalar();

            if (countCats > 0) return true;

            return false;
        }




        public void CreateProductsTable()
        {
            

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

            using var cmd = new NpgsqlCommand(sql, _connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            // Console.WriteLine($"Created Products table. Affected rows count: {affectedRowsCount}");
        }

        public void CreateOrdersTable()
        {
            

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

            using var cmd = new NpgsqlCommand(sql, _connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            
        }

        public void CreateOrder_ProductTable()
        {
       

            var sql = @"
               CREATE TABLE order_product
                (
                    order_id BIGINT REFERENCES orders (id) ON UPDATE CASCADE ON DELETE CASCADE,
                    product_id BIGINT REFERENCES products (id) ON UPDATE CASCADE,                             
                    CONSTRAINT order_product_pkey PRIMARY KEY (order_id, product_id)
                );

                ";

            using var cmd = new NpgsqlCommand(sql, _connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            // Console.WriteLine($"Created Order_Product table. Affected rows count: {affectedRowsCount}");
        }


        public void InsertProducts(Category category)
        {
            

            var price = 55.55;

            for (int i = 0; i < 5; i++)
            {
                var sql = @"INSERT INTO products(name, category_id, description, price)
                    VALUES(@name, @category_id, @description, @price)";

                using var cmd = new NpgsqlCommand(sql, _connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("category_id", category.Id));
                parameters.Add(new NpgsqlParameter("name", $"Product_00{i} for categoryId={category.Name}"));
                parameters.Add(new NpgsqlParameter("description", $" Description Product_00{i} for categoryId={category.Name}"));
                parameters.Add(new NpgsqlParameter("price", price = price +7.88));
               

                cmd.ExecuteNonQuery();
            }

            


        }


        public void InsertOrders()
        {
           

            for (int i = 0; i < 20; i++)
            {
                var sql = @"INSERT INTO orders(user_id, created)
                    VALUES(@user_id, @created)";

                using var cmd = new NpgsqlCommand(sql, _connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("user_id", i+1));
                parameters.Add(new NpgsqlParameter("created", DateTime.Now));
               
                cmd.ExecuteNonQuery();
            }

            //Console.WriteLine($"Добавление прошло успешно");


        }


        public void InsertOrdersProducts()
        {
           
            for (int i = 0; i < 10; i++)
            {

                Random rnd = new Random();
                var orderId = i+1;
                var productId = rnd.Next(1, (int)GetCountProducts());

                var sql = @"INSERT INTO order_product(order_id, product_id)
                    VALUES(@order_id, @product_id)";

                using var cmd = new NpgsqlCommand(sql, _connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("order_id", orderId));
                parameters.Add(new NpgsqlParameter("product_id", productId));

                cmd.ExecuteNonQuery();
            }

            //Console.WriteLine($"Добавление прошло успешно");


        }


        public void CreateCategoriesTable()
        {
            

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

            using var cmd = new NpgsqlCommand(sql, _connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            // Console.WriteLine($"Created Categories table. Affected rows count: {affectedRowsCount}");
        }

        public void InsertCategoriesAndProducts()
        {
            // using var connection = new NpgsqlConnection(connectionString);
            // connection.Open();

            

            for (int i = 0; i < 5; i++)
            {
                var sql = @"INSERT INTO categories(name)
                    VALUES(@name)
                    RETURNING id;";

                using var cmdCategory = new NpgsqlCommand(sql, _connection);
                var parameters = cmdCategory.Parameters;
                parameters.Add(new NpgsqlParameter("name", $"Category_00{i}"));


                var categoryId = (long)cmdCategory.ExecuteScalar();

                InsertProducts(new Category { Id = categoryId, Name = $"Category_00{i}" });

            }

            //Console.WriteLine($"Добавление прошло успешно");
        }

    }
}