using System;
using System.Collections.Generic;
using System.Text;

namespace TextAdventureGame
{
    class PickUpItem : GameObject
    {
        protected string NAME;
        protected int QTY;

        public PickUpItem(string name, int qty)
        {
            this.NAME = name;
            this.QTY = qty;
        }

        public override string Name
        {
            get
            {
                return this.NAME;
            }
        }

        public override int Qty
        {
            get
            {
                return this.QTY;
            }

            set
            {
                this.QTY = value;
            }
        }

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("? ");
            Console.ResetColor();
        }

        public override bool IsItem()
        {
            return true;
        }

        public override void UseItem(Player player)
        {
            Console.Write("Cannot use this item");
        }
    }

    class HealthPotion : PickUpItem
    {
        public HealthPotion(string name, int qty) : base(name, qty)
        {
            this.NAME = name;
            this.QTY = qty;
        }

        public override void UseItem(Player player)
        {
            if (player.Hp >= player.MaxHP)
            {
                Console.Write("You are already at max health!");
                return;
            }


            player.Hp += 5;
            if (player.Hp >= player.MaxHP) player.Hp = player.MaxHP;

            this.QTY -= 1;

            if (this.QTY <= 0) player.RemoveItem(this);

            this.Qty = 1;

            Console.Write("Restored 5 health");
            Program.AddMessageHistory("Restored 5 health", false);
        }

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("U ");
            Console.ResetColor();
        }
    }

    class Key : PickUpItem
    {
        public Key(string name, int qty) : base(name, qty)
        {
            this.NAME = name;
            this.QTY = qty;
        }

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("::");
            Console.ResetColor();
        }
    }
}
