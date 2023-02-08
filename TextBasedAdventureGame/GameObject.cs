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

        public virtual bool IsWall() {
            return false;
        }

        public virtual bool IsItem() {
            return false;
        }

        public virtual bool IsEnemy() {
            return false;
        }

        public Vector2 GetGameObjectPosition()
        {
            return new Vector2(xPosition, yPosition);
        }

        public virtual string GetName() {
            return "N/A";
        }

        public virtual int GetQTY() {
            return -1;
        }

        public virtual void SetQTY(int q) {  }

        public virtual void SetHP(int hp) { }
    }

    class Wall : GameObject
    {
        public override void Draw()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("= ");
            Console.ResetColor();
        }

        public override bool IsWall() {
            return true;
        }
    }

    class LockedDoor : Wall
    {
        public override void Draw()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("88");
            Console.ResetColor();
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

        public override string GetName() { return this.NAME; }

        public override int GetQTY() { return this.QTY; }

        public override void SetQTY(int q) { this.QTY = q; }

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
