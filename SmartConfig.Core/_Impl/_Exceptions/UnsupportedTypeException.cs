﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when a type was used that is not supported by an object converter.
    /// </summary>
    public class UnsupportedTypeException : Exception
    {
        internal UnsupportedTypeException(Type converterType, Type settingType)
        {
            ConverterType = converterType;
            SettingType = settingType;
        }

        public override string Message
        {
            get
            {
                return "Converter = \"$ConverterTypeName\" does not support SettingType = \"$SettingTypeName\"".FormatWith(new
                {
                    ConverterTypeName = ConverterType.Name,
                    SettingTypeName = SettingType.Name
                });
            }
        }

        public Type ConverterType { get; private set; }

        public Type SettingType { get; private set; }
    }
}
