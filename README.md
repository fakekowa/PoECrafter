# 🏹 PoECrafter - Automated Path of Exile Item Crafting

[![Version](https://img.shields.io/badge/Version-2.0.0-brightgreen.svg)](https://github.com/your-repo/PoECrafter)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://github.com/your-repo/PoECrafter)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> **⚠️ IMPORTANT**: Always use PoECrafter responsibly and in accordance with Path of Exile's Terms of Service. This tool is designed to assist with item crafting automation - use at your own discretion.

## 🎮 What is PoECrafter?

PoECrafter is an advanced Windows automation tool for Path of Exile that helps you craft items more efficiently. It automatically applies crafting currencies (Alteration Orbs, Augmentation Orbs, Chaos Orbs) until your item meets specific modifier requirements.

### 🌟 Key Features

- **🎯 Smart Modifier Detection**: Automatically detects Physical Damage %, Attack Speed, Critical Strike modifiers, and more
- **⚡ Speed Crafting**: SHIFT-holding automation for 50-70% faster crafting
- **🧠 Intelligent Currency Selection**: Automatically chooses the best currency based on item state
- **🔄 Hybrid Detection**: Correctly distinguishes between true hybrid modifiers and separate prefix+suffix combinations
- **🛡️ Safety Features**: Emergency stop (F12), currency limits, pre-crafting validation
- **📊 Real-time Analysis**: Live item analysis showing detected modifiers and recommendations

---

## 🚀 Quick Start Guide

### 📋 Prerequisites

- **Windows OS** (Windows 10/11 recommended)
- **Path of Exile** (Steam or standalone client)
- **Administrator privileges** (required for automation)

### 📦 Installation

1. **Download the latest release** from the [Releases page](releases)
2. **Extract the ZIP file** to your desired location (e.g., `C:\PoECrafter\`)
3. **Run PoECrafter.exe as Administrator** (right-click → "Run as administrator")

> 💡 **Tip**: Create a desktop shortcut for easy access!

---

## ⚙️ Setup Guide

### 🎯 Step 1: Configure Currency Locations

Before you can start crafting, you need to tell PoECrafter where your currencies are located in your inventory.

1. **Open Path of Exile** and navigate to your stash or inventory
2. **In PoECrafter**, click the **"Locations"** button
3. **For each currency type** (Alteration, Augmentation, Chaos, Crafting Mat):
   - Click the **"Select"** button next to the currency
   - **Move your mouse** to the currency in your PoE inventory/stash
   - **Click** to set the position
   - The coordinates will be saved automatically

#### 🎯 Currency Setup Tips:
- **Alteration Orbs**: Used for rerolling magic items
- **Augmentation Orbs**: Used to add modifiers to magic items with 1 affix
- **Chaos Orbs**: Used for rerolling rare items
- **Crafting Mat**: The item you want to craft (put it in a consistent location)

### 🎯 Step 2: Select Your Modifiers

1. **Copy your item** to clipboard (hover over item in PoE and press **Ctrl+C**)
2. **In PoECrafter**, go to the **"Two Handed Axes"** tab (currently supports 2H axes only)
3. **Select the modifiers you want**:
   - **Physical Damage %**: Select your desired tier (T1 = best, T10 = worst)
   - **Flat Physical Damage**: Additional flat damage rolls  
   - **Attack Speed**: Faster attack speed percentage
   - **Critical Strike Chance**: Increased critical strike chance
   - **Critical Strike Multiplier**: Higher critical damage
   - **Hybrid Physical/Accuracy**: Combined physical damage + accuracy modifier

#### 🎯 Modifier Selection Tips:
- **Hierarchical Selection**: Selecting T3 automatically includes T1 and T2 (better tiers)
- **Smart Targeting**: The system knows what modifiers are achievable based on your item
- **Real-time Validation**: See immediately if your item already meets requirements

---

## 🎮 How to Use PoECrafter

### 🚀 Basic Crafting Workflow

1. **Prepare Your Setup**:
   - Have your base item ready (white/magic/rare two-handed axe)
   - Ensure you have sufficient currency
   - Set up currency locations (see Setup Guide above)

2. **Configure Your Craft**:
   - Copy your item to clipboard (**Ctrl+C** in PoE)
   - Select desired modifiers in PoECrafter
   - Choose advanced options if needed (see below)

3. **Start Crafting**:
   - Click **"Start Colors"** button
   - PoECrafter will automatically:
     - Check if your item already meets requirements
     - Apply appropriate currencies (Alt/Aug/Chaos)
     - Stop when target modifiers are achieved

4. **Emergency Stop**:
   - Press **F12** at any time to stop crafting immediately
   - Essential for safety if something goes wrong

### ⚡ Advanced Features

#### 🧠 Smart Augmentation
Enable **"Smart Augmentation"** for intelligent currency usage:
- **1 Prefix + Want Suffix**: Uses Augmentation to add suffix
- **1 Suffix + Want Prefix**: Uses Augmentation to add prefix  
- **Same Affix Type**: Uses Alteration to reroll
- **2 Affixes**: Uses Alteration to reroll
- **Rare Items**: Always uses Chaos Orbs

#### 🔄 OR Logic
Enable **"Use OR Logic (Any Match)"** for flexible targeting:
- **Without OR Logic**: Must get ALL selected modifiers
- **With OR Logic**: Stops when ANY selected modifier group is achieved
- Perfect for: "Get T1-T3 Physical Damage % OR T1-T3 Hybrid Physical/Accuracy"

#### ⚡ Speed Crafting
Enable **"Speed Crafting"** for maximum performance:
- Uses SHIFT-holding automation (native PoE mechanic)
- **50-70% faster** than standard crafting
- Eliminates back-and-forth mouse movement
- Compatible with all other features

---

## 🔧 Settings & Configuration

### 🛡️ Safety Settings

- **Max Currency Usage**: Set limits to prevent overspending
  - Max Alteration Orbs to use
  - Max Augmentation Orbs to use  
  - Max Chaos Orbs to use
- **Auto-stop on Success**: Automatically stops when target is achieved
- **Emergency Stop**: F12 hotkey for immediate termination

### 📊 Analysis & Logging

- **Real-time Item Analysis**: Shows detected modifiers and their tiers
- **Detailed Logging**: View crafting progress with emoji indicators
- **Log Window**: Separate window for detailed crafting feedback
- **Progress Tracking**: See how many currencies have been used

### ⚙️ Automation Settings

- **Delay Controls**: Adjust automation speed (faster = more efficient, but riskier)
- **Location Memory**: Saved currency positions persist between sessions
- **Pre-crafting Validation**: Checks if item already meets requirements

---

## ❗ Important Notes & Best Practices

### 🛡️ Safety First

1. **Start with Cheap Items**: Test PoECrafter on low-value items first
2. **Set Currency Limits**: Always set reasonable maximum currency usage
3. **Monitor Progress**: Keep an eye on the crafting process
4. **Use Emergency Stop**: Don't hesitate to press F12 if needed
5. **Administrator Rights**: Required for mouse/keyboard automation

### 🎯 Optimization Tips

1. **Item Preparation**:
   - Start with white items for maximum control
   - Use transmutation orbs to make items magic before using PoECrafter
   - Consider item level requirements for your target modifiers

2. **Currency Management**:
   - Keep currencies in consistent inventory locations
   - Have sufficient stock before starting long crafting sessions
   - Consider currency ratios (Alts are most commonly used)

3. **Modifier Selection**:
   - Understand that higher tiers (T1, T2, T3) are much rarer
   - Use OR logic for more flexible targeting
   - Enable Smart Augmentation for efficient magic item crafting

### 📊 Understanding Modifier Tiers

- **T1**: Best possible rolls (rarest)
- **T2-T3**: High-tier rolls (rare) 
- **T4-T6**: Mid-tier rolls (common)
- **T7-T10**: Lower-tier rolls (very common)

**Example**: Physical Damage % tiers
- T1: Merciless (180-209%)
- T2: Tyrannical (155-179%)  
- T3: Cruel (130-154%)
- T6: Wicked (65-84%)

---

## 🐛 Troubleshooting

### Common Issues

1. **PoECrafter doesn't respond**:
   - Ensure you're running as Administrator
   - Check that Path of Exile window is active
   - Verify currency locations are set correctly

2. **Currency locations not working**:
   - Re-set currency locations using "Select" buttons
   - Make sure PoE is in windowed or windowed fullscreen mode
   - Check that your inventory layout hasn't changed

3. **Modifier detection not working**:
   - Copy item text properly with Ctrl+C in PoE
   - Ensure item is a supported type (currently: two-handed axes)
   - Check that modifier names match exactly

4. **Speed Crafting issues**:
   - Disable Speed Crafting if experiencing problems
   - Increase delay settings for more stability
   - Ensure adequate system performance

### 📧 Getting Help

- **Check the log window** for error messages
- **Try running as Administrator** if automation fails
- **Reset currency locations** if positions seem off
- **Start with default settings** and adjust gradually

---

## 📋 System Requirements

- **OS**: Windows 10/11 (64-bit)
- **RAM**: 4GB minimum, 8GB recommended
- **Disk Space**: 200MB for application
- **Graphics**: Any DirectX 11 compatible
- **Internet**: Not required for operation

---

## 🎯 Supported Items & Modifiers

### Currently Supported

- **Item Types**: Two-Handed Axes
- **Modifiers**:
  - Physical Damage % (Tiers 1-10)
  - Flat Physical Damage (Tiers 1-10)
  - Attack Speed (Tiers 1-10)
  - Critical Strike Chance (Tiers 1-10)
  - Critical Strike Multiplier (Tiers 1-10)
  - Hybrid Physical/Accuracy (Tiers 1-8)

### Coming Soon

- One-handed weapons
- Armor pieces
- Jewelry (rings, amulets)
- Additional modifier types
- Fossil crafting support

---

## 📄 Version History

### v2.0.0 - Current Release
- ✅ Fixed critical hybrid detection bug
- ✅ Added comprehensive unit test suite (18 tests)
- ✅ Implemented Speed Crafting with SHIFT-holding automation
- ✅ Enhanced Smart Augmentation logic
- ✅ Added OR Logic for flexible targeting
- ✅ Improved pre-crafting validation
- ✅ Enhanced logging with real-time analysis

### Previous Versions
- See [CHANGELOG.md](CHANGELOG.md) for full version history

---

## ⚖️ Legal & Disclaimer

**USE AT YOUR OWN RISK**: This tool automates mouse and keyboard actions in Path of Exile. While designed to be safe and efficient:

- Always review Path of Exile's Terms of Service
- Start with low-value items to test functionality
- Monitor automation processes actively
- Use reasonable currency limits
- The developers are not responsible for any in-game losses

**This tool is provided as-is without warranty. Use responsibly and at your own discretion.**

---

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### 🐛 Bug Reports
- Use the [Issues](issues) page to report bugs
- Include your item text and selected modifiers
- Describe the expected vs actual behavior

### 💡 Feature Requests
- Suggest new item types or modifiers
- Propose UI/UX improvements
- Share crafting strategy ideas

---

## 📧 Support

- **Documentation**: This README and in-app help
- **Issues**: [GitHub Issues](issues)
- **Discussions**: [GitHub Discussions](discussions)

---

*Happy Crafting, Exile! 🏹*
