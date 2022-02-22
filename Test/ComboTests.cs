using System;
using System.Linq;

using XIVComboExpandedPlugin;
using Xunit;

namespace Test
{
    /// <summary>
    /// Unit tests.
    /// </summary>
    public class ComboTests
    {
        /// <summary>
        /// Test unique enum values.
        /// </summary>
        [Fact]
        public void PresetTest()
        {
            var values1 = Enum
                .GetValues<CustomComboPreset>()
                .Cast<int>()
                .ToArray();

            var values2 = values1
                .Distinct()
                .ToArray();

            Assert.True(values1.Length == values2.Length, "CustomComboPreset collision detected");
        }
    }
}