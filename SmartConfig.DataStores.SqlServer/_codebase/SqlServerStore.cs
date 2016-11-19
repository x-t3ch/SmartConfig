﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Reusable;
using Reusable.Data;
using Reusable.Extensions;
using Reusable.Validations;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServerStore : DataStore
    {
        public SqlServerStore(string nameOrConnectionString, Action<SettingTableConfiguration.Builder> configure = null) : base(new[] { typeof(string) })
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            var connectionStringName = nameOrConnectionString.ToConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = AppConfigRepository.GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var settingTablePropertiesBuilder = new SettingTableConfiguration.Builder();
            configure?.Invoke(settingTablePropertiesBuilder);
            SettingTableConfiguration = settingTablePropertiesBuilder.Build();
        }

        internal AppConfigRepository AppConfigRepository { get; } = new AppConfigRepository();

        public string ConnectionString { get; }

        public SettingTableConfiguration SettingTableConfiguration { get; }

        public Type MapDataType(Type settingType) => typeof(string);

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var commandFactory = new CommandFactory(SettingTableConfiguration);
                using (var command = commandFactory.CreateSelectCommand(connection, setting))
                {
                    connection.Open();
                    command.Prepare();

                    using (var settingReader = command.ExecuteReader())
                    {
                        while (settingReader.Read())
                        {
                            var result = new Setting
                            {
                                Name = SettingUrn.Parse((string)settingReader[nameof(Setting.Name)]),
                                Value = settingReader[nameof(Setting.Value)]
                            };
                            foreach (var attribute in setting.Attributes)
                            {
                                result[attribute.Key] = settingReader[attribute.Key];
                            }
                            yield return result;
                        }
                    }
                }
            }
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var commandFactory = new CommandFactory(SettingTableConfiguration);

                using (var transaction = connection.BeginTransaction())
                {
                    var groups = settings.GroupBy(x => x.WeakId);

                    var rowsAffected = 0;
                    try
                    {
                        foreach (var group in groups)
                        {
                            var deleted = false;
                            foreach (var setting in group)
                            {
                                // Before adding this group of settings delete the old ones first.
                                if (!deleted)
                                {
                                    using (var deleteCommand = commandFactory.CreateDeleteCommand(connection, setting))
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.Prepare();
                                        rowsAffected += deleteCommand.ExecuteNonQuery();
                                    }
                                    deleted = true;
                                }

                                // insert new setting
                                using (var insertCommand = commandFactory.CreateInsertCommand(connection, setting))
                                {
                                    insertCommand.Transaction = transaction;
                                    insertCommand.Prepare();
                                    rowsAffected += insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
                        return rowsAffected;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }

    public static class ConfigurationBuilderExtensions
    {
        public static Configuration.ConfigurationBuilder FromSqlServer(
            this Configuration.ConfigurationBuilder configurationBuilder,
            string nameOrConnectionString,
            Action<SettingTableConfiguration.Builder> configure = null
        )
        {
            return configurationBuilder.From(new SqlServerStore(nameOrConnectionString, configure));
        }
    }
}

