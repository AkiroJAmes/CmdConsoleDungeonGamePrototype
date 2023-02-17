using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureGame
{
    class PickUpItem : GameObject
    {
        protected string NAME;
        protected int QTY;

        public ItemType MyItemType { get; set; }

        public enum ItemType { 
            Junk,
            LesserHealthPotion,
            GreaterHealthPotion,
            Key
        }

        public PickUpItem(string name, int qty)
        {
            this.NAME = name;
            this.QTY = qty;
        }

        public PickUpItem() { }

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

        public override bool UseItem(Player player, PickUpItem item, Program.GameState gameState)
        {
            Console.Write("Cannot use this item");
            return false;
        }
    }

    class LesserHealthPotion : PickUpItem
    {
        public LesserHealthPotion(string name, int qty) : base(name, qty)
        {
            this.NAME = name;
            this.QTY = qty;
            MyItemType = ItemType.LesserHealthPotion;
        }

        public LesserHealthPotion(PickUpItem copy)
        {
            this.NAME = copy.Name;
            this.QTY = copy.Qty;
            MyItemType = copy.MyItemType;
        }

        public override bool UseItem(Player player, PickUpItem item, Program.GameState gameState)
        {
            if (player.Hp >= player.MaxHP)
            {
                Console.Write("You are already at max health!");
                return false;
            }


            player.Hp += 5;
            if (player.Hp >= player.MaxHP) player.Hp = player.MaxHP;

            this.QTY -= 1;

            if (this.QTY <= 0) {
                player.RemoveItem(item);
                this.Qty = 1;
            }

            if(gameState == Program.GameState.InBattle)
                Program.AddBattleMessageHistory("Restored 5 health", true);
            if (gameState == Program.GameState.InDungeon)
                Program.AddMessageHistory("Restored 5 health", false);

            return true;
        }

/*        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("U ");
            Console.ResetColor();
        }*/
    }

    class GreaterHealthPotion : PickUpItem
    {
        public GreaterHealthPotion(string name, int qty) : base(name, qty)
        {
            this.NAME = name;
            this.QTY = qty;
            MyItemType = ItemType.GreaterHealthPotion;
        }

        public GreaterHealthPotion(PickUpItem copy)
        {
            this.NAME = copy.Name;
            this.QTY = copy.Qty;
            MyItemType = copy.MyItemType;
        }

        public override bool UseItem(Player player, PickUpItem item, Program.GameState gameState)
        {
            if (player.Hp >= player.MaxHP)
            {
                Console.Write("You are already at max health!");
                return false;
            }


            player.Hp = player.MaxHP;
            
            this.QTY -= 1;

            if (this.QTY <= 0)
            {
                player.RemoveItem(item);
                this.Qty = 1;
            }

            if (gameState == Program.GameState.InBattle)
                Program.AddBattleMessageHistory("Restored health to full", true);
            if (gameState == Program.GameState.InDungeon)
                Program.AddMessageHistory("Restored health to full", false);

            return true;
        }

        /*        public override void Draw()
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("U ");
                    Console.ResetColor();
                }*/
    }

    class Key : PickUpItem
    {
        public Key(string name, int qty) : base(name, qty)
        {
            this.NAME = name;
            this.QTY = qty;
            MyItemType = ItemType.Key;
        }

        public Key(PickUpItem copy)
        {
            this.NAME = copy.Name;
            this.QTY = copy.Qty;
            MyItemType = copy.MyItemType;
        }

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("::");
            Console.ResetColor();
        }
    }
}
