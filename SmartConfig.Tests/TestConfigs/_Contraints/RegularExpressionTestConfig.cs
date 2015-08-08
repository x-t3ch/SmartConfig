﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class RegularExpressionTestConfig
    {
        [RegularExpression(@"\d{2}")]
        public static string StringField;
    }
}