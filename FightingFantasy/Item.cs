using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class Item
    {
        public string Name { get; }
        public (string, int) Effect
        { 
            get;
            private set;
        }

        public string Type
        {
            get;
            private set;
        }

        public Item(string name, (string,int) effect = default((string, int)), string type = null)
        {
            Name = name;
            Effect = effect;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj is Item item)
                return this.Name == item.Name;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
