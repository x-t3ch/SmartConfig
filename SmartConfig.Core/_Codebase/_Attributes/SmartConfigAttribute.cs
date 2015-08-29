﻿using System;
using System.Text.RegularExpressions;

namespace SmartConfig
{
    /// <summary>
    /// Marks a type as <c>SmartConfig</c> and allows to set additional options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        private string _name;

        /// <summary>
        /// Gets or sets a custom config name. The name must be a valid CLR identifier.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Name");
                }

                // https://regex101.com/r/dW3gF3/1
                if (!Regex.IsMatch(value, @"^[A-Z_][A-Z0-9_]+", RegexOptions.IgnoreCase))
                {
                    throw new ArgumentOutOfRangeException("Name", "Config name must be a valid CLR identifier.");
                }

                _name = value;
            }
        }
    }
}
