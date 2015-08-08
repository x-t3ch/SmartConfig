﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class ValueTypesTestConfig
    {
        public static bool BooleanField;
        public static char CharField;
        public static short Int16Field;
        public static int Int32Field;
        public static long Int64Field;
        public static float SingleField;
        public static double DoubleField;
        public static decimal DecimalField;
    }
}