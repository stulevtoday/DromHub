using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DromHubSettings.Models;
using System.Diagnostics;

namespace DromHubSettings.Serviсes
{
    class DataService
    {
        private static readonly string ConnectionString = "Host=localhost;Username=postgres;Password=admin;Database=postgres";

        public static async Task<List<BrandMarkup>> GetBrandMarkupsAsync()
        {
            var brandMarkups = new List<BrandMarkup>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, brand_name, markup FROM brand_markups";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetGuid(reader.GetOrdinal("id"));
                var brandName = reader.GetString(reader.GetOrdinal("brand_name"));
                var markup = reader.GetDouble(reader.GetOrdinal("markup"));

                brandMarkups.Add(new BrandMarkup
                {
                    Id = id,
                    BrandName = brandName,
                    Markup = markup
                });
            }

            return brandMarkups;
        }


        /// <summary>
        /// Добавляет новую запись в таблицу brand_markups.
        /// Название бренда преобразуется в нижний регистр.
        /// </summary>
        public static async Task AddBrandMarkupAsync(BrandMarkup brandMarkup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO brand_markups (id, brand_name, markup) VALUES (@id, @brandName, @markup)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", brandMarkup.Id);
            command.Parameters.AddWithValue("brandName", brandMarkup.BrandName.ToLowerInvariant());
            command.Parameters.AddWithValue("markup", brandMarkup.Markup);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Удаляет запись из таблицы brand_markups по имени бренда.
        /// Название бренда приводится к нижнему регистру для корректного сравнения.
        /// </summary>
        public static async Task DeleteBrandMarkupAsync(BrandMarkup brandMarkup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM brand_markups WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", brandMarkup.Id);
            await command.ExecuteNonQueryAsync();
        }

        // Сохранение всех изменений для брендов (опционально, с транзакцией)
        public static async Task SaveBrandMarkupsAsync(IEnumerable<BrandMarkup> brands)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            var sql = "UPDATE brand_markups SET brand_name = @brandName, markup = @markup WHERE id = @id";
            foreach (var brand in brands)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("brandName", brand.BrandName.ToLowerInvariant());
                command.Parameters.AddWithValue("markup", brand.Markup);
                command.Parameters.AddWithValue("id", brand.Id);
                await command.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        }


        /// <summary>
        /// Загружает список диапазонов цен из таблицы price_range_markups.
        /// </summary>
        public static async Task<List<PriceRangeMarkup>> GetPriceRangeMarkupsAsync()
        {
            var rangeMarkups = new List<PriceRangeMarkup>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Добавляем столбец id в запрос
            var sql = "SELECT id, min_price, max_price, markup FROM price_range_markups";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Считываем id как Guid
                var id = reader.GetGuid(reader.GetOrdinal("id"));
                var minPrice = reader.GetDouble(reader.GetOrdinal("min_price"));
                var maxPrice = reader.GetDouble(reader.GetOrdinal("max_price"));
                var markup = reader.GetDouble(reader.GetOrdinal("markup"));

                rangeMarkups.Add(new PriceRangeMarkup
                {
                    Id = id,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Markup = markup
                });
            }

            return rangeMarkups;
        }


        /// <summary>
        /// Добавляет новый диапазон цен в таблицу price_range_markups.
        /// </summary>
        public static async Task AddPriceRangeMarkupAsync(PriceRangeMarkup rangeMarkup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO price_range_markups (id, min_price, max_price, markup) VALUES (@id, @minPrice, @maxPrice, @markup)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", rangeMarkup.Id);
            command.Parameters.AddWithValue("minPrice", rangeMarkup.MinPrice);
            command.Parameters.AddWithValue("maxPrice", rangeMarkup.MaxPrice);
            command.Parameters.AddWithValue("markup", rangeMarkup.Markup);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Удаляет диапазон цен из таблицы price_range_markups по уникальному идентификатору.
        /// </summary>
        public static async Task DeletePriceRangeMarkupAsync(PriceRangeMarkup rangeMarkup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM price_range_markups WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", rangeMarkup.Id);
            await command.ExecuteNonQueryAsync();
        }

        // Сохранение всех изменений диапазонов (опционально, с транзакцией)
        public static async Task SavePriceRangeMarkupsAsync(IEnumerable<PriceRangeMarkup> markups)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            var sql = "UPDATE price_range_markups SET min_price = @minPrice, max_price = @maxPrice, markup = @markup WHERE id = @id";
            foreach (var markup in markups)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("minPrice", markup.MinPrice);
                command.Parameters.AddWithValue("maxPrice", markup.MaxPrice);
                command.Parameters.AddWithValue("markup", markup.Markup);
                command.Parameters.AddWithValue("id", markup.Id);
                await command.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        }

        public static async Task<List<Supplier>> GetSuppliersAsync()
        {
            var suppliers = new List<Supplier>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, name, email, locality_name, index FROM suppliers";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetGuid(reader.GetOrdinal("id"));
                var name = reader.GetString(reader.GetOrdinal("name"));
                var email = reader.GetString(reader.GetOrdinal("email"));
                var localityName = reader.GetString(reader.GetOrdinal("locality_name"));
                var index = reader.GetInt32(reader.GetOrdinal("index"));

                suppliers.Add(new Supplier
                {
                    Id = id,
                    Name = name,
                    Email = email,
                    LocalityName = localityName,
                    Index = index
                });
            }

            return suppliers;
        }

        public static async Task AddSupplierAsync(Supplier supplier)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO suppliers (id, name, email, locality_name, index) " +
                      "VALUES (@id, @name, @email, @localityName, @index)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", supplier.Id);
            command.Parameters.AddWithValue("name", supplier.Name);
            command.Parameters.AddWithValue("email", supplier.Email);
            command.Parameters.AddWithValue("localityName", supplier.LocalityName);
            command.Parameters.AddWithValue("index", supplier.Index);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task SaveSuppliersAsync(IEnumerable<Supplier> suppliers)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Начинаем транзакцию, чтобы сохранить все изменения атомарно
            using var transaction = await connection.BeginTransactionAsync();

            var sql = "UPDATE suppliers SET name = @name, email = @email, locality_name = @localityName, index = @index WHERE id = @id";

            foreach (var s in suppliers)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("name", s.Name);
                command.Parameters.AddWithValue("email", s.Email);
                command.Parameters.AddWithValue("localityName", s.LocalityName);
                command.Parameters.AddWithValue("index", s.Index);
                command.Parameters.AddWithValue("id", s.Id);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }


        public static async Task DeleteSupplierAsync(Supplier supplier)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM suppliers WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", supplier.Id);
            await command.ExecuteNonQueryAsync();
        }
    }
}
