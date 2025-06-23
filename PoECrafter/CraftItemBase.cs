using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text.RegularExpressions;

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
                // **NEW: Use magic item name analysis (much faster and more reliable)**
                var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(analysis.ItemText);
                int totalAffixes = (hasPrefix ? 1 : 0) + (hasSuffix ? 1 : 0);
                
                // **FALLBACK: If name analysis fails (empty ItemText or parsing issues), use old method**
                if (totalAffixes == 0 && !string.IsNullOrEmpty(analysis.ItemText) && analysis.DetectedAffixes.Count > 0)
                {
                    // Use the old affix-based counting as fallback
                    totalAffixes = analysis.PrefixCount + analysis.SuffixCount;
                    hasPrefix = analysis.PrefixCount > 0;
                    hasSuffix = analysis.SuffixCount > 0;
                }
                else if (totalAffixes == 0 && string.IsNullOrEmpty(analysis.ItemText) && analysis.DetectedAffixes.Count > 0)
                {
                    // For tests with no ItemText but DetectedAffixes, use the old method entirely
                    totalAffixes = analysis.PrefixCount + analysis.SuffixCount;
                    hasPrefix = analysis.PrefixCount > 0;
                    hasSuffix = analysis.SuffixCount > 0;
                }
                
                // **CRITICAL FIX: Handle full magic items (2 affixes)**
                if (totalAffixes >= 2)
                {
                    // Magic item is full (2 affixes), must use alteration to reroll
                    return CurrencyType.Alteration;
                }
                
                // Smart augmentation logic for 1-affix items
                if (totalAffixes == 1)
                {
                    // Determine what types of affixes the user wants
                    var targetAffixTypes = GetTargetAffixTypes(config);
                    
                    // Current affix types based on name analysis or fallback
                    var currentAffixTypes = new HashSet<AffixType>();
                    if (hasPrefix) currentAffixTypes.Add(AffixType.Prefix);
                    if (hasSuffix) currentAffixTypes.Add(AffixType.Suffix);
                    
                    // **ENHANCED LOGIC: Use augmentation if we want affix types that the item doesn't have**
                    // This covers both cases:
                    // 1. We want completely different types (old logic)
                    // 2. We want additional types beyond what the item has (new logic)
                    var missingTypes = new HashSet<AffixType>(targetAffixTypes);
                    missingTypes.ExceptWith(currentAffixTypes);
                    
                    if (missingTypes.Count > 0)
                    {
                        return CurrencyType.Augmentation; // Add the missing affix type(s)
                    }
                }
                
                // Default: Use alteration (no affixes, or want same type as current)
                return CurrencyType.Alteration;
            }

            // Default fallback
            return CurrencyType.Alteration;
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
            
            // **ENHANCED: Elemental Damage patterns**
            if (modifierText.Contains("Adds") && modifierText.Contains("Lightning Damage"))
                return "Added Lightning Damage";
            if (modifierText.Contains("Adds") && modifierText.Contains("Fire Damage"))
                return "Added Fire Damage";
            if (modifierText.Contains("Adds") && modifierText.Contains("Cold Damage"))
                return "Added Cold Damage";
            
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
            // **ENHANCED: Better accuracy detection**
            if (modifierText.Contains("to Accuracy Rating") || modifierText.Contains("Accuracy Rating"))
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

    // **NEW: Magic Item Name Analysis (More Reliable)**
    public static class MagicItemNameAnalyzer
    {
        // Magic item naming patterns:
        // Prefix only: "{Prefix} {BaseType}"
        // Suffix only: "{BaseType} of {Suffix}"  
        // Both: "{Prefix} {BaseType} of {Suffix}"
        
        public static (bool hasPrefix, bool hasSuffix) AnalyzeMagicItemName(string itemText)
        {
            if (string.IsNullOrEmpty(itemText))
                return (false, false);

            // Extract the item name line (after "Rarity: Magic")
            var lines = itemText.Split('\n');
            string itemName = "";
            
            bool foundRarityMagic = false;
            foreach (var line in lines)
            {
                if (line.Trim() == "Rarity: Magic")
                {
                    foundRarityMagic = true;
                    continue;
                }
                
                if (foundRarityMagic && !string.IsNullOrWhiteSpace(line.Trim()) && 
                    !line.Contains("--------"))
                {
                    itemName = line.Trim();
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(itemName))
                return (false, false);
            
            return AnalyzeItemNamePattern(itemName);
        }
        
        private static (bool hasPrefix, bool hasSuffix) AnalyzeItemNamePattern(string itemName)
        {
            // Check if item has " of " pattern (indicates suffix)
            bool hasSuffix = itemName.Contains(" of ");
            
            // Get list of known base types to check for prefix
            var knownBaseTypes = GetKnownBaseTypes();
            
            if (hasSuffix)
            {
                // Pattern: "{Prefix} {BaseType} of {Suffix}" or "{BaseType} of {Suffix}"
                var beforeOf = itemName.Split(" of ")[0];
                
                // Check if there's a prefix before the base type
                bool hasPrefix = false;
                foreach (var baseType in knownBaseTypes)
                {
                    if (beforeOf.EndsWith(baseType, StringComparison.OrdinalIgnoreCase))
                    {
                        // If there's text before the base type, it's a prefix
                        var potentialPrefix = beforeOf.Substring(0, beforeOf.Length - baseType.Length).Trim();
                        hasPrefix = !string.IsNullOrEmpty(potentialPrefix);
                        break;
                    }
                }
                
                return (hasPrefix, true);
            }
            else
            {
                // Pattern: "{Prefix} {BaseType}" or just "{BaseType}"
                // Check if any known base type is at the end
                foreach (var baseType in knownBaseTypes)
                {
                    if (itemName.EndsWith(baseType, StringComparison.OrdinalIgnoreCase))
                    {
                        // If there's text before the base type, it's a prefix
                        var potentialPrefix = itemName.Substring(0, itemName.Length - baseType.Length).Trim();
                        bool hasPrefix = !string.IsNullOrEmpty(potentialPrefix);
                        return (hasPrefix, false);
                    }
                }
                
                // If we can't identify the base type, assume it has a prefix if it's not just the base
                return (true, false);
            }
        }
        
        public static HashSet<string> GetKnownBaseTypes()
        {
            return new HashSet<string>
            {
                "Vaal Axe", "Siege Axe", "Poleaxe", "Labrys", "Karui Chopper",
                "Maraketh Two-Handed Sword", "Exquisite Blade", "Lion Sword",
                "Infernal Sword", "Ezomyte Blade", "Oro's Sacrifice",
                // Add more base types as needed...
                "Two-Handed Sword", "Two-Handed Axe", "Two-Handed Mace",
                "Staff", "Bow", "Claw", "Dagger", "One-Handed Sword", 
                "One-Handed Axe", "One-Handed Mace", "Sceptre", "Wand"
            };
        }
    }

    // **NEW: Magic Item Prefix/Suffix Name Database for Fast Crafting Success Detection**
    public static class MagicItemNameDatabase
    {
        // Maps magic item prefix names to their corresponding modifiers
        public static Dictionary<string, List<string>> PrefixNames = new Dictionary<string, List<string>>
        {
            // Physical Damage % Prefixes (T8-T1)
            {"Heavy", new List<string> {"Physical Damage %"}}, // T8
            {"Serrated", new List<string> {"Physical Damage %"}}, // T7
            {"Wicked", new List<string> {"Physical Damage %"}}, // T6
            {"Vicious", new List<string> {"Physical Damage %"}}, // T5
            {"Bloodthirsty", new List<string> {"Physical Damage %"}}, // T4
            {"Cruel", new List<string> {"Physical Damage %"}}, // T3
            {"Tyrannical", new List<string> {"Physical Damage %"}}, // T2
            {"Merciless", new List<string> {"Physical Damage %"}}, // T1
            
            // Flat Physical Damage Prefixes (T9-T1)
            {"Burnished", new List<string> {"Flat Physical Damage"}}, // T8
            {"Polished", new List<string> {"Flat Physical Damage"}}, // T7
            {"Honed", new List<string> {"Flat Physical Damage"}}, // T6
            {"Gleaming", new List<string> {"Flat Physical Damage"}}, // T5
            {"Annealed", new List<string> {"Flat Physical Damage"}}, // T4
            {"Razor-sharp", new List<string> {"Flat Physical Damage"}}, // T3 - MISSING!
            {"Tempered", new List<string> {"Flat Physical Damage"}}, // T2 - MISSING!
            {"Flaring", new List<string> {"Flat Physical Damage"}}, // T1
            
            // Elemental Damage Prefixes
            {"Burning", new List<string> {"Added Fire Damage"}},
            {"Scorching", new List<string> {"Added Fire Damage"}},
            {"Incinerating", new List<string> {"Added Fire Damage"}},
            {"Freezing", new List<string> {"Added Cold Damage"}},
            {"Frigid", new List<string> {"Added Cold Damage"}},
            {"Entombing", new List<string> {"Added Cold Damage"}},
            {"Sparking", new List<string> {"Added Lightning Damage"}},
            {"Glinting", new List<string> {"Added Lightning Damage"}}, // User's example
            {"Crackling", new List<string> {"Added Lightning Damage"}},
            {"Discharging", new List<string> {"Added Lightning Damage"}},
            
            // Hybrid Physical/Accuracy Prefixes (T8-T1)
            {"Squire's", new List<string> {"Hybrid Physical/Accuracy"}}, // T8
            {"Journeyman's", new List<string> {"Hybrid Physical/Accuracy"}}, // T7
            {"Reaver's", new List<string> {"Hybrid Physical/Accuracy"}}, // T6
            {"Mercenary's", new List<string> {"Hybrid Physical/Accuracy"}}, // T5
            {"Champion's", new List<string> {"Hybrid Physical/Accuracy"}}, // T4
            {"Conqueror's", new List<string> {"Hybrid Physical/Accuracy"}}, // T3
            {"Emperor's", new List<string> {"Hybrid Physical/Accuracy"}}, // T2 - MISSING!
            {"Dictator's", new List<string> {"Hybrid Physical/Accuracy"}}, // T1
            
            // Life/Defense Prefixes
            {"Healthy", new List<string> {"Life"}},
            {"Vigorous", new List<string> {"Life"}},
            {"Stalwart", new List<string> {"Armour"}},
            {"Fortified", new List<string> {"Armour"}},
            
            // Accuracy Prefixes
            {"Marksman's", new List<string> {"Accuracy Rating"}},
            {"Ranger's", new List<string> {"Accuracy Rating"}},
            {"Sniper's", new List<string> {"Accuracy Rating"}},
        };
        
        // Maps magic item suffix names to their corresponding modifiers
        public static Dictionary<string, List<string>> SuffixNames = new Dictionary<string, List<string>>
        {
            // Attack Speed Suffixes (T6-T1)
            {"of Celebration", new List<string> {"Attack Speed %"}}, // T1
            {"of Skill", new List<string> {"Attack Speed %"}}, // T2, User's example
            {"of Alacrity", new List<string> {"Attack Speed %"}}, // T3
            {"of Infamy", new List<string> {"Attack Speed %"}}, // T4
            {"of Fame", new List<string> {"Attack Speed %"}}, // T5
            {"of Renown", new List<string> {"Attack Speed %"}}, // T6
            
            // Critical Strike Chance Suffixes
            {"of Precision", new List<string> {"Critical Strike Chance %"}},
            {"of Technique", new List<string> {"Critical Strike Chance %"}},
            {"of Lethality", new List<string> {"Critical Strike Chance %"}},
            {"of Deadliness", new List<string> {"Critical Strike Chance %"}},
            
            // Critical Strike Multiplier Suffixes
            {"of Destruction", new List<string> {"Critical Strike Multiplier %"}},
            {"of Ruin", new List<string> {"Critical Strike Multiplier %"}},
            {"of Devastation", new List<string> {"Critical Strike Multiplier %"}},
            {"of Annihilation", new List<string> {"Critical Strike Multiplier %"}},
            
            // Accuracy Rating Suffixes
            {"of Accuracy", new List<string> {"Accuracy Rating"}},
            {"of the Marksman", new List<string> {"Accuracy Rating"}},
            {"of the Sniper", new List<string> {"Accuracy Rating"}},
            {"of the Archer", new List<string> {"Accuracy Rating"}},
            
            // Resistance Suffixes
            {"of the Salamander", new List<string> {"Fire Resistance"}},
            {"of the Volcano", new List<string> {"Fire Resistance"}},
            {"of the Yeti", new List<string> {"Cold Resistance"}},
            {"of the Glacier", new List<string> {"Cold Resistance"}},
            {"of the Thunderbird", new List<string> {"Lightning Resistance"}},
            {"of the Storm", new List<string> {"Lightning Resistance"}},
            
            // Life/Defense Suffixes
            {"of Life", new List<string> {"Life"}},
            {"of Vitality", new List<string> {"Life"}},
            {"of the Ox", new List<string> {"Life"}},
            {"of the Tortoise", new List<string> {"Armour"}},
            {"of the Rhino", new List<string> {"Armour"}},
            
            // Mana Suffixes
            {"of Mana", new List<string> {"Mana"}},
            {"of the Mind", new List<string> {"Mana"}},
            {"of Clarity", new List<string> {"Mana"}},
        };
        
        public static bool CheckMagicItemSuccess(string itemText, CraftingConfiguration config)
        {
            var (hasPrefix, hasSuffix) = MagicItemNameAnalyzer.AnalyzeMagicItemName(itemText);
            
            if (!hasPrefix && !hasSuffix)
                return false; // No affixes
                
            // Extract the actual prefix and suffix names
            var (prefixName, suffixName) = ExtractPrefixSuffixNames(itemText);
            
            // Get modifiers from names
            var itemModifiers = new List<string>();
            var foundNames = new List<string>();
            
            if (!string.IsNullOrEmpty(prefixName) && PrefixNames.ContainsKey(prefixName))
            {
                itemModifiers.AddRange(PrefixNames[prefixName]);
                foundNames.Add($"prefix '{prefixName}'");
            }
            
            if (!string.IsNullOrEmpty(suffixName) && SuffixNames.ContainsKey(suffixName))
            {
                itemModifiers.AddRange(SuffixNames[suffixName]);
                foundNames.Add($"suffix '{suffixName}'");
            }
            
            // Check if any of the item's modifiers match what we want
            bool result = CheckModifierMatch(itemModifiers, config, foundNames, prefixName, suffixName);
            return result;
        }
        
        public static (string prefixName, string suffixName) ExtractPrefixSuffixNames(string itemText)
        {
            // Extract the item name line
            var lines = itemText.Split('\n');
            string itemName = "";
            
            bool foundRarityMagic = false;
            foreach (var line in lines)
            {
                if (line.Trim() == "Rarity: Magic")
                {
                    foundRarityMagic = true;
                    continue;
                }
                
                if (foundRarityMagic && !string.IsNullOrWhiteSpace(line.Trim()) && 
                    !line.Contains("--------"))
                {
                    itemName = line.Trim();
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(itemName))
                return ("", "");
                
            // Parse the name pattern
            string prefixName = "";
            string suffixName = "";
            
            if (itemName.Contains(" of "))
            {
                var parts = itemName.Split(" of ");
                suffixName = "of " + parts[1];
                
                var beforeOf = parts[0];
                // Check for prefix before base type
                foreach (var baseType in MagicItemNameAnalyzer.GetKnownBaseTypes())
                {
                    if (beforeOf.EndsWith(baseType, StringComparison.OrdinalIgnoreCase))
                    {
                        var potentialPrefix = beforeOf.Substring(0, beforeOf.Length - baseType.Length).Trim();
                        if (!string.IsNullOrEmpty(potentialPrefix))
                        {
                            prefixName = potentialPrefix;
                        }
                        break;
                    }
                }
            }
            else
            {
                // Only prefix, no suffix
                foreach (var baseType in MagicItemNameAnalyzer.GetKnownBaseTypes())
                {
                    if (itemName.EndsWith(baseType, StringComparison.OrdinalIgnoreCase))
                    {
                        var potentialPrefix = itemName.Substring(0, itemName.Length - baseType.Length).Trim();
                        if (!string.IsNullOrEmpty(potentialPrefix))
                        {
                            prefixName = potentialPrefix;
                        }
                        break;
                    }
                }
            }
            
            return (prefixName, suffixName);
        }
        
        private static bool CheckModifierMatch(List<string> itemModifiers, CraftingConfiguration config, List<string> foundNames, string prefixName, string suffixName)
        {
            if (itemModifiers.Count == 0)
            {
                LogHelper.Print("❌ Name-based detection: No recognized modifiers from item names", System.Drawing.Color.Orange);
                return false;
            }
                
            // Check against target modifiers using the same OR/AND logic
            foreach (var group in config.ModifierGroups.Where(g => g.IsEnabled))
            {
                foreach (var targetModifier in group.SelectedModifiers)
                {
                    foreach (var itemModifier in itemModifiers)
                    {
                        if (itemModifier.Contains(targetModifier) || targetModifier.Contains(itemModifier))
                        {
                            // Enhanced logging for successful match
                            var nameInfo = foundNames.Count > 0 ? string.Join(" and ", foundNames) : "item name";
                            LogHelper.Print($"🎯 MATCH SUCCESS: Found {nameInfo} - this matches target modifier '{targetModifier}'!", System.Drawing.Color.Lime);
                            
                            if (!string.IsNullOrEmpty(prefixName))
                                LogHelper.Print($"   ✅ Prefix: \"{prefixName}\" → {itemModifier}", System.Drawing.Color.Green);
                            if (!string.IsNullOrEmpty(suffixName))
                                LogHelper.Print($"   ✅ Suffix: \"{suffixName}\" → {itemModifier}", System.Drawing.Color.Green);
                                
                            return true; // Found a match
                        }
                    }
                }
            }
            
            // Log when no match found
            var availableModifiers = string.Join(", ", itemModifiers);
            var targetModifiers = string.Join(", ", config.ModifierGroups.Where(g => g.IsEnabled).SelectMany(g => g.SelectedModifiers));
            LogHelper.Print($"❌ Name-based detection failed: Item has [{availableModifiers}] but need [{targetModifiers}]", System.Drawing.Color.Orange);
            
            return false;
        }
    }

    public static class LogHelper
    {
        private static ProgressBar mainForm = null;
        
        public static void Initialize(ProgressBar form)
        {
            mainForm = form;
        }
        
        public static void Print(string message, Color color)
        {
            // Try to access the main form's print method
            if (mainForm != null)
            {
                try
                {
                    mainForm.print(message, color);
                }
                catch
                {
                    // Fallback to console if form access fails
                    System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
                }
            }
            else
            {
                // No form available, fallback to console
                System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }
    }
}