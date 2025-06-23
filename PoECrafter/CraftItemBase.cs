using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication3
{
    public class CraftItemBase
    {
        public Affixes Affixes = new Affixes();
        private Itemtype Itemtype;
        private Damages Damages;
        private Armours Armours;
        private Requirements Requirements;
        private int? Sockets;
        private int? Links;
        private int? ItemLevel;
        private int? ImplicitNumber;

        public bool HasArmourStat { get; set; }
        public bool HasLifeStat { get; set; }
        
        // Two-handed axe physical damage modifiers
        public bool HasPhysicalDamagePercent { get; set; }
        public int PhysicalDamagePercentValue { get; set; }
        
        public bool HasFlatPhysicalDamage { get; set; }
        public int FlatPhysicalDamageMin { get; set; }
        public int FlatPhysicalDamageMax { get; set; }
        
        public bool HasHybridPhysicalAccuracy { get; set; }
        public int HybridPhysicalPercentValue { get; set; }
        public int HybridAccuracyValue { get; set; }
    }

    // **NEW: Advanced Modifier Logic Data Structures**

    public enum AffixType
    {
        Prefix,
        Suffix,
        Unknown
    }

    public enum CraftingStrategy
    {
        AltSpamOnly,
        SmartAugmentation,
        CustomStrategy
    }

    public enum LogicOperator
    {
        AND,
        OR
    }

    public class ModifierGroup
    {
        public string GroupName { get; set; }
        public List<string> SelectedModifiers { get; set; } = new List<string>();
        public LogicOperator Operator { get; set; } = LogicOperator.AND; // Operator BETWEEN this group and the next
        public bool IsEnabled { get; set; } = true;
        public bool IsSatisfied { get; set; } = false;

        public ModifierGroup(string groupName)
        {
            GroupName = groupName;
        }
    }

    public class CraftingConfiguration
    {
        public List<ModifierGroup> ModifierGroups { get; set; } = new List<ModifierGroup>();
        public CraftingStrategy Strategy { get; set; } = CraftingStrategy.AltSpamOnly;
        public bool UseORLogicBetweenGroups { get; set; } = false; // Global OR logic override
        public int MaxCurrencyUsage { get; set; } = 100;
        public bool EnableSmartAugmentation { get; set; } = false;

        public bool IsCraftingComplete()
        {
            if (UseORLogicBetweenGroups)
            {
                // If using OR logic, ANY satisfied group means we're done
                return ModifierGroups.Any(g => g.IsEnabled && g.IsSatisfied);
            }
            else
            {
                // Default AND logic: ALL enabled groups must be satisfied
                return ModifierGroups.Where(g => g.IsEnabled).All(g => g.IsSatisfied);
            }
        }

        public void ResetSatisfactionStates()
        {
            foreach (var group in ModifierGroups)
            {
                group.IsSatisfied = false;
            }
        }
    }

    public class ItemAnalysis
    {
        public List<DetectedAffix> DetectedAffixes { get; set; } = new List<DetectedAffix>();
        public int PrefixCount { get; set; } = 0;
        public int SuffixCount { get; set; } = 0;
        public bool IsMagic { get; set; } = false;
        public bool IsRare { get; set; } = false;
        public string ItemText { get; set; } = "";
        public DateTime LastAnalyzed { get; set; } = DateTime.Now;
    }

    public class DetectedAffix
    {
        public string ModifierName { get; set; }
        public AffixType Type { get; set; }
        public int Value { get; set; }
        public int SecondaryValue { get; set; } // For hybrid modifiers
        public int Tier { get; set; }
        public string MatchedText { get; set; }
    }

    public class SmartCurrencySelector
    {
        public enum CurrencyType
        {
            Transmutation,
            Augmentation,
            Alteration,
            Alchemy,
            Chaos,
            Regal,
            Exalted,
            Divine
        }

        public static CurrencyType SelectOptimalCurrency(ItemAnalysis analysis, CraftingConfiguration config)
        {
            if (analysis.IsRare)
            {
                return CurrencyType.Chaos; // Rare items need Chaos
            }

            if (analysis.IsMagic && config.EnableSmartAugmentation)
            {
                // Smart augmentation logic
                if (analysis.PrefixCount + analysis.SuffixCount == 1)
                {
                    // Item has 1 affix - check if we want the opposite type
                    var currentAffixTypes = analysis.DetectedAffixes.Select(a => a.Type).ToHashSet();
                    var targetAffixTypes = GetTargetAffixTypes(config);
                    
                    // If we want both prefix and suffix types, and current item only has one type
                    if (targetAffixTypes.Contains(AffixType.Prefix) && targetAffixTypes.Contains(AffixType.Suffix))
                    {
                        if (currentAffixTypes.Count == 1)
                        {
                            return CurrencyType.Augmentation; // Add the missing type
                        }
                    }
                }
            }

            return CurrencyType.Alteration; // Default: Alt spam
        }

        private static HashSet<AffixType> GetTargetAffixTypes(CraftingConfiguration config)
        {
            var types = new HashSet<AffixType>();
            
            foreach (var group in config.ModifierGroups.Where(g => g.IsEnabled))
            {
                foreach (var modifier in group.SelectedModifiers)
                {
                    var affixType = AffixDatabase.GetAffixType(modifier);
                    if (affixType != AffixType.Unknown)
                    {
                        types.Add(affixType);
                    }
                }
            }
            
            return types;
        }
    }

    // **EXPANDED: Affix Database for Prefix/Suffix Classification**
    public static class AffixDatabase
    {
        public static Dictionary<string, AffixType> KnownAffixes = new Dictionary<string, AffixType>
        {
            // Prefixes (item damage, defense, flat stats)
            {"Physical Damage %", AffixType.Prefix},
            {"Flat Physical Damage", AffixType.Prefix},
            {"Hybrid Physical/Accuracy", AffixType.Prefix},
            {"Added Fire Damage", AffixType.Prefix},
            {"Added Cold Damage", AffixType.Prefix},
            {"Added Lightning Damage", AffixType.Prefix},
            {"Life", AffixType.Prefix},
            {"Mana", AffixType.Prefix},
            {"Energy Shield", AffixType.Prefix},
            {"Armour", AffixType.Prefix},
            
            // Suffixes (player stats, resistances, percentages)  
            {"Attack Speed %", AffixType.Suffix},
            {"Critical Strike Chance %", AffixType.Suffix},
            {"Critical Strike Multiplier %", AffixType.Suffix},
            {"Fire Resistance", AffixType.Suffix},
            {"Cold Resistance", AffixType.Suffix},
            {"Lightning Resistance", AffixType.Suffix},
            {"All Resistances", AffixType.Suffix},
            {"Movement Speed", AffixType.Suffix},
            {"Accuracy Rating", AffixType.Suffix}
        };

        public static AffixType GetAffixType(string modifierName)
        {
            // Try exact match first
            if (KnownAffixes.ContainsKey(modifierName))
                return KnownAffixes[modifierName];

            // Try partial matches for complex modifier names
            foreach (var kvp in KnownAffixes)
            {
                if (modifierName.Contains(kvp.Key))
                    return kvp.Value;
            }

            return AffixType.Unknown;
        }

        public static string ClassifyModifierFromItemText(string modifierText)
        {
            // Physical Damage patterns
            if (modifierText.Contains("increased Physical Damage") && !modifierText.Contains("Accuracy"))
                return "Physical Damage %";
            if (modifierText.Contains("Adds") && modifierText.Contains("Physical Damage"))
                return "Flat Physical Damage";
            if (modifierText.Contains("increased Physical Damage") && modifierText.Contains("Accuracy"))
                return "Hybrid Physical/Accuracy";
            
            // Attack modifiers
            if (modifierText.Contains("increased Attack Speed"))
                return "Attack Speed %";
            if (modifierText.Contains("increased Critical Strike Chance"))
                return "Critical Strike Chance %";
            if (modifierText.Contains("increased Critical Strike Multiplier"))
                return "Critical Strike Multiplier %";
            
            // Defensive stats
            if (modifierText.Contains("to maximum Life"))
                return "Life";
            if (modifierText.Contains("to Armour"))
                return "Armour";
            if (modifierText.Contains("to Accuracy Rating"))
                return "Accuracy Rating";
            
            // Resistances
            if (modifierText.Contains("Fire Resistance"))
                return "Fire Resistance";
            if (modifierText.Contains("Cold Resistance"))
                return "Cold Resistance";
            if (modifierText.Contains("Lightning Resistance"))
                return "Lightning Resistance";
            if (modifierText.Contains("to all Resistances"))
                return "All Resistances";

            return "Unknown";
        }
    }

    internal class Armours
    {
    }

    public class Affixes
    {
        public List<String> AffixArray = new List<String>();
    }

    internal class Requirements
    {
    }

    internal class Damages
    {
    }

    public class Itemtype
    {
        public string ItemClass;
        private string Rarity;
        private string ItemName;
        private string ItemType;
    }

    public static class TwoHandedAxeModifiers
    {
        public static bool CheckPhysicalDamagePercent(string itemText, int minValue, out int value)
        {
            value = 0;
            // Pattern: "# to #% increased Physical Damage" or "#% increased Physical Damage"
            var patterns = new[]
            {
                @"(\d+)% increased Physical Damage",
                @"(\d+) to (\d+)% increased Physical Damage"
            };
            
            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(itemText, pattern);
                if (match.Success)
                {
                    if (match.Groups.Count == 2)
                    {
                        // Single value pattern
                        value = int.Parse(match.Groups[1].Value);
                    }
                    else if (match.Groups.Count == 3)
                    {
                        // Range pattern - take the max value
                        value = int.Parse(match.Groups[2].Value);
                    }
                    return value >= minValue;
                }
            }
            return false;
        }
        
        public static bool CheckFlatPhysicalDamage(string itemText, int minLow, int minHigh, out int lowValue, out int highValue)
        {
            lowValue = 0;
            highValue = 0;
            
            // Pattern: "Adds # to # Physical Damage"
            var pattern = @"Adds (\d+) to (\d+) Physical Damage";
            var match = System.Text.RegularExpressions.Regex.Match(itemText, pattern);
            
            if (match.Success)
            {
                lowValue = int.Parse(match.Groups[1].Value);
                highValue = int.Parse(match.Groups[2].Value);
                return lowValue >= minLow && highValue >= minHigh;
            }
            return false;
        }
        
        public static bool CheckHybridPhysicalAccuracy(string itemText, int minPhysPercent, int minAccuracy, out int physValue, out int accValue)
        {
            physValue = 0;
            accValue = 0;
            
            // Pattern: "#% increased Physical Damage\n+# to Accuracy Rating" (hybrid modifier)
            var physPattern = @"(\d+)% increased Physical Damage";
            var accPattern = @"\+(\d+) to Accuracy Rating";
            
            var physMatch = System.Text.RegularExpressions.Regex.Match(itemText, physPattern);
            var accMatch = System.Text.RegularExpressions.Regex.Match(itemText, accPattern);
            
            if (physMatch.Success && accMatch.Success)
            {
                physValue = int.Parse(physMatch.Groups[1].Value);
                accValue = int.Parse(accMatch.Groups[1].Value);
                
                // Check if they're close together in the text (within 100 characters) indicating they're from the same modifier
                int physIndex = physMatch.Index;
                int accIndex = accMatch.Index;
                
                if (Math.Abs(physIndex - accIndex) <= 100)
                {
                    return physValue >= minPhysPercent && accValue >= minAccuracy;
                }
            }
            return false;
        }
    }
}