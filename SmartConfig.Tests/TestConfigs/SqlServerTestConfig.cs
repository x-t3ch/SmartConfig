﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(ConnectionStringName = "SmartConfigEntities", Version = "1.0.0")]
    public class SqlServerTestConfig
    {
        public static int Int32Field;
    }
}
