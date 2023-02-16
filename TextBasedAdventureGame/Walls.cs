using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureGame
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
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("'");
                    Console.ResetColor();
                    Console.Write("|");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(";");
                    Console.ResetColor();
                    Console.Write("|");
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("`");
                    Console.ResetColor();
                    Console.Write("|");
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(".");
                    Console.ResetColor();
                    Console.Write("|");
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
            Console.Write("  ");
        }
    }

    class BottomLeftCorner : Wall
    {
        public override void Draw()
        {
            Console.Write("|_");
        }
    }

    class BottomRightCorner : Wall
    {
        public override void Draw()
        {
            Console.Write("_|");
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
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("'");
                    Console.ResetColor();
                    break;
                case 2:
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(";");
                    Console.ResetColor();
                    break;
                case 3:
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("`");
                    Console.ResetColor();
                    break;
                case 4:
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(".");
                    Console.ResetColor();
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
            Console.Write("D|");
            Console.ResetColor();
            Console.ResetColor();
        }

        public override bool IsKeyWall()
        {
            return true;
        }
    }
}
