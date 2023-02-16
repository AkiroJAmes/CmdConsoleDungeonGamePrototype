using System;
using System.Collections.Generic;
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

                // Prevent draw call overlap with lock
        private static readonly object myLock = new object();

        public static void DrawMainDungeonScreen(Room[,] map, int colUserInputSize, int rowUserInputSize)
        {
            lock (myLock) {
                Console.SetCursorPosition(0, 0);
                DrawDungeon(map, colUserInputSize, rowUserInputSize);
                p.WriteStats();
                DrawMessageHistoryBox();
                DrawMessageHistory();
                WriteControls();
            }
/*            // Buffer draw calls made to ensure no draws are overlapped

            if (!currentlyDrawing) {
                currentlyDrawing = true;



                // Very very small wait to ensure write has time to finish to avoid artifacts
                Thread.Sleep(1);

                currentlyDrawing = false;
                return;
            }

            if (currentlyDrawing)
            {
                while (true) {
                    if (!currentlyDrawing) break;
                }

                currentlyDrawing = true;

                Console.SetCursorPosition(0, 0);
                DrawDungeon();
                p.WriteStats();
                WriteControls();

                Thread.Sleep(1);

                currentlyDrawing = false;
                return;
            }*/
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

        public static void DrawMessageHistoryBox() {
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.SetCursorPosition(2 + i, 25 + j);
                    if((i == 0 && j == 0) || (i == 0 && j == 4) || (i == 39 && j == 0) || (i == 39 && j == 4)) {
                        Console.Write(" ");
                        continue;
                    }

                    if (i == 0) Console.Write("|");
                    if (i == 39) Console.Write("|");
                    if (j == 0) Console.Write("-");
                    if (j == 4) Console.Write("-");
                }
            }
        }

        public static void DrawMessageHistory() {
            for (int i = dungeonMessages.Length - 1; i >= 0; i--)
            {

                if (dungeonMessages[i] != null) {
                    Console.SetCursorPosition(4, 26 + i);
                    Console.Write("                                     ");
                    Console.SetCursorPosition(4, 26 + i);
                    Console.Write(dungeonMessages[i]);
                }
            }
        }

        public static void DrawBattleMessageHistoryBox() {
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.SetCursorPosition(2 + i, 25 + j);
                    if((i == 0 && j == 0) || (i == 0 && j == 4) || (i == 39 && j == 0) || (i == 39 && j == 4)) {
                        Console.Write(" ");
                        continue;
                    }

                    if (i == 0) Console.Write("|");
                    if (i == 39) Console.Write("|");
                    if (j == 0) Console.Write("-");
                    if (j == 4) Console.Write("-");
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
            Console.Write("Game legend here\n");

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  ");
            Console.ResetColor();

            Console.WriteLine(" - Player");

            Console.WriteLine("oD - Spider");
            Console.WriteLine("o/ - Rat");
            Console.WriteLine("[] - Mimic");
            Console.WriteLine("{8 - Skeleton");
            Console.WriteLine(@"\/ - Bat");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("? ");
            Console.ResetColor();
            Console.WriteLine("- Random Item");

            Console.WriteLine("D| - Locked exit");

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

        public static void WriteControls() {
            Console.SetCursorPosition(10, 31);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("   W    E < inventory   ");
            Console.SetCursorPosition(10, 32);
            Console.Write(" A S D    F < interact  ");
            Console.ResetColor();
        }
    }
}
