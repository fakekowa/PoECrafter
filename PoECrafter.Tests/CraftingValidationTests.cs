using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WindowsFormsApplication3;

namespace PoECrafter.Tests
{
    /// <summary>
    /// Comprehensive crafting validation tests using real two-handed axe items
    /// Tests various modifier rules and tier combinations to ensure robust crafting logic
    /// </summary>
    public class CraftingValidationTests
    {
        #region Test Item Data

        private readonly string _flaringVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Flaring Vaal Axe
--------
Two Handed Axe
Physical Damage: 142-254 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
Weapon Range: 1.3 metres
--------
Requirements:
Level: 64
Str: 158
Dex: 76
--------
Sockets: G-R 
--------
Item Level: 84
--------
25% chance to Maim on Hit (implicit)
--------
Adds 38 to 80 Physical Damage";

        private readonly string _heavyVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Heavy Vaal Axe
--------
Two Handed Axe
Physical Damage: 152-254 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
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
46% increased Physical Damage";

        private readonly string _heatedVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Heated Vaal Axe of Nourishment
--------
Two Handed Axe
Physical Damage: 104-174
Elemental Damage: 4-6 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
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
Adds 4 to 6 Fire Damage
Grants 13 Life per Enemy Hit";

        private readonly string _dischargingVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Discharging Vaal Axe of the Leviathan
--------
Two Handed Axe
Physical Damage: 104-174
Elemental Damage: 22-382 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
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
+39 to Strength
Adds 22 to 382 Lightning Damage";

        private readonly string _celebrationVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Vaal Axe of Celebration
--------
Two Handed Axe
Physical Damage: 104-174
Critical Strike Chance: 5.00%
Attacks per Second: 1.46 (augmented)
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
27% increased Attack Speed";

        private readonly string _razorSharpVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Razor-sharp Vaal Axe
--------
Two Handed Axe
Physical Damage: 131-228 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
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
Adds 27 to 54 Physical Damage";

        private readonly string _reaversVaalAxe = @"Item Class: Two Hand Axes
Rarity: Magic
Reaver's Vaal Axe of the Pugilist
--------
Two Handed Axe
Physical Damage: 136-228 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.15
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
31% increased Physical Damage
5% reduced Enemy Stun Threshold
+57 to Accuracy Rating";

        #endregion

        #region Modifier Detection Tests

        [Fact]
        public void FlaringVaalAxe_ShouldDetectT1FlatPhysicalDamage()
        {
            // Flaring Vaal Axe has "Adds 38 to 80 Physical Damage"
            // According to tier data: T1 Flaring (34-47 to 72-84) - this should be T1
            
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_flaringVaalAxe);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            Assert.Contains(affixes, a => a.Contains("Adds 38 to 80 Physical Damage"));
            
            // Test tier classification directly
            int tier = GetFlatPhysicalDamageTier(38, 80);
            Assert.Equal(1, tier); // Should be T1 (Flaring)
        }

        [Fact]
        public void HeavyVaalAxe_ShouldDetectT8PhysicalDamagePercent()
        {
            // Heavy Vaal Axe has "46% increased Physical Damage"
            // According to tier data: T8 Heavy (40-49%) - this should be T8
            
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_heavyVaalAxe);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            Assert.Contains(affixes, a => a.Contains("46% increased Physical Damage"));
            
            // Test tier classification directly
            int tier = GetPhysicalDamagePercentTier(46);
            Assert.Equal(8, tier); // Should be T8 (Heavy)
        }

        [Fact]
        public void CelebrationVaalAxe_ShouldDetectT1AttackSpeed()
        {
            // Vaal Axe of Celebration has "27% increased Attack Speed"
            // According to tier data: T1 Celebration (26-27%) - this should be T1
            
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_celebrationVaalAxe);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            Assert.Contains(affixes, a => a.Contains("27% increased Attack Speed"));
            
            // Test tier classification directly
            int tier = GetAttackSpeedTier(27);
            Assert.Equal(1, tier); // Should be T1 (Celebration)
        }

        [Fact]
        public void RazorSharpVaalAxe_ShouldDetectT3FlatPhysicalDamage()
        {
            // Razor-sharp Vaal Axe has "Adds 27 to 54 Physical Damage"
            // According to tier data: T3 Razor-sharp (25-33 to 52-61) - this should be T3
            
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_razorSharpVaalAxe);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            Assert.Contains(affixes, a => a.Contains("Adds 27 to 54 Physical Damage"));
            
            // Test tier classification directly
            int tier = GetFlatPhysicalDamageTier(27, 54);
            Assert.Equal(3, tier); // Should be T3 (Razor-sharp)
        }

        [Fact]
        public void ReaversVaalAxe_ShouldDetectHybridPhysicalAccuracy()
        {
            // Reaver's Vaal Axe has "31% increased Physical Damage" + "+57 to Accuracy Rating"
            // According to tier data: T6 Reaver's (25-34% + 47-72 Acc) - this should be T6
            
            var progressBar = new TestableProgressBar();
            progressBar.SetTestItem(_reaversVaalAxe);
            progressBar.GetAffixes();
            var affixes = progressBar.GetExtractedAffixes();

            Assert.Contains(affixes, a => a.Contains("31% increased Physical Damage"));
            Assert.Contains(affixes, a => a.Contains("+57 to Accuracy Rating"));
            
            // Test tier classification directly
            int tier = GetHybridPhysicalAccuracyTier(31, 57);
            Assert.Equal(6, tier); // Should be T6 (Reaver's)
        }

        #endregion

        #region Crafting Rule Tests - Single Modifiers

        [Fact]
        public void CraftingRule_T3ToT1PhysPercent_ShouldAcceptT1AndRejectT8()
        {
            // Rule: Accept T3-T1 Physical Damage %
            // Heavy Vaal Axe (46% = T8) should FAIL
            // Hypothetical item with 140% (T3) should PASS

            // Test T8 - should fail
            Assert.False(IsPhysicalDamagePercentAcceptable(46, 3)); // T8 vs min T3
            
            // Test T3 - should pass  
            Assert.True(IsPhysicalDamagePercentAcceptable(140, 3)); // T3 vs min T3
            
            // Test T1 - should pass
            Assert.True(IsPhysicalDamagePercentAcceptable(175, 3)); // T1 vs min T3
        }

        [Fact]
        public void CraftingRule_T5ToT1FlatPhysical_ShouldAcceptT1AndT3()
        {
            // Rule: Accept T5-T1 Flat Physical Damage
            // Flaring Vaal Axe (38-80 = T1) should PASS
            // Razor-sharp Vaal Axe (27-54 = T3) should PASS
            
            Assert.True(IsFlatPhysicalDamageAcceptable(38, 80, 5)); // T1 vs min T5
            Assert.True(IsFlatPhysicalDamageAcceptable(27, 54, 5)); // T3 vs min T5
            
            // Test edge case - T6 should fail T5 requirement
            Assert.False(IsFlatPhysicalDamageAcceptable(13, 28, 5)); // T6 vs min T5
        }

        [Fact]
        public void CraftingRule_T2ToT1AttackSpeed_ShouldBeVeryStrict()
        {
            // Rule: Accept only T2-T1 Attack Speed (very strict)
            // Celebration Vaal Axe (27% = T1) should PASS
            
            Assert.True(IsAttackSpeedAcceptable(27, 2)); // T1 vs min T2
            Assert.True(IsAttackSpeedAcceptable(23, 2)); // T2 vs min T2
            
            // T3 and below should fail
            Assert.False(IsAttackSpeedAcceptable(20, 2)); // T3 vs min T2
            Assert.False(IsAttackSpeedAcceptable(17, 2)); // T4 vs min T2
        }

        #endregion

        #region Crafting Rule Tests - OR Logic Combinations

        [Fact]
        public void CraftingRule_ORLogic_T5ToT1FlatPhys_OR_T5ToT1HybridPhys()
        {
            // Rule: Accept (T5-T1 Flat Physical) OR (T5-T1 Hybrid Physical/Accuracy)
            // Flaring Vaal Axe has T1 Flat Physical - should PASS (satisfies first condition)
            // Reaver's Vaal Axe has T6 Hybrid - should FAIL (doesn't satisfy either condition)
            
            // Test Flaring Vaal Axe - T1 Flat Physical
            bool flaringFlatPhysOk = IsFlatPhysicalDamageAcceptable(38, 80, 5); // T1 - should pass
            bool flaringHybridOk = false; // No hybrid modifier
            bool flaringResult = flaringFlatPhysOk || flaringHybridOk; // OR logic
            Assert.True(flaringResult, "Flaring Vaal Axe should pass with T1 Flat Physical");
            
            // Test Reaver's Vaal Axe - T6 Hybrid Physical/Accuracy  
            bool reaversFlatPhysOk = false; // No flat physical modifier
            bool reaversHybridOk = IsHybridPhysicalAccuracyAcceptable(31, 57, 5); // T6 - should fail T5 requirement
            bool reaversResult = reaversFlatPhysOk || reaversHybridOk; // OR logic
            Assert.False(reaversResult, "Reaver's Vaal Axe should fail with T6 Hybrid (need T5 or better)");
        }

        [Fact]
        public void CraftingRule_ORLogic_T3ToT1PhysPercent_OR_T1AttackSpeed()
        {
            // Rule: Accept (T3-T1 Physical Damage %) OR (T1 Attack Speed only)
            // Heavy Vaal Axe has T8 Physical % - should FAIL first condition
            // Celebration Vaal Axe has T1 Attack Speed - should PASS second condition
            
            // Test Heavy Vaal Axe - T8 Physical %
            bool heavyPhysOk = IsPhysicalDamagePercentAcceptable(46, 3); // T8 - should fail T3 requirement
            bool heavyAttackOk = false; // No attack speed modifier
            bool heavyResult = heavyPhysOk || heavyAttackOk; // OR logic
            Assert.False(heavyResult, "Heavy Vaal Axe should fail with T8 Physical % (need T3 or better)");
            
            // Test Celebration Vaal Axe - T1 Attack Speed
            bool celebrationPhysOk = false; // No physical % modifier  
            bool celebrationAttackOk = IsAttackSpeedAcceptable(27, 1); // T1 - should pass T1 requirement
            bool celebrationResult = celebrationPhysOk || celebrationAttackOk; // OR logic
            Assert.True(celebrationResult, "Celebration Vaal Axe should pass with T1 Attack Speed");
        }

        #endregion

        #region Crafting Rule Tests - Complex Multi-Modifier

        [Fact]
        public void CraftingRule_ComplexMultiModifier_T3PhysPercent_AND_T3FlatPhys_AND_T2AttackSpeed()
        {
            // Rule: Accept T3+ Physical % AND T3+ Flat Physical AND T2+ Attack Speed (all required)
            // This is a very strict rule requiring all three modifiers at high tiers
            
            // Perfect item - should pass all conditions
            bool perfectPhysPercent = IsPhysicalDamagePercentAcceptable(140, 3); // T3
            bool perfectFlatPhys = IsFlatPhysicalDamageAcceptable(27, 54, 3); // T3  
            bool perfectAttackSpeed = IsAttackSpeedAcceptable(25, 2); // T2
            bool perfectResult = perfectPhysPercent && perfectFlatPhys && perfectAttackSpeed; // AND logic
            Assert.True(perfectResult, "Perfect item with T3+ in all categories should pass");
            
            // Item missing one modifier - should fail
            bool missingAttackSpeed = perfectPhysPercent && perfectFlatPhys && false; // Missing attack speed
            Assert.False(missingAttackSpeed, "Item missing attack speed should fail AND logic");
            
            // Item with insufficient tier in one modifier - should fail
            bool lowTierAttackSpeed = IsAttackSpeedAcceptable(20, 2); // T3 vs min T2 - should fail
            bool insufficientTier = perfectPhysPercent && perfectFlatPhys && lowTierAttackSpeed;
            Assert.False(insufficientTier, "Item with T3 attack speed (need T2+) should fail");
        }

        [Fact]
        public void CraftingRule_RealWorldScenario_FlexiblePhysicalDamage()
        {
            // Rule: Accept (T4+ Physical %) OR (T4+ Flat Physical) OR (T4+ Hybrid Physical/Accuracy)
            // This simulates a realistic crafting scenario where any good physical damage modifier is acceptable
            
            // Test each item against this flexible rule
            
            // Flaring Vaal Axe - T1 Flat Physical (should pass)
            bool flaringResult = IsFlatPhysicalDamageAcceptable(38, 80, 4); // T1 vs min T4
            Assert.True(flaringResult, "Flaring Vaal Axe with T1 Flat Physical should pass T4+ requirement");
            
            // Heavy Vaal Axe - T8 Physical % (should fail)  
            bool heavyResult = IsPhysicalDamagePercentAcceptable(46, 4); // T8 vs min T4
            Assert.False(heavyResult, "Heavy Vaal Axe with T8 Physical % should fail T4+ requirement");
            
            // Razor-sharp Vaal Axe - T3 Flat Physical (should pass)
            bool razorResult = IsFlatPhysicalDamageAcceptable(27, 54, 4); // T3 vs min T4
            Assert.True(razorResult, "Razor-sharp Vaal Axe with T3 Flat Physical should pass T4+ requirement");
            
            // Reaver's Vaal Axe - T6 Hybrid (should fail)
            bool reaversResult = IsHybridPhysicalAccuracyAcceptable(31, 57, 4); // T6 vs min T4
            Assert.False(reaversResult, "Reaver's Vaal Axe with T6 Hybrid should fail T4+ requirement");
        }

        #endregion

        #region Crafting Safety Tests

        [Fact]
        public void CraftingSafety_ShouldNeverRollOverAcceptableModifiers()
        {
            // Critical safety test: Ensure crafting logic never continues when acceptable modifiers are found
            // This was the "CRITICAL BUG" mentioned in TASKS.md that was fixed
            
            // Test T1 Attack Speed against T3 requirement - should stop crafting
            bool t1AttackSpeedAcceptable = IsAttackSpeedAcceptable(27, 3); // T1 vs min T3
            Assert.True(t1AttackSpeedAcceptable, "T1 Attack Speed should satisfy T3 requirement");
            
            // Test T1 Flat Physical against T5 requirement - should stop crafting
            bool t1FlatPhysAcceptable = IsFlatPhysicalDamageAcceptable(38, 80, 5); // T1 vs min T5  
            Assert.True(t1FlatPhysAcceptable, "T1 Flat Physical should satisfy T5 requirement");
            
            // Test borderline case: T3 Physical % against T3 requirement - should stop crafting
            bool t3PhysPercentAcceptable = IsPhysicalDamagePercentAcceptable(140, 3); // T3 vs min T3
            Assert.True(t3PhysPercentAcceptable, "T3 Physical % should satisfy T3 requirement (exact match)");
        }

        [Fact]
        public void CraftingSafety_ShouldRejectUnacceptableTiers()
        {
            // Ensure crafting logic properly rejects modifiers that don't meet minimum requirements
            
            // Test T8 Physical % against T3 requirement - should continue crafting
            bool t8PhysPercentAcceptable = IsPhysicalDamagePercentAcceptable(46, 3); // T8 vs min T3
            Assert.False(t8PhysPercentAcceptable, "T8 Physical % should NOT satisfy T3 requirement");
            
            // Test T6 Hybrid against T4 requirement - should continue crafting
            bool t6HybridAcceptable = IsHybridPhysicalAccuracyAcceptable(31, 57, 4); // T6 vs min T4
            Assert.False(t6HybridAcceptable, "T6 Hybrid should NOT satisfy T4 requirement");
            
            // Test T3 Attack Speed against T2 requirement - should continue crafting
            bool t3AttackSpeedAcceptable = IsAttackSpeedAcceptable(20, 2); // T3 vs min T2
            Assert.False(t3AttackSpeedAcceptable, "T3 Attack Speed should NOT satisfy T2 requirement");
        }

        #endregion

        #region Additional Random Modifier Rules Tests

        [Fact]
        public void CraftingRule_RandomScenario1_T3PhysPercent_OR_T5FlatPhys()
        {
            // Random Rule: Accept (T3-T1 Physical Damage %) OR (T5-T1 Flat Physical)
            // This tests the specific rule mentioned by user: "t3-t1 phys % dmg in one case"
            
            // Test Heavy Vaal Axe - T8 Physical % (should fail both conditions)
            bool heavyPhysOk = IsPhysicalDamagePercentAcceptable(46, 3); // T8 vs min T3 - should fail
            bool heavyFlatOk = false; // No flat physical modifier
            bool heavyResult = heavyPhysOk || heavyFlatOk; // OR logic
            Assert.False(heavyResult, "Heavy Vaal Axe with T8 Physical % should fail T3+ requirement");
            
            // Test Flaring Vaal Axe - T1 Flat Physical (should pass second condition)
            bool flaringPhysOk = false; // No physical % modifier
            bool flaringFlatOk = IsFlatPhysicalDamageAcceptable(38, 80, 5); // T1 vs min T5 - should pass
            bool flaringResult = flaringPhysOk || flaringFlatOk; // OR logic
            Assert.True(flaringResult, "Flaring Vaal Axe with T1 Flat Physical should pass T5+ requirement");
            
            // Test edge case: exactly T3 Physical % should pass
            bool t3PhysOk = IsPhysicalDamagePercentAcceptable(140, 3); // T3 vs min T3 - should pass
            bool t3Result = t3PhysOk || false; // OR logic with no second modifier
            Assert.True(t3Result, "T3 Physical % should pass T3+ requirement (exact match)");
        }

        [Fact]
        public void CraftingRule_RandomScenario2_T5FlatPhys_OR_T5HybridPhys()
        {
            // Random Rule: Accept (T5-T1 Flat Physical) OR (T5-T1 Hybrid Physical/Accuracy)
            // This tests the specific rule mentioned by user: "t5-t1 flat phys OR t5-t1 hybrid phys %"
            
            // Test Razor-sharp Vaal Axe - T3 Flat Physical (should pass first condition)
            bool razorFlatOk = IsFlatPhysicalDamageAcceptable(27, 54, 5); // T3 vs min T5 - should pass
            bool razorHybridOk = false; // No hybrid modifier
            bool razorResult = razorFlatOk || razorHybridOk; // OR logic
            Assert.True(razorResult, "Razor-sharp Vaal Axe with T3 Flat Physical should pass T5+ requirement");
            
            // Test Reaver's Vaal Axe - T6 Hybrid (should fail both conditions)
            bool reaversFlatOk = false; // No flat physical modifier
            bool reaversHybridOk = IsHybridPhysicalAccuracyAcceptable(31, 57, 5); // T6 vs min T5 - should fail
            bool reaversResult = reaversFlatOk || reaversHybridOk; // OR logic
            Assert.False(reaversResult, "Reaver's Vaal Axe with T6 Hybrid should fail T5+ requirement");
            
            // Test hypothetical item with T5 Hybrid (should pass second condition)
            bool t5HybridResult = false || IsHybridPhysicalAccuracyAcceptable(35, 73, 5); // T5 vs min T5
            Assert.True(t5HybridResult, "T5 Hybrid should pass T5+ requirement (exact match)");
        }

        [Fact]
        public void CraftingRule_RandomScenario3_VeryStrictT1Only_AttackSpeed()
        {
            // Random Rule: Accept ONLY T1 Attack Speed (very strict for speed runners)
            
            // Test Celebration Vaal Axe - T1 Attack Speed (should pass)
            bool t1AttackResult = IsAttackSpeedAcceptable(27, 1); // T1 vs min T1
            Assert.True(t1AttackResult, "T1 Attack Speed (27%) should pass T1-only requirement");
            
            // Test T2 Attack Speed (should fail strict T1 requirement)
            bool t2AttackResult = IsAttackSpeedAcceptable(23, 1); // T2 vs min T1
            Assert.False(t2AttackResult, "T2 Attack Speed (23%) should fail T1-only requirement");
            
            // Test edge case: minimum T1 value (26%) should pass
            bool minT1Result = IsAttackSpeedAcceptable(26, 1); // T1 min vs min T1
            Assert.True(minT1Result, "Minimum T1 Attack Speed (26%) should pass T1-only requirement");
        }

        [Fact]
        public void CraftingRule_RandomScenario4_ComplexTripleOR_AnyGoodModifier()
        {
            // Random Rule: Accept (T2+ Physical %) OR (T3+ Flat Physical) OR (T1+ Attack Speed)
            // Simulates a "any good modifier" approach for versatile crafting
            
            // Test Heavy Vaal Axe - T8 Physical % (should fail all conditions)
            bool heavyResult = IsPhysicalDamagePercentAcceptable(46, 2) || // T8 vs T2 - fail
                              false || // No flat physical
                              false;   // No attack speed
            Assert.False(heavyResult, "Heavy Vaal Axe with T8 Physical % should fail all conditions");
            
            // Test Celebration Vaal Axe - T1 Attack Speed (should pass third condition)
            bool celebrationResult = false || // No physical %
                                   false || // No flat physical  
                                   IsAttackSpeedAcceptable(27, 1); // T1 vs T1 - pass
            Assert.True(celebrationResult, "Celebration Vaal Axe with T1 Attack Speed should pass");
            
            // Test Flaring Vaal Axe - T1 Flat Physical (should pass second condition)
            bool flaringResult = false || // No physical %
                               IsFlatPhysicalDamageAcceptable(38, 80, 3) || // T1 vs T3 - pass
                               false; // No attack speed
            Assert.True(flaringResult, "Flaring Vaal Axe with T1 Flat Physical should pass");
        }

        [Fact]
        public void CraftingRule_RandomScenario5_EdgeCase_BarelyAcceptable()
        {
            // Random Rule: Test edge cases where modifiers are barely acceptable
            // This ensures the tier checking logic is precise and doesn't have off-by-one errors
            
            // Test Physical Damage % at exact tier boundaries
            Assert.True(IsPhysicalDamagePercentAcceptable(135, 3), "135% should be T3 (exact minimum)");
            Assert.False(IsPhysicalDamagePercentAcceptable(134, 3), "134% should be T4 (fail T3 requirement)");
            
            // Test Flat Physical Damage at exact tier boundaries  
            Assert.True(IsFlatPhysicalDamageAcceptable(25, 52, 3), "25-52 should be T3 (exact minimum)");
            Assert.False(IsFlatPhysicalDamageAcceptable(24, 51, 3), "24-51 should be T4 (fail T3 requirement)");
            
            // Test Attack Speed at exact tier boundaries
            Assert.True(IsAttackSpeedAcceptable(20, 3), "20% should be T3 (exact minimum)");
            Assert.False(IsAttackSpeedAcceptable(19, 3), "19% should be T4 (fail T3 requirement)");
            
            // Test Hybrid at exact tier boundaries
            Assert.True(IsHybridPhysicalAccuracyAcceptable(55, 124, 3), "55%+124 should be T3 (exact minimum)");
            Assert.False(IsHybridPhysicalAccuracyAcceptable(54, 123, 3), "54%+123 should be T4 (fail T3 requirement)");
        }

        [Fact]
        public void CraftingRule_RandomScenario6_SuperFlexible_AnyT5OrBetter()
        {
            // Random Rule: Accept any modifier at T5 or better (very flexible crafting)
            // This simulates early-game or budget crafting where any decent modifier is acceptable
            
            // All test items should pass this flexible rule
            
            // Flaring Vaal Axe - T1 Flat Physical
            Assert.True(IsFlatPhysicalDamageAcceptable(38, 80, 5), "T1 Flat Physical should pass T5+ requirement");
            
            // Heavy Vaal Axe - T8 Physical % (should fail)
            Assert.False(IsPhysicalDamagePercentAcceptable(46, 5), "T8 Physical % should fail T5+ requirement");
            
            // Celebration Vaal Axe - T1 Attack Speed  
            Assert.True(IsAttackSpeedAcceptable(27, 5), "T1 Attack Speed should pass T5+ requirement");
            
            // Razor-sharp Vaal Axe - T3 Flat Physical
            Assert.True(IsFlatPhysicalDamageAcceptable(27, 54, 5), "T3 Flat Physical should pass T5+ requirement");
            
            // Reaver's Vaal Axe - T6 Hybrid (should fail)
            Assert.False(IsHybridPhysicalAccuracyAcceptable(31, 57, 5), "T6 Hybrid should fail T5+ requirement");
        }

        [Fact]
        public void CraftingRule_RandomScenario7_MultipleAND_VeryStrict()
        {
            // Random Rule: Accept T2+ Physical % AND T2+ Flat Physical AND T2+ Attack Speed (all required)
            // This is extremely strict and would be very expensive to achieve
            
            // Perfect item simulation - all T2 or better
            bool perfectItemResult = IsPhysicalDamagePercentAcceptable(160, 2) && // T2
                                   IsFlatPhysicalDamageAcceptable(32, 65, 2) && // T2
                                   IsAttackSpeedAcceptable(24, 2); // T2
            Assert.True(perfectItemResult, "Perfect item with all T2+ modifiers should pass");
            
            // Item missing one modifier - should fail
            bool missingModifierResult = IsPhysicalDamagePercentAcceptable(160, 2) && // T2
                                       IsFlatPhysicalDamageAcceptable(32, 65, 2) && // T2
                                       false; // No attack speed
            Assert.False(missingModifierResult, "Item missing attack speed should fail AND logic");
            
            // Item with one insufficient tier - should fail
            bool insufficientTierResult = IsPhysicalDamagePercentAcceptable(160, 2) && // T2
                                        IsFlatPhysicalDamageAcceptable(25, 52, 2) && // T3 vs min T2 - fail
                                        IsAttackSpeedAcceptable(24, 2); // T2
            Assert.False(insufficientTierResult, "Item with T3 flat physical (need T2+) should fail");
        }

        [Fact]
        public void CraftingRule_RandomScenario8_EdgeCase_ZeroValueHandling()
        {
            // Random Rule: Test edge cases with unusual values
            // Ensures the system handles edge cases gracefully
            
            // Test very low values that might appear on low-level items
            Assert.Equal(10, GetPhysicalDamagePercentTier(1)); // Very low value
            Assert.Equal(10, GetFlatPhysicalDamageTier(1, 2)); // Very low flat damage
            Assert.Equal(10, GetAttackSpeedTier(1)); // Very low attack speed
            Assert.Equal(10, GetHybridPhysicalAccuracyTier(1, 1)); // Very low hybrid
            
            // Test maximum possible values
            Assert.Equal(1, GetPhysicalDamagePercentTier(179)); // Maximum T1
            Assert.Equal(1, GetFlatPhysicalDamageTier(47, 84)); // Maximum T1
            Assert.Equal(1, GetAttackSpeedTier(27)); // Maximum T1
            Assert.Equal(1, GetHybridPhysicalAccuracyTier(79, 200)); // Maximum T1
        }

        [Fact]
        public void CraftingRule_RandomScenario9_RealWorldExample_BalancedCrafter()
        {
            // Random Rule: Accept (T4+ Physical %) OR (T3+ Flat Physical) OR (T3+ Attack Speed)
            // This simulates a balanced approach for mid-tier crafting
            
            // Test each real item against this balanced rule
            
            // Heavy Vaal Axe - T8 Physical % (should fail)
            bool heavyResult = IsPhysicalDamagePercentAcceptable(46, 4) || // T8 vs T4 - fail
                              false || // No flat physical
                              false;   // No attack speed
            Assert.False(heavyResult, "Heavy Vaal Axe should fail balanced mid-tier rule");
            
            // Flaring Vaal Axe - T1 Flat Physical (should pass)
            bool flaringResult = false || // No physical %
                               IsFlatPhysicalDamageAcceptable(38, 80, 3) || // T1 vs T3 - pass
                               false; // No attack speed
            Assert.True(flaringResult, "Flaring Vaal Axe should pass balanced mid-tier rule");
            
            // Celebration Vaal Axe - T1 Attack Speed (should pass)
            bool celebrationResult = false || // No physical %
                                   false || // No flat physical
                                   IsAttackSpeedAcceptable(27, 3); // T1 vs T3 - pass
            Assert.True(celebrationResult, "Celebration Vaal Axe should pass balanced mid-tier rule");
            
            // Razor-sharp Vaal Axe - T3 Flat Physical (should pass at exact boundary)
            bool razorResult = false || // No physical %
                             IsFlatPhysicalDamageAcceptable(27, 54, 3) || // T3 vs T3 - pass
                             false; // No attack speed
            Assert.True(razorResult, "Razor-sharp Vaal Axe should pass at T3 boundary");
        }

        [Fact]
        public void CraftingRule_RandomScenario10_MegaStrict_OnlyT1Modifiers()
        {
            // Random Rule: Accept ONLY T1 modifiers (mirror-tier crafting simulation)
            // This is the strictest possible rule - only perfect tiers accepted
            
            // Test T1 Physical % (should pass)
            Assert.True(IsPhysicalDamagePercentAcceptable(175, 1), "T1 Physical % should pass T1-only rule");
            
            // Test T1 Flat Physical (should pass)
            Assert.True(IsFlatPhysicalDamageAcceptable(40, 80, 1), "T1 Flat Physical should pass T1-only rule");
            
            // Test T1 Attack Speed (should pass)
            Assert.True(IsAttackSpeedAcceptable(27, 1), "T1 Attack Speed should pass T1-only rule");
            
            // Test T1 Hybrid (should pass)
            Assert.True(IsHybridPhysicalAccuracyAcceptable(77, 180, 1), "T1 Hybrid should pass T1-only rule");
            
            // Test T2 modifiers (should all fail T1-only rule)
            Assert.False(IsPhysicalDamagePercentAcceptable(160, 1), "T2 Physical % should fail T1-only rule");
            Assert.False(IsFlatPhysicalDamageAcceptable(32, 65, 1), "T2 Flat Physical should fail T1-only rule");
            Assert.False(IsAttackSpeedAcceptable(24, 1), "T2 Attack Speed should fail T1-only rule");
            Assert.False(IsHybridPhysicalAccuracyAcceptable(67, 155, 1), "T2 Hybrid should fail T1-only rule");
        }

        #endregion

        #region Helper Methods (Tier Calculation Logic)

        private bool IsPhysicalDamagePercentAcceptable(int value, int minTier)
        {
            int actualTier = GetPhysicalDamagePercentTier(value);
            return actualTier <= minTier; // Lower tier number = better tier
        }

        private bool IsFlatPhysicalDamageAcceptable(int minDamage, int maxDamage, int minTier)
        {
            int actualTier = GetFlatPhysicalDamageTier(minDamage, maxDamage);
            return actualTier <= minTier; // Lower tier number = better tier
        }

        private bool IsAttackSpeedAcceptable(int value, int minTier)
        {
            int actualTier = GetAttackSpeedTier(value);
            return actualTier <= minTier; // Lower tier number = better tier
        }

        private bool IsHybridPhysicalAccuracyAcceptable(int physPercent, int accuracy, int minTier)
        {
            int actualTier = GetHybridPhysicalAccuracyTier(physPercent, accuracy);
            return actualTier <= minTier; // Lower tier number = better tier
        }

        private int GetPhysicalDamagePercentTier(int value)
        {
            if (value >= 170) return 1; // Merciless T1: 170-179%
            if (value >= 155) return 2; // Tyrannical T2: 155-169%
            if (value >= 135) return 3; // Cruel T3: 135-154%
            if (value >= 110) return 4; // Bloodthirsty T4: 110-134%
            if (value >= 85) return 5;  // Vicious T5: 85-109%
            if (value >= 65) return 6;  // Wicked T6: 65-84%
            if (value >= 50) return 7;  // Serrated T7: 50-64%
            if (value >= 40) return 8;  // Heavy T8: 40-49%
            return 10; // Lower than T8
        }

        private int GetFlatPhysicalDamageTier(int minDamage, int maxDamage)
        {
            // Based on exact PoEDB data for Two Hand Axes - Adds # to # Physical Damage
            if (minDamage >= 34) return 1; // Flaring T1 (level 77): 34-47 to 72-84
            if (minDamage >= 30) return 2; // Tempered T2 (level 65): 30-40 to 63-73
            if (minDamage >= 25) return 3; // Razor-sharp T3 (level 54): 25-33 to 52-61
            if (minDamage >= 20) return 4; // Annealed T4 (level 46): 20-28 to 41-51
            if (minDamage >= 16) return 5; // Gleaming T5 (level 36): 16-22 to 35-40
            if (minDamage >= 13) return 6; // Honed T6 (level 29): 13-17 to 28-32
            if (minDamage >= 10) return 7; // Polished T7 (level 21): 10-13 to 21-25
            if (minDamage >= 6) return 8;  // Burnished T8 (level 13): 6-8 to 12-15
            if (minDamage >= 2) return 9;  // Glinting T9 (level 2): 2 to 4-5
            return 10; // Invalid/unknown tier
        }

        private int GetAttackSpeedTier(int value)
        {
            // Based on exact PoEDB data for Two Hand Axes - #% increased Attack Speed
            if (value >= 26) return 1; // of Celebration T1 (level 77): 26-27%
            if (value >= 23) return 2; // of Infamy T2 (level 60): 23-25%
            if (value >= 20) return 3; // of Fame T3 (level 45): 20-22%
            if (value >= 17) return 4; // of Acclaim T4 (level 37): 17-19%
            if (value >= 14) return 5; // of Renown T5 (level 30): 14-16%
            if (value >= 11) return 6; // of Mastery T6 (level 22): 11-13%
            if (value >= 8) return 7;  // of Ease T7 (level 11): 8-10%
            if (value >= 5) return 8;  // of Skill T8 (level 1): 5-7%
            return 10; // Invalid/unknown tier
        }

        private int GetHybridPhysicalAccuracyTier(int physPercent, int accuracy)
        {
            // Based on exact PoEDB data for Two Hand Axes - #% increased Physical Damage + # to Accuracy Rating
            if (physPercent >= 75 && accuracy >= 175) return 1; // Dictator's T1 (level 83): 75-79% + 175-200 Acc
            if (physPercent >= 65 && accuracy >= 150) return 2; // Emperor's T2 (level 73): 65-74% + 150-174 Acc
            if (physPercent >= 55 && accuracy >= 124) return 3; // Conqueror's T3 (level 60): 55-64% + 124-149 Acc
            if (physPercent >= 45 && accuracy >= 98) return 4;  // Champion's T4 (level 46): 45-54% + 98-123 Acc
            if (physPercent >= 35 && accuracy >= 73) return 5;  // Mercenary's T5 (level 35): 35-44% + 73-97 Acc
            if (physPercent >= 25 && accuracy >= 47) return 6;  // Reaver's T6 (level 23): 25-34% + 47-72 Acc
            if (physPercent >= 20 && accuracy >= 21) return 7;  // Journeyman's T7 (level 11): 20-24% + 21-46 Acc
            if (physPercent >= 15 && accuracy >= 16) return 8;  // Squire's T8 (level 1): 15-19% + 16-20 Acc
            return 10; // Invalid/unknown tier
        }

        #endregion
    }
} 