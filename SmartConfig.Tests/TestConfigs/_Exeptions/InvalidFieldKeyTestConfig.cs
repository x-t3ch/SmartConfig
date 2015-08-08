﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    // This is commented out by purpose to test the missing attribute.
    [SmartConfig]
    [FieldKey("abc")]
    public static class InvalidFieldKeyTestConfig
    {
        public static string StringField;
    }
}
