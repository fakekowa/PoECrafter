using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace WindowsFormsApplication3
{
    /// <summary>
    /// Comprehensive Settings Manager for Priority 2: Enhanced Settings Persistence
    /// Handles all application settings including modifier rules, UI state, and preferences
    /// </summary>
    public static class SettingsManager
    {
        #region Enhanced Modifier Rule Management

        /// <summary>
        /// Saves modifier selections organized by item type (e.g., TwoHandedAxes, OneHandedSwords)
        /// </summary>
        public static void SaveModifierSelectionsByItemType(string itemType, List<string> selectedModifiers)
        {
            try
            {
                var existingSelections = LoadModifierSelectionsByItemType();
                existingSelections[itemType] = selectedModifiers;
                
                string json = JsonConvert.SerializeObject(existingSelections, Formatting.Indented);
                Properties.Settings.Default.SelectedModifiersByItemType = json;
                Properties.Settings.Default.Save();
                
                if (Properties.Settings.Default.AutoSaveModifierChanges)
                {
                    Console.WriteLine($"💾 Auto-saved {selectedModifiers.Count} modifiers for {itemType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error saving modifier selections: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads modifier selections for all item types
        /// </summary>
        public static Dictionary<string, List<string>> LoadModifierSelectionsByItemType()
        {
            try
            {
                string json = Properties.Settings.Default.SelectedModifiersByItemType;
                if (string.IsNullOrEmpty(json))
                    return new Dictionary<string, List<string>>();
                
                return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json) 
                       ?? new Dictionary<string, List<string>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading modifier selections: {ex.Message}");
                return new Dictionary<string, List<string>>();
            }
        }

        /// <summary>
        /// Gets modifier selections for a specific item type
        /// </summary>
        public static List<string> GetModifierSelectionsForItemType(string itemType)
        {
            var allSelections = LoadModifierSelectionsByItemType();
            return allSelections.ContainsKey(itemType) ? allSelections[itemType] : new List<string>();
        }

        /// <summary>
        /// Saves a named rule set with modifier selections and logic preferences
        /// </summary>
        public static void SaveModifierRuleSet(string ruleSetName, ModifierRuleSet ruleSet)
        {
            try
            {
                var existingRuleSets = LoadModifierRuleSets();
                existingRuleSets[ruleSetName] = ruleSet;
                
                string json = JsonConvert.SerializeObject(existingRuleSets, Formatting.Indented);
                Properties.Settings.Default.ModifierRuleSets = json;
                Properties.Settings.Default.LastUsedRuleSet = ruleSetName;
                Properties.Settings.Default.Save();
                
                Console.WriteLine($"💾 Saved rule set: {ruleSetName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error saving rule set: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all saved modifier rule sets
        /// </summary>
        public static Dictionary<string, ModifierRuleSet> LoadModifierRuleSets()
        {
            try
            {
                string json = Properties.Settings.Default.ModifierRuleSets;
                if (string.IsNullOrEmpty(json))
                    return new Dictionary<string, ModifierRuleSet>();
                
                return JsonConvert.DeserializeObject<Dictionary<string, ModifierRuleSet>>(json) 
                       ?? new Dictionary<string, ModifierRuleSet>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading rule sets: {ex.Message}");
                return new Dictionary<string, ModifierRuleSet>();
            }
        }

        #endregion

        #region UI State Management

        /// <summary>
        /// Saves comprehensive window state including log window
        /// </summary>
        public static void SaveWindowState(Form mainForm, Form logWindow = null)
        {
            try
            {
                // Save main window state
                Properties.Settings.Default.WindowLocation = mainForm.Location;
                Properties.Settings.Default.WindowSize = mainForm.Size;
                Properties.Settings.Default.WindowState = mainForm.WindowState.ToString();
                
                // Save log window state if provided
                if (logWindow != null)
                {
                    Properties.Settings.Default.LogWindowVisible = logWindow.Visible;
                    Properties.Settings.Default.LogWindowLocation = logWindow.Location;
                    Properties.Settings.Default.LogWindowSize = logWindow.Size;
                }
                
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error saving window state: {ex.Message}");
            }
        }

        /// <summary>
        /// Restores comprehensive window state with validation
        /// </summary>
        public static void RestoreWindowState(Form mainForm, Form logWindow = null)
        {
            try
            {
                if (!Properties.Settings.Default.RememberWindowPositions)
                    return;

                // Restore main window with bounds checking
                var savedLocation = Properties.Settings.Default.WindowLocation;
                var savedSize = Properties.Settings.Default.WindowSize;
                
                if (IsLocationValid(savedLocation))
                {
                    mainForm.Location = savedLocation;
                }
                else
                {
                    mainForm.StartPosition = FormStartPosition.CenterScreen;
                }

                if (savedSize.Width > 400 && savedSize.Height > 300)
                {
                    mainForm.Size = savedSize;
                }

                // Restore window state
                if (Enum.TryParse<FormWindowState>(Properties.Settings.Default.WindowState, out FormWindowState state))
                {
                    mainForm.WindowState = state;
                }

                // Restore log window if provided
                if (logWindow != null)
                {
                    logWindow.Visible = Properties.Settings.Default.LogWindowVisible;
                    var logLocation = Properties.Settings.Default.LogWindowLocation;
                    var logSize = Properties.Settings.Default.LogWindowSize;
                    
                    if (IsLocationValid(logLocation))
                    {
                        logWindow.Location = logLocation;
                    }
                    
                    if (logSize.Width > 200 && logSize.Height > 200)
                    {
                        logWindow.Size = logSize;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error restoring window state: {ex.Message}");
            }
        }

        private static bool IsLocationValid(Point location)
        {
            return location.X >= -100 && location.Y >= -100 &&
                   location.X < Screen.PrimaryScreen.WorkingArea.Width &&
                   location.Y < Screen.PrimaryScreen.WorkingArea.Height;
        }

        #endregion

        #region Crafting Configuration Management

        /// <summary>
        /// Saves comprehensive crafting configuration
        /// </summary>
        public static void SaveCraftingConfiguration(CraftingConfigData config)
        {
            try
            {
                Properties.Settings.Default.UseORLogic = config.UseORLogic;
                Properties.Settings.Default.SmartAugmentation = config.SmartAugmentation;
                Properties.Settings.Default.DefaultCraftingStrategy = config.DefaultStrategy;
                Properties.Settings.Default.MaxChaosToUse = config.MaxChaosToUse;
                Properties.Settings.Default.MaxAlterationToUse = config.MaxAlterationToUse;
                Properties.Settings.Default.MaxAugmentationToUse = config.MaxAugmentationToUse;
                Properties.Settings.Default.AutoStopOnSuccess = config.AutoStopOnSuccess;
                Properties.Settings.Default.PlaySoundOnSuccess = config.PlaySoundOnSuccess;
                Properties.Settings.Default.SafetyDelayMS = config.SafetyDelayMS;
                Properties.Settings.Default.MaxCraftingIterations = config.MaxCraftingIterations;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error saving crafting configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads comprehensive crafting configuration
        /// </summary>
        public static CraftingConfigData LoadCraftingConfiguration()
        {
            try
            {
                return new CraftingConfigData
                {
                    UseORLogic = Properties.Settings.Default.UseORLogic,
                    SmartAugmentation = Properties.Settings.Default.SmartAugmentation,
                    DefaultStrategy = Properties.Settings.Default.DefaultCraftingStrategy,
                    MaxChaosToUse = Properties.Settings.Default.MaxChaosToUse,
                    MaxAlterationToUse = Properties.Settings.Default.MaxAlterationToUse,
                    MaxAugmentationToUse = Properties.Settings.Default.MaxAugmentationToUse,
                    AutoStopOnSuccess = Properties.Settings.Default.AutoStopOnSuccess,
                    PlaySoundOnSuccess = Properties.Settings.Default.PlaySoundOnSuccess,
                    SafetyDelayMS = Properties.Settings.Default.SafetyDelayMS,
                    MaxCraftingIterations = Properties.Settings.Default.MaxCraftingIterations
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading crafting configuration: {ex.Message}");
                return new CraftingConfigData(); // Return defaults
            }
        }

        #endregion

        #region Session Management

        /// <summary>
        /// Saves current crafting session data
        /// </summary>
        public static void SaveCraftingSession(CraftingSessionData session)
        {
            try
            {
                string json = JsonConvert.SerializeObject(session, Formatting.Indented);
                Properties.Settings.Default.LastCraftingSession = json;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error saving crafting session: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads last crafting session data
        /// </summary>
        public static CraftingSessionData LoadCraftingSession()
        {
            try
            {
                string json = Properties.Settings.Default.LastCraftingSession;
                if (string.IsNullOrEmpty(json))
                    return new CraftingSessionData();
                
                return JsonConvert.DeserializeObject<CraftingSessionData>(json) ?? new CraftingSessionData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading crafting session: {ex.Message}");
                return new CraftingSessionData();
            }
        }

        #endregion

        #region Import/Export Functionality

        /// <summary>
        /// Exports all settings to a JSON file for backup/sharing
        /// </summary>
        public static bool ExportSettings(string filePath)
        {
            try
            {
                var exportData = new SettingsExportData
                {
                    ModifierSelections = LoadModifierSelectionsByItemType(),
                    ModifierRuleSets = LoadModifierRuleSets(),
                    CraftingConfig = LoadCraftingConfiguration(),
                    CraftingSession = LoadCraftingSession(),
                    ExportDate = DateTime.Now,
                    Version = "1.0.0"
                };

                string json = JsonConvert.SerializeObject(exportData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                
                Console.WriteLine($"💾 Settings exported to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error exporting settings: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Imports settings from a JSON file
        /// </summary>
        public static bool ImportSettings(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("⚠️ Import file not found");
                    return false;
                }

                string json = File.ReadAllText(filePath);
                var importData = JsonConvert.DeserializeObject<SettingsExportData>(json);
                
                if (importData == null)
                {
                    Console.WriteLine("⚠️ Invalid import file format");
                    return false;
                }

                // Import modifier selections
                if (importData.ModifierSelections != null)
                {
                    string selectionsJson = JsonConvert.SerializeObject(importData.ModifierSelections);
                    Properties.Settings.Default.SelectedModifiersByItemType = selectionsJson;
                }

                // Import rule sets
                if (importData.ModifierRuleSets != null)
                {
                    string ruleSetsJson = JsonConvert.SerializeObject(importData.ModifierRuleSets);
                    Properties.Settings.Default.ModifierRuleSets = ruleSetsJson;
                }

                // Import crafting configuration
                if (importData.CraftingConfig != null)
                {
                    SaveCraftingConfiguration(importData.CraftingConfig);
                }

                Properties.Settings.Default.Save();
                Console.WriteLine($"✅ Settings imported from: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error importing settings: {ex.Message}");
                return false;
            }
        }

        #endregion
    }

    #region Data Classes

    public class ModifierRuleSet
    {
        public string Name { get; set; } = "";
        public string ItemType { get; set; } = "";
        public List<string> SelectedModifiers { get; set; } = new List<string>();
        public bool UseORLogic { get; set; } = false;
        public bool SmartAugmentation { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = "";
    }

    public class CraftingConfigData
    {
        public bool UseORLogic { get; set; } = false;
        public bool SmartAugmentation { get; set; } = false;
        public string DefaultStrategy { get; set; } = "SmartAugmentation";
        public int MaxChaosToUse { get; set; } = 50;
        public int MaxAlterationToUse { get; set; } = 100;
        public int MaxAugmentationToUse { get; set; } = 50;
        public bool AutoStopOnSuccess { get; set; } = true;
        public bool PlaySoundOnSuccess { get; set; } = true;
        public int SafetyDelayMS { get; set; } = 100;
        public int MaxCraftingIterations { get; set; } = 500;
    }

    public class CraftingSessionData
    {
        public DateTime SessionStart { get; set; } = DateTime.Now;
        public string LastItemType { get; set; } = "TwoHandedAxes";
        public string LastRuleSet { get; set; } = "";
        public int TotalCurrencyUsed { get; set; } = 0;
        public int SuccessfulCrafts { get; set; } = 0;
        public List<string> CraftingLog { get; set; } = new List<string>();
    }

    public class SettingsExportData
    {
        public Dictionary<string, List<string>> ModifierSelections { get; set; }
        public Dictionary<string, ModifierRuleSet> ModifierRuleSets { get; set; }
        public CraftingConfigData CraftingConfig { get; set; }
        public CraftingSessionData CraftingSession { get; set; }
        public DateTime ExportDate { get; set; }
        public string Version { get; set; } = "1.0.0";
    }

    #endregion
} 