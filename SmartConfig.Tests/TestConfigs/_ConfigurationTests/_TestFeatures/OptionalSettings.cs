﻿namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class OptionalSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [Optional]
        public static string StringSetting { get; set; } = "abc";
    }
}
