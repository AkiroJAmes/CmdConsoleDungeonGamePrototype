using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        static Timer timer;
        static string[] messages = new string[] { "Explore the dungeon...", null, null };
        static GameState gameState;

        enum GameState 
        {
            InDungeon,
            InInventory,
            InBattle
        }

        public enum BattleOptions
        {
            Attack,
            Defend,
            Item
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(rowUserInputSize * 2, colUserInputSize * 2);

            while (true) {
                gameState = GameState.InDungeon;

                Console.Clear();

                map = new Room[rowUserInputSize, colUserInputSize];
                CreateDungeonRooms();


                var r = new Random();
                p = new Player(5, 10, 2, 0, 1);
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

            DrawMainDungeonScreen();

            while (true) {
                if(gameState == GameState.InDungeon) { 
                    var pPosition = p.GetGameObjectPosition();

                    ConsoleKey actionUserInput = ConsoleKey.F24;

                    if(Console.KeyAvailable) {
                        actionUserInput = Console.ReadKey(true).Key;
                    } else {
                        continue;
                    }

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
                        case ConsoleKey.X:
                        case ConsoleKey.E:
                            PlayerInventory();
                            DrawMainDungeonScreen();
                            break;
                        case ConsoleKey.Z:
                        case ConsoleKey.F:
                            PickUpNewItem(pPosition);
                            break;
                        case ConsoleKey.P:
                            p.AddEXP(10);
                            p.WriteStats();
      

                            break;
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

                    if (!mapPosition.CheckIfEmpty() && gameState != GameState.InBattle) {
                        GameObject e = mapPosition.CheckIfEnemy();
                        if (e != null) {
                            RedrawObject(pPosition, newPlayerPosition);
                            gameState = GameState.InBattle;
                            AddMessageHistory($"You attacked {e.Name}!");

                            Console.ReadKey(true);

                            if (!FightSequence(e, p)) {
                                Console.Clear();
                                Console.Write("You lost!");
                                Console.ReadKey();
                                break;
                            } else {
                                Console.Clear();
                                Console.Write("You won!");
                                Console.ReadKey();
                                AddMessageHistory($"You won against {e.Name}!");
                                DrawMainDungeonScreen();
                            }

                            gameState = GameState.InDungeon;
                        }
                    }

                    RedrawObject(pPosition, newPlayerPosition);

                    EnemyAction();

                    timer = Utility.ResetEnemyTimer(timer);

                    RedrawObject(pPosition, newPlayerPosition);
                }
            }
        }

        private static void RedrawObject(Vector2 position, Vector2 newPosition)
        {
            Console.SetCursorPosition((int)position.X * 2, (int)position.Y);
            map[(int)position.X, (int)position.Y].Draw();

            Console.SetCursorPosition((int)newPosition.X * 2, (int)newPosition.Y);
            map[(int)newPosition.X, (int)newPosition.Y].Draw();
        }

        static void AddMessageHistory(string newMessage) {
            bool added = false;

            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i] == null)
                {
                    messages[i] = newMessage;
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                for (int i = 0; i < messages.Length - 1; i++)
                {
                    messages[i] = messages[i + 1];
                }

                messages[messages.Length - 1] = newMessage;
            }

            DrawMessageHistory();
        }

        static void DrawMessageHistoryBox() {
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

        static void DrawMessageHistory() {
            for (int i = messages.Length - 1; i >= 0; i--)
            {

                if (messages[i] != null) {
                    Console.SetCursorPosition(4, 26 + i);
                    Console.Write("                                     ");
                    Console.SetCursorPosition(4, 26 + i);
                    Console.Write(messages[i]);
                }
            }
        }

        // Prevent draw call overlap with lock
        private static readonly object myLock = new object();

        private static void DrawMainDungeonScreen()
        {
            lock (myLock) {
                Console.SetCursorPosition(0, 0);
                DrawDungeon();
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

                    RedrawObject(currentPosition, newPosition);

                    if(playerPosition == newPosition) {
                        RedrawObject(currentPosition, newPosition);
                        gameState = GameState.InBattle;
                        AddMessageHistory($"{enemy.Name} has attacked you!");

                        Console.ReadKey(true);

                        if (!FightSequence(enemy, enemy)) {
                            Console.Clear();
                            Console.Write("You lost!");
                            Console.ReadKey();
                            break;
                        } else {
                            Console.Clear();
                            Console.Write("You won!");
                            Console.ReadKey();
                            AddMessageHistory($"You won against {enemy.Name}!");
                            DrawMainDungeonScreen();
                        }
                        gameState = GameState.InDungeon;
                    }
                }
            }
        }

        private static bool FightSequence(GameObject enemy, GameObject advantage)
        {
            while (true)
            {
                Console.Clear();
                Console.Write($"Fighting enemy {enemy.Name}, {advantage.Name} has the advantage ");
                Console.WriteLine($"{p.GetGameObjectPosition()}, {enemy.GetGameObjectPosition()}");

                var input = Console.ReadKey(true).Key;

                int battleMenuPosition = 0;
                int maxBattleOptions = 2; // Attack Defend Item
                BattleOptions currentBattleOption = BattleOptions.Attack;

                while (true)
                {
                    currentBattleOption = DrawBattleMenu(battleMenuPosition, currentBattleOption);

                    var key = Console.ReadKey(true).Key;

                    if (p.IsAlive()) {
                        return false;
                    }
                    if (enemy.IsAlive()) {
                        p.AddEXP(new Random().Next(2, 5));
                        return true;
                    }

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            battleMenuPosition--;
                            currentBattleOption = Utility.Previous(currentBattleOption);
                            if (battleMenuPosition < 0) battleMenuPosition = maxBattleOptions;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            battleMenuPosition++;
                            currentBattleOption = Utility.Next(currentBattleOption);
                            if (battleMenuPosition > maxBattleOptions) battleMenuPosition = 0;
                            break;
                        case ConsoleKey.Z:
                        case ConsoleKey.E:
                            BattleAction(currentBattleOption, enemy);
                            Console.SetCursorPosition(0, 1);
                            continue;
                        default:
                            Console.SetCursorPosition(0, 1);
                            continue;
                    }

                    Console.SetCursorPosition(0, 1);
                }
            }
        }

        private static void BattleAction(BattleOptions currentBattleOption, GameObject enemy)
        {
            switch (currentBattleOption)
            {
                case BattleOptions.Attack:
                    Console.Write("Attacking the enemy, this is a long message to appear in the console");
                    var mapPosition = p.GetGameObjectPosition();
                    map[(int)mapPosition.X, (int)mapPosition.Y].RemoveGameObject(enemy);
                    enemy.Hp = 0;
                    break;
                case BattleOptions.Defend:
                    Console.Write("Defending to take less damage, this is a long message to appear in the console");
                    break;
                case BattleOptions.Item:
                    Console.Write("Using an item, this is a long message to appear in the console ");
                    break;
                default:
                    break;
            }
        }

        private static BattleOptions DrawBattleMenu(int battleMenuPosition, BattleOptions currentBattleOption)
        {
            BattleOptions[] arr = (BattleOptions[])Enum.GetValues(currentBattleOption.GetType());

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

                Console.WriteLine($"\r{arr[i]}");
                Console.ResetColor();
            }

            return currentBattleOption;
        }

        static void PlayerInventory() {
            gameState = GameState.InInventory;
            Console.Clear();

            int inventoryCursorPositon = 0;
            int maxItemsLength = p.GetItems().Length - 1;

            while(true) {
                DrawInventory(inventoryCursorPositon);

                var key = Console.ReadKey(true).Key;

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
                        Console.Clear();
                        gameState = GameState.InDungeon;
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

            if(item != null) {
                AddMessageHistory($"You picked up {item.Qty}x {item.Name}");
                p.AddItem(item);
                mapPosition.RemoveGameObject(item);
            }

            if (map[(int)pPosition.X + 1, (int)pPosition.Y].CheckIfKeyWall())
            {
                var items = p.GetItems();

                foreach (var i in items)
                {
                    if (i.Name == "Strange Key") {
                        map[(int)pPosition.X + 1, (int)pPosition.Y].RemoveGameObject(map[(int)pPosition.X + 1, (int)pPosition.Y].GetGameObjects()[0]);
                        map[(int)pPosition.X + 1, (int)pPosition.Y].Draw();
                        return;
                    }
                }

                AddMessageHistory($"You need a key to open this door");
                return;
            }

            if (item == null)
            {
                AddMessageHistory("Nothing is there to pick up");
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
            map[2, 2].AddGameObject(p, 2, 2);

            // Random Y position for locked door
            var randomCollumnWall = r.Next(4, colUserInputSize - 4);

            // Add walls
            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    if (row == rowUserInputSize - 1 && col == randomCollumnWall) {
                        map[row, col].AddGameObject(new LockedDoor(), row, col);
                        continue;
                    }
                    if (col == 0 || col == rowUserInputSize - 1 || row == 0 || row == colUserInputSize - 1) {

                        if((row == 0 && col == 0) || (row == rowUserInputSize - 1 && col == 0) /*|| (row == 0 && col == colUserInputSize - 1) || (row == rowUserInputSize - 1 && col == colUserInputSize - 1)*/) {
                            map[row, col].AddGameObject(new CornerWall(), row, col);
                            continue;
                        }

                        if (col == 0) map[row, col].AddGameObject(new TopWall(), row, col);
                        if(col == colUserInputSize - 1) map[row, col].AddGameObject(new BottomWall(), row, col);
                        if (row == 0) map[row, col].AddGameObject(new LeftWall(), row, col);
                        if (row == rowUserInputSize - 1) map[row, col].AddGameObject(new RightWall(), row, col);
                    }
                }
            }

            var randomRowWall = r.Next(0, rowUserInputSize - (rowUserInputSize / 2));
            var randomColCorner = r.Next(colUserInputSize - 5, colUserInputSize - 1);

            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    if(col > randomColCorner && row < randomRowWall)
                    {
                        foreach (var gameObject in map[row, col].GetGameObjects())
                        {
                            if(gameObject != null)
                            {
                                map[row, col].RemoveGameObject(gameObject);
                            }
                        }

                        map[row, col].AddGameObject(new Void(), row, col);
                    }

                    if (col == randomColCorner && row == 0) {
                        map[row, col].AddGameObject(new LeftWall(), row, col);
                    }

                    else if (col == randomColCorner && row <= randomRowWall)
                    {
                        map[row, col].AddGameObject(new TopWall(), row, col);
                    }

                    if(row == randomRowWall && col > randomColCorner)
                    {
                        if(col == randomColCorner)
                        {
                            map[row, col].AddGameObject(new LeftWallDetail(), row, col);
                        }
                        map[row, col].AddGameObject(new LeftWall(), row, col);
                    }
                }
            }

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
                string[] itemTypes = new string[] { "Junk", "Health Potion", "Pokeball" , "Mana Potion", "Stick", "Strength Potion", "Strange Key"};
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
                e[i] = new Enemy(1, r.Next(9, 12), 1, i.ToString());

                while(true) {
                    var randomPosition = new Vector2(r.Next(1, rowUserInputSize - 1), r.Next(1, colUserInputSize - 1));
                    if (map[(int)randomPosition.X, (int)randomPosition.Y].CheckIfEmpty()) {
                        map[(int)randomPosition.X, (int)randomPosition.Y].AddGameObject(e[i], (int)randomPosition.X, (int)randomPosition.Y);
                        break;
                    }
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

                Console.WriteLine($"{items[i].Qty}x {items[i].Name}");
                Console.ResetColor();
            }
        }

        static void WriteControls() {
            Console.SetCursorPosition(10, 31);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("   W    E < inventory ");
            Console.SetCursorPosition(10, 32);
            Console.Write(" A S D    F < pickup  ");
            Console.ResetColor();
        }

        static void InitTimer() {
            timer = new Timer(TimerCallback, null, 1000, 1000);

        }
        public static void TimerCallback(Object stateinfo) {
            if(gameState == GameState.InDungeon) { 
                EnemyAction();
            }
        }
    }
}
