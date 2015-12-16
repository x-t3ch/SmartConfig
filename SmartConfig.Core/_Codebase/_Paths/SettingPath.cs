﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    public class SettingPath
    {
        private readonly List<string> _names = new List<string>();

        //public static string Create<T>(Expression<Func<T>> expression)
        //{
        //    var memberInfo = Utilities.GetMemberInfo(expression);
        //    var settingInfo = SettingInfo.Create(expression);

        //    // Replace configuration namespace and class name with application name.
        //    var elementName = Regex.Replace(memberInfo.ReflectedType.FullName, @"^" + settingInfo.ConfigurationType.FullName + @"\.?", string.Empty);

        //    // Replace subclass separator "+" with a "."
        //    elementName = Regex.Replace(elementName, @"\+", ".");

        //    // Add member name.
        //    //elementName = Combine(new[] { elementName, memberInfo.Name });

        //    // Add application name if available.
        //    //var configName = Combine(new[] { settingInfo.ConfigName, elementName });

        //    // Remove invalid "." at the beginning. It's easier and cleaner to remove it here then to prevent it above.
        //    //elementName = Regex.Replace(elementName, @"^\.", string.Empty);

        //    //return configName;
        //    return String.Empty;
        //}

        //public SettingPath() { }

        protected SettingPath() { }

        public SettingPath(IEnumerable<string> names)
        {
            _names.AddRange(names);
        }

        public SettingPath(string name) : this(new[] { name }) { }

        public SettingPath(SettingPath path) : this(path._names) { }

        public SettingPath(params string[] names) : this((IEnumerable<string>)names) { }

        public string Delimiter { get; set; } = ".";

        public virtual IReadOnlyCollection<string> Names => _names.AsReadOnly();

        public override string ToString()
        {
            return string.Join(Delimiter, _names);
        }       

        public static implicit operator string(SettingPath settingPath)
        {
            return settingPath.ToString();
        }
    }
}