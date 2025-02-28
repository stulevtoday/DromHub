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
        /// Загружает список диапазонов цен из таблицы range_markups.
        /// </summary>
        public static async Task<List<RangeMarkup>> LoadRangeMarkupsAsync()
        {
            var rangeMarkups = new List<RangeMarkup>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Добавляем столбец id в запрос
            var sql = "SELECT id, min_price, max_price, markup FROM range_markups";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Считываем id как Guid
                var id = reader.GetGuid(reader.GetOrdinal("id"));
                var minPrice = reader.GetDouble(reader.GetOrdinal("min_price"));
                var maxPrice = reader.GetDouble(reader.GetOrdinal("max_price"));
                var markup = reader.GetDouble(reader.GetOrdinal("markup"));

                rangeMarkups.Add(new RangeMarkup
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
        /// Добавляет новый диапазон цен в таблицу range_markups.
        /// </summary>
        public static async Task AddRangeMarkupAsync(RangeMarkup rangeMarkup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO range_markups (id, min_price, max_price, markup) VALUES (@id, @minPrice, @maxPrice, @markup)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", rangeMarkup.Id);
            command.Parameters.AddWithValue("minPrice", rangeMarkup.MinPrice);
            command.Parameters.AddWithValue("maxPrice", rangeMarkup.MaxPrice);
            command.Parameters.AddWithValue("markup", rangeMarkup.Markup);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Удаляет диапазон цен из таблицы range_markups по уникальному идентификатору.
        /// </summary>
        public static async Task DeleteRangeMarkupAsync(RangeMarkup rangeMarkup)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM range_markups WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", rangeMarkup.Id);
            await command.ExecuteNonQueryAsync();
        }

        // Сохранение всех изменений диапазонов (опционально, с транзакцией)
        public static async Task SaveRangeMarkupsAsync(IEnumerable<RangeMarkup> markups)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            var sql = "UPDATE range_markups SET min_price = @minPrice, max_price = @maxPrice, markup = @markup WHERE id = @id";
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

        public static async Task<List<Supplier>> LoadSuppliersAsync()
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

                // 3. Вместо старой таблицы (supplier_excel_mappings) создаём запись в supplier_excel_settings
                var settingId = Guid.NewGuid();
                var sqlSetting = @"
            INSERT INTO supplier_excel_settings (id, supplier_id, initial_row)
            VALUES (@id, @supplierId, @initialRow)";
                using (var cmdSetting = new NpgsqlCommand(sqlSetting, connection, transaction))
                {
                    cmdSetting.Parameters.AddWithValue("id", settingId);
                    cmdSetting.Parameters.AddWithValue("supplierId", supplier.Id);
                    cmdSetting.Parameters.AddWithValue("initialRow", 2); // Например, по умолчанию начинаем со 2-й строки
                    await cmdSetting.ExecuteNonQueryAsync();
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
            await transaction.CommitAsync();
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
        public static async Task<List<SupplierMarkup>> LoadSupplierMarkupsAsync()
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

        // Получение списка локальностей
        public static async Task<List<Locality>> LoadLocalitiesAsync()
        {
            var options = new List<Locality>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, name, delivery_time, email FROM localities";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                options.Add(new Locality
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    DeliveryTime = reader.GetInt32(reader.GetOrdinal("delivery_time")),
                    Email = reader.GetString(reader.GetOrdinal("email"))
                });
            }

            return options;
        }


        // Добавление новой локальности
        public static async Task AddLocalityAsync(Locality locality)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO localities (id, name, delivery_time, email) " +
                      "VALUES (@id, @name, @deliveryTime, @email)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", locality.Id);
            command.Parameters.AddWithValue("name", locality.Name);
            command.Parameters.AddWithValue("deliveryTime", locality.DeliveryTime);
            command.Parameters.AddWithValue("email", locality.Email);
            await command.ExecuteNonQueryAsync();
        }


        // Обновление локальности
        public static async Task SaveLocalitiesAsync(IEnumerable<Locality> localities)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            var sql = "UPDATE localities SET name = @name, delivery_time = @deliveryTime, email = @email WHERE id = @id";

            foreach (var loc in localities)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("name", loc.Name);
                command.Parameters.AddWithValue("deliveryTime", loc.DeliveryTime);
                command.Parameters.AddWithValue("id", loc.Id);
                command.Parameters.AddWithValue("email", loc.Email);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }


        // Удаление локальности
        public static async Task DeleteLocalityAsync(Locality locality)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM localities WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", locality.Id);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<EmailAuthentication> LoadEmailAuthenticationsAsync()
        {
            EmailAuthentication settings = null;
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, download_email, download_password, upload_email, upload_password FROM mail_settings LIMIT 1";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                settings = new EmailAuthentication
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

        public static async Task SaveEmailAuthenticationsAsync(EmailAuthentication settings)
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

        public static async Task<List<ExcelMapping>> ReadExcelMappingsAsync()
        {
            var excelMappings = new List<ExcelMapping>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, name, property FROM excel_mappings";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetGuid(reader.GetOrdinal("id"));
                var name = reader.GetString(reader.GetOrdinal("name"));
                var property = reader.GetString(reader.GetOrdinal("property"));

                excelMappings.Add(new ExcelMapping
                {
                    Id = id,
                    Name = name,
                    Property = property
                });
            }

            return excelMappings;
        }
        public static async Task CreateSupplierExcelMappingAsync(SupplierExcelMapping mapping)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Можно обернуть в транзакцию, если необходимо
            var sql = @"
        INSERT INTO supplier_excel_mappings (id, supplier_setting_id, excel_mapping_id, column_index)
        VALUES (@id, @supplierSetting, @excelMappingId, @columnIndex)";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", mapping.Id == Guid.Empty ? Guid.NewGuid() : mapping.Id);
            command.Parameters.AddWithValue("supplierSetting", mapping.SupplierExcelSettingsId);
            command.Parameters.AddWithValue("excelMappingId", mapping.ExcelMappingId);
            command.Parameters.AddWithValue("columnIndex", mapping.ColumnIndex);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task<List<SupplierExcelSetting>> ReadSupplierExcelSettingsAsync()
        {
            var result = new List<SupplierExcelSetting>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // 1) Считываем все настройки вместе с именем поставщика (JOIN)
            var sqlSettings = @"
SELECT ses.id, 
       ses.supplier_id, 
       ses.initial_row, 
       s.name AS supplier_name
FROM supplier_excel_settings ses
JOIN suppliers s ON ses.supplier_id = s.id";

            using var cmdSettings = new NpgsqlCommand(sqlSettings, connection);
            using var readerSettings = await cmdSettings.ExecuteReaderAsync();
            while (await readerSettings.ReadAsync())
            {
                var setting = new SupplierExcelSetting
                {
                    Id = readerSettings.GetGuid(0),
                    SupplierId = readerSettings.GetGuid(1),
                    InitialRow = readerSettings.GetInt32(2),
                    SupplierName = readerSettings.GetString(3), // получаем имя поставщика
                    Mappings = new List<SupplierExcelMapping>()
                };
                result.Add(setting);
            }
            readerSettings.Close();

            // 2) Считываем все маппинги с JOIN'ом для получения ExcelMappingName
            var dictMappings = new Dictionary<Guid, List<SupplierExcelMapping>>();
            var sqlMappings = @"
SELECT sem.id, 
       sem.supplier_setting_id, 
       sem.excel_mapping_id, 
       sem.column_index,
       em.name AS excel_mapping_name
FROM supplier_excel_mappings sem
JOIN excel_mappings em ON sem.excel_mapping_id = em.id";

            using var cmdMappings = new NpgsqlCommand(sqlMappings, connection);
            using var readerMappings = await cmdMappings.ExecuteReaderAsync();
            while (await readerMappings.ReadAsync())
            {
                var mapping = new SupplierExcelMapping
                {
                    Id = readerMappings.GetGuid(0),
                    SupplierExcelSettingsId = readerMappings.GetGuid(1),
                    ExcelMappingId = readerMappings.GetGuid(2),
                    ColumnIndex = readerMappings.GetInt32(3),
                    ExcelMappingName = readerMappings.GetString(4)
                };

                if (!dictMappings.ContainsKey(mapping.SupplierExcelSettingsId))
                    dictMappings[mapping.SupplierExcelSettingsId] = new List<SupplierExcelMapping>();

                dictMappings[mapping.SupplierExcelSettingsId].Add(mapping);
            }
            readerMappings.Close();

            // 3) Присоединяем маппинги к соответствующим настройкам
            foreach (var setting in result)
            {
                if (dictMappings.TryGetValue(setting.Id, out var listOfMappings))
                {
                    foreach (var m in listOfMappings)
                    {
                        setting.Mappings.Add(m);
                    }
                }
            }

            return result;
        }



        public static async Task UpdateSupplierExcelSettingsAsync(IEnumerable<SupplierExcelSetting> settings)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var setting in settings)
                {
                    // 1) Проверяем, есть ли запись в supplier_excel_settings
                    var checkSql = "SELECT COUNT(*) FROM supplier_excel_settings WHERE id = @id";
                    using var checkCmd = new NpgsqlCommand(checkSql, connection, transaction);
                    checkCmd.Parameters.AddWithValue("id", setting.Id);
                    var count = (long)await checkCmd.ExecuteScalarAsync();

                    if (count == 0)
                    {
                        // Вставляем новую запись
                        var insertSql = @"
                    INSERT INTO supplier_excel_settings (id, supplier_id, initial_row)
                    VALUES (@id, @supplierId, @initialRow)";
                        using var insertCmd = new NpgsqlCommand(insertSql, connection, transaction);
                        insertCmd.Parameters.AddWithValue("id", setting.Id);
                        insertCmd.Parameters.AddWithValue("supplierId", setting.SupplierId);
                        insertCmd.Parameters.AddWithValue("initialRow", setting.InitialRow);
                        await insertCmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        // Обновляем существующую
                        var updateSql = @"
                    UPDATE supplier_excel_settings
                    SET supplier_id = @supplierId,
                        initial_row = @initialRow
                    WHERE id = @id";
                        using var updateCmd = new NpgsqlCommand(updateSql, connection, transaction);
                        updateCmd.Parameters.AddWithValue("id", setting.Id);
                        updateCmd.Parameters.AddWithValue("supplierId", setting.SupplierId);
                        updateCmd.Parameters.AddWithValue("initialRow", setting.InitialRow);
                        await updateCmd.ExecuteNonQueryAsync();
                    }

                    // 2) Удаляем старые маппинги для данной настройки
                    var deleteMappingsSql = "DELETE FROM supplier_excel_mappings WHERE supplier_setting_id = @settingId";
                    using var delCmd = new NpgsqlCommand(deleteMappingsSql, connection, transaction);
                    delCmd.Parameters.AddWithValue("settingId", setting.Id);
                    await delCmd.ExecuteNonQueryAsync();

                    // 3) Вставляем актуальные маппинги заново
                    if (setting.Mappings != null)
                    {
                        foreach (var mapping in setting.Mappings)
                        {
                            var insertMappingSql = @"
                        INSERT INTO supplier_excel_mappings (id, supplier_setting_id, excel_mapping_id, column_index)
                        VALUES (@id, @settingId, @excelMappingId, @columnIndex)";

                            using var insMapCmd = new NpgsqlCommand(insertMappingSql, connection, transaction);
                            insMapCmd.Parameters.AddWithValue("id", mapping.Id == Guid.Empty ? Guid.NewGuid() : mapping.Id);
                            insMapCmd.Parameters.AddWithValue("settingId", setting.Id);
                            insMapCmd.Parameters.AddWithValue("excelMappingId", mapping.ExcelMappingId); // или PropertyId
                            insMapCmd.Parameters.AddWithValue("columnIndex", mapping.ColumnIndex);
                            await insMapCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
