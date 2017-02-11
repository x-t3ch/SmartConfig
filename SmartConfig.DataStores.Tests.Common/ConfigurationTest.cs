﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.Tests.Common
{
    public class ConfigurationTestBase
    {
        protected IDictionary<Type, DataStore> DataStores { get; set; }

        [TestMethod]
        public void Load_Modify_Save_Load_TestConfig1()
        {
            // Load 1

            Configuration.Builder.From(DataStores[typeof(TestConfig1)]).Select(typeof(TestConfig1));
            
            // Verify load 1

            TestConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß");
            TestConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó");

            TestConfig1.ArraySetting.Length.Verify().IsEqual(2);
            TestConfig1.ArraySetting[0].Verify().IsEqual(5);
            TestConfig1.ArraySetting[1].Verify().IsEqual(8);

            TestConfig1.DictionarySetting.Count.Verify().IsEqual(2);
            TestConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            TestConfig1.DictionarySetting["bar"].Verify().IsEqual(34);

            TestConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            TestConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Ignored value");

            // Modify & save

            TestConfig1.Utf8SettingDE = "äöüß--";
            TestConfig1.Utf8SettingPL = "ąęśćżźó--";

            TestConfig1.ArraySetting = new[] { 5, 8, 13 };
            TestConfig1.DictionarySetting["baz"] = 55;

            Configuration.Save(typeof(TestConfig1));

            // Reset before load 2

            TestConfig1.Utf8SettingDE = null;
            TestConfig1.Utf8SettingPL = null;

            TestConfig1.ArraySetting = null;
            TestConfig1.DictionarySetting = null;

            // Load 2

            Configuration.Load(typeof(TestConfig1));

            // Verify load 2

            TestConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß--");
            TestConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó--");

            TestConfig1.ArraySetting.Length.Verify().IsEqual(3);
            TestConfig1.ArraySetting[0].Verify().IsEqual(5);
            TestConfig1.ArraySetting[1].Verify().IsEqual(8);
            TestConfig1.ArraySetting[2].Verify().IsEqual(13);

            TestConfig1.DictionarySetting.Count.Verify().IsEqual(3);
            TestConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            TestConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            TestConfig1.DictionarySetting["baz"].Verify().IsEqual(55);

            TestConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            TestConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Ignored value");
        }
    }
}
