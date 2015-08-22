﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class TestDataSource : DataSource<TestSetting>
    {
        public Func<string, string> SelectFunc;

        public Action<string, string> UpdateAction;

        public override string Select(string defaultKeyValue)
        {
            return SelectFunc(defaultKeyValue);
        }

        public override void Update(string defaultKeyValue, string value)
        {
            UpdateAction(defaultKeyValue, value);
        }
    }
}
