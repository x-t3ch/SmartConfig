﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class Setting
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public Setting() { }

        public Setting(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            Name = path;
            if (namespaces != null)
            {
                foreach (var ns in namespaces)
                {
                    _values.Add(ns.Key, ns.Value);
                }
            }
        }

        public Setting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
            : this(path, namespaces)
        {
            Value = value;
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public SettingPath Name
        {
            [DebuggerStepThrough]
            get { return (SettingPath)_values[nameof(Name)]; }

            [DebuggerStepThrough]
            set { _values[nameof(Name)] = value; }
        }

        public object Value
        {
            [DebuggerStepThrough]
            get { return _values[nameof(Value)]; }

            [DebuggerStepThrough]
            set { _values[nameof(Value)] = value; }
        }

        public string ConfigName
        {
            [DebuggerStepThrough]
            get { return (string)_values[nameof(ConfigName)]; }

            [DebuggerStepThrough]
            set { _values[nameof(ConfigName)] = value; }
        }

        public IReadOnlyDictionary<string, object> Namespaces
        {
            get
            {
                var namespaces = _values.Where(x => !x.Key.Equals(nameof(Name)) && !x.Key.Equals(nameof(Value)));
                return namespaces.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            }
        }

        public bool NamespaceEquals(string key, object value)
        {
            var temp = (object)null;
            return Namespaces.TryGetValue(key, out temp) && temp.Equals(value);
        }

        public bool IsLike(Setting setting)
        {
            return
                Name.IsLike(setting.Name) &&
                Namespaces.All(ns => setting.NamespaceEquals(ns.Key, ns.Value));
        }

        //public static bool operator ==(Setting x, Setting y)
        //{
        //    if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

        //    return 
        //        x.na;
        //}

        //public static bool operator !=(Setting x, Setting y)
        //{
        //    return !(x == y);
        //}
    }
}
