using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WindowsFormsApplication3
{
    public class CurrencyTemplate
    {
        public string Name { get; set; }
        public Point GridPosition { get; set; }  // Grid coordinates in stash (row, col)
        public Color DominantColor { get; set; }
        public Color SecondaryColor { get; set; }
        public double MatchThreshold { get; set; }
        public Rectangle TemplateRegion { get; set; }  // Region in reference screenshot
        public string Description { get; set; }
    }

    public static class CurrencyTemplates
    {
        /// <summary>
        /// Currency templates based on premium stash tab layout from user screenshot
        /// Grid positions are 0-indexed (row, column)
        /// </summary>
        public static readonly List<CurrencyTemplate> Templates = new List<CurrencyTemplate>
        {
            // Top row currencies (row 0)
            new CurrencyTemplate
            {
                Name = "Transmutation",
                GridPosition = new Point(0, 0),
                DominantColor = Color.FromArgb(200, 200, 210), // Silver-white
                SecondaryColor = Color.FromArgb(150, 150, 160),
                MatchThreshold = 0.75,
                Description = "Orb of Transmutation - Silver/white orb"
            },
            new CurrencyTemplate
            {
                Name = "Alteration",
                GridPosition = new Point(0, 1),
                DominantColor = Color.FromArgb(100, 150, 220), // Blue
                SecondaryColor = Color.FromArgb(80, 120, 180),
                MatchThreshold = 0.75,
                Description = "Orb of Alteration - Blue orb"
            },
            new CurrencyTemplate
            {
                Name = "Annulment",
                GridPosition = new Point(0, 2),
                DominantColor = Color.FromArgb(120, 160, 200), // Light blue
                SecondaryColor = Color.FromArgb(90, 130, 170),
                MatchThreshold = 0.75,
                Description = "Orb of Annulment - Light blue with dark pattern"
            },
            new CurrencyTemplate
            {
                Name = "Chance",
                GridPosition = new Point(0, 3),
                DominantColor = Color.FromArgb(220, 180, 100), // Golden
                SecondaryColor = Color.FromArgb(180, 140, 60),
                MatchThreshold = 0.75,
                Description = "Orb of Chance - Golden orb"
            },
            new CurrencyTemplate
            {
                Name = "Augmentation",
                GridPosition = new Point(0, 4),
                DominantColor = Color.FromArgb(150, 100, 200), // Purple
                SecondaryColor = Color.FromArgb(120, 80, 160),
                MatchThreshold = 0.75,
                Description = "Orb of Augmentation - Purple orb"
            },
            new CurrencyTemplate
            {
                Name = "Exalted",
                GridPosition = new Point(0, 5),
                DominantColor = Color.FromArgb(255, 220, 120), // Bright golden
                SecondaryColor = Color.FromArgb(220, 180, 80),
                MatchThreshold = 0.8, // Higher threshold for valuable currency
                Description = "Exalted Orb - Bright golden orb"
            },
            new CurrencyTemplate
            {
                Name = "Regal",
                GridPosition = new Point(0, 6),
                DominantColor = Color.FromArgb(100, 180, 160), // Teal/blue-green
                SecondaryColor = Color.FromArgb(80, 140, 120),
                MatchThreshold = 0.75,
                Description = "Regal Orb - Teal/blue-green orb"
            },
            new CurrencyTemplate
            {
                Name = "Alchemy",
                GridPosition = new Point(0, 7),
                DominantColor = Color.FromArgb(200, 160, 100), // Golden-brown
                SecondaryColor = Color.FromArgb(160, 120, 60),
                MatchThreshold = 0.75,
                Description = "Orb of Alchemy - Golden-brown orb"
            },
            new CurrencyTemplate
            {
                Name = "Chaos",
                GridPosition = new Point(0, 8),
                DominantColor = Color.FromArgb(80, 60, 40), // Dark with golden swirls
                SecondaryColor = Color.FromArgb(180, 140, 60),
                MatchThreshold = 0.8, // Higher threshold for main crafting currency
                Description = "Chaos Orb - Dark orb with golden swirls"
            },
            new CurrencyTemplate
            {
                Name = "Blessed",
                GridPosition = new Point(0, 9),
                DominantColor = Color.FromArgb(180, 160, 100), // Golden
                SecondaryColor = Color.FromArgb(140, 120, 60),
                MatchThreshold = 0.75,
                Description = "Blessed Orb - Golden orb"
            },
            
            // Second row currencies (row 1)
            new CurrencyTemplate
            {
                Name = "Divine",
                GridPosition = new Point(1, 9), // Far right, second row
                DominantColor = Color.FromArgb(220, 200, 120), // Bright golden
                SecondaryColor = Color.FromArgb(180, 160, 80),
                MatchThreshold = 0.8, // Higher threshold for valuable currency
                Description = "Divine Orb - Bright golden orb"
            }
        };

        /// <summary>
        /// Standard PoE stash tab grid dimensions
        /// </summary>
        public static readonly int GRID_SLOT_SIZE = 47; // Pixels per inventory slot
        public static readonly int GRID_SPACING = 1;    // Spacing between slots
        public static readonly int GRID_COLS = 12;      // Columns in stash tab
        public static readonly int GRID_ROWS = 12;      // Rows in stash tab

        /// <summary>
        /// Get currency template by name
        /// </summary>
        public static CurrencyTemplate GetTemplate(string currencyName)
        {
            return Templates.FirstOrDefault(t => 
                string.Equals(t.Name, currencyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all currencies for a specific crafting type
        /// </summary>
        public static List<CurrencyTemplate> GetCraftingCurrencies()
        {
            return Templates.Where(t => 
                t.Name == "Chaos" || 
                t.Name == "Alteration" || 
                t.Name == "Augmentation" ||
                t.Name == "Regal" ||
                t.Name == "Exalted").ToList();
        }

        /// <summary>
        /// Convert grid position to screen coordinates
        /// </summary>
        public static Point GridToScreen(Point gridPos, Rectangle stashBounds)
        {
            int slotSize = GRID_SLOT_SIZE + GRID_SPACING;
            int x = stashBounds.X + (gridPos.Y * slotSize) + (GRID_SLOT_SIZE / 2);
            int y = stashBounds.Y + (gridPos.X * slotSize) + (GRID_SLOT_SIZE / 2);
            return new Point(x, y);
        }

        /// <summary>
        /// Convert screen coordinates to grid position
        /// </summary>
        public static Point ScreenToGrid(Point screenPos, Rectangle stashBounds)
        {
            int slotSize = GRID_SLOT_SIZE + GRID_SPACING;
            int col = (screenPos.X - stashBounds.X) / slotSize;
            int row = (screenPos.Y - stashBounds.Y) / slotSize;
            return new Point(row, col);
        }

        /// <summary>
        /// Detect if colors match within tolerance
        /// </summary>
        public static bool ColorsMatch(Color color1, Color color2, double tolerance = 0.15)
        {
            double distance = Math.Sqrt(
                Math.Pow(color1.R - color2.R, 2) +
                Math.Pow(color1.G - color2.G, 2) +
                Math.Pow(color1.B - color2.B, 2)
            ) / (255.0 * Math.Sqrt(3));
            
            return distance <= tolerance;
        }

        /// <summary>
        /// Get expected stash tab bounds based on typical PoE UI layout
        /// </summary>
        public static Rectangle GetExpectedStashBounds(Size screenSize)
        {
            // PoE stash tab is typically positioned in the right side of screen
            // These are approximations based on common resolutions
            int stashWidth = GRID_COLS * (GRID_SLOT_SIZE + GRID_SPACING);
            int stashHeight = GRID_ROWS * (GRID_SLOT_SIZE + GRID_SPACING);
            
            // Typical stash position (right side, with some margin)
            int x = screenSize.Width - stashWidth - 50;
            int y = 150; // Below top UI
            
            return new Rectangle(x, y, stashWidth, stashHeight);
        }
    }
} 