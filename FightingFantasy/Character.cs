using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class Character
    {
        public int stamina;
        public int skill;

        public int AttackStrength()
        {
            return Dice.SumRoll(2) + skill;
        }

        public virtual int DealDmg()
        {
            return 2;
        }

        public virtual void TakeDmg(int dmg)
        {
            stamina -= dmg;
        }
    }

    class Protagonist:Character
    {
        public int luck;
        private Dictionary<Item,int> inventory;

        public Protagonist()
        {
            stamina = Dice.SumRoll(2) + 12;
            skill = Dice.SumRoll(1) + 6;
            luck = Dice.SumRoll(1) + 6;

            inventory = new Dictionary<Item, int>();
        }

        public bool TestLuck()
        {
            bool is_lucky = Dice.SumRoll(2) <= luck;
            luck -= 1;
            return is_lucky;
        }

        public void AddToInventory(Item item, int amount)
        {
            if (HasItem(item))
                inventory[item] += amount;
            else
                inventory.Add(item, amount);
        }

        public void RemoveFromInventory(Item item)
        {
            inventory[item] -= 1;
            if (inventory[item] == 0)
                inventory.Remove(item);
        }

        public bool HasItem(Item item)
        {
            return inventory.ContainsKey(item);
        }
    }

    class Enemy : Character
    {
        public string name;
        public Enemy(string name, int stamina, int skill)
        {
            this.name = name;
            this.stamina = stamina;
            this.skill = skill;
        }
    }
}
