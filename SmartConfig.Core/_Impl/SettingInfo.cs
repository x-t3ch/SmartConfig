﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    [DebuggerDisplay("ConfigType.Name = {ConfigType.Name} SettingPath = \"{SettingPath}\" IsInternal = \"{IsInternal}\"")]
    public class SettingInfo
    {
        private readonly FieldInfo _fieldInfo;
        private readonly Type _settingType;

        internal SettingInfo(Type configType, string settingName, Type settingType)
        {
            ConfigType = configType;
            ConfigName = configType.GetCustomAttribute<SmartConfigAttribute>(false).Name;
            _settingType = settingType;
            SettingPath = new SettingPath(ConfigName, settingName);
        }

        internal SettingInfo(MemberInfo member)
        {
            _fieldInfo = (FieldInfo)member;

            var path = new List<string> { member.Name };

            var type = member.DeclaringType;
            while (type != null)
            {
                var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>(false);
                var isSmartConfigType = smartConfigAttribute != null;
                if (isSmartConfigType)
                {
                    path.Add(smartConfigAttribute.Name);
                    ConfigType = type;
                    ConfigName = smartConfigAttribute.Name;
                    SettingPath = new SettingPath(((IEnumerable<string>)path).Reverse());
                }
                path.Add(type.Name);
                type = type.DeclaringType;

                if (type == null && smartConfigAttribute == null)
                {
                    throw new SmartConfigTypeNotFoundException();
                }
            }
        }

        internal static SettingInfo From<TField>(Expression<Func<TField>> expression)
        {
            Debug.Assert(expression != null);

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) throw new ArgumentException("Member expression expected.");

            return new SettingInfo(memberExpression.Member);
        }

        #region Config Info

        public Type ConfigType { get; private set; }

        public string ConfigName { get; private set; }

        #endregion

        #region Setting Info

        public Type SettingType
        {
            get { return IsInternal ? _settingType : _fieldInfo.FieldType; }
        }

        public Type ConverterType
        {
            get
            {
                if (IsInternal)
                {
                    return SettingType;
                }

                if (SettingType.BaseType == typeof(Enum))
                {
                    return typeof(Enum);
                }

                var objectConverterAttribute = _fieldInfo.GetCustomAttribute<ObjectConverterAttribute>(false);
                if (objectConverterAttribute != null)
                {
                    return objectConverterAttribute.Type;
                }

                return SettingType;
            }
        }

        public SettingPath SettingPath { get; private set; }

        public IEnumerable<ConstraintAttribute> SettingConstraints
        {
            get
            {
                return
                  _fieldInfo == null
                  ? Enumerable.Empty<ConstraintAttribute>()
                  : _fieldInfo.GetCustomAttributes<ConstraintAttribute>(false);
            }
        }

        public bool IsOptional
        {
            get { return _fieldInfo.GetCustomAttribute<OptionalAttribute>() != null; }
        }

        //public bool IsNullable
        //{
        //    get
        //    {
        //        var isNullable =
        //            (_fieldInfo.FieldType.IsValueType && _fieldInfo.FieldType.IsNullable())
        //            || _fieldInfo.GetCustomAttribute<OptionalAttribute>() != null;
        //        return isNullable;
        //    }
        //}

        public object Value
        {
            get { return _fieldInfo.GetValue(null); }
            set { _fieldInfo.SetValue(null, value); }
        }
        #endregion

        internal bool IsInternal
        {
            get { return _settingType != null; }
        }
    }
}