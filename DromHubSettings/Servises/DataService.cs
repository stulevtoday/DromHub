using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DromHubSettings.Models;

namespace DromHubSettings.Servises
{
    class DataService
    {
        private static readonly string ConnectionString = "Host=localhost;Username=postgres;Password=admin;Database=postgres";

        public static async Task<List<BrandMarkup>> GetBrandMarkupsAsync()
        {
            var brandMarkups = new List<BrandMarkup>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT brand_name, markup FROM brand_markups";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var brandName = reader["brand_name"].ToString();
                var markup = reader.GetDouble(reader.GetOrdinal("markup"));
                brandMarkups.Add(new BrandMarkup
                {
                    // При необходимости можно сохранить бренд в нижнем регистре, хотя в БД он уже сохранён так
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
        public static async Task AddBrandMarkupAsync(BrandMarkup markup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            var sql = "INSERT INTO brand_markups (brand_name, markup) VALUES (@brandName, @markup)";
            using var command = new NpgsqlCommand(sql, connection);
            // Преобразуем название бренда в нижний регистр
            command.Parameters.AddWithValue("brandName", markup.BrandName.ToLowerInvariant());
            command.Parameters.AddWithValue("markup", markup.Markup);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Удаляет запись из таблицы brand_markups по имени бренда.
        /// Название бренда приводится к нижнему регистру для корректного сравнения.
        /// </summary>
        public static async Task DeleteBrandMarkupAsync(BrandMarkup markup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            var sql = "DELETE FROM brand_markups WHERE brand_name = @brandName";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("brandName", markup.BrandName.ToLowerInvariant());
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Обновляет значение наценки для указанного бренда.
        /// Название бренда приводится к нижнему регистру.
        /// </summary>
        public static async Task UpdateBrandMarkupAsync(BrandMarkup markup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            var sql = "UPDATE brand_markups SET markup = @markup WHERE brand_name = @brandName";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("brandName", markup.BrandName.ToLowerInvariant());
            command.Parameters.AddWithValue("markup", markup.Markup);
            await command.ExecuteNonQueryAsync();
        }
    }
}
