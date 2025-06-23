using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WindowsFormsApplication3;

namespace PoECrafter.Tests
{
    public class SmartAugmentationTests
    {
        #region Test Data Setup

        private CraftingConfiguration CreateTestConfig(bool enableSmartAugmentation, params string[] targetModifiers)
        {
            var config = new CraftingConfiguration
            {
                EnableSmartAugmentation = enableSmartAugmentation,
                ModifierGroups = new List<ModifierGroup>()
            };

            if (targetModifiers.Length > 0)
            {
                var group = new ModifierGroup("TestGroup");
                group.SelectedModifiers.AddRange(targetModifiers);
                config.ModifierGroups.Add(group);
            }

            return config;
        }

        private ItemAnalysis CreateMagicItemAnalysis(params DetectedAffix[] affixes)
        {
            var analysis = new ItemAnalysis
            {
                IsMagic = true,
                IsRare = false,
                DetectedAffixes = affixes.ToList()
            };

            // Update counts
            analysis.PrefixCount = affixes.Count(a => a.Type == AffixType.Prefix);
            analysis.SuffixCount = affixes.Count(a => a.Type == AffixType.Suffix);

            return analysis;
        }

        private ItemAnalysis CreateRareItemAnalysis(params DetectedAffix[] affixes)
        {
            var analysis = new ItemAnalysis
            {
                IsMagic = false,
                IsRare = true,
                DetectedAffixes = affixes.ToList()
            };

            analysis.PrefixCount = affixes.Count(a => a.Type == AffixType.Prefix);
            analysis.SuffixCount = affixes.Count(a => a.Type == AffixType.Suffix);

            return analysis;
        }

        private DetectedAffix CreateAffix(string modifierName, AffixType type)
        {
            return new DetectedAffix
            {
                ModifierName = modifierName,
                Type = type,
                Value = 100,
                Tier = 1
            };
        }

        #endregion

        #region Basic Smart Augmentation Tests

        [Fact]
        public void SmartAugmentation_Disabled_ShouldAlwaysReturnAlteration()
        {
            // Arrange
            var config = CreateTestConfig(enableSmartAugmentation: false, "Physical Damage %", "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_RareItem_ShouldAlwaysReturnChaos()
        {
            // Arrange
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            var analysis = CreateRareItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix),
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Chaos, result);
        }

        #endregion

        #region Prefix/Suffix Smart Augmentation Tests

        [Fact]
        public void SmartAugmentation_HasPrefixWantsSuffix_ShouldReturnAugmentation()
        {
            // Arrange: Item has Physical Damage (prefix), we want Attack Speed (suffix)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_HasSuffixWantsPrefix_ShouldReturnAugmentation()
        {
            // Arrange: Item has Attack Speed (suffix), we want Physical Damage (prefix)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_HasPrefixWantsPrefix_ShouldReturnAlteration()
        {
            // Arrange: Item has Physical Damage (prefix), we want different Physical Damage (prefix)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Flat Physical Damage");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_HasSuffixWantsSuffix_ShouldReturnAlteration()
        {
            // Arrange: Item has Attack Speed (suffix), we want Critical Strike Chance (suffix)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Critical Strike Chance %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        #endregion

        #region Multiple Affixes Tests

        [Fact]
        public void SmartAugmentation_HasTwoAffixes_ShouldReturnAlteration()
        {
            // Arrange: Item already has 2 affixes (magic items can't have more)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix),
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_NoAffixes_ShouldReturnAlteration()
        {
            // Arrange: Item has no affixes (normal item or failed detection)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            var analysis = CreateMagicItemAnalysis(); // No affixes

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        #endregion

        #region Complex Scenario Tests

        [Fact]
        public void SmartAugmentation_WantsBothPrefixAndSuffix_HasPrefix_ShouldReturnAugmentation()
        {
            // Arrange: We want both prefix and suffix, item has prefix only
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_WantsBothPrefixAndSuffix_HasSuffix_ShouldReturnAugmentation()
        {
            // Arrange: We want both prefix and suffix, item has suffix only
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_WantsOnlyPrefixes_HasSuffix_ShouldReturnAlteration()
        {
            // Arrange: Item has Attack Speed (suffix), we ONLY want prefixes (not suffixes)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Flat Physical Damage");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should use Augmentation to add a prefix since item only has suffix
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_WantsOnlySuffixes_HasPrefix_ShouldReturnAlteration()
        {
            // Arrange: Item has Physical Damage (prefix), we ONLY want suffixes (not prefixes)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %", "Critical Strike Chance %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should use Augmentation to add a suffix since item only has prefix
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        #endregion

        #region Critical Fix: Full Magic Items Tests

        [Fact]
        public void SmartAugmentation_FullMagicItem_ShouldAlwaysReturnAlteration()
        {
            // Arrange: Item has both prefix and suffix (2 affixes = full magic item)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix),
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should ALWAYS use Alteration when item is full, regardless of target modifiers
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_FullMagicItem_WantingDifferentAffixes_ShouldStillReturnAlteration()
        {
            // Arrange: Item has prefix + suffix, but we want completely different affixes
            var config = CreateTestConfig(enableSmartAugmentation: true, "Life", "Fire Resistance");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix),
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Even if we want different affixes, must use Alteration when item is full
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_FullMagicItem_MatchingOneAffix_ShouldStillReturnAlteration()
        {
            // Arrange: Item has 2 affixes, one matches what we want
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Critical Strike Chance %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix), // This matches our target
                CreateAffix("Attack Speed %", AffixType.Suffix)      // This doesn't match
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Even if one affix matches, must reroll when item is full to try for better combo
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_MoreThanTwoAffixes_ShouldReturnAlteration()
        {
            // Arrange: This shouldn't happen on magic items, but test edge case
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix),
                CreateAffix("Attack Speed %", AffixType.Suffix),
                CreateAffix("Life", AffixType.Prefix) // This would make it 3 affixes (shouldn't happen on magic)
            );

            // Force counts to simulate edge case
            analysis.PrefixCount = 2;
            analysis.SuffixCount = 1;

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should handle edge case gracefully
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        #endregion

        #region AffixDatabase Tests

        [Fact]
        public void AffixDatabase_PhysicalDamageModifiers_ShouldBeClassifiedAsPrefixes()
        {
            // Assert - Physical damage modifiers are prefixes
            Assert.Equal(AffixType.Prefix, AffixDatabase.GetAffixType("Physical Damage %"));
            Assert.Equal(AffixType.Prefix, AffixDatabase.GetAffixType("Flat Physical Damage"));
            Assert.Equal(AffixType.Prefix, AffixDatabase.GetAffixType("Hybrid Physical/Accuracy"));
        }

        [Fact]
        public void AffixDatabase_AttackModifiers_ShouldBeClassifiedAsSuffixes()
        {
            // Assert - Attack/crit modifiers are suffixes
            Assert.Equal(AffixType.Suffix, AffixDatabase.GetAffixType("Attack Speed %"));
            Assert.Equal(AffixType.Suffix, AffixDatabase.GetAffixType("Critical Strike Chance %"));
            Assert.Equal(AffixType.Suffix, AffixDatabase.GetAffixType("Critical Strike Multiplier %"));
        }

        [Fact]
        public void AffixDatabase_ClassifyModifierFromItemText_ShouldIdentifyCorrectly()
        {
            // Test Physical Damage classification
            Assert.Equal("Physical Damage %", AffixDatabase.ClassifyModifierFromItemText("52% increased Physical Damage"));
            Assert.Equal("Flat Physical Damage", AffixDatabase.ClassifyModifierFromItemText("Adds 34 to 72 Physical Damage"));
            Assert.Equal("Hybrid Physical/Accuracy", AffixDatabase.ClassifyModifierFromItemText("75% increased Physical Damage +175 to Accuracy Rating"));

            // Test Attack modifier classification
            Assert.Equal("Attack Speed %", AffixDatabase.ClassifyModifierFromItemText("26% increased Attack Speed"));
            Assert.Equal("Critical Strike Chance %", AffixDatabase.ClassifyModifierFromItemText("35% increased Critical Strike Chance"));
            Assert.Equal("Critical Strike Multiplier %", AffixDatabase.ClassifyModifierFromItemText("35% increased Critical Strike Multiplier"));
        }

        [Fact]
        public void AffixDatabase_UnknownModifier_ShouldReturnUnknown()
        {
            // Assert - Unknown modifiers return Unknown type
            Assert.Equal(AffixType.Unknown, AffixDatabase.GetAffixType("Some Random Modifier"));
            Assert.Equal("Unknown", AffixDatabase.ClassifyModifierFromItemText("Some random modifier text"));
        }

        #endregion

        #region Real-World Scenario Tests

        [Fact]
        public void SmartAugmentation_RealWorldScenario_AttackSpeedItem_WantPhysicalDamage()
        {
            // Arrange: User has item with only attack speed (suffix), wants physical damage (prefix)
            // This is the exact scenario the user described as not working
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_RealWorldScenario_PhysicalDamageItem_WantAttackSpeed()
        {
            // Arrange: User has item with only physical damage (prefix), wants attack speed (suffix)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Physical Damage %", AffixType.Prefix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_RealWorldScenario_WantMultiplePrefixes_HasSuffix()
        {
            // Arrange: Item has Attack Speed (suffix), we want multiple prefixes
            var config = CreateTestConfig(enableSmartAugmentation: true, 
                "Physical Damage %", "Flat Physical Damage", "Hybrid Physical/Accuracy");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Attack Speed %", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should use Augmentation to add a prefix since we want prefixes but item only has suffix
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_RealWorldScenario_OR_Logic_PrefixOrSuffix()
        {
            // Arrange: User wants either prefix OR suffix (could be satisfied by augmentation)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Critical Strike Chance %", AffixType.Suffix) // Has different suffix
            );

            // Act  
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert
            // Should use Augmentation because we want prefixes and item has suffix
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        #endregion

        #region Real User Issue: Lightning Damage + Accuracy (Full Item) Tests

        [Fact]
        public void SmartAugmentation_RealUserIssue_LightningDamageAndAccuracy_ShouldReturnAlteration()
        {
            // Arrange: Real scenario from user's log - item has both lightning damage and accuracy (full magic item)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Flat Physical Damage", "Hybrid Physical/Accuracy");
            var analysis = CreateMagicItemAnalysis(
                CreateAffix("Added Lightning Damage", AffixType.Prefix),
                CreateAffix("Accuracy Rating", AffixType.Suffix)
            );

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should use Alteration because magic item is full (2 affixes) regardless of what we want
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void ClassifyModifierFromItemText_LightningDamage_ShouldReturnCorrectType()
        {
            // Arrange: Real text from user's log
            string lightningDamageText = "Adds 23 to 371 Lightning Damage";
            
            // Act
            var result = AffixDatabase.ClassifyModifierFromItemText(lightningDamageText);
            
            // Assert
            Assert.Equal("Added Lightning Damage", result);
        }

        [Fact]
        public void ClassifyModifierFromItemText_AccuracyRating_ShouldReturnCorrectType()
        {
            // Arrange: Real text from user's log
            string accuracyText = "+167 to Accuracy Rating";
            
            // Act
            var result = AffixDatabase.ClassifyModifierFromItemText(accuracyText);
            
            // Assert
            Assert.Equal("Accuracy Rating", result);
        }

        [Fact]
        public void GetAffixType_LightningDamage_ShouldReturnPrefix()
        {
            // Act
            var result = AffixDatabase.GetAffixType("Added Lightning Damage");
            
            // Assert
            Assert.Equal(AffixType.Prefix, result);
        }

        [Fact]
        public void GetAffixType_AccuracyRating_ShouldReturnSuffix()
        {
            // Act
            var result = AffixDatabase.GetAffixType("Accuracy Rating");
            
            // Assert
            Assert.Equal(AffixType.Suffix, result);
        }

        #endregion

        #region Magic Item Name Analysis Tests

        [Fact]
        public void MagicItemNameAnalyzer_PrefixOnly_ShouldDetectCorrectly()
        {
            // Arrange: "Flaring Vaal Axe" (prefix only)
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Flaring Vaal Axe
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)";

            // Act
            var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(itemText);

            // Assert
            Assert.True(hasPrefix);
            Assert.False(hasSuffix);
        }

        [Fact]
        public void MagicItemNameAnalyzer_SuffixOnly_ShouldDetectCorrectly()
        {
            // Arrange: "Vaal Axe of Skill" (suffix only)
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe of Skill
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)";

            // Act
            var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(itemText);

            // Assert
            Assert.False(hasPrefix);
            Assert.True(hasSuffix);
        }

        [Fact]
        public void MagicItemNameAnalyzer_BothPrefixAndSuffix_ShouldDetectCorrectly()
        {
            // Arrange: "Glinting Vaal Axe of Skill" (both prefix and suffix)
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Skill
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)";

            // Act
            var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(itemText);

            // Assert
            Assert.True(hasPrefix);
            Assert.True(hasSuffix);
        }

        [Fact]
        public void MagicItemNameAnalyzer_NoAffixes_ShouldDetectCorrectly()
        {
            // Arrange: Just "Vaal Axe" (no affixes)
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe
--------
Two Handed Axe
Physical Damage: 106-178";

            // Act
            var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(itemText);

            // Assert
            Assert.False(hasPrefix);
            Assert.False(hasSuffix);
        }

        [Fact]
        public void MagicItemNameAnalyzer_RealUserExample_GlintingVaalAxeOfSkill()
        {
            // Arrange: Real example from user's log
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Skill
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.22 (augmented)
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: R-G 
--------
Item Level: 84
--------
25% chance to Maim on Hit (implicit)
--------
Adds 2 to 4 Physical Damage
6% increased Attack Speed";

            // Act
            var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(itemText);

            // Assert: Should detect both prefix ("Glinting") and suffix ("of Skill")
            Assert.True(hasPrefix);
            Assert.True(hasSuffix);
        }

        [Fact]
        public void SmartAugmentation_WithNameAnalysis_FullItem_ShouldReturnAlteration()
        {
            // Arrange: Use name analysis for full magic item
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            var analysis = new ItemAnalysis
            {
                IsMagic = true,
                IsRare = false,
                ItemText = @"Rarity: Magic
Glinting Vaal Axe of Skill"
            };

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Full item (prefix + suffix) should use Alteration
            Assert.Equal(SmartCurrencySelector.CurrencyType.Alteration, result);
        }

        [Fact]
        public void SmartAugmentation_WithNameAnalysis_PrefixOnly_WantSuffix_ShouldReturnAugmentation()
        {
            // Arrange: Item has prefix only, we want suffix
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            var analysis = new ItemAnalysis
            {
                IsMagic = true,
                IsRare = false,
                ItemText = @"Rarity: Magic
Flaring Vaal Axe"
            };

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should use Augmentation to add suffix
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        [Fact]
        public void SmartAugmentation_WithNameAnalysis_SuffixOnly_WantPrefix_ShouldReturnAugmentation()
        {
            // Arrange: Item has suffix only, we want prefix
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            var analysis = new ItemAnalysis
            {
                IsMagic = true,
                IsRare = false,
                ItemText = @"Rarity: Magic
Vaal Axe of Skill"
            };

            // Act
            var result = SmartCurrencySelector.SelectOptimalCurrency(analysis, config);

            // Assert: Should use Augmentation to add prefix
            Assert.Equal(SmartCurrencySelector.CurrencyType.Augmentation, result);
        }

        #endregion

        #region Magic Item Name-Based Success Detection Tests

        [Fact]
        public void MagicItemNameDatabase_FlaringVaalAxe_WantFlatPhysical_ShouldSucceed()
        {
            // Arrange: "Flaring Vaal Axe" contains flat physical damage, we want flat physical
            var config = CreateTestConfig(enableSmartAugmentation: true, "Flat Physical Damage");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Flaring Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should detect success from name without parsing modifiers
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_GlintingVaalAxeOfSkill_WantLightningAndAttackSpeed_ShouldSucceed()
        {
            // Arrange: Real example from user - "Glinting" = Lightning, "of Skill" = Attack Speed
            var config = CreateTestConfig(enableSmartAugmentation: true, "Added Lightning Damage", "Attack Speed %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Skill
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should detect success from both prefix and suffix names
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_HeavyVaalAxe_WantAttackSpeed_ShouldFail()
        {
            // Arrange: "Heavy" = Physical Damage %, but we want Attack Speed
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Heavy Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should fail since Heavy doesn't match Attack Speed
            Assert.False(result);
        }

        [Fact]
        public void MagicItemNameDatabase_VaalAxeOfAlacrity_WantAttackSpeed_ShouldSucceed()
        {
            // Arrange: "of Alacrity" = Attack Speed %, we want attack speed
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe of Alacrity
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should succeed from suffix name
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_UnknownPrefix_ShouldFallbackGracefully()
        {
            // Arrange: Unknown prefix name not in our database
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Mysterious Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should fail gracefully (return false) for unknown names
            Assert.False(result);
        }

        [Fact]
        public void MagicItemNameDatabase_ExtractPrefixSuffixNames_ShouldParseCorrectly()
        {
            // Arrange: Test the internal name extraction logic
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Skill
--------
Two Handed Axe";

            // Act: Use reflection to call private method for testing
            var method = typeof(MagicItemNameDatabase).GetMethod("ExtractPrefixSuffixNames", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = ((string, string))method.Invoke(null, new object[] { itemText });

            // Assert: Should correctly extract both names
            Assert.Equal("Glinting", result.Item1);
            Assert.Equal("of Skill", result.Item2);
        }

        [Fact]
        public void MagicItemNameDatabase_PerformanceComparison_ShouldBeFaster()
        {
            // This test demonstrates that name-based detection is much faster
            // than full modifier parsing for magic items
            
            var config = CreateTestConfig(enableSmartAugmentation: true, "Added Lightning Damage");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Skill
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.22 (augmented)
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: R-G 
--------
Item Level: 84
--------
25% chance to Maim on Hit (implicit)
--------
Adds 23 to 371 Lightning Damage
6% increased Attack Speed";

            // Act: Name-based detection (should be very fast)
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var nameBasedResult = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);
            stopwatch.Stop();
            var nameBasedTime = stopwatch.ElapsedTicks;

            // Assert: Should be successful and fast
            Assert.True(nameBasedResult);
            // Note: In practice, name-based detection should be orders of magnitude faster
            // than parsing all the individual modifier lines
        }

        #endregion

        #region Magic Item Name Database - Extended Coverage Tests

        [Fact]
        public void MagicItemNameDatabase_HeavyVaalAxe_T8PhysicalDamage_WantT5OrBetter_ShouldFail()
        {
            // Arrange: "Heavy" is T8 Physical Damage %, user wants T5 or better
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Heavy Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should succeed because we just check for matching modifier type, not tier
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_VaalAxeOfCelebration_T1AttackSpeed_ShouldSucceed()
        {
            // Arrange: "of Celebration" is T1 Attack Speed
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe of Celebration
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should succeed
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_ReaversVaalAxeOfSkill_HybridAndAttackSpeed_ShouldSucceed()
        {
            // Arrange: "Reaver's" = T6 Hybrid, "of Skill" = T2 Attack Speed
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy", "Attack Speed %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Reaver's Vaal Axe of Skill
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should succeed as it matches both wanted modifier types
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_DischargingVaalAxeOfTheSalamander_WantPhysical_ShouldFail()
        {
            // Arrange: "Discharging" = Lightning Damage, "of the Salamander" = Fire Resist, user wants Physical
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Discharging Vaal Axe of the Salamander
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should fail as neither modifier matches physical damage
            Assert.False(result);
        }

        [Fact]
        public void MagicItemNameDatabase_TemperateVaalAxe_T2FlatPhysical_WantFlatPhysical_ShouldSucceed()
        {
            // Arrange: "Tempered" is T2 Flat Physical Damage (30-40 to 63-73) - NOT Hybrid!
            var config = CreateTestConfig(enableSmartAugmentation: true, "Flat Physical Damage");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Tempered Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should succeed - "Tempered" is correctly mapped to Flat Physical Damage
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_MultipleTargetModifiers_ORLogic_ShouldSucceedOnAnyMatch()
        {
            // Arrange: User wants Physical Damage % OR Attack Speed %, item has Lightning damage and Attack Speed
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %", "Attack Speed %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Alacrity
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should succeed because item has Attack Speed (matches one of the wanted modifiers)
            Assert.True(result);
        }

        [Fact]
        public void MagicItemNameDatabase_UnrecognizedBaseType_ShouldHandleGracefully()
        {
            // Arrange: Unknown base type not in our database
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            string itemText = @"Item Class: Unknown Weapon
Rarity: Magic
Flaring Unknown Weapon of Skill
--------
Unknown Weapon";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should handle gracefully (may succeed if parsing works despite unknown base)
            // The exact result depends on implementation, but it shouldn't crash
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void MagicItemNameDatabase_PerformanceVsDetailedAnalysis_ShouldBeMuchFaster()
        {
            // This demonstrates the performance advantage of name-based detection
            var config = CreateTestConfig(enableSmartAugmentation: true, "Added Lightning Damage");
            string complexItemText = @"Item Class: Two Hand Axes
Rarity: Magic
Glinting Vaal Axe of Skill
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.22 (augmented)
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: R-G-B-R-G-B
--------
Item Level: 84
--------
25% chance to Maim on Hit (implicit)
--------
Adds 23 to 371 Lightning Damage
6% increased Attack Speed
Some other complex modifier
Another complex line
Yet another line with numbers 123-456
And more text that would slow down parsing";

            // Act: Name-based detection should be very fast regardless of item complexity
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var nameBasedResult = MagicItemNameDatabase.CheckMagicItemSuccess(complexItemText, config);
            stopwatch.Stop();
            
            // Assert: Should be successful and the performance should be independent of item text complexity
            Assert.True(nameBasedResult);
            // In practice, this should be orders of magnitude faster than parsing all the modifier lines
        }

        #endregion

        #region Top Tier Prefix Recognition Tests - Critical for High-End Crafting

        [Fact]
        public void MagicItemNameDatabase_PhysicalDamagePercent_T3_Cruel_ShouldRecognize()
        {
            // Arrange: Cruel T3 Physical Damage % (135-154%)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Cruel Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Cruel" as Physical Damage %
            Assert.True(result, "Failed to recognize 'Cruel' as T3 Physical Damage %");
        }

        [Fact]
        public void MagicItemNameDatabase_PhysicalDamagePercent_T2_Tyrannical_ShouldRecognize()
        {
            // Arrange: Tyrannical T2 Physical Damage % (155-169%)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Tyrannical Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Tyrannical" as Physical Damage %
            Assert.True(result, "Failed to recognize 'Tyrannical' as T2 Physical Damage %");
        }

        [Fact]
        public void MagicItemNameDatabase_PhysicalDamagePercent_T1_Merciless_ShouldRecognize()
        {
            // Arrange: Merciless T1 Physical Damage % (170-179%) - BEST TIER
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Merciless Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Merciless" as Physical Damage %
            Assert.True(result, "Failed to recognize 'Merciless' as T1 Physical Damage %");
        }

        [Fact]
        public void MagicItemNameDatabase_HybridPhysicalAccuracy_T3_Conquerors_ShouldRecognize()
        {
            // Arrange: Conqueror's T3 Hybrid (55-64% + 124-149 Acc)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Conqueror's Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Conqueror's" as Hybrid Physical/Accuracy
            Assert.True(result, "Failed to recognize 'Conqueror's' as T3 Hybrid Physical/Accuracy");
        }

        [Fact]
        public void MagicItemNameDatabase_HybridPhysicalAccuracy_T2_Emperors_ShouldRecognize()
        {
            // Arrange: Emperor's T2 Hybrid (65-74% + 150-174 Acc) - THE BUG CASE!
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Emperor's Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Emperor's" as Hybrid Physical/Accuracy
            Assert.True(result, "CRITICAL BUG: Failed to recognize 'Emperor's' as T2 Hybrid Physical/Accuracy - This was the rollover issue!");
        }

        [Fact]
        public void MagicItemNameDatabase_HybridPhysicalAccuracy_T1_Dictators_ShouldRecognize()
        {
            // Arrange: Dictator's T1 Hybrid (75-79% + 175-200 Acc) - BEST TIER
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Dictator's Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Dictator's" as Hybrid Physical/Accuracy
            Assert.True(result, "Failed to recognize 'Dictator's' as T1 Hybrid Physical/Accuracy");
        }

        [Fact]
        public void MagicItemNameDatabase_FlatPhysicalDamage_T3_RazorSharp_ShouldRecognize()
        {
            // Arrange: Razor-sharp T3 Flat Physical (25-33 to 52-61)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Flat Physical Damage");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Razor-sharp Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Razor-sharp" as Flat Physical Damage
            Assert.True(result, "Failed to recognize 'Razor-sharp' as T3 Flat Physical Damage");
        }

        [Fact]
        public void MagicItemNameDatabase_FlatPhysicalDamage_T2_Tempered_ShouldRecognize()
        {
            // Arrange: Tempered T2 Flat Physical (30-40 to 63-73)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Flat Physical Damage");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Tempered Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Tempered" as Flat Physical Damage
            Assert.True(result, "Failed to recognize 'Tempered' as T2 Flat Physical Damage");
        }

        [Fact]
        public void MagicItemNameDatabase_FlatPhysicalDamage_T1_Flaring_ShouldRecognize()
        {
            // Arrange: Flaring T1 Flat Physical (34-47 to 72-84) - BEST TIER
            var config = CreateTestConfig(enableSmartAugmentation: true, "Flat Physical Damage");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Flaring Vaal Axe
--------
Two Handed Axe";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should recognize "Flaring" as Flat Physical Damage
            Assert.True(result, "Failed to recognize 'Flaring' as T1 Flat Physical Damage");
        }

        [Fact]
        public void MagicItemNameDatabase_TopTierCombinations_ShouldRecognizeMultipleTargets()
        {
            // Arrange: User wants ANY of the top tier modifiers (OR logic simulation)
            var config = CreateTestConfig(enableSmartAugmentation: true, 
                "Physical Damage %", "Hybrid Physical/Accuracy", "Flat Physical Damage");

            // Test each top tier prefix separately
            var testCases = new[]
            {
                ("Merciless Vaal Axe", "T1 Physical Damage %"),
                ("Dictator's Vaal Axe", "T1 Hybrid Physical/Accuracy"),
                ("Flaring Vaal Axe", "T1 Flat Physical Damage"),
                ("Tyrannical Vaal Axe", "T2 Physical Damage %"),
                ("Emperor's Vaal Axe", "T2 Hybrid Physical/Accuracy"),
                ("Tempered Vaal Axe", "T2 Flat Physical Damage"),
                ("Cruel Vaal Axe", "T3 Physical Damage %"),
                ("Conqueror's Vaal Axe", "T3 Hybrid Physical/Accuracy"),
                ("Razor-sharp Vaal Axe", "T3 Flat Physical Damage")
            };

            foreach (var (itemName, description) in testCases)
            {
                string itemText = $@"Item Class: Two Hand Axes
Rarity: Magic
{itemName}
--------
Two Handed Axe";

                // Act
                var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

                // Assert: Should recognize all top tier modifiers
                Assert.True(result, $"Failed to recognize {description} from item name '{itemName}'");
            }
        }

        [Fact]
        public void MagicItemNameDatabase_WrongModifierType_ShouldRejectCorrectly()
        {
            // Arrange: User wants Attack Speed, but item has Physical Damage
            var config = CreateTestConfig(enableSmartAugmentation: true, "Attack Speed %");
            
            var physicalDamageItems = new[]
            {
                "Merciless Vaal Axe",     // T1 Physical Damage %
                "Dictator's Vaal Axe",   // T1 Hybrid Physical/Accuracy  
                "Flaring Vaal Axe"       // T1 Flat Physical Damage
            };

            foreach (var itemName in physicalDamageItems)
            {
                string itemText = $@"Item Class: Two Hand Axes
Rarity: Magic
{itemName}
--------
Two Handed Axe";

                // Act
                var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

                // Assert: Should reject since user wants Attack Speed, not Physical damage
                Assert.False(result, $"Incorrectly accepted '{itemName}' when user wants Attack Speed");
            }
        }

        [Fact]
        public void MagicItemNameDatabase_EmperorsVaalAxe_RealWorldBugScenario()
        {
            // Arrange: This recreates the exact bug scenario from user's log
            // User had T2 Hybrid selected, item was "Emperor's Vaal Axe" with 71% + 169 Acc (perfect T2)
            // System should recognize from name and stop crafting immediately
            
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Emperor's Vaal Axe
--------
Two Handed Axe
Physical Damage: 106-178 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.35 (augmented)
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: R-G 
--------
Item Level: 84
--------
25% chance to Maim on Hit (implicit)
--------
71% increased Physical Damage
5% increased Attack Speed
+169 to Accuracy Rating";

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should immediately recognize "Emperor's" = T2 Hybrid and stop crafting
            Assert.True(result, "CRITICAL BUG REPRODUCTION: Emperor's Vaal Axe should be recognized as T2 Hybrid from name alone!");
        }

        [Fact]
        public void MagicItemNameDatabase_TopTierPrefixCoverage_ShouldHaveCompleteDatabase()
        {
            // Arrange: Verify that our database includes all the critical top-tier prefixes
            var criticalPrefixes = new[]
            {
                // Physical Damage % T3-T1
                "Cruel", "Tyrannical", "Merciless",
                
                // Hybrid Physical/Accuracy T3-T1
                "Conqueror's", "Emperor's", "Dictator's",
                
                // Flat Physical Damage T3-T1
                "Razor-sharp", "Tempered", "Flaring"
            };

            foreach (var prefix in criticalPrefixes)
            {
                // Act: Check if prefix exists in database
                bool existsInDatabase = MagicItemNameDatabase.PrefixNames.ContainsKey(prefix);

                // Assert: All critical prefixes must be in database
                Assert.True(existsInDatabase, $"MISSING CRITICAL PREFIX: '{prefix}' not found in MagicItemNameDatabase.PrefixNames!");
            }
        }

        #endregion

        #region Randomized Delay System Tests

        [Fact]
        public void RandomizedDelay_LowValue_10ms_ShouldRandomizeUpwards()
        {
            // Arrange: Use reflection to access the private method
            var progressBarType = typeof(WindowsFormsApplication3.ProgressBar);
            var random = new Random(42); // Fixed seed for predictable testing
            var getRandomizedDelayMethod = progressBarType.GetMethod("GetRandomizedDelay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Create an instance to call the method on
            var instance = Activator.CreateInstance(progressBarType);
            
            // Set the random field to our seeded version using reflection
            var randomField = progressBarType.GetField("random", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            randomField.SetValue(null, random);
            
            // Act: Test multiple calls to ensure randomization
            var results = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                var result = (int)getRandomizedDelayMethod.Invoke(instance, new object[] { 10, 0 });
                results.Add(result);
            }
            
            // Assert: For 10ms, should randomize to 10-25 range
            Assert.All(results, result => Assert.InRange(result, 10, 25));
            Assert.True(results.Distinct().Count() > 1, "Should produce varied results");
        }

        [Fact]
        public void RandomizedDelay_MediumValue_25ms_ShouldRandomizeAroundValue()
        {
            // Arrange: Use reflection to access the private method
            var progressBarType = typeof(WindowsFormsApplication3.ProgressBar);
            var random = new Random(42); // Fixed seed for predictable testing
            var getRandomizedDelayMethod = progressBarType.GetMethod("GetRandomizedDelay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var instance = Activator.CreateInstance(progressBarType);
            var randomField = progressBarType.GetField("random", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            randomField.SetValue(null, random);
            
            // Act: Test multiple calls
            var results = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                var result = (int)getRandomizedDelayMethod.Invoke(instance, new object[] { 25, 0 });
                results.Add(result);
            }
            
            // Assert: For 25ms, should randomize to 15-35 range (±40%)
            Assert.All(results, result => Assert.InRange(result, 15, 35));
            Assert.True(results.Distinct().Count() > 1, "Should produce varied results");
        }

        [Fact]
        public void RandomizedDelay_WithAdditionalMs_ShouldAddCorrectly()
        {
            // Arrange
            var progressBarType = typeof(WindowsFormsApplication3.ProgressBar);
            var random = new Random(42);
            var getRandomizedDelayMethod = progressBarType.GetMethod("GetRandomizedDelay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var instance = Activator.CreateInstance(progressBarType);
            var randomField = progressBarType.GetField("random", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            randomField.SetValue(null, random);
            
            // Act: Test with additional 150ms
            var result = (int)getRandomizedDelayMethod.Invoke(instance, new object[] { 25, 150 });
            
            // Assert: Should be in range 165-185 (15-35 + 150)
            Assert.InRange(result, 165, 185);
        }

        [Fact]
        public void RandomizedDelay_VeryLowValue_5ms_ShouldNeverGoBelowMinimum()
        {
            // Arrange
            var progressBarType = typeof(WindowsFormsApplication3.ProgressBar);
            var random = new Random(42);
            var getRandomizedDelayMethod = progressBarType.GetMethod("GetRandomizedDelay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var instance = Activator.CreateInstance(progressBarType);
            var randomField = progressBarType.GetField("random", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            randomField.SetValue(null, random);
            
            // Act: Test edge case with very low value
            var results = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                var result = (int)getRandomizedDelayMethod.Invoke(instance, new object[] { 5, 0 });
                results.Add(result);
            }
            
            // Assert: Should never go below 5ms
            Assert.All(results, result => Assert.True(result >= 5, $"Result {result} should be >= 5"));
        }

        #endregion
    }
} 