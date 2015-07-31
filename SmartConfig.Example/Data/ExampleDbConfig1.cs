﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Example.Data
{
    [SmartConfig(Name = "ExampleApp1")]
    static class ExampleDbConfig1
    {
        [Optional]
        public static string Welcome = "Hello World!";
    }
}
