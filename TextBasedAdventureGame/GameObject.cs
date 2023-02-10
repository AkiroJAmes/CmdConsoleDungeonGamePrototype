using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TextBasedAdventureGame
{
    abstract class GameObject
    {
        protected int xPosition;
        protected int yPosition;

        public abstract void Draw();

        public virtual void UpdatePosition(int xPosition, int yPosition)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public virtual string Name { get; }

        public virtual int Qty { get; set; }

        public virtual bool IsWall() {
            return false;
        }

        public virtual bool IsItem() {
            return false;
        }

        public virtual bool IsEnemy() {
            return false;
        }

        public virtual bool IsKeyWall() { return false; }

        public virtual GameObject GetGameObject() { return this; }

        public virtual bool IsAlive() { return false; }

        public Vector2 GetGameObjectPosition()
        {
            return new Vector2(xPosition, yPosition);
        }

        public virtual int Hp { get; set; }
    }

    class Void : GameObject
    {
        public override void Draw()
        {
            Console.Write("  ");
        }
    }

    class PowerUp : GameObject
    {
        public PowerUp()
        {
        }

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("! ");
            Console.ResetColor();
        }
    }

    class PickUpItem : GameObject
    {
        protected string NAME;
        protected int QTY;

        public PickUpItem(string name, int qty) {
            this.NAME = name;
            this.QTY = qty;
        }

        public override string Name { 
            get {
                return this.NAME;
            }
        }

        public override int Qty { 
            get {
                return this.QTY;
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
    }

    class Key : PickUpItem
    {
        public Key(string name, int qty) : base (name, qty) {
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
