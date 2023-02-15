using System;
using System.Collections.Generic;
using System.Text;

using static TextAdventureGame.Program;

namespace TextAdventureGame
{
    abstract class Character : GameObject
    {
        protected string NAME;

        protected int AT;
        protected int MAXHP;
        protected int HP;
        protected int DF;

        public override bool IsAlive()
        {
            return HP <= 0;
        }

        public override int Hp {
            get {
                return this.HP;
            }

            set {
                this.HP = value;
            }
        }

        public int MaxHP { 
            get {
                return MAXHP;
            }
        }

        public override string Name { get { return NAME; } }

        public int At { get; }
        public int Df { get; }
    }

    class Player : Character
    {
        protected int LVL;
        protected int EXP;
        List<GameObject> ITEMS = new List<GameObject>();

        public Player(int at, int hp, int df, int exp, int lvl) {
            this.AT = at;
            this.MAXHP = hp;
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

        public int Lvl { get { return LVL; } }

        public int NextLevel() {
            float exponent = 1.5f;
            float baseEXP = 5f;

            return (int)Math.Round(baseEXP * (MathF.Pow(Lvl, exponent)));
        }

        public int GetEXP() {
            if (EXP >= NextLevel()) {
                EXP -=NextLevel();
                UpdateLVL();
                return EXP;
            }
            return EXP;
        }

        void UpdateLVL() {
            LVL++;
        }

        public void WriteStats() {

            GetEXP();

            // Make sure the line is clear
            Console.SetCursorPosition(2, 23);
            Console.Write("\r                                            ");
            Console.SetCursorPosition(2, 23);
            Console.Write($"Level: {Lvl} | EXP: {GetEXP()} / {NextLevel()} | ");
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

                if (ITEMS[i].Name == item.Name) {
                    ITEMS[i].Qty = item.Qty + ITEMS[i].Qty;
                    return;
                } 
            }

            this.ITEMS.Add(item);
        }

        public void RemoveItem(GameObject removeItem) {
            foreach (var item in ITEMS)
            {
                if(item.Name == removeItem.Name) {
                    ITEMS.Remove(item);
                    return;
                }
            }
        }

        public GameObject[] GetItems() { return this.ITEMS.ToArray(); }
    }

    class Enemy : Character
    {
        protected EnemyBattleAIState AI;
        public Species MySpecies { get; protected set; }

        public enum Species
        {
            Spider,
            Mimic,
            Rat,
            Skeleton
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

        public override EnemyBattleAIState Ai
        {
            get { return AI; }
        }

        public virtual bool ISActive { set { } }
    }

    class Spider : Enemy
    {
        public Spider(int at, int hp, int df, string name, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            MySpecies = Species.Spider;
        }

        public Spider(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write("oD");
        }
    }

    class Mimic : Enemy
    {
        protected bool IsActive = false;

        public Mimic(int at, int hp, int df, string name, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            MySpecies = Species.Mimic;
        }

        public Mimic(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            if (!IsActive) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("? ");
                Console.ResetColor();
            } else {
                Console.Write("[]");
            }
        }

        public override bool ISActive { set { IsActive = value; } }
    }

    class Rat : Enemy
    {
        public Rat(int at, int hp, int df, string name, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            MySpecies= Species.Rat;
        }

        public Rat(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write("o/");
        }
    }

    class Skeleton : Enemy
    {
        public Skeleton(int at, int hp, int df, string name, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            MySpecies = Species.Skeleton;
        }

        public Skeleton(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write("{8");
        }
    }
}
