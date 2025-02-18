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

        public static async Task<List<BrandMarkup>> LoadBrandMarkupsAsync()
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

            // Обновляем запрос: выбираем locality_id вместо locality_name
            var sql = @"
                SELECT 
                    s.id,
                    s.name,
                    s.email,
                    s.locality_id,
                    l.name as locality_name,
                    s.index,
                    s.is_active
                FROM suppliers s
                JOIN localities l ON s.locality_id = l.id";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetGuid(reader.GetOrdinal("id"));
                var name = reader.GetString(reader.GetOrdinal("name"));
                var email = reader.GetString(reader.GetOrdinal("email"));
                var localityId = reader.GetGuid(reader.GetOrdinal("locality_id")); // теперь читаем id
                var localityName = reader.GetString(reader.GetOrdinal("locality_name")); // теперь читаем id
                var index = reader.GetInt32(reader.GetOrdinal("index"));
                var isActive = reader.GetBoolean(reader.GetOrdinal("is_active"));

                suppliers.Add(new Supplier
                {
                    Id = id,
                    Name = name,
                    Email = email,
                    LocalityId = localityId,  // присваиваем значение id локальности
                    LocalityName = localityName,
                    Index = index,
                    IsActive = isActive
                });
            }

            return suppliers;
        }



        public static async Task AddSupplierAsync(Supplier supplier)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Добавляем запись в suppliers
                var sqlSupplier = @"
                    INSERT INTO suppliers (id, name, email, locality_id, index, is_active)
                    VALUES (@id, @name, @email, @localityId, @index, @isActive)";
                using (var command = new NpgsqlCommand(sqlSupplier, connection, transaction))
                {
                    command.Parameters.AddWithValue("id", supplier.Id);
                    command.Parameters.AddWithValue("name", supplier.Name);
                    command.Parameters.AddWithValue("email", supplier.Email);
                    command.Parameters.AddWithValue("localityId", supplier.LocalityId);
                    command.Parameters.AddWithValue("index", supplier.Index);
                    command.Parameters.AddWithValue("isActive", supplier.IsActive);
                    await command.ExecuteNonQueryAsync();
                }

                // 2. Создаём запись в supplier_markups (без имени)
                var sqlMarkup = @"
                    INSERT INTO supplier_markups (supplier_id, markup)
                    VALUES (@supplierId, @markup)";
                using (var cmdMarkup = new NpgsqlCommand(sqlMarkup, connection, transaction))
                {
                    cmdMarkup.Parameters.AddWithValue("supplierId", supplier.Id);
                    cmdMarkup.Parameters.AddWithValue("markup", 1.25); // базовое значение, к примеру 125%
                    await cmdMarkup.ExecuteNonQueryAsync();
                }

                // 3. Вставляем запись разметки для поставщика
                var sqlMapping = @"
                    INSERT INTO supplier_excel_mappings (supplier_id, column_product_name, column_brand, column_catalog_name, column_quantity, column_price, column_multiple)
                    VALUES (@supplierId, @columnProductName, @columnBrand, @columnCatalogName, @columnQuantity, @columnPrice, @columnMultiple)";
                using (var cmdMapping = new NpgsqlCommand(sqlMapping, connection, transaction))
                {
                    cmdMapping.Parameters.AddWithValue("supplierId", supplier.Id);
                    cmdMapping.Parameters.AddWithValue("columnProductName", 1);
                    cmdMapping.Parameters.AddWithValue("columnBrand", 2);
                    cmdMapping.Parameters.AddWithValue("columnCatalogName", 3);
                    cmdMapping.Parameters.AddWithValue("columnQuantity", 4);
                    cmdMapping.Parameters.AddWithValue("columnPrice", 5);
                    cmdMapping.Parameters.AddWithValue("columnMultiple", 0);
                    await cmdMapping.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public static async Task SaveSuppliersAsync(IEnumerable<Supplier> suppliers)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Начинаем транзакцию, чтобы сохранить все изменения атомарно
            using var transaction = await connection.BeginTransactionAsync();

            

            var sql = @"
                UPDATE suppliers
                SET name = @name,
                    email = @email,
                    locality_id = @localityId,
                    index = @index,
                    is_active = @isActive
                WHERE id = @id";

            foreach (var s in suppliers)
            {
                Debug.WriteLine(s.LocalityId);
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", s.Id);
                command.Parameters.AddWithValue("name", s.Name);
                command.Parameters.AddWithValue("email", s.Email);
                command.Parameters.AddWithValue("localityId", s.LocalityId);
                command.Parameters.AddWithValue("index", s.Index);
                command.Parameters.AddWithValue("isActive", s.IsActive);

                await command.ExecuteNonQueryAsync();
            }
        }


        public static async Task DeleteSupplierAsync(Supplier supplier)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                // Удаляем запись из suppliers
                var sqlDeleteSupplier = "DELETE FROM suppliers WHERE id = @id";
                using (var cmdSupplier = new NpgsqlCommand(sqlDeleteSupplier, connection, transaction))
                {
                    cmdSupplier.Parameters.AddWithValue("id", supplier.Id);
                    await cmdSupplier.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public static async Task DisableSupplierAsync(Supplier supplier)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "UPDATE suppliers SET is_active = false WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", supplier.Id);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task UpdateSupplierAsync(Supplier supplier)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE suppliers
                SET name = @name,
                    email = @email,
                    locality_id = @localityId,
                    index = @index,
                    is_active = @isActive
                WHERE id = @id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", supplier.Id);
            command.Parameters.AddWithValue("name", supplier.Name);
            command.Parameters.AddWithValue("email", supplier.Email);
            command.Parameters.AddWithValue("localityId", supplier.LocalityId);  // Используем новое поле
            command.Parameters.AddWithValue("index", supplier.Index);
            command.Parameters.AddWithValue("isActive", supplier.IsActive);
            await command.ExecuteNonQueryAsync();
        }





        // Получение списка наценок поставщиков
        public static async Task<List<SupplierMarkup>> GetSupplierMarkupsAsync()
        {
            var list = new List<SupplierMarkup>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Запрос с JOIN для получения имени поставщика
            var sql = @"
                SELECT 
                    sm.supplier_id,
                    s.name AS supplier_name,
                    sm.markup
                FROM supplier_markups sm
                JOIN suppliers s ON sm.supplier_id = s.id";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var supplierId = reader.GetGuid(reader.GetOrdinal("supplier_id"));
                var supplierName = reader.GetString(reader.GetOrdinal("supplier_name"));
                var markup = reader.GetDouble(reader.GetOrdinal("markup"));

                list.Add(new SupplierMarkup
                {
                    SupplierId = supplierId,
                    SupplierName = supplierName,
                    Markup = markup
                });
            }

            return list;
        }



        // Сохранение (обновление) наценок для всех поставщиков в транзакции
        public static async Task SaveSupplierMarkupsAsync(IEnumerable<SupplierMarkup> supplierMarkups)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            // Обновляем запись по supplier_id
            var sql = "UPDATE supplier_markups SET markup = @markup WHERE supplier_id = @supplierId";

            foreach (var sm in supplierMarkups)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("markup", sm.Markup);
                command.Parameters.AddWithValue("supplierId", sm.SupplierId);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }


        public static async Task<List<SupplierExcelMapping>> GetSupplierExcelMappingsAsync()
        {
            var list = new List<SupplierExcelMapping>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT 
                    sem.supplier_id,
                    sem.column_product_name,
                    sem.column_brand,
                    sem.column_catalog_name,
                    sem.column_quantity,
                    sem.column_price,
                    sem.column_multiple,
                    s.name AS supplier_name
                FROM supplier_excel_mappings sem
                JOIN suppliers s ON sem.supplier_id = s.id";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new SupplierExcelMapping
                {
                    SupplierId = reader.GetGuid(reader.GetOrdinal("supplier_id")),
                    ColumnProductName = reader.GetInt32(reader.GetOrdinal("column_product_name")),
                    ColumnBrand = reader.GetInt32(reader.GetOrdinal("column_brand")),
                    ColumnCatalogName = reader.GetInt32(reader.GetOrdinal("column_catalog_name")),
                    ColumnQuantity = reader.GetInt32(reader.GetOrdinal("column_quantity")),
                    ColumnPrice = reader.GetInt32(reader.GetOrdinal("column_price")),
                    ColumnMultiple = reader.GetInt32(reader.GetOrdinal("column_multiple")),
                    SupplierName = reader.GetString(reader.GetOrdinal("supplier_name"))
                });
            }

            return list;
        }


        public static async Task SaveSupplierExcelMappingsAsync(IEnumerable<SupplierExcelMapping> mappings)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            var sql = @"
                UPDATE supplier_excel_mappings 
                SET column_product_name = @columnProductName,
                    column_brand = @columnBrand,
                    column_catalog_name = @columnCatalogName,
                    column_quantity = @columnQuantity,
                    column_price = @columnPrice,
                    column_multiple = @columnMultiple
                WHERE supplier_id = @supplierId";

            foreach (var mapping in mappings)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("columnProductName", mapping.ColumnProductName);
                command.Parameters.AddWithValue("columnBrand", mapping.ColumnBrand);
                command.Parameters.AddWithValue("columnCatalogName", mapping.ColumnCatalogName); // либо "column_catalog_name"
                command.Parameters.AddWithValue("columnQuantity", mapping.ColumnQuantity);
                command.Parameters.AddWithValue("columnPrice", mapping.ColumnPrice);
                command.Parameters.AddWithValue("columnMultiple", mapping.ColumnMultiple); // всегда передаем число (0, если не задано)
                command.Parameters.AddWithValue("supplierId", mapping.SupplierId);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }

        // Получение списка локальностей
        public static async Task<List<LocalityOption>> GetLocalityOptionsAsync()
        {
            var options = new List<LocalityOption>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, name, delivery_time, export_email FROM localities";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                options.Add(new LocalityOption
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    DeliveryTime = reader.GetInt32(reader.GetOrdinal("delivery_time")),
                    ExportEmail = reader.GetString(reader.GetOrdinal("export_email"))
                });
            }

            return options;
        }


        // Добавление новой локальности
        public static async Task AddLocalityAsync(LocalityOption locality)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO localities (id, name, delivery_time, export_email) " +
                      "VALUES (@id, @name, @deliveryTime, @exportEmail)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", locality.Id);
            command.Parameters.AddWithValue("name", locality.Name);
            command.Parameters.AddWithValue("deliveryTime", locality.DeliveryTime);
            command.Parameters.AddWithValue("exportEmail", locality.ExportEmail);
            await command.ExecuteNonQueryAsync();
        }


        // Обновление локальности
        public static async Task SaveLocalitiesAsync(IEnumerable<LocalityOption> localities)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            var sql = "UPDATE localities SET name = @name, delivery_time = @deliveryTime, export_email = @exportEmail WHERE id = @id";

            foreach (var loc in localities)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("name", loc.Name);
                command.Parameters.AddWithValue("deliveryTime", loc.DeliveryTime);
                command.Parameters.AddWithValue("id", loc.Id);
                command.Parameters.AddWithValue("exportEmail", loc.ExportEmail);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }


        // Удаление локальности
        public static async Task DeleteLocalityAsync(LocalityOption locality)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM localities WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", locality.Id);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<MailSettings> GetMailSettingsAsync()
        {
            MailSettings settings = null;
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, download_email, download_password, upload_email, upload_password FROM mail_settings LIMIT 1";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                settings = new MailSettings
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    DownloadEmail = reader.GetString(reader.GetOrdinal("download_email")),
                    // Расшифровываем пароли после загрузки
                    DownloadPassword = Helpers.CryptoHelper.DecryptString(reader.GetString(reader.GetOrdinal("download_password"))),
                    UploadEmail = reader.GetString(reader.GetOrdinal("upload_email")),
                    UploadPassword = Helpers.CryptoHelper.DecryptString(reader.GetString(reader.GetOrdinal("upload_password")))
                };
            }
            return settings;
        }

        public static async Task SaveMailSettingsAsync(MailSettings settings)
        {
            using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sqlUpsert = @"
                INSERT INTO mail_settings (id, download_email, download_password, upload_email, upload_password)
                VALUES (@id, @downloadEmail, @downloadPassword, @uploadEmail, @uploadPassword)
                ON CONFLICT (id)
                DO UPDATE SET
                    download_email = EXCLUDED.download_email,
                    download_password = EXCLUDED.download_password,
                    upload_email = EXCLUDED.upload_email,
                    upload_password = EXCLUDED.upload_password;";


            using var cmdUpdate = new Npgsql.NpgsqlCommand(sqlUpsert, connection);
            cmdUpdate.Parameters.AddWithValue("id", settings.Id);
            cmdUpdate.Parameters.AddWithValue("downloadEmail", settings.DownloadEmail);
            cmdUpdate.Parameters.AddWithValue("downloadPassword", Helpers.CryptoHelper.EncryptString(settings.DownloadPassword));
            cmdUpdate.Parameters.AddWithValue("uploadEmail", settings.UploadEmail);
            cmdUpdate.Parameters.AddWithValue("uploadPassword", Helpers.CryptoHelper.EncryptString(settings.UploadPassword));

            await cmdUpdate.ExecuteNonQueryAsync();
        }

        public static async Task<List<ExportLayoutMapping>> GetExportLayoutMappingsAsync()
        {
            var list = new List<ExportLayoutMapping>();

            using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, field_key, header_text, column_number FROM export_layout_mappings ORDER BY column_number";
            using var command = new Npgsql.NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new ExportLayoutMapping
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    FieldKey = reader.GetString(reader.GetOrdinal("field_key")),
                    HeaderText = reader.GetString(reader.GetOrdinal("header_text")),
                    ColumnNumber = reader.GetInt32(reader.GetOrdinal("column_number"))
                });
            }
            return list;
        }
        public static async Task SaveExportLayoutMappingsAsync(IEnumerable<ExportLayoutMapping> mappings)
        {
            using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            // Для каждого маппинга выполняем UPSERT (INSERT с обновлением при конфликте)
            var sql = @"
                INSERT INTO export_layout_mappings (id, header_text, column_number, field_key)
                VALUES (@id, @headerText, @columnNumber, @fieldKey)
                ON CONFLICT (id) DO UPDATE
                SET header_text = EXCLUDED.header_text,
                    column_number = EXCLUDED.column_number,
                    field_key = EXCLUDED.field_key;";

            foreach (var mapping in mappings)
            {
                using var command = new Npgsql.NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("headerText", mapping.HeaderText);
                command.Parameters.AddWithValue("columnNumber", mapping.ColumnNumber);
                command.Parameters.AddWithValue("id", mapping.Id);
                command.Parameters.AddWithValue("fieldKey", mapping.FieldKey); ;

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }


        public static async Task<ExportLayoutSettings> GetExportLayoutSettingsAsync()
        {
            ExportLayoutSettings settings = null;
            using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, start_row FROM export_layout_settings LIMIT 1";
            using var command = new Npgsql.NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                settings = new ExportLayoutSettings
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    StartRow = reader.GetInt32(reader.GetOrdinal("start_row"))
                };
            }
            return settings;
        }

        public static async Task SaveExportLayoutSettingsAsync(ExportLayoutSettings settings)
        {
            using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sqlUpsert = @"
                INSERT INTO export_layout_settings (id, start_row)
                VALUES (@id, @startRow)
                ON CONFLICT (id)
                DO UPDATE SET start_row = EXCLUDED.start_row;";

            using var command = new Npgsql.NpgsqlCommand(sqlUpsert, connection);
            command.Parameters.AddWithValue("id", settings.Id);
            command.Parameters.AddWithValue("startRow", settings.StartRow);
            await command.ExecuteNonQueryAsync();
        }

    }
}
