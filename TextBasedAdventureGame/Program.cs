using System;
using System.Collections;
using System.Numerics;
using System.Threading;

namespace TextBasedAdventureGame
{
    internal class Program
    {
        static int rowUserInputSize = 22;
        static int colUserInputSize = 22;

        static Room[,] map;
        static Enemy[] e;
        static Player p;

        static ConsoleKey playerInput;

        static Timer timer;
        static bool currentlyDrawing;

        static GameState gameState;

        enum GameState 
        {
            InDungeon,
            InInventory,
            InBattle
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            while (true) {
                gameState = GameState.InDungeon;

                Console.Clear();

                map = new Room[rowUserInputSize, colUserInputSize];
                CreateDungeonRooms();


                var r = new Random();
                p = new Player(5, 10, 2, 1, 0);
                e = new Enemy[r.Next(2, 4)];
                PowerUp[] pu = new PowerUp[r.Next(1, 2)];
                PickUpItem[] pui = new PickUpItem[r.Next(4, 7)];

                AddGameObjects(pu, pui);
                InitTimer();
                GameLoop(pu, pui);

                break;
            }
        }

        private static void GameLoop(PowerUp[] pu, PickUpItem[] pui)
        {
            PlayerAction();
        }

        static void PlayerAction() {
            Vector2 newPlayerPosition = Vector2.Zero;
            Vector2 playerPositionLastTurn = Vector2.Zero;

            while (true) {
                if(gameState == GameState.InDungeon) { 
                    DrawMainDungeonScreen();

                    var pPosition = p.GetGameObjectPosition();

                    var actionUserInput = Console.ReadKey().Key;

                    switch (actionUserInput)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            newPlayerPosition = new Vector2(pPosition.X, pPosition.Y - 1);
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            newPlayerPosition = new Vector2(pPosition.X, pPosition.Y + 1);
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            newPlayerPosition = new Vector2(pPosition.X + 1, pPosition.Y);
                            break;
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            newPlayerPosition = new Vector2(pPosition.X - 1, pPosition.Y);
                            break;
/*                        case ConsoleKey.X:
                        case ConsoleKey.E:
                            PlayerInventory();
                            Console.Clear();
                            continue;
                        case ConsoleKey.Z:
                        case ConsoleKey.F:
                            PickUpNewItem(pPosition);
                            Console.Clear();
                            continue;*/
                    }

                    // Check if new player position is valid
                    var mapPosition = map[(int)newPlayerPosition.X, (int)newPlayerPosition.Y];

                    if (!mapPosition.CheckIfEmpty() && mapPosition.CheckIfWall()) {
/*                        Console.WriteLine($"You attempted to move to {newPlayerPosition} but there is a wall there");
                        Console.ReadKey();
                        Console.SetCursorPosition(0, 0);*/
                        continue;
                    }

                    switch (actionUserInput) {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            map[(int)pPosition.X, (int)pPosition.Y].RemoveGameObject(p);
                            map[(int)newPlayerPosition.X, (int)newPlayerPosition.Y].AddGameObject(p, (int)newPlayerPosition.X, (int)newPlayerPosition.Y);
                            break;
                    }

                
/*                    if (!mapPosition.CheckIfEmpty()) {
                        GameObject e = mapPosition.CheckIfEnemy();
                        if (e != null) {
                            Console.SetCursorPosition(0, 0);
                            DrawDungeon();
                            p.WriteStats();
                            WriteControls();
                            Console.Write("You ran into an enemy!");

                            Console.ReadKey();

                            FightSequence(p, e, p);
                        }
                    }*/

                    Console.SetCursorPosition(0, 0);
                }
            }
        }

        private static readonly object myLock = new object();

        private static void DrawMainDungeonScreen()
        {
            lock (myLock) {
                Console.SetCursorPosition(0, 0);
                DrawDungeon();
                p.WriteStats();
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

        private static void EnemyAction()
        {
            foreach (var enemy in e) {
                if(!enemy.IsAlive()) { 
                    var currentPosition = enemy.GetGameObjectPosition();
                    var playerPosition = p.GetGameObjectPosition();

                    // How far the enemy can see the player
                    var enemyViewDistance = 7;

                    var newPosition = currentPosition;
                    var r = new Random();

                    // Move towards the player otherwise choose a random direction
                    if(Vector2.Distance(currentPosition, playerPosition) < enemyViewDistance) {

                        if (playerPosition.Y > currentPosition.Y) { newPosition = currentPosition + Vector2.UnitY; }
                        else if (playerPosition.X > currentPosition.X) { newPosition = currentPosition + Vector2.UnitX; }
                        else if (playerPosition.Y < currentPosition.Y) { newPosition = currentPosition + -Vector2.UnitY; }

                        else if (playerPosition.X < currentPosition.X) { newPosition = currentPosition + -Vector2.UnitX; }


                    } else {
                        while(true) {

                            // Only move on x and y axis NOT diagonal
                            if(r.Next(0, 2) == 0) {
                                newPosition = currentPosition + new Vector2(0, r.Next(-1, 2));
                            } else {
                                newPosition = currentPosition + new Vector2(r.Next(-1, 2), 0);
                            }

                            if (map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty()) {
                                break;
                            }
                        }
                    }

                    map[(int)currentPosition.X, (int)currentPosition.Y].RemoveGameObject(enemy);
                    map[(int)newPosition.X, (int)newPosition.Y].AddGameObject(enemy, (int)newPosition.X, (int)newPosition.Y);

/*                    if(playerPosition == newPosition) {
                        Console.SetCursorPosition(0, 0);
                        DrawDungeon();
                        p.WriteStats();
                        WriteControls();
                        Console.Write("An enemy ran into you!");

                        FightSequence(p, enemy, enemy);
                    }*/
                }
            }
        }

        private static void FightSequence(Player player, GameObject enemy, GameObject advantage)
        {
            gameState = GameState.InBattle;
            while (true)
            {
                Console.Clear();
                Console.Write($"Fighting enemy {enemy.GetName()}, {advantage.GetName()} has the advantage ");
                Console.WriteLine($"{player.GetGameObjectPosition()}, {enemy.GetGameObjectPosition()}");
                Console.ReadKey();

                var mapPosition = player.GetGameObjectPosition();
                map[(int)mapPosition.X, (int)mapPosition.Y].RemoveGameObject(enemy);
                enemy.SetHP(0);
                Console.Clear();
            }
        }

        static void PlayerInventory() {
            gameState = GameState.InInventory;
            Console.Clear();

            int inventoryCursorPositon = 0;
            int maxItemsLength = p.GetItems().Length - 1;

            while(true) {
                DrawInventory(inventoryCursorPositon);

                var key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        inventoryCursorPositon--;
                        if (inventoryCursorPositon < 0) inventoryCursorPositon = maxItemsLength;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        inventoryCursorPositon++;
                        if (inventoryCursorPositon > maxItemsLength) inventoryCursorPositon = 0;
                        break;
                    case ConsoleKey.X:
                    case ConsoleKey.Escape:
                        return;
                    default:
                        Console.SetCursorPosition(0, 0);
                        continue;
                }

                Console.SetCursorPosition(0, 0);
            }
        }

        static void PickUpNewItem(Vector2 pPosition)
        {
            var mapPosition = map[(int)pPosition.X, (int)pPosition.Y];
            var item = mapPosition.CheckIfItem();

            if(item == null) { 
                Console.WriteLine("You went to pick up an item but nothing is there");
                Console.ReadKey();
            }

            if(item != null) {
                Console.WriteLine($"You picked up {item.GetQTY()}x {item.GetName()}");
                p.AddItem(item);
                mapPosition.RemoveGameObject(item);
                Console.ReadKey();
            }
        }

        static void CreateDungeonRooms() {
            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    map[row, col] = new Room();
                }
            }
        }

        static void AddGameObjects(PowerUp[] pu, PickUpItem[] pui) {
            var r = new Random();


            // Add Player
            map[(int)(rowUserInputSize / 2), (int)(colUserInputSize / 2)].AddGameObject(p, (int)(rowUserInputSize / 2), (int)(colUserInputSize / 2));

            // Add powerups
            for (int i = 0; i < pu.Length; i++) {
                pu[i] = new PowerUp();

                while(true) {
                    var randomPosition = new Vector2(r.Next(1, rowUserInputSize - 1), r.Next(1, colUserInputSize - 1));
                    if (map[(int)randomPosition.X, (int)randomPosition.Y].CheckIfEmpty()) {
                        map[(int)randomPosition.X, (int)randomPosition.Y].AddGameObject(pu[i], (int)randomPosition.X, (int)randomPosition.Y);
                        break;
                    }
                }
            }

            // Add pickup items
            for (int i = 0; i < pui.Length; i++) {
                string[] itemTypes = new string[] { "Junk", "Health Potion", "Pokeball" , "Mana Potion", "Stick", "Strength Potion"};
                pui[i] = new PickUpItem(itemTypes[r.Next(0, itemTypes.Length)], r.Next(1, 5));

                while(true) {
                    var randomPosition = new Vector2(r.Next(1, rowUserInputSize - 1), r.Next(1, colUserInputSize - 1));
                    if (map[(int)randomPosition.X, (int)randomPosition.Y].CheckIfEmpty()) {
                        map[(int)randomPosition.X, (int)randomPosition.Y].AddGameObject(pui[i], (int)randomPosition.X, (int)randomPosition.Y);
                        break;
                    }
                }
            }

            // Add enemies
            for (int i = 0; i < e.Length; i++) {
                e[i] = new Enemy(1, 1, 1, i.ToString());

                while(true) {
                    var randomPosition = new Vector2(r.Next(1, rowUserInputSize - 1), r.Next(1, colUserInputSize - 1));
                    if (map[(int)randomPosition.X, (int)randomPosition.Y].CheckIfEmpty()) {
                        map[(int)randomPosition.X, (int)randomPosition.Y].AddGameObject(e[i], (int)randomPosition.X, (int)randomPosition.Y);
                        break;
                    }
                }
            }

            // Random Y position for locked door
            var randomCollumnWall = r.Next(4, colUserInputSize - 4); 

            // Add walls
            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    if (row == rowUserInputSize - 1 && col == randomCollumnWall) {
                        map[row, col].AddGameObject(new LockedDoor(), row, col);
                        continue;
                    }
                    if (col == 0 || col == rowUserInputSize - 1 || row == 0 || row == colUserInputSize - 1)
                        map[row, col].AddGameObject(new Wall(), row, col);
                }
            }
        }

        static void DrawDungeon() {
            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    map[row, col].Draw();
                }
                Console.WriteLine();
            }
        }

        static void DrawInventory(int inventoryCursor) {
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

                Console.WriteLine($"{items[i].GetQTY()}x {items[i].GetName()}");
                Console.ResetColor();
            }
        }

        static void WriteControls() {
            Console.WriteLine("                                         \n");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("   W    E < inventory \n A S D    F < pickup  ");
            Console.ResetColor();
        }

        static void InitTimer() {
            timer = new Timer(TimerCallback, null, 0, 1000);

        }

        static void TimerCallback(Object stateinfo) {
            if(gameState == GameState.InDungeon) { 
                EnemyAction();
                DrawMainDungeonScreen();
            }
        }
    }
}
