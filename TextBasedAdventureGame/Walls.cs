using System;
using System.Collections.Generic;
using System.Text;

namespace TextBasedAdventureGame
{
    class Wall : GameObject
    {
        public override void Draw()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  ");
            Console.ResetColor();
        }

        public override bool IsWall()
        {
            return true;
        }
    }

    class LeftWall : Wall
    {
        public override void Draw()
        {
            switch (new Random().Next(0, 6))
            {
                case 0:
                    Console.Write(" |");
                    break;
                case 1:
                    Console.Write("'|");
                    break;
                case 2:
                    Console.Write(";|");
                    break;
                case 3:
                    Console.Write("`|");
                    break;
                case 4:
                    Console.Write(".|");
                    break;
                case 5:
                    Console.Write(" ]");
                    break;

            }
        }
    }

    class LeftWallDetail : Wall
    {
        public override void Draw()
        {
            
        }
    }

    class RightWall : Wall
    {
        public override void Draw()
        {
            switch (new Random().Next(0, 6))
            {
                case 0:
                    Console.Write("| ");
                    break;
                case 1:
                    Console.Write("|'");
                    break;
                case 2:
                    Console.Write("|;");
                    break;
                case 3:
                    Console.Write("|`");
                    break;
                case 4:
                    Console.Write("|.");
                    break;
                case 5:
                    Console.Write("[ ");
                    break;
            }
        }
    }

    class TopWall : Wall
    {
        public override void Draw()
        {
            Console.Write("__");
        }
    }

    class BottomWall : Wall
    {
        public override void Draw()
        {
            Console.Write("__");
        }
    }

    class CornerWall : Wall
    {
        public override void Draw()
        {
            Console.Write("  ");
        }
    }

    class LockedDoor : Wall
    {
        public override void Draw()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("8|");
            Console.ResetColor();
        }

        public override bool IsKeyWall()
        {
            return true;
        }
    }
}
