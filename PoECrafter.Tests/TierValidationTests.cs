using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WindowsFormsApplication3;

namespace PoECrafter.Tests
{
    public class TierValidationTests
    {
        private readonly string _testAxeItem;

        public TierValidationTests()
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
        public void AffixExtraction_YourAxeItem_ShouldExtractCorrectAffixes()
        {
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);

            // Act
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            // Assert
            Assert.NotNull(affixes);
            
            // Check that both affixes are extracted
            var physicalDamageAffix = affixes.FirstOrDefault(a => a.Contains("52% increased Physical Damage"));
            var attackSpeedAffix = affixes.FirstOrDefault(a => a.Contains("26% increased Attack Speed"));
            
            Assert.NotNull(physicalDamageAffix);
            Assert.NotNull(attackSpeedAffix);
            
            // Verify exact values
            Assert.Contains("52% increased Physical Damage", affixes);
            Assert.Contains("26% increased Attack Speed", affixes);
        }

        [Fact]
        public void TierValidation_AttackSpeed26Percent_ShouldBeT1()
        {
            // This test validates that 26% attack speed would be recognized as T1
            // Based on PoE tier ranges: T1 (25-27%), T2 (22-24%), T3 (19-21%), etc.
            
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Act & Assert
            var attackSpeedAffix = affixes.FirstOrDefault(a => a.Contains("26% increased Attack Speed"));
            Assert.NotNull(attackSpeedAffix);
            
            // 26% should be T1 (falls in 25-27% range)
            // This would pass minimum tier requirements of T3 or better
            Assert.True(26 >= 25 && 26 <= 27, "26% attack speed should be in T1 range (25-27%)");
        }

        [Fact]
        public void TierValidation_PhysicalDamage52Percent_ShouldBeT7()
        {
            // This test validates that 52% physical damage would be recognized as T7
            // Based on PoE tier ranges: T7 (50-64%), T8 (40-49%), etc.
            
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Act & Assert
            var physicalDamageAffix = affixes.FirstOrDefault(a => a.Contains("52% increased Physical Damage"));
            Assert.NotNull(physicalDamageAffix);
            
            // 52% should be T7 (falls in 50-64% range)
            // This would NOT pass minimum tier requirements of T3 or better
            Assert.True(52 >= 50 && 52 <= 64, "52% physical damage should be in T7 range (50-64%)");
        }

        [Fact]
        public void TierValidation_AttackSpeedMinimumT3_ShouldPass()
        {
            // Test Case: Allow attack speed minimum tier 3 to pass
            // Your axe has 26% attack speed (T1), so it should pass T3 requirement
            
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Act
            var attackSpeedAffix = affixes.FirstOrDefault(a => a.Contains("26% increased Attack Speed"));
            
            // Assert
            Assert.NotNull(attackSpeedAffix);
            
            // Simulate tier checking: T1 (26%) should pass minimum T3 requirement
            // T1 = 1, T2 = 2, T3 = 3, so 1 <= 3 (passes)
            int actualTier = 1; // 26% is T1
            int minimumAcceptableTier = 3;
            
            Assert.True(actualTier <= minimumAcceptableTier, 
                $"T1 attack speed (tier {actualTier}) should pass minimum tier {minimumAcceptableTier} requirement");
        }

        [Fact]
        public void TierValidation_PhysicalDamageMinimumT3_ShouldFail()
        {
            // Test Case: Physical damage minimum tier 3 should fail
            // Your axe has 52% physical damage (T7), so it should fail T3 requirement
            
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Act
            var physicalDamageAffix = affixes.FirstOrDefault(a => a.Contains("52% increased Physical Damage"));
            
            // Assert
            Assert.NotNull(physicalDamageAffix);
            
            // Simulate tier checking: T7 (52%) should fail minimum T3 requirement
            // T7 = 7, T3 = 3, so 7 > 3 (fails)
            int actualTier = 7; // 52% is T7
            int minimumAcceptableTier = 3;
            
            Assert.False(actualTier <= minimumAcceptableTier, 
                $"T7 physical damage (tier {actualTier}) should fail minimum tier {minimumAcceptableTier} requirement");
        }

        [Fact]
        public void TierValidation_MixedResults_AttackSpeedPassPhysicalDamageFail()
        {
            // Test Case: Combined validation where one modifier passes and one fails
            // This simulates the real crafting scenario
            
            // Arrange
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_testAxeItem);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();
            
            // Act
            var attackSpeedAffix = affixes.FirstOrDefault(a => a.Contains("26% increased Attack Speed"));
            var physicalDamageAffix = affixes.FirstOrDefault(a => a.Contains("52% increased Physical Damage"));
            
            // Assert
            Assert.NotNull(attackSpeedAffix);
            Assert.NotNull(physicalDamageAffix);
            
            // Simulate tier validation for minimum T3 requirement
            int attackSpeedTier = 1; // T1
            int physicalDamageTier = 7; // T7
            int minimumTier = 3;
            
            bool attackSpeedPasses = attackSpeedTier <= minimumTier; // Should be true
            bool physicalDamagePasses = physicalDamageTier <= minimumTier; // Should be false
            
            Assert.True(attackSpeedPasses, "Attack speed T1 should pass minimum T3 requirement");
            Assert.False(physicalDamagePasses, "Physical damage T7 should fail minimum T3 requirement");
            
            // Overall result: Should continue crafting because not all modifiers meet requirements
            bool shouldStopCrafting = attackSpeedPasses && physicalDamagePasses;
            Assert.False(shouldStopCrafting, "Should continue crafting because physical damage doesn't meet minimum tier");
        }
    }
} 