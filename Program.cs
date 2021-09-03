using System;
using Npgsql;

namespace DB_001
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateCategoriesTable();
            CreateProductsTable();
            //InsertProducts();

            

            Console.ReadKey();
        }

        const string connectionString = "Host=localhost;Username=otus4;Password=otus4;Database=otus4";

        

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

        static void InsertProducts()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var price = 55.55;

            for (int i = 0; i < 5; i++)
            {
                var sql = @"INSERT INTO products(name, description, price)
                    VALUES(@name, @description, @price)";

                using var cmd = new NpgsqlCommand(sql, connection);
                var parameters = cmd.Parameters;
                parameters.Add(new NpgsqlParameter("name", $"Product_00{i}"));
                parameters.Add(new NpgsqlParameter("description", $"Product_00{i} Description"));
                parameters.Add(new NpgsqlParameter("price", price = price +7.88));


                cmd.ExecuteNonQuery();
            }

            Console.WriteLine($"Добавление прошло успешно");


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
    }
}
