using System;
using System.Collections.Generic;
using System.Text;

namespace TextBasedAdventureGame
{
    abstract class Character : GameObject
    {
        protected string NAME;

        protected int AT;
        protected int HP;
        protected int DF;

        public override bool IsAlive()
        {
            return HP <= 0;
        }

        public override void SetHP(int hp) { 
            this.HP = hp;
        }

        public override string GetName() { return NAME; }

        public int GetAT() { return AT; }
        public int GetHP() { return HP; }
        public int GetDF() { return DF; }
    }

    class Player : Character
    {
        protected int LVL;
        protected int EXP;
        List<GameObject> ITEMS = new List<GameObject>();

        public Player(int at, int hp, int df, int exp, int lvl) {
            this.AT = at;
            this.HP = hp;
            this.DF = df;
            this.EXP = exp;
            this.LVL = lvl;
        }

        public override void Draw() {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("  ");
            Console.ResetColor();
        }

        public int GetLVL() { return LVL; }

        public int NextLevel() {
            return (int)Math.Round(0.04 * Math.Pow(LVL, 3) + 0.8 * Math.Pow(LVL, 2) + 2 * LVL);
        }

        public int GetEXP() {
            if (EXP >= NextLevel()) {
                UpdateLVL();
                return 0;
            }
            return EXP; 
        }

        void UpdateLVL() {
            LVL++;
            EXP = 0;
        }

        public void WriteStats() {

            GetEXP();

            // Make sure the line is clear
            Console.WriteLine("\r                                     ");
            Console.Write($"Level: {GetLVL()} | EXP: {GetEXP()} / {NextLevel()} | ");
            Console.Write($"HP{HP} : AT{AT} : DF{DF}");
        }

        public void AddEXP(int exp) {
            EXP += exp;
            GetEXP();
        }

        public void AddItem(GameObject item) {
            for (int i = 0; i < this.ITEMS.Count; i++)
            {
                // Add item to stack with existing items

                if (ITEMS[i].GetName() == item.GetName()) {
                    ITEMS[i].SetQTY(item.GetQTY() + ITEMS[i].GetQTY());
                    return;
                } 
            }

            this.ITEMS.Add(item);
        }

        public GameObject[] GetItems() { return this.ITEMS.ToArray(); }
    }

    class Enemy : Character
    {
        public Enemy() { 
        }

        public Enemy(int at, int hp, int df, string name) {
            this.AT = at;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
        }

        public override bool IsEnemy()
        {
            return true;
        }

        public override void Draw() {
            

            if (this.HP > 10) {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("X ");
                Console.ResetColor();
            }
            else {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("x ");
                Console.ResetColor();
             }
        }
    }
}
