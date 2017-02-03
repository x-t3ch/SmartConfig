﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Reusable;
using Reusable.Data.Annotations;
using Reusable.Fuse;
using SmartConfig.Data.Annotations;

namespace SmartConfig.Data
{
    // stores information about a single setting
    [DebuggerDisplay("{DebuggerDispaly,nq}")]
    internal class SettingProperty
    {
        internal SettingProperty(PropertyInfo property)
        {
            Property = property;
            Path = new SettingPath(Property.GetPath());

            // an itemzed setting must be an enumerable
            if (IsItemized && !IsEnumerable) throw new ItemizedSettingPropertyException();            
        }

        private PropertyInfo Property { get; }

        public bool IsOptional => Property.GetCustomAttribute<OptionalAttribute>() != null;

        public bool IsItemized => Property.GetCustomAttribute<ItemizedAttribute>() != null;

        public IEnumerable<ValidationAttribute> Validations => Property.GetCustomAttributes<ValidationAttribute>();

        public bool IsEnumerable => Property.PropertyType.IsEnumerable();

        public Type Type => Property.PropertyType;

        public SettingPath Path { get; }

        public object Value
        {
            get { return Property.GetValue(null); }
            set { Property.SetValue(null, value); }
        }

        private string DebuggerDispaly => $"Urn = \"{Path.StrongFullName}\"";

        public override bool Equals(object obj)
        {
            var other = obj as SettingProperty;
            return other != null && other.Property == Property;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode();
        }

        public override string ToString()
        {
            // FullConfig2.Foo.Bar[baz] (required) System.Collections.Generic.List < int >
            //return $"{ConfigType.Name}.{Path.StrongFullName} {(IsOptional ? "optional" : "required")} {Type.ToShortString()}";
            return $"{Path.StrongFullName} {(IsOptional ? "optional" : "required")} {Type.ToShortString()}";
        }
    }
}
