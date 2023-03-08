using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using static AdventureGame.Program;

namespace AdventureGame
{
    internal class DrawCalls
    {

        public static void DrawDungeon(Room[,] map, int colUserInputSize, int rowUserInputSize) {
            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    map[row, col].Draw();
                }
                Console.WriteLine();
            }
        }

        public static BattleOption DrawBattleMenu(int battleMenuPosition, BattleOption currentBattleOption)
        {
            BattleOption[] arr = (BattleOption[])Enum.GetValues(currentBattleOption.GetType());

            for (int i = 0; i < arr.Length; i++)
            {
                if (i == battleMenuPosition)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.SetCursorPosition(25, 14 + i);
                Console.WriteLine($"{arr[i]}");
                Console.ResetColor();
            }

            return currentBattleOption;
        }

        // Prevent draw call overlap
        private static readonly object myLock = new object();

        public static void DrawMainDungeonScreen(Room[,] map, int colUserInputSize, int rowUserInputSize)
        {
            lock (myLock) {
                Console.SetCursorPosition(0, 0);
                DrawDungeon(map, colUserInputSize, rowUserInputSize);
                p.WriteStats(23);
                DrawBox(5, 41, 2, 26);
                DrawMessageHistory();
                WriteControls();
            }
        }

        public static void DrawMainMenu(int menuCursorPosition)
        {
            Console.SetCursorPosition(22 - 17, 1);
            Console.Write("  _____");
            Console.SetCursorPosition(22 - 16, 2);
            Console.Write("|  __ \\");
            Console.SetCursorPosition(22 - 16, 3);
            Console.Write("| |  | | ___  ___ _ __   ___ _ __");
            Console.SetCursorPosition(22 - 16, 4);
            Console.Write("| |  | |/ _ \\/ _ \\ '_ \\ / _ \\ '__|");
            Console.SetCursorPosition(22 - 16, 5);
            Console.Write("| |__| |  __/  __/ |_) |  __/ |");
            Console.SetCursorPosition(22 - 16, 6);
            Console.Write("|_____/ \\___|\\___| .__/ \\___|_|");
            Console.SetCursorPosition(22 - 16, 7);
            Console.Write("                 | |");
            Console.SetCursorPosition(22 - 16, 8);
            Console.Write("                 |_|");
            MenuButtonState[] arr = (MenuButtonState[])Enum.GetValues(MenuButtonState.Start.GetType());

            for (int i = 0; i < 5; i++)
            {
                if (i == menuCursorPosition) {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                } else {
                    Console.ResetColor();
                }


                Console.SetCursorPosition(22 - (arr[i].ToString().Length / 2), 10 + i + i);
                Console.Write(arr[i].ToString());
                Console.ResetColor();
            }

            Console.SetCursorPosition(0, 43);
            Console.Write("V1.1");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" (Debug build)");
            Console.ResetColor();
        }

        public static void RedrawObject(Vector2 position, Vector2 newPosition)
        {
            Console.SetCursorPosition((int)position.X * 2, (int)position.Y);
            map[(int)position.X, (int)position.Y].Draw();

            Console.SetCursorPosition((int)newPosition.X * 2, (int)newPosition.Y);
            map[(int)newPosition.X, (int)newPosition.Y].Draw();
        }

        public static void DrawBattleScreen(Enemy enemy)
        {
            DrawBox(5, 40, 2, 25);
            DrawBattleMessageHistory();
            p.WriteStats(21);

            DrawBox(18, 42, 1, 1);

            Console.SetCursorPosition(24, 9);
            Console.Write($"{enemy.Name}: {enemy.Hp} HP   ");

            Console.SetCursorPosition(25, 7);
            enemy.Sprite();

            Console.SetCursorPosition(5, 12);
            p.Sprite();
        }

        public static void UpdateEnemyDrawHP(Enemy enemy)
        {
            Console.SetCursorPosition(24, 9);

            int hp = enemy.Hp;
            if (hp < 0) hp = 0;

            Console.Write($"{enemy.Name}: {hp} HP   ");
        }

        public static void DrawBox(int height, int width, int left, int top) {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.SetCursorPosition(left + i, top + j);
                    if((i == 0 && j == 0) || (i == 0 && j == height - 1) || (i == width - 1 && j == 0) || (i == width - 1 && j == height - 1)) {
                        Console.Write(" ");
                        continue;
                    }

                    if (i == 0) Console.Write("|");
                    if (i == width - 1) Console.Write("|");
                    if (j == 0) Console.Write("-");
                    if (j == height -1) Console.Write("-");
                }
            }
        }

        public static void DrawMessageHistory() {
            for (int i = dungeonMessages.Length - 1; i >= 0; i--)
            {

                if (dungeonMessages[i] != null) {
                    Console.SetCursorPosition(4, 27 + i);
                    Console.Write("                                     ");
                    Console.SetCursorPosition(4, 27 + i);
                    Console.Write(dungeonMessages[i]);
                }
            }
        }

        public static void DrawBattleMessageHistory() {
            for (int i = battleMessages.Length - 1; i >= 0; i--)
            {

                if (battleMessages[i] != null) {
                    Console.SetCursorPosition(4, 26 + i);
                    Console.Write("                                     ");
                    Console.SetCursorPosition(4, 26 + i);
                    Console.Write(battleMessages[i]);
                }
            }
        }
       

        public static void DrawCredits()
        {
            Console.Clear();
            Console.SetCursorPosition(12, 10);
            Console.Write("Created by Akiro Ames");
            Console.ReadKey(true);
        }

        public static void DrawLegend()
        {
            Console.Clear();
            Console.SetCursorPosition(22 - 5, 2);
            Console.Write("Game legend");

            Console.SetCursorPosition(22 - 5, 4);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  ");
            Console.ResetColor();

            Console.Write(" - Player");

            Console.SetCursorPosition(22 - 5, 5);
            Console.Write("oD - Spider");
            Console.SetCursorPosition(22 - 4, 6);
            Console.Write("o/ - Rat");
            Console.SetCursorPosition(22 - 5, 7);
            Console.Write("[] - Mimic");
            Console.SetCursorPosition(22 - 6, 8);
            Console.Write("{8 - Skeleton");
            Console.SetCursorPosition(22 - 4, 9);
            Console.Write(@"\/ - Bat");

            Console.SetCursorPosition(22 - 7, 10);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("? ");
            Console.ResetColor();
            Console.Write(" - Random Item");

            Console.SetCursorPosition(22 - 7, 11);
            Console.Write("D| - Locked exit");

            Console.ReadKey(true);
        }

        public static void DrawControls() {
            Console.Clear();

            Console.SetCursorPosition(22 - 4, 2);
            Console.Write("Controls");
            Console.SetCursorPosition(22 - 14, 4);
            Console.Write("Move - W A S D or Arrow Keys");
            Console.SetCursorPosition(22 - 10, 5);
            Console.Write("Interact - F Z Enter");
            Console.SetCursorPosition(22 - 11, 6);
            Console.Write("Inventory | Exit - X E");

            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(22 - 7, 9);
            Console.Write("Debug controls");
            Console.SetCursorPosition(22 - 7, 11);
            Console.Write("F1 - Add 10exp");
            Console.SetCursorPosition(22 - 6, 12);
            Console.Write("F2 - Add key");
            Console.SetCursorPosition(22 - 11, 13);
            Console.Write("F3 - Add health potion");
            Console.SetCursorPosition(22 - 8, 14);
            Console.Write("F4 - Kill player");
            Console.ResetColor();

            Console.ReadKey(true);
        }

        public static void DrawInventory(int inventoryCursor) {
            Console.SetCursorPosition(0, 0);
            Console.Write("Inventory\n");

            var items = p.GetItems();

            for (int i = 0; i < items.Length; i++)
            {
                if (i == inventoryCursor) {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                } else {
                    Console.ResetColor();
                }

                Console.WriteLine($"{items[i].Qty}x {items[i].Name}");
                Console.ResetColor();
            }
        }

        public static void DrawLost(int floor, Player p) {
            Console.Clear();

            Console.SetCursorPosition(22 - 12, 4);
            switch (floor)
            {
                case 1:
                    Console.Write("You died on the 1st floor");
                    break;
                case 2:
                    Console.Write("You died on the 2nd floor");
                    break;
                case 3:
                    Console.Write("You died on the 3rd floor");
                    break;
                default:
                    Console.Write($"You died on the {floor}th floor");
                    break;
            }

            Console.SetCursorPosition(22 - ((26 + floor.ToString().Length + (floor > bestFloor ? floor.ToString().Length : bestFloor.ToString().Length)) / 2) + 1, 6);
            Console.Write($"Cleared floors: ");

            ConsoleColor floorColour = floor > bestFloor ? ConsoleColor.DarkYellow : ConsoleColor.DarkGray;

            if (floor > bestFloor) {
                bestFloor = floor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.Write($"{floor}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" (Best : ");
            Console.ForegroundColor = floorColour;
            Console.Write($"{bestFloor}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(")");
            Console.ResetColor();

            Console.SetCursorPosition(22 - ((17 + p.Lvl.ToString().Length + (p.Lvl > bestLevel ? p.Lvl.ToString().Length : bestLevel.ToString().Length)) / 2), 7);
            Console.Write("Level: ");

            ConsoleColor levelColour = p.Lvl > bestLevel ? ConsoleColor.DarkYellow : ConsoleColor.DarkGray;

            if (p.Lvl > bestLevel) {
                bestLevel = p.Lvl;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.Write($"{p.Lvl}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" (Best : ");
            Console.ForegroundColor = levelColour;
            Console.Write($"{bestLevel}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(")");
            Console.ResetColor();

            Console.SetCursorPosition(22 - ((15 + p.TotalEXP.ToString().Length + (p.TotalEXP > bestEXP ? p.TotalEXP.ToString().Length : bestEXP.ToString().Length)) / 2), 8);
            Console.Write("EXP: ");

            ConsoleColor expColour = p.TotalEXP > bestEXP ? ConsoleColor.DarkYellow : ConsoleColor.DarkGray;

            if (p.TotalEXP > bestEXP)
            {
                bestEXP = p.TotalEXP;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.Write($"{p.TotalEXP}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" (Best : ");
            Console.ForegroundColor = expColour;
            Console.Write($"{bestEXP}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(")");
            Console.ResetColor();
        }

        public static void WriteControls() {
            Console.SetCursorPosition(10, 32);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("   W    E < inventory   ");
            Console.SetCursorPosition(10, 33);
            Console.Write(" A S D    F < interact  ");
            Console.ResetColor();
        }
    }
}
