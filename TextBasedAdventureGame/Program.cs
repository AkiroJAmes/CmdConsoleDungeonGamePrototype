using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

using static AdventureGame.DrawCalls;
using static AdventureGame.Utility;

namespace AdventureGame
{
    internal class Program
    {
        public static int rowUserInputSize = 22;
        public static int colUserInputSize = 22;

        public static Room[,] map;
        static object[] maps = new object[0];
        static Enemy[] e;
        public static Player p;

        static int levelTrack;

        public static int bestFloor = 1;
        public static int bestLevel = 1;
        public static int bestEXP = 0;

        static Timer timer;
        public static string[] dungeonMessages;
        public static string[] battleMessages;

        public static GameState gameState;

        public enum GameState 
        {
            NewGame,
            InDungeon,
            InInventory,
            InBattle,
            Dead,
            Escaped,
        }

        public enum MenuButtonState
        {
            Start,
            Legends,
            Controls,
            Credits,
            Exit
        }

        public enum BattleOption
        {
            Attack,
            Defend,
            Item
        }

        enum EnemyBattleState { 
            Attack,
            Defend,
            Run
        }

        public enum EnemyBattleAIState
        {
            Brute,
            Defensive,
            Coward,
            Random
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(rowUserInputSize * 2, colUserInputSize * 2);

            Console.Clear();

            int menuCursorPosition = 0;
            MenuButtonState currenMenuOption = MenuButtonState.Start;

            while (true) {
                // Reset game
                dungeonMessages = new string[] { "Explore the dungeon...", null, null };
                battleMessages = new string[] { null, null, null };
                gameState = GameState.NewGame;
                levelTrack = -1;

                DrawMainMenu(menuCursorPosition);
                var key = Console.ReadKey(true).Key;

                // Menu scrolling
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        menuCursorPosition--;
                        currenMenuOption = Previous(currenMenuOption);
                        if (menuCursorPosition < 0) menuCursorPosition = 4;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        menuCursorPosition++;
                        currenMenuOption = Next(currenMenuOption);
                        if (menuCursorPosition > 4) menuCursorPosition = 0;
                        break;
                    case ConsoleKey.Z:
                    case ConsoleKey.F:
                    case ConsoleKey.Enter:
                        MenuAction(currenMenuOption);
                        DrawMainMenu(menuCursorPosition);
                        break;
                    default:
                        Console.SetCursorPosition(0, 0);
                        continue;
                }
            }
        }

        private static void MenuAction(MenuButtonState currenMenuOption)
        {
            switch (currenMenuOption)
            {
                case MenuButtonState.Start:
                     CreateNewMap();
                     GameLoop();

                     Console.Clear();

                    break;
                case MenuButtonState.Legends:
                    DrawLegend();
                    break;
                case MenuButtonState.Controls:
                    DrawControls();
                    break;
                case MenuButtonState.Credits:
                    DrawCredits();
                    break;
                case MenuButtonState.Exit:
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }

            Console.Clear();
        }

        private static void CreateNewMap()
        {
            if (gameState == GameState.Escaped || gameState == GameState.NewGame) {
                levelTrack++;
                var mapsRef = maps;
                maps = new object[maps.Length + 1];

                for (int i = 0; i < mapsRef.Length; i++)
                {
                    maps[i] = mapsRef[i];
                }

                map = new Room[rowUserInputSize, colUserInputSize];

                maps[maps.Length - 1] = map;
            }

            map = (Room[,])maps[levelTrack];

            if (gameState == GameState.NewGame)
                p = new Player(5, 10, 3, 0, 1);

            Console.Clear();

            if (gameState == GameState.NewGame || gameState == GameState.Escaped)
            {
                CreateDungeonRooms();
                var r = new Random();
                e = new Enemy[r.Next(2, 4)];
                PowerUp[] pu = new PowerUp[r.Next(0, 2)];
                PickUpItem[] pui = new PickUpItem[r.Next(4, 7)];

                AddGameObjects(pu, pui);

                InitTimer();
            }

            gameState = GameState.InDungeon;
        }

        static void GameLoop()
        {
            DrawMainDungeonScreen(map, colUserInputSize, rowUserInputSize);

            while (gameState != GameState.Dead) {
                if(gameState == GameState.InDungeon) { 
                    ConsoleKey actionUserInput = ConsoleKey.F24;

                    if(Console.KeyAvailable) {
                        actionUserInput = Console.ReadKey(true).Key;
                    } else {
                        continue;
                    }

                    if(MovePlayer(actionUserInput)) continue;

                    EnemyAction();

                    timer = ResetEnemyTimer(timer);
                }
            }

            if (gameState == GameState.Dead) timer.Dispose();
        }

        private static bool MovePlayer(ConsoleKey actionUserInput)
        {
            Vector2 newPlayerPosition = Vector2.Zero;
            var pPosition = p.GetGameObjectPosition();

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
                    PlayerInventory(GameState.InDungeon);
                    gameState = GameState.InDungeon;
                    DrawMainDungeonScreen(map, colUserInputSize, rowUserInputSize);
                    break;
                case ConsoleKey.Z:
                case ConsoleKey.F:
                    PickUpNewItem(pPosition);
                    break;
                case ConsoleKey.F1:
                    p.AddEXP(10);
                    AddMessageHistory($"Debug testing: Added 10 exp", true);
                    p.WriteStats(23);
                    break;
                case ConsoleKey.F2:
                    p.AddItem(new Key("Debug Key", 1));
                    AddMessageHistory($"Debug testing: Added key", true);
                    break;
                case ConsoleKey.F3:
                    p.AddItem(new GreaterHealthPotion("Debug Potion", 1));
                    AddMessageHistory($"Debug testing: Added potion", true);
                    break;
                case ConsoleKey.F4:
                    AddMessageHistory($"Debug testing: Killing player", true);
                    PlayerDead(true);
                    return true;
            }


            if (newPlayerPosition.X == rowUserInputSize)
            {
                gameState = GameState.Escaped;
                map[(int)pPosition.X, (int)pPosition.Y].RemoveGameObject(p);
                timer.Dispose();
                CreateNewMap();
                DrawMainDungeonScreen(map, colUserInputSize, rowUserInputSize);
                return false;
            }

            if (newPlayerPosition.X == -1)
            {
                AddMessageHistory("The way back is blocked", true);
                return true;
            }

            // Check if new player position is valid
            Room mapPosition = map[(int)newPlayerPosition.X, (int)newPlayerPosition.Y];

            if (!mapPosition.CheckIfEmpty() && mapPosition.CheckIfWall())
            {
                return true;
            }

            switch (actionUserInput)
            {
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

            if (!mapPosition.CheckIfEmpty() && gameState != GameState.InBattle)
            {
                Enemy e = (Enemy)mapPosition.CheckIfEnemy();
                if (e != null)
                {
                    RedrawObject(pPosition, newPlayerPosition);
                    gameState = GameState.InBattle;
                    if (e.MySpecies == Enemy.Species.Mimic)
                    {
                        AddMessageHistory($"{e.Name} attcked you!", true);
                    }
                    else
                        AddMessageHistory($"You attacked {e.Name}!", true);

                    Console.ReadKey(true);

                    FightSequence(e, p);
                    if (gameState == GameState.Dead) return true;

                    gameState = GameState.InDungeon;
                }
            }

            RedrawObject(pPosition, newPlayerPosition);

            return false;
        }

        public static void AddMessageHistory(string newMessage, bool redraw) {
            bool added = false;

            for (int i = 0; i < dungeonMessages.Length; i++)
            {
                if (dungeonMessages[i] == null)
                {
                    dungeonMessages[i] = newMessage;
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                for (int i = 0; i < dungeonMessages.Length - 1; i++)
                {
                    dungeonMessages[i] = dungeonMessages[i + 1];
                }

                dungeonMessages[dungeonMessages.Length - 1] = newMessage;
            }

            if(redraw)
                DrawMessageHistory();
        }

        public static void AddBattleMessageHistory(string newMessage, bool redraw) {
            bool added = false;

            for (int i = 0; i < battleMessages.Length; i++)
            {
                if (battleMessages[i] == null)
                {
                    battleMessages[i] = newMessage;
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                for (int i = 0; i < battleMessages.Length - 1; i++)
                {
                    battleMessages[i] = battleMessages[i + 1];
                }

                battleMessages[battleMessages.Length - 1] = newMessage;
            }

            if (redraw)
                DrawBattleMessageHistory();
        }

        private static void EnemyAction()
        {
            foreach (var enemy in e) {
                if(enemy != null && !enemy.IsAlive()) { 
                    var currentPosition = enemy.GetGameObjectPosition();
                    var playerPosition = p.GetGameObjectPosition();

                    // How far the enemy can see the player
                    var enemyViewDistance = enemy.ViewDistance;

                    var newPosition = currentPosition;
                    var r = new Random();

                    // Move towards the player otherwise choose a random direction
                    if(Vector2.Distance(currentPosition, playerPosition) < enemyViewDistance) {
                        for (int i = 0; i < 10; i++)
                        {
                            switch (enemy.MySpecies)
                            {
                                case Enemy.Species.Spider:
                                    if (playerPosition.Y < currentPosition.Y && playerPosition.X > currentPosition.X)
                                    {
                                        newPosition = currentPosition + new Vector2(1, -1);
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(-1, 1);
                                    }
                                    else if (playerPosition.Y < currentPosition.Y && playerPosition.X < currentPosition.X)
                                    {
                                        newPosition = currentPosition + -Vector2.One;
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(1, -1);
                                    }
                                    else if (playerPosition.Y < currentPosition.Y && playerPosition.X == currentPosition.X) {
                                        newPosition = currentPosition + -Vector2.One;
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(1, -1);
                                    }
                                    else if (playerPosition.Y > currentPosition.Y && playerPosition.X == currentPosition.X)
                                    {
                                        newPosition = currentPosition + Vector2.One;
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(-1, 1);
                                    }
                                    else if (playerPosition.Y == currentPosition.Y && playerPosition.X > currentPosition.X)
                                    {
                                        newPosition = currentPosition + Vector2.One;
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(-1, 1);
                                    }
                                    else if (playerPosition.Y == currentPosition.Y && playerPosition.X < currentPosition.X)
                                    {
                                        newPosition = currentPosition + -Vector2.One;
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(1, -1);
                                    }
                                    else if (playerPosition.Y > currentPosition.Y && playerPosition.X > currentPosition.X)
                                    {
                                        newPosition = currentPosition + new Vector2(1, -1);
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(-1, 1);
                                    }
                                    else if (playerPosition.Y > currentPosition.Y && playerPosition.X < currentPosition.X)
                                    {
                                        newPosition = currentPosition + new Vector2(-1, 1);
                                        if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && playerPosition != newPosition)
                                            newPosition = currentPosition + new Vector2(1, -1);
                                    }

                                    if (newPosition == playerPosition) goto End;

                                    if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty())
                                    {
                                        int[] move = new int[] { -1, 1 };
                                        newPosition = currentPosition + new Vector2(move[r.Next(0, move.Length)], move[r.Next(0, move.Length)]);
                                    }

                                    break;
                                case Enemy.Species.Mimic:
                                    enemy.ISActive = true;
                                    break;
                                case Enemy.Species.Skeleton:
                                    if (playerPosition.Y < currentPosition.Y) { newPosition = currentPosition + -Vector2.UnitY; }
                                    if (playerPosition.Y > currentPosition.Y) { newPosition = currentPosition + Vector2.UnitY; }
                                    break;
                                case Enemy.Species.Rat:
                                case Enemy.Species.Bat:
                                    if (playerPosition.Y < currentPosition.Y) { newPosition = currentPosition + -Vector2.UnitY; }
                                    if (playerPosition.Y > currentPosition.Y) { newPosition = currentPosition + Vector2.UnitY; }
                                    if (playerPosition.X < currentPosition.X) { newPosition = currentPosition + -Vector2.UnitX; }
                                    if (playerPosition.X > currentPosition.X) { newPosition = currentPosition + Vector2.UnitX; }
                                    break;
                                default:
                                    break;
                            }

                            if (map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() || newPosition == playerPosition) goto End;
                        }

                        newPosition = currentPosition;

                        End:;
                    } else {
                        while(true) {

                            switch (enemy.MySpecies)
                            {
                                case Enemy.Species.Spider: // Diagonal
                                    int[] move = new int[] { -1, 1 };
                                    newPosition = currentPosition + new Vector2(move[r.Next(0, move.Length)], move[r.Next(0, move.Length)]);
                                    break;
                                case Enemy.Species.Mimic: // Dont move
                                    enemy.ISActive = false;
                                    goto End;
                                case Enemy.Species.Rat: // Random
                                    if (r.Next(0, 2) == 0) newPosition = currentPosition + new Vector2(0, r.Next(-1, 2));
                                    else newPosition = currentPosition + new Vector2(r.Next(-1, 2), 0);
                                    break;
                                case Enemy.Species.Skeleton: // Vertical
                                    newPosition = currentPosition + new Vector2(0, r.Next(-1, 2));
                                    break;
                                case Enemy.Species.Bat: // Chance to idle
                                    if(r.Next(0, 5) <= 3) {
                                        if (r.Next(0, 2) == 0) newPosition = currentPosition + new Vector2(0, r.Next(-1, 2));
                                        else newPosition = currentPosition + new Vector2(r.Next(-1, 2), 0);
                                    } else newPosition = currentPosition;

                                    if (!map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty() && newPosition == currentPosition) goto End;

                                    break;
                            }

                            if (map[(int)newPosition.X, (int)newPosition.Y].CheckIfEmpty()) break;
                        }

                        End:;
                    }

                    map[(int)currentPosition.X, (int)currentPosition.Y].RemoveGameObject(enemy);
                    map[(int)newPosition.X, (int)newPosition.Y].AddGameObject(enemy, (int)newPosition.X, (int)newPosition.Y);

                    RedrawObject(currentPosition, newPosition);

                    if(playerPosition == newPosition) {
                        RedrawObject(currentPosition, newPosition);
                        gameState = GameState.InBattle;
                        AddMessageHistory($"A {enemy.Name} has attacked you!", true);

                        Console.ReadKey(true);

                        FightSequence(enemy, enemy);
                        if (gameState == GameState.Dead)
                        {
                            timer.Dispose();
                            return;
                        }

                        gameState = GameState.InDungeon;
                    }
                }
            }
        }

        private static void FightSequence(Enemy enemy, GameObject advantage)
        {
            gameState = GameState.InBattle;
            while (true)
            {
                Console.Clear();
                DrawBattleScreen(enemy);

                if(advantage == enemy || enemy.MySpecies == Enemy.Species.Mimic) {
                    AddBattleMessageHistory($"{enemy.Name} has first turn", true);
                    EnemyBattleAction(EnemyBattleState.Attack, BattleOption.Attack, true, enemy);
                    p.WriteStats(21);

                    if (p.IsAlive()) {
                        PlayerDead(false);
                        return;
                    }
                }

                var input = Console.ReadKey(true).Key;

                int battleMenuPosition = 0;
                int maxBattleOptions = 2; // Attack Defend Item
                BattleOption currentBattleOption = BattleOption.Attack;

                while (true)
                {

                    DrawBattleScreen(enemy);

                    currentBattleOption = DrawBattleMenu(battleMenuPosition, currentBattleOption);

                    if (p.IsAlive()) {
                        PlayerDead(true);
                        return;
                    }
                    if (enemy.IsAlive()) {
                        var enemyName = enemy.Name;

                        if(enemy.Hp == 0) {
                            p.AddEXP(new Random().Next(2, 5));

                            AddBattleMessageHistory($"You won against the {enemyName}!", true);
                            Console.ReadKey(true);
                            Console.Clear();
                            AddMessageHistory($"You won against the {enemyName}!", true);
                        }

                        if(enemy.Hp == -1) {
                            AddBattleMessageHistory($"The {enemyName} fled!", true);
                            Console.ReadKey(true);
                            Console.Clear();
                            AddMessageHistory($"The {enemyName} fled!", true);
                        }

                        for (int i = 0; i < e.Length; i++) {
                            if (e[i] == enemy) e[i] = null;
                        }

                        foreach (var e in e) {
                            if (e != null) goto SkipKey;
                        }

                        p.AddItem(new Key("Strange Key", 1));
                        AddMessageHistory($"The {enemyName} dropped 1x Strange Key", true);

                        SkipKey:

                        DrawMainDungeonScreen(map, colUserInputSize, rowUserInputSize);
                        battleMessages = new string[] { null, null, null };

                        return;
                    }

                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            battleMenuPosition--;
                            currentBattleOption = Previous(currentBattleOption);
                            if (battleMenuPosition < 0) battleMenuPosition = maxBattleOptions;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            battleMenuPosition++;
                            currentBattleOption = Next(currentBattleOption);
                            if (battleMenuPosition > maxBattleOptions) battleMenuPosition = 0;
                            break;
                        case ConsoleKey.Z:
                        case ConsoleKey.F:
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

        private static void BattleAction(BattleOption currentBattleOption, Enemy enemy)
        {
            var enemyAction = EnemyBattleAction(enemy.Ai);

            if (enemyAction == EnemyBattleState.Defend && currentBattleOption != BattleOption.Item) {
                AddBattleMessageHistory($"The {enemy.Name} defended", true);
                Thread.Sleep(500);
            }

            switch (currentBattleOption)
            {
                case BattleOption.Attack:
                    int atDmg = 1;

                    if (enemyAction == EnemyBattleState.Defend) {
                        // Enemy defending calculation
                        atDmg = (int)(p.At * ((100 / (100 + (float)Math.Pow(enemy.Df, 1.5))) / 2));
                    }
                    else {
                        // Base attack calculation
                        atDmg = (int)(p.At * (100 / (100 + (float)Math.Pow(enemy.Df, 1.5))));
                    }

                    // Default to 1 dmg if damage is too low
                    if (atDmg < 1) atDmg = 1;


                    AddBattleMessageHistory($"Dealt {atDmg}dmg to the {enemy.Name}", true);

                    enemy.Hp -= atDmg;
                    UpdateEnemyDrawHP(enemy);

                    if (enemy.Hp <= 0) {
                        var mapPosition = p.GetGameObjectPosition();
                        map[(int)mapPosition.X, (int)mapPosition.Y].RemoveGameObject(enemy);
                        enemy.Hp = 0;

                        Thread.Sleep(500);
                        return;
                    }

                    //for (int i = 0; i < e.Length; i++) {
                    //    if (e[i] == enemy) e[i] = null;
                    //}

                    break;
                case BattleOption.Defend:
                    AddBattleMessageHistory("You took a defensive stance", true);
                    break;
                case BattleOption.Item:
                    PlayerInventory(GameState.InBattle);
                    gameState = GameState.InBattle;
                    DrawBattleScreen(enemy);
                    return;
                default:
                    break;
            }

            if (enemyAction == EnemyBattleState.Defend && currentBattleOption == BattleOption.Item) {
                AddBattleMessageHistory($"The {enemy.Name} defended", true);
            }

            Thread.Sleep(500);

            EnemyBattleAction(enemyAction, currentBattleOption, false, enemy);
            p.WriteStats(21);
        }

        private static void EnemyBattleAction(EnemyBattleState currentEnemyBattleAction, BattleOption currentBattleOption, bool firstTurn, Enemy enemy)
        {
            switch (currentEnemyBattleAction)
            {
                case EnemyBattleState.Attack:
                    int atDmg = 1;
                    if (currentBattleOption == BattleOption.Defend) {
                        // Enemy defending calculation
                        atDmg = (int)(enemy.At * ((100 / (100 + (float)Math.Pow(p.Df, 1.5))) / 2));
                    }
                    else {
                        // Base attack calculation
                        atDmg = (int)(enemy.At * (100 / (100 + (float)Math.Pow(p.Df, 1.5))));
                    }

                    if (atDmg < 1) atDmg = 1;

                    p.Hp -= atDmg;

                    AddBattleMessageHistory($"The {enemy.Name} did {atDmg}dmg!", true);

                    break;
                case EnemyBattleState.Run:
                    var mapPosition = p.GetGameObjectPosition();
                    map[(int)mapPosition.X, (int)mapPosition.Y].RemoveGameObject(enemy);
                    enemy.Hp = -1;
                    break;
            }
        }

        private static EnemyBattleState EnemyBattleAction(EnemyBattleAIState ai)
        {
            // Base odds
            (EnemyBattleState, double)[] odds = new[] { (EnemyBattleState.Attack, 4.0), (EnemyBattleState.Defend, 1.0), (EnemyBattleState.Run, 0.0) };

            switch (ai)
            {
                case EnemyBattleAIState.Defensive:
                    odds = new[] {
                        (EnemyBattleState.Attack, 2.0),
                        (EnemyBattleState.Defend, 3.0),
                        (EnemyBattleState.Run, 0.0)
                    };
                    break;
                case EnemyBattleAIState.Coward:
                    odds = new[] {
                        (EnemyBattleState.Attack, 0.0),
                        (EnemyBattleState.Defend, 0.0),
                        (EnemyBattleState.Run, 1.0)
                    };
                    break;
                case EnemyBattleAIState.Random:
                    odds = new[] {
                        (EnemyBattleState.Attack, 2.0),
                        (EnemyBattleState.Defend, 1.0),
                        (EnemyBattleState.Run, 0.0)
                    };
                    break;
            }

            double totalOdds = 0.0;

            foreach (var odd in odds)
            {
                totalOdds += odd.Item2;
            }

            double pick = new Random().NextDouble() * totalOdds;

            for (int i = 0; i < odds.Length; i++)
            {
                pick -= odds[i].Item2;
                if(pick <= 0) {
                    return odds[i].Item1;
                }
            }

            return odds.Last().Item1;
        }

        static void PlayerInventory(GameState currentGameState) {
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
                    case ConsoleKey.E:
                    case ConsoleKey.Escape:
                        Console.Clear();
                        return;
                    case ConsoleKey.Z:
                    case ConsoleKey.F:
                        try { 
                            if(p.GetItems()[inventoryCursorPositon].UseItem(p, (PickUpItem)p.GetItems()[inventoryCursorPositon], currentGameState)) {
                                if (currentGameState == GameState.InBattle) goto EndInventory;
                            } } catch { break; }

                        DrawInventory(inventoryCursorPositon);
                        break;
                    default:
                        Console.SetCursorPosition(0, 0);
                        continue;
                }

                Console.SetCursorPosition(0, 0);
            }

            EndInventory:;
            Console.Clear();
        }

        private static void PlayerDead(bool dispose)
        {
            if(dispose)
                timer.Dispose();

            Console.ReadKey(true);
            DrawLost(levelTrack + 1, p);
            gameState = GameState.Dead;
            Console.ReadKey(true);
        }

        static void PickUpNewItem(Vector2 pPosition)
        {
            var mapPosition = map[(int)pPosition.X, (int)pPosition.Y];
            var item = mapPosition.CheckIfItem();

            if(item != null) {
                AddMessageHistory($"You picked up {item.Qty}x {item.Name}", true);
                p.AddItem((PickUpItem)item);
                mapPosition.RemoveGameObject(item);
            }

            if (pPosition.X + 1 < rowUserInputSize && map[(int)pPosition.X + 1, (int)pPosition.Y].CheckIfKeyWall())
            {
                var items = p.GetItems();

                foreach (var i in items)
                {
                    if (i.Name.Contains("Key")) {
                        AddMessageHistory($"Door unlocked with {i.Name}", true);
                        int posX = (int)pPosition.X + 1;
                        int posY = (int)pPosition.Y;


                        map[posX, posY].RemoveGameObject(map[posX, posY].GetGameObjects()[0]);
                        RedrawObject(new Vector2(posX, posY), new Vector2(posX, posY));

                        i.UseItem(p, (PickUpItem)i, gameState);
                        return;
                    }
                }

                AddMessageHistory($"You need a key to open this door", true);
                return;
            }

            if (item == null)
            {
                AddMessageHistory("Nothing is there to pick up", true);
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
            var pPosition = new Vector2();


            // Add Player
            if(gameState == GameState.NewGame)
                map[2, 2].AddGameObject(p, 2, 2);

            if(gameState == GameState.Escaped) {
                pPosition = new Vector2(0, r.Next(2, 10));
                map[(int)pPosition.X, (int)pPosition.Y].AddGameObject(p, (int)pPosition.X, (int)pPosition.Y);
            }

            // Random Y position for locked door
            var randomKeyPosition = r.Next(1, colUserInputSize - 4);

            // Add walls
            for (int col = 0; col < colUserInputSize; col++) {
                for (int row = 0; row < rowUserInputSize; row++) {
                    if (row == rowUserInputSize - 1 && col == randomKeyPosition) {
                        map[row, col].AddGameObject(new LockedDoor(), row, col);
                        continue;
                    }

                    if(gameState == GameState.Escaped && row == (int)pPosition.X && col == (int)pPosition.Y) {
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

            // Hard coding random map varition, yikes
            var randomRowWall = r.Next(3, rowUserInputSize - (rowUserInputSize / 2));
            var randomColCorner = r.Next(colUserInputSize - 8, colUserInputSize - 4);

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

            if(randomKeyPosition >= 1 && randomKeyPosition <= 6) {
                randomRowWall = r.Next(rowUserInputSize - 8, rowUserInputSize - 5);
                randomColCorner = r.Next(5, 15);

                for (int col = 0; col < colUserInputSize; col++)
                {
                    for (int row = 0; row < rowUserInputSize; row++)
                    {
                        if (col < randomColCorner && row > randomRowWall && row <= rowUserInputSize - 4)
                        {
                            foreach (var gameObject in map[row, col].GetGameObjects())
                            {
                                if (gameObject != null)
                                {
                                    map[row, col].RemoveGameObject(gameObject);
                                }
                            }

                            map[row, col].AddGameObject(new Void(), row, col);
                        }

                        if (col == randomColCorner && row >= randomRowWall + 1 && row <= rowUserInputSize - 4)
                        {
                            map[row, col].AddGameObject(new TopWall(), row, col);
                        }

                        if(row == randomRowWall + 1 && col <= randomColCorner && col > 0)
                        {
                            map[row, col].AddGameObject(new RightWall(), row, col);
                        }

                        if (row == rowUserInputSize - 4 && col <= randomColCorner && col > 0) {
                            map[row, col].AddGameObject(new LeftWall(), row, col);
                        }

                        if (row == randomRowWall + 1 && col == randomColCorner) {
                            map[row, col].AddGameObject(new BottomLeftCorner(), row, col);
                        }

                        if (row == rowUserInputSize - 4 && col == randomColCorner) {
                            map[row, col].AddGameObject(new BottomRightCorner(), row, col);
                        }
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

            PickUpItem[] itemList = new PickUpItem[] { 
                new LesserHealthPotion("Lesser Health Potion", r.Next(3, 5)), 
                new GreaterHealthPotion("Max Health Potion", r.Next(1, 3)),
                new PickUpItem("Stick", r.Next(1, 5)),
                new PickUpItem("Rock", r.Next(1, 5)),
                new PickUpItem("Pebble", r.Next(1, 5)),
                new PickUpItem("Junk", r.Next(1, 5))
            };

            // Add pickup items
            for (int i = 0; i < pui.Length; i++) {
                int index = r.Next(0, itemList.Length);
                pui[i] = itemList[index];

                switch (pui[i].MyItemType)
                {
                    case PickUpItem.ItemType.LesserHealthPotion:
                        pui[i] = new LesserHealthPotion(itemList[index]);
                        break;
                    default:
                        break;
                }

                while(true) {
                    var randomPosition = new Vector2(r.Next(1, rowUserInputSize - 1), r.Next(1, colUserInputSize - 1));
                    if (map[(int)randomPosition.X, (int)randomPosition.Y].CheckIfEmpty()) {
                        map[(int)randomPosition.X, (int)randomPosition.Y].AddGameObject(pui[i], (int)randomPosition.X, (int)randomPosition.Y);
                        break;
                    }
                }
            }

            Enemy[] enemyList = new Enemy[] {
                new Spider(r.Next(4, 6), 10, 4, "Spider", 5, EnemyBattleAIState.Brute),
                new Mimic(r.Next(3, 6), 15, 7, "Mimic", 0, EnemyBattleAIState.Defensive),
                new Rat(1, 3, 0, "Rat", 10, EnemyBattleAIState.Coward),
                new Skeleton(r.Next(4, 5), 10, 5, "Skeleton", 5, EnemyBattleAIState.Random),
                new Bat(3, r.Next(6, 10), 5, "Bat", 7, EnemyBattleAIState.Brute)
            };

            // Add enemies
            for (int i = 0; i < e.Length; i++) {
                int index = r.Next(0, enemyList.Length);

                e[i] = enemyList[index];

                switch (e[i].MySpecies)
                {
                    case Enemy.Species.Spider:
                        e[i] = new Spider(enemyList[index]);
                        break;
                    case Enemy.Species.Mimic:
                        e[i] = new Mimic(enemyList[index]);
                        break;
                    case Enemy.Species.Rat:
                        e[i] = new Rat(enemyList[index]);
                        break;
                    case Enemy.Species.Skeleton:
                        e[i] = new Skeleton(enemyList[index]);
                        break;
                    case Enemy.Species.Bat:
                        e[i] = new Bat(enemyList[index]);
                        break;
                }

                while (true) {
                    var randomPosition = new Vector2(r.Next(1, rowUserInputSize - 1), r.Next(1, colUserInputSize - 1));
                    if (map[(int)randomPosition.X, (int)randomPosition.Y].CheckIfEmpty()) {
                        map[(int)randomPosition.X, (int)randomPosition.Y].AddGameObject(e[i], (int)randomPosition.X, (int)randomPosition.Y);
                        break;
                    }
                }
            }
        }

        static void InitTimer() {
            timer = new Timer(TimerCallback, null, 1000, 1000);

        }

        public static void TimerCallback(Object stateinfo) {
            if(gameState == GameState.InDungeon && gameState != GameState.Dead) { 
                EnemyAction();
            }
        }
    }
}
