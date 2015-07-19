﻿using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class ColorConverterTests
    {
        [TestMethod]
        public void TestDeserializeObject()
        {
            var converter = new global::SmartConfig.Converters.ColorConverter();
            Assert.AreEqual(Color.FromArgb(255, 0, 0), (Color)(Color32)converter.DeserializeObject("Red", typeof(Color)));
            Assert.AreEqual(Color.FromArgb(1, 2, 3), (Color)(Color32)converter.DeserializeObject("1,2,3", typeof(Color)));
            Assert.AreEqual(Color.FromArgb(255, 1, 2), (Color)(Color32)converter.DeserializeObject("#FF0102", typeof(Color)));
        }

        [TestMethod]
        public void TestSerializeObject()
        {
            //var valueTypeConverter = new global::SmartConfig.Converters.ColorConverter();
            //Assert.AreEqual("123", valueTypeConverter.SerializeObject(123));
            //Assert.AreEqual("123", valueTypeConverter.SerializeObject((Int32?)123));
            //Assert.IsNull(valueTypeConverter.SerializeObject((Int32?)null));
        }
    }
}
