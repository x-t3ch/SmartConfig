using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Converters;
using Reusable.Data.Annotations;
using Reusable.Drawing;
using SmartConfig;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
using SmartConfig.DataStores;

namespace SmartConfig.Core.Tests
{
    // ReSharper disable BuiltInTypeReferenceStyle
    // ReSharper disable InconsistentNaming
    // ReSharper disable CheckNamespace

    using Reusable.Fuse;
    using Reusable.Fuse.Testing;

    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void LoadSave_EmptyConfig()
        {
            var getSettingsCallCount = 0;
            var saveSettingsCallCount = 0;

            var testStore = new TestStore
            {
                GetSettingsCallback = s =>
                {
                    getSettingsCallCount++;
                    return Enumerable.Empty<Setting>();
                },
                SaveSettingsCallback = s =>
                {
                    saveSettingsCallCount++;
                    return 0;
                }
            };

            Configuration.Loader.From(testStore).Select(typeof(EmptyConfig));            

            getSettingsCallCount.Verify().IsEqual(0);
            saveSettingsCallCount.Verify().IsEqual(0);
        }

        [TestMethod]
        public void LoadSave_IntegralConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Loader.From(new MemoryStore
            {
                {nameof(IntegralConfig.SByte), SByte.MaxValue.ToString()},
                {nameof(IntegralConfig.Byte), Byte.MaxValue.ToString()},
                {nameof(IntegralConfig.Char), Char.MaxValue.ToString()},
                {nameof(IntegralConfig.Int16), Int16.MaxValue.ToString()},
                {nameof(IntegralConfig.Int32), Int32.MaxValue.ToString()},
                {nameof(IntegralConfig.Int64), Int64.MaxValue.ToString()},
                {nameof(IntegralConfig.UInt16), UInt16.MaxValue.ToString()},
                {nameof(IntegralConfig.UInt32), UInt32.MaxValue.ToString()},
                {nameof(IntegralConfig.UInt64), UInt64.MaxValue.ToString()},
                {nameof(IntegralConfig.Single), Single.MaxValue.ToString("R", culture)},
                {nameof(IntegralConfig.Double), Double.MaxValue.ToString("R", culture)},
                {nameof(IntegralConfig.Decimal), Decimal.MaxValue.ToString(culture)},

                {nameof(IntegralConfig.String), "foo"},
                {nameof(IntegralConfig.False), bool.FalseString},
                {nameof(IntegralConfig.True), bool.TrueString},
                {nameof(IntegralConfig.Enum), TestEnum.TestValue2.ToString()},
            })
            .Select(typeof(IntegralConfig));

            IntegralConfig.SByte.Verify().IsEqual(SByte.MaxValue);
            IntegralConfig.Byte.Verify().IsEqual(Byte.MaxValue);
            IntegralConfig.Char.Verify().IsEqual(Char.MaxValue);
            IntegralConfig.Int16.Verify().IsEqual(Int16.MaxValue);
            IntegralConfig.Int32.Verify().IsEqual(Int32.MaxValue);
            IntegralConfig.Int64.Verify().IsEqual(Int64.MaxValue);
            IntegralConfig.UInt16.Verify().IsEqual(UInt16.MaxValue);
            IntegralConfig.UInt32.Verify().IsEqual(UInt32.MaxValue);
            IntegralConfig.UInt64.Verify().IsEqual(UInt64.MaxValue);
            IntegralConfig.Single.Verify().IsEqual(Single.MaxValue);
            IntegralConfig.Double.Verify().IsEqual(Double.MaxValue);
            IntegralConfig.Decimal.Verify().IsEqual(Decimal.MaxValue);

            IntegralConfig.String.Verify().IsEqual("foo");
            IntegralConfig.False.Verify().IsEqual(false);
            IntegralConfig.True.Verify().IsEqual(true);
            IntegralConfig.Enum.Verify().IsTrue(x => x == TestEnum.TestValue2);

            Configuration.Save(typeof(IntegralConfig));
        }

        [TestMethod]
        public void LoadSave_DateTimeConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Loader.From(new MemoryStore
            {
                {nameof(DateTimeConfig.DateTime), new DateTime(2016, 7, 30).ToString(culture)},
            })
            .Select(typeof(DateTimeConfig));

            DateTimeConfig.DateTime.Verify().IsEqual(new DateTime(2016, 7, 30));

            Configuration.Save(typeof(DateTimeConfig));
        }

        [TestMethod]
        public void LoadSave_ColorConfig()
        {
            Configuration.Loader.From(new MemoryStore
            {
                {nameof(ColorConfig.ColorName), Color.DarkRed.Name},
                {nameof(ColorConfig.ColorDec), $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}"},
                {nameof(ColorConfig.ColorHex), Color.Beige.ToArgb().ToString("X")},
            })
            .Select(typeof(ColorConfig));

            ColorConfig.ColorName.ToArgb().Verify().IsEqual(Color.DarkRed.ToArgb());
            ColorConfig.ColorDec.ToArgb().Verify().IsEqual(Color.Plum.ToArgb());
            ColorConfig.ColorHex.ToArgb().Verify().IsEqual(Color.Beige.ToArgb());

            Configuration.Save(typeof(ColorConfig));
        }

        [TestMethod]
        public void LoadSave_JsonConfig()
        {
            Configuration.Loader.From(new MemoryStore
            {
                {nameof(JsonConfig.JsonArray), "[5, 8, 13]"},
            })
            .Select(typeof(JsonConfig));

            JsonConfig.JsonArray.Verify().IsTrue(x => x.SequenceEqual(new List<int> { 5, 8, 13 }));

            Configuration.Save(typeof(JsonConfig));
        }

        [TestMethod]
        public void LoadSave_ItemizedArrayConfig()
        {
            Configuration.Loader.From(new MemoryStore
            {
                {$"{nameof(ItemizedArrayConfig.ItemizedArray)}[0]", "5"},
                {$"{nameof(ItemizedArrayConfig.ItemizedArray)}[1]", "8"},
            })
            .Select(typeof(ItemizedArrayConfig));

            ItemizedArrayConfig.ItemizedArray.Length.Verify().IsEqual(2);
            ItemizedArrayConfig.ItemizedArray[0].Verify().IsEqual(5);
            ItemizedArrayConfig.ItemizedArray[1].Verify().IsEqual(8);

            Configuration.Save(typeof(ItemizedArrayConfig));
        }

        [TestMethod]
        public void LoadSave_ItemizedDictionaryConfig()
        {
            Configuration.Loader.From(new MemoryStore
            {
                {$"{nameof(ItemizedDictionaryConfig.ItemizedDictionary)}[foo]", "21"},
                {$"{nameof(ItemizedDictionaryConfig.ItemizedDictionary)}[bar]", "34"},
            })
            .Select(typeof(ItemizedDictionaryConfig));

            ItemizedDictionaryConfig.ItemizedDictionary.Count.Verify().IsEqual(2);
            ItemizedDictionaryConfig.ItemizedDictionary["foo"].Verify().IsEqual(21);
            ItemizedDictionaryConfig.ItemizedDictionary["bar"].Verify().IsEqual(34);

            Configuration.Save(typeof(ItemizedDictionaryConfig));
        }

        [TestMethod]
        public void LoadSave_NestedConfig()
        {
            Configuration.Loader.From(new MemoryStore
            {
                {nameof(NestedConfig.SubConfig) + "." + nameof(NestedConfig.SubConfig.NestedString), "Quux"},
            })
            .Select(typeof(NestedConfig));

            NestedConfig.SubConfig.NestedString.Verify().IsEqual("Quux");

            Configuration.Save(typeof(NestedConfig));
        }

        [TestMethod]
        public void LoadSave_IgnoredConfig()
        {
            Configuration.Loader.From(new MemoryStore()).Select(typeof(IgnoredConfig));
            IgnoredConfig.SubConfig.IgnoredString.Verify().IsEqual("Grault");

            Configuration.Save(typeof(IgnoredConfig));
        }

        [TestMethod]
        public void LoadSave_OptionalConfig()
        {
            Configuration.Loader.From(new MemoryStore()).Select(typeof(OptionalConfig));
            OptionalConfig.OptionalSetting.Verify().IsEqual("Waldo");

            Configuration.Save(typeof(OptionalConfig));
        }      

        [TestMethod]
        public void LoadSave_Where_FromExpression()
        {
            var tags = default(TagCollection);
            Configuration.Loader
                .From(new TestStore
                {
                    GetSettingsCallback = s =>
                    {
                        tags = s.Tags;
                        return Enumerable.Empty<Setting>();
                    }
                })
                .Where(() => TestConfig.Foo)
                .Select(typeof(TestConfig));

            tags.Verify().IsNotNull();
            tags["Foo"].Verify().IsTrue(x => x.ToString() == "Bar");
        }

        // Test invalid usage errors

        [TestMethod]
        public void Load_NonStaticConfigType_Throws_ValidationException()
        {
            new Action(() =>
            {
                Configuration.Loader.From(new MemoryStore()).Select(typeof(NonStaticConfig));
            })
            .Verify()
            .Throws<ValidationException>(ex =>
            {
                // Config type "SmartConfig.Core.Tests.Integration.ConfigurationTestConfigs.NonStaticConfig" must be static.
                /*
#2
ConfigurationLoadException: Could not load "TestConfig" from "SqlServerStore".
- ConfigType: SmartConfig.DataStores.SqlServerStore.Tests.TestConfig
- DataSourceType: SmartConfig.DataStores.SqlServerStore
- Data:[]

#1
SettingNotFoundException: Setting "TestSetting" not found. You need to provide a value for it or decorate it with the "OptionalAttribute".
- WeakFullName: TestSetting
                 
                 */
                Debug.Write(ex.Message);
            });
        }

        [TestMethod]
        public void Load_ValueNull()
        {
            new Action(() => Configuration.Loader.From(new MemoryStore()).Where("foo", null)).Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void Load_Throws_SettingNotFoundException()
        {
            new Action(() =>
            {
                Configuration.Loader.From(new TestStore()).Select(typeof(SettingNotFoundConfig));
            })
            .Verify().Throws<ConfigurationException>(exception =>
            {
                exception.InnerException.Verify().IsInstanceOfType(typeof(AggregateException));
                //(exception.InnerException as AggregateException).InnerExceptions.OfType<SettingNotFoundException>().Count().Verify().IsEqual(1);
            });
        }

        [TestMethod]
        public void Load_ConfigTypeNotDecorated_ThrowsValidationException()
        {
            new Action(() =>
            {
                Configuration.Loader.From(new MemoryStore()).Select(typeof(ConfigNotDecorated));
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void Load_RequiredSettingNotFound()
        {
            new Action(() =>
            {
                Configuration.Loader.From(new MemoryStore()).Select(typeof(RequiredSettings));
            })
            .Verify().Throws<ConfigurationException>();
        }

        [TestMethod]
        public void Load_PropertyNameNullOrEmpty()
        {
            new Action(() =>
            {
                Configuration.Loader.From(new MemoryStore()).Where(null, null);
            })
                .Verify().Throws<ValidationException>();

            new Action(() =>
            {
                Configuration.Loader.From(new MemoryStore()).Where(string.Empty, null);
            })
                .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void Save_IntegralConfig_IntegralSettings()
        {

        }
    }
}