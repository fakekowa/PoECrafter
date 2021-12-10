using System;
using System.Collections.Generic;

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
    }
    internal class Armours
    {
    }

    public class Affixes
    {
        public new List<String> AffixArray = new List<String>();
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
}