using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AdventureGame
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

        public virtual bool IsAlive() { return false; }

        public Vector2 GetGameObjectPosition()
        {
            return new Vector2(xPosition, yPosition);
        }

        public virtual bool UseItem(Player player, PickUpItem item, Program.GameState gameState) { return false; }

        public virtual int Hp { get; set; }

        public virtual Program.EnemyBattleAIState Ai { get; }
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
}
