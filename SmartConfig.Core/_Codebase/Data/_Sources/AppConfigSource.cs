﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SmartConfig.Collections;
using SmartConfig.Paths;
using SmartUtilities.Collections;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements the app.configuration as data source.
    /// </summary>
    public class AppConfigSource : DataSource<Setting>
    {
        private readonly System.Configuration.Configuration _exeConfiguration;

        private readonly IDictionary<string, IAppConfigSectionSource> _appConfigSectionSources;

        public AppConfigSource()
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            _appConfigSectionSources = new object[]
            {
                new AppSettingsSectionSource(_exeConfiguration.AppSettings),
                new ConnectionStringsSectionSource(_exeConfiguration.ConnectionStrings),
            }
            .Cast<IAppConfigSectionSource>()
            .ToDictionary(x => x.SectionName, x => x, StringComparer.OrdinalIgnoreCase);
        }
       
        private IAppConfigSectionSource GetAppConfigSectionSource(NameKey nameKey)
        {
            Debug.Assert(nameKey != null);

            var sectionName = new AppConfigPath(nameKey.Value).SectionName;

            IAppConfigSectionSource appConfigSectionSource;
            if (!_appConfigSectionSources.TryGetValue(sectionName, out appConfigSectionSource))
            {
                // todo: throw section not found exception
            }

            return appConfigSectionSource;
        }

        public override IReadOnlyCollection<Type> SupportedSettingValueTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyCollection keys)
        {
            if (keys == null) { throw new ArgumentNullException(nameof(keys)); }
            if (!keys.Any()) { throw new InvalidOperationException("There must be at least one key defined."); }

            var appConfigSectionSource = GetAppConfigSectionSource(keys.NameKey);

            var settingName = new AppConfigPath(keys.NameKey.Value).ToString();
            var value = appConfigSectionSource.Select(settingName);

            return value;
        }

        public override void Update(SettingKeyCollection keys, object value)
        {
            Debug.Assert(keys != null && keys.Any());

            var appConfigSectionSource = GetAppConfigSectionSource(keys.NameKey);
            var settingName = new AppConfigPath(keys.NameKey.Value).ToString();

            appConfigSectionSource.Update(settingName, value?.ToString());
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
