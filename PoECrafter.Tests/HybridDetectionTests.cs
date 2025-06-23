using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WindowsFormsApplication3;

namespace PoECrafter.Tests
{
    public class HybridDetectionTests
    {
        #region Test Data - Real Items from User

        /// <summary>
        /// User's actual item: "Wicked Vaal Axe of Precision"
        /// This has separate prefix + suffix modifiers, NOT a hybrid
        /// - "Wicked" prefix → Physical Damage % T6 (71%)  
        /// - "of Precision" suffix → Accuracy Rating (+164)
        /// </summary>
        private const string WickedVaalAxeItem = @"Item Class: Two Hand Axes
Rarity: Magic
Wicked Vaal Axe of Precision
--------
Two Handed Axe
Physical Damage: 178-298 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: G-R-R 
--------
Item Level: 83
--------
25% chance to Maim on Hit (implicit)
--------
71% increased Physical Damage
+164 to Accuracy Rating";

        /// <summary>
        /// Example of a TRUE hybrid item for comparison
        /// "Emperor's" is a real hybrid prefix (T2: 65-74% + 150-174 Acc)
        /// </summary>
        private const string EmperorsVaalAxeItem = @"Item Class: Two Hand Axes
Rarity: Magic
Emperor's Vaal Axe
--------
Two Handed Axe
Physical Damage: 178-298 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: G-R-R 
--------
Item Level: 83
--------
25% chance to Maim on Hit (implicit)
--------
71% increased Physical Damage
+164 to Accuracy Rating";

        #endregion

        #region Helper Methods

        private CraftingConfiguration CreateTestConfig(bool enableSmartAugmentation, params string[] selectedModifiers)
        {
            var config = new CraftingConfiguration
            {
                UseORLogicBetweenGroups = true, // Use OR logic for easier testing
                EnableSmartAugmentation = enableSmartAugmentation,
                MaxCurrencyUsage = 100,
                ModifierGroups = new List<ModifierGroup>()
            };

            // Create modifier groups from selected modifiers
            foreach (var modifier in selectedModifiers)
            {
                string groupName = GetModifierGroupName(modifier);
                var existingGroup = config.ModifierGroups.FirstOrDefault(g => g.GroupName == groupName);
                if (existingGroup == null)
                {
                    var newGroup = new ModifierGroup(groupName)
                    {
                        IsEnabled = true,
                        SelectedModifiers = new List<string>()
                    };
                    newGroup.SelectedModifiers.Add(modifier);
                    config.ModifierGroups.Add(newGroup);
                }
                else
                {
                    existingGroup.SelectedModifiers.Add(modifier);
                }
            }

            return config;
        }

        private string GetModifierGroupName(string modifier)
        {
            if (modifier.Contains("Physical Damage %")) return "Physical Damage %";
            if (modifier.Contains("Hybrid Phys/Acc")) return "Hybrid Phys/Acc";
            if (modifier.Contains("Flat Physical Damage")) return "Flat Physical Damage";
            if (modifier.Contains("Attack Speed")) return "Attack Speed";
            return "Other";
        }

        #endregion

        #region Critical Bug Fix Tests - Based on User's Real Issue

        [Fact]
        public void WickedVaalAxe_ShouldNOT_BeDetectedAsHybrid_WhenSelectingHybridPhysAcc()
        {
            // Arrange: User's actual item with Hybrid Physical/Accuracy selected (this should fail the prefix name check)
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            string itemText = WickedVaalAxeItem;

            // Act: Try to detect this as hybrid (should FAIL)
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should NOT be detected as hybrid because "Wicked" is not a hybrid prefix
            Assert.False(result, "Wicked Vaal Axe should NOT be detected as hybrid - it has separate prefix + suffix modifiers!");
        }

        [Fact]
        public void WickedVaalAxe_ShouldBeDetectedAsPhysicalDamage_WhenSelectingPhysicalDamagePercent()
        {
            // Arrange: User's actual item with Physical Damage % selected
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = WickedVaalAxeItem;

            // Act: Detect as Physical Damage % (should SUCCESS)
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should be detected as Physical Damage % because "Wicked" is a valid Physical Damage % prefix
            Assert.True(result, "Wicked Vaal Axe should be detected as Physical Damage % T6!");
        }

        [Fact]
        public void EmperorsVaalAxe_ShouldBeDetectedAsHybrid_WhenSelectingHybridPhysAcc()
        {
            // Arrange: True hybrid item with Hybrid Physical/Accuracy selected
            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            string itemText = EmperorsVaalAxeItem;

            // Act: Detect as hybrid (should SUCCESS)
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should be detected as hybrid because "Emperor's" IS a hybrid prefix
            Assert.True(result, "Emperor's Vaal Axe should be detected as true hybrid modifier!");
        }

        [Fact]
        public void PrefixNameExtraction_ShouldCorrectlyIdentifyWickedAndEmperors()
        {
            // Arrange
            string wickedItem = WickedVaalAxeItem;
            string emperorsItem = EmperorsVaalAxeItem;

            // Act
            var (wickedPrefix, wickedSuffix) = MagicItemNameDatabase.ExtractPrefixSuffixNames(wickedItem);
            var (emperorsPrefix, emperorsSuffix) = MagicItemNameDatabase.ExtractPrefixSuffixNames(emperorsItem);

            // Assert
            Assert.Equal("Wicked", wickedPrefix);
            Assert.Equal("of Precision", wickedSuffix);
            Assert.Equal("Emperor's", emperorsPrefix);
            Assert.Equal("", emperorsSuffix); // Hybrid items typically only have prefix, so suffix should be empty string
        }

        #endregion

        #region Comprehensive Hybrid Detection Logic Tests

        [Theory]
        [InlineData("Wicked", false)] // Physical Damage % prefix
        [InlineData("Cruel", false)] // Physical Damage % prefix  
        [InlineData("Tyrannical", false)] // Physical Damage % prefix
        [InlineData("Merciless", false)] // Physical Damage % prefix
        [InlineData("Emperor's", true)] // TRUE hybrid prefix
        [InlineData("Dictator's", true)] // TRUE hybrid prefix
        [InlineData("Conqueror's", true)] // TRUE hybrid prefix
        [InlineData("Champion's", true)] // TRUE hybrid prefix
        public void HybridPrefixDetection_ShouldCorrectlyIdentifyHybridVsNonHybrid(string prefixName, bool expectedIsHybrid)
        {
            // Arrange: Create item text with the specified prefix
            string itemText = $@"Item Class: Two Hand Axes
Rarity: Magic
{prefixName} Vaal Axe
--------
Two Handed Axe
--------
71% increased Physical Damage
+164 to Accuracy Rating";

            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert
            if (expectedIsHybrid)
            {
                Assert.True(result, $"'{prefixName}' should be detected as a hybrid prefix!");
            }
            else
            {
                Assert.False(result, $"'{prefixName}' should NOT be detected as a hybrid prefix!");
            }
        }

        [Fact] 
        public void CraftingLogic_ShouldStopCrafting_WhenStartingItemAlreadyMeetsRequirements()
        {
            // Arrange: Item that already meets Physical Damage % requirement
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string perfectItemText = WickedVaalAxeItem; // 71% = T6

            // Act: Check if item already meets requirements (pre-crafting check)
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(perfectItemText, config);

            // Assert: Should return true, indicating no crafting needed
            Assert.True(result, "System should recognize that Wicked T6 (71%) already meets T6 requirement and stop crafting!");
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public void HybridDetection_ShouldHandleItemsWithNoPrefix()
        {
            // Arrange: Item with no identifiable prefix
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe
--------
71% increased Physical Damage
+164 to Accuracy Rating";

            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should not be detected as hybrid
            Assert.False(result, "Items with no prefix should not be detected as hybrid!");
        }

        [Fact]
        public void HybridDetection_ShouldHandleItemsWithOnlyPhysicalDamage()
        {
            // Arrange: Item with only Physical Damage % (no Accuracy)
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Wicked Vaal Axe
--------
71% increased Physical Damage";

            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should not be detected as hybrid (missing Accuracy)
            Assert.False(result, "Items with only Physical Damage % should not be detected as hybrid!");
        }

        [Fact]
        public void HybridDetection_ShouldHandleItemsWithOnlyAccuracy()
        {
            // Arrange: Item with only Accuracy (no Physical Damage %)
            string itemText = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe of Precision
--------
+164 to Accuracy Rating";

            var config = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");

            // Act
            var result = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);

            // Assert: Should not be detected as hybrid (missing Physical Damage %)
            Assert.False(result, "Items with only Accuracy should not be detected as hybrid!");
        }

        #endregion

        #region Integration Tests - Full Crafting Workflow

        [Fact]
        public void FullCraftingWorkflow_ShouldCorrectlyHandleWickedVaalAxe_WithPhysicalDamageSelection()
        {
            // Arrange: Real user scenario - select Physical Damage % with the user's actual item
            var config = CreateTestConfig(enableSmartAugmentation: true, "Physical Damage %");
            string itemText = WickedVaalAxeItem;

            // Act: Simulate the full crafting analysis
            var nameBasedResult = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, config);
            var (prefixName, suffixName) = MagicItemNameDatabase.ExtractPrefixSuffixNames(itemText);

            // Assert: Complete workflow validation
            Assert.True(nameBasedResult, "Name-based detection should succeed for Wicked → Physical Damage %");
            Assert.Equal("Wicked", prefixName);
            Assert.Equal("of Precision", suffixName);
            
            // Verify it's NOT treated as hybrid
            var hybridConfig = CreateTestConfig(enableSmartAugmentation: true, "Hybrid Physical/Accuracy");
            var hybridResult = MagicItemNameDatabase.CheckMagicItemSuccess(itemText, hybridConfig);
            Assert.False(hybridResult, "Same item should NOT be detected as hybrid when hybrid is selected");
        }

        #endregion

        #region Documentation Tests

        [Fact]
        public void DocumentationTest_ExplainsHybridVsSeparateModifiers()
        {
            // This test serves as documentation for the hybrid detection logic
            
            // ✅ TRUE HYBRID EXAMPLE:
            // "Emperor's Vaal Axe" = Single hybrid prefix modifier
            // - Provides BOTH Physical Damage % AND Accuracy in one modifier
            // - Takes up 1 affix slot
            // - Has specific tier ranges for BOTH stats combined
            
            // ❌ SEPARATE MODIFIERS EXAMPLE: 
            // "Wicked Vaal Axe of Precision" = Two separate modifiers
            // - "Wicked" prefix = Physical Damage % only
            // - "of Precision" suffix = Accuracy only  
            // - Takes up 2 affix slots
            // - Each stat has independent tier ranges
            
            Assert.True(true, "This test exists to document the difference between hybrid and separate modifiers");
        }

        #endregion
    }
} 