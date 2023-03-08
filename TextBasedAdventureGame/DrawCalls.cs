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
            Console.SetCursorPosition(0, 0);
            Console.Write("Title here:");
            MenuButtonState[] arr = (MenuButtonState[])Enum.GetValues(MenuButtonState.Start.GetType());

            for (int i = 0; i < 4; i++)
            {
                if (i == menuCursorPosition) {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                } else {
                    Console.ResetColor();
                }


                Console.SetCursorPosition(22 - (arr[i].ToString().Length / 2), 10 + i + i);
                Console.WriteLine(arr[i].ToString());
                Console.ResetColor();
            }
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
        
        public static void DrawControls()
        {
            Console.Clear();
            
            Console.ReadKey(true);
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

            Console.SetCursorPosition(22 - 7, 16);
            Console.Write("Debug controls");
            Console.SetCursorPosition(22 - 7, 17);
            Console.Write("F1 - Add 10exp");
            Console.SetCursorPosition(22 - 6, 18);
            Console.Write("F2 - Add key");
            Console.SetCursorPosition(22 - 7, 19);
            Console.Write("F3 - Add potion");
            Console.SetCursorPosition(22 - 8, 20);
            Console.Write("F4 - Kill player");

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

        public static void DrawLost(int floor) {
            Console.Clear();

            Console.SetCursorPosition((Console.WindowWidth / 2) - 20, 5);
            Console.Write(" _  _                 ___              _ ");
            Console.SetCursorPosition((Console.WindowWidth / 2) - 20, 6);
            Console.Write("| || | ___   _  _    |   \\   _   ___  | |");
            Console.SetCursorPosition((Console.WindowWidth / 2) - 20, 7);
            Console.Write(" \\  / /   \\ | || |   | [] \\ | | / _/ /  |");
            Console.SetCursorPosition((Console.WindowWidth / 2) - 20, 8);
            Console.WriteLine("  ||  \\___/  \\__/    |____/ |_| \\__| \\__|");

            if (floor == 1) {
                Console.WriteLine("You died on the 1st floor");
                Console.Write("Tip: Use health potions to restore HP");
            }
            else {
                Console.Write($"You made it {floor} floors deep into the dungeon");
            }
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
