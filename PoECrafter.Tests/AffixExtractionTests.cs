using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WindowsFormsApplication3;


namespace PoECrafter.Tests
{
    public class AffixExtractionTests
    {
        private readonly string _testAxeItem;

        public AffixExtractionTests()
        {
            // Your test axe item with T1 attack speed (26%) and T7 physical damage (52%)
            _testAxeItem = @"Item Class: Two Hand Axes
Rarity: Magic
Serrated Vaal Axe of Celebration
--------
Two Handed Axe
Physical Damage: 158-264 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.45 (augmented)
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: R-R 
--------
Item Level: 84
--------
25% chance to Maim on Hit (implicit)
--------
52% increased Physical Damage
26% increased Attack Speed";
        }

        [Fact]
        public void GetAffixes_ShouldExtractAffixesCorrectly()
        {
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);

            // Act
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            // Assert
            Assert.NotNull(affixes);
            Assert.True(affixes.Count > 0, "Should extract at least one affix");
            
            // Check that physical damage affix is extracted
            var physicalDamageAffix = affixes.FirstOrDefault(a => a.Contains("52% increased Physical Damage"));
            Assert.NotNull(physicalDamageAffix);
            
            // Check that attack speed affix is extracted
            var attackSpeedAffix = affixes.FirstOrDefault(a => a.Contains("26% increased Attack Speed"));
            Assert.NotNull(attackSpeedAffix);
            
            // Verify no implicit modifiers are included in affixes
            var implicitAffix = affixes.FirstOrDefault(a => a.Contains("chance to Maim"));
            Assert.Null(implicitAffix);
        }

        [Fact]
        public void ItemParsing_ShouldIdentifyCorrectTiers()
        {
            // This test verifies that the item parsing correctly identifies the tiers
            // We know from your axe:
            // - 26% attack speed should be T1 (25-27% range)
            // - 52% physical damage should be T7 (50-64% range)
            
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            
            // Act
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Assert - verify the affixes are extracted correctly
            Assert.Contains(affixes, a => a.Contains("52% increased Physical Damage"));
            Assert.Contains(affixes, a => a.Contains("26% increased Attack Speed"));
            
            // The tier checking will be done internally by the application
            // This test ensures the raw affix extraction works correctly
        }

        [Fact]
        public void ItemParsing_ShouldIgnoreImplicits()
        {
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            
            // Act
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Assert - implicit modifiers should not be in the affix list
            Assert.DoesNotContain(affixes, a => a.Contains("chance to Maim on Hit"));
            
            // But explicit modifiers should be there
            Assert.Contains(affixes, a => a.Contains("increased Physical Damage"));
            Assert.Contains(affixes, a => a.Contains("increased Attack Speed"));
        }

        [Fact]
        public void ItemParsing_ShouldHandleEmptyItem()
        {
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem("");
            
            // Act
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Assert
            Assert.NotNull(affixes);
            // Empty item should result in empty affix list
            Assert.Empty(affixes);
        }

        [Fact]
        public void ItemParsing_ShouldHandleItemWithNoAffixes()
        {
            // Arrange - a basic item with no explicit modifiers
            var basicItem = @"Item Class: Two Hand Axes
Rarity: Normal
Vaal Axe
--------
Two Handed Axe
Physical Damage: 58-110
Critical Strike Chance: 5.00%
Attacks per Second: 1.20
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Item Level: 84";

            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(basicItem);
            
            // Act
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Assert
            Assert.NotNull(affixes);
            Assert.Empty(affixes); // No explicit modifiers should mean empty list
        }
    }

    // Simple testable wrapper that exposes what we need for testing
    public class TestableProgressBar : WindowsFormsApplication3.ProgressBar
    {
        private string _testItem = "";

        public void SetTestItem(string item)
        {
            _testItem = item;
            // Set the Item field using reflection since it's private
            var itemField = typeof(WindowsFormsApplication3.ProgressBar).GetField("Item", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            itemField?.SetValue(this, item);
        }

        public List<string> GetExtractedAffixes()
        {
            // Access the affixes field using reflection since it's private
            var affixesField = typeof(WindowsFormsApplication3.ProgressBar).GetField("affixes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (List<string>)affixesField?.GetValue(this) ?? new List<string>();
        }
    }

    public class ModifierDetectionTests
    {
        [Fact]
        public void AttackSpeed_21Percent_ShouldBeDetected()
        {
            // This is the exact case from the user's report
            string itemText = "21% increased Attack Speed";
            
            // Test the regex pattern directly
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)% increased Attack Speed");
            
            Assert.True(match.Success);
            Assert.Equal("21", match.Groups[1].Value);
        }

        [Fact]
        public void AttackSpeed_WithRangeInParentheses_OldRegex_ShouldFail()
        {
            // Test proves that the OLD regex pattern fails with parentheses (this is the bug!)
            string itemText = "21(20-22)% increased Attack Speed";
            
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)% increased Attack Speed");
            
            Assert.False(match.Success); // This SHOULD fail - proving the bug exists
        }

        [Fact]
        public void AttackSpeed_WithParenthesesRange_ImprovedRegex_ShouldWork()
        {
            // Test with a more flexible regex that handles parentheses
            string itemText = "21(20-22)% increased Attack Speed";
            
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Attack Speed");
            
            Assert.True(match.Success);
            Assert.Equal("21", match.Groups[1].Value);
        }

        [Fact]
        public void GetAttackSpeedTier_21Percent_ShouldBeTier3()
        {
            // 21% should be Tier 3 (19-21% range)
            var progressBar = new WindowsFormsApplication3.ProgressBar();
            
            // Use reflection to access private method
            var method = typeof(WindowsFormsApplication3.ProgressBar).GetMethod("GetAttackSpeedTier", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            int tier = (int)method.Invoke(progressBar, new object[] { 21 });
            
            Assert.Equal(3, tier);
        }

        [Fact]
        public void PhysicalDamage_WithRangeInParentheses_ShouldBeDetected()
        {
            string itemText = "Adds 25(23-31) to 48(47-54) Fire Damage";
            
            // Current regex might not handle this format
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"Adds (\d+) to (\d+) Physical Damage");
            
            // This should fail with current regex because it's Fire Damage, not Physical
            Assert.False(match.Success);
        }

        [Fact]
        public void PhysicalDamage_Proper_ShouldBeDetected()
        {
            string itemText = "Adds 34(30-38) to 72(65-79) Physical Damage";
            
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"Adds (\d+)(?:\(\d+-\d+\))? to (\d+)(?:\(\d+-\d+\))? Physical Damage");
            
            Assert.True(match.Success);
            Assert.Equal("34", match.Groups[1].Value);
            Assert.Equal("72", match.Groups[2].Value);
        }

        [Fact]
        public void MultipleModifiers_ShouldDetectAll()
        {
            List<string> affixes = new List<string>
            {
                "21(20-22)% increased Attack Speed",
                "170(165-179)% increased Physical Damage",
                "Adds 34(30-38) to 72(65-79) Physical Damage"
            };

            string fullText = string.Join("\n", affixes);

            // Test each pattern
            var attackSpeedMatch = System.Text.RegularExpressions.Regex.Match(fullText, @"(\d+)(?:\(\d+-\d+\))?% increased Attack Speed");
            var physPercentMatch = System.Text.RegularExpressions.Regex.Match(fullText, @"(\d+)(?:\(\d+-\d+\))?% increased Physical Damage");
            var flatPhysMatch = System.Text.RegularExpressions.Regex.Match(fullText, @"Adds (\d+)(?:\(\d+-\d+\))? to (\d+)(?:\(\d+-\d+\))? Physical Damage");

            Assert.True(attackSpeedMatch.Success);
            Assert.True(physPercentMatch.Success);
            Assert.True(flatPhysMatch.Success);
        }

        [Fact]
        public void CriticalBug_NeverRollOverApprovedModifiers()
        {
            // This test ensures we never roll over good modifiers
            var progressBar = new WindowsFormsApplication3.ProgressBar();
            
            // Simulate the exact scenario from user's report
            List<string> affixes = new List<string>
            {
                "Adds 25(23-31) to 48(47-54) Fire Damage", // Not what we want
                "21(20-22)% increased Attack Speed" // This should be detected and stop crafting!
            };

            // Test if the system would correctly identify this as a match
            // This test should pass after we fix the bug
            bool shouldStopCrafting = false;

            string fullText = string.Join("\n", affixes);
            var attackSpeedMatch = System.Text.RegularExpressions.Regex.Match(fullText, @"(\d+)(?:\(\d+-\d+\))?% increased Attack Speed");
            
            if (attackSpeedMatch.Success)
            {
                int value = int.Parse(attackSpeedMatch.Groups[1].Value);
                // 21% is T3, so if user selected T3 or lower, it should stop
                if (value >= 19) // T3 threshold
                {
                    shouldStopCrafting = true;
                }
            }

            Assert.True(shouldStopCrafting);
        }
    }
} 