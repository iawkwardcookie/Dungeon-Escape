using System;

namespace Dungeon_Escape
{
    class Game
    {
        public Keys keys = new();
        private Key key1 = new();
        private Key key2 = new();
        private Key key3 = new();
        private Key key4 = new();
        private Key key5 = new();
        private Key key6 = new();
        private Key key7 = new();
        private Player player1 = new();
        private Player player2 = new();
        private Enemy enemy = new();
        private Board dungeon = new();
        public Game(int difficulty, int players)
        {
            SetUpGame(difficulty, players);
            Play();
        }
        private void Play()
        {

            enemy.Visibility = 3;
            dungeon.DisplayAll(enemy);
            if (keys.KeyCount>0)
                Board.DisplaySideMenu(keys.KeyCount, true);

            bool exit = false;

            while (!exit)
                exit = Turn();
        }

        private bool Turn()
        {
            char direction; //direction of player movement
            int type; //either player1 (1, wasd) or player2 (2, arrow keys) movement, or exit (3, escape key)

            bool validInput = false;

            // get move from user, take direction and type //
            var input = GetMove(); 
            direction = input.Item1;
            type = input.Item2;

            if (!player1.onKey)
                dungeon.DisplayMessage(1, true, true, false);

            if(!player2.onKey)
                dungeon.DisplayMessage(2, true, true, false);

            if (keys.HoldingKeys(player1))
                dungeon.DisplayMessage(1, false, false, false);
            else if (!keys.HoldingKeys(player1))
                dungeon.DisplayMessage(1, false, true, false);

            if (keys.HoldingKeys(player2))
                dungeon.DisplayMessage(2, false, false, false);
            else if (!keys.HoldingKeys(player2))
                dungeon.DisplayMessage(2, false, true, false);

            switch (type)
            {
                case 0:
                    // pause menu //
                    Console.Clear();
                    //Console.WriteLine("PAUSE MENU\n\nPress any key to continue");

                    int choice = MenuSetup.MenuDisplay(3);
                    switch (choice)
                    {
                        case 0:
                            dungeon.DisplayAll(enemy);
                            if (keys.KeyCount > 0)
                            {
                                Board.DisplaySideMenu(keys.KeyCount, false);
                            }
                            return false;
                        case 1:
                            Console.WriteLine();
                            Console.ReadKey();
                            return false;
                        case 2:
                            Console.WriteLine();
                            Console.ReadKey();
                            return false;
                        case 3:
                            return true;
                    }
                    break;
                case 1:
                    if (player1.ItemMovement(direction, keys))
                        dungeon.DisplayMessage(1, true, false, false);
                    validInput = true;
                    break;
                case 2:
                    if (player2.GetCoord().x != 0)
                    {
                        if (player2.ItemMovement(direction, keys))
                            dungeon.DisplayMessage(2, true, false, false);
                        validInput = true;
                    }
                    break;
                case 3:
                    if (direction == 'P')
                    {
                        if (keys.PickKey(player1))
                        {
                            dungeon.DisplayMessage(1, true, true, false);
                            dungeon.DisplayMessage(1, true, false, true);
                        }
                    }
                    else
                    {
                        if (keys.DropKey(player1))
                        {
                            dungeon.DisplayMessage(1, false, true, false);
                            dungeon.DisplayMessage(1, false, false, true);
                        }
                    }
                    break;
                case 4:
                    if (direction == 'P')
                        if (keys.PickKey(player2))
                        {
                            dungeon.DisplayMessage(2, true, true, false);
                            dungeon.DisplayMessage(2, true, false, true);
                        }
                    else
                    {
                        if (keys.DropKey(player2))
                        {
                            dungeon.DisplayMessage(2, false, true, false);
                            dungeon.DisplayMessage(2, false, false, true);
                        }
                    }
                    break;
            }

            if (validInput)
            {
                Coord wanderCoord = default;
                enemy.onKey = enemy.ItemMovement(enemy.GetNextMove(wanderCoord, true), keys);
            }

            return false;
        }


        //game setup//
        //
        private void SetUpGame(int difficulty, int players)
        {
            dungeon.SetBoardLayout(0); //board layout generated

            //player coordinate(s) generated, placed on board//
            player1.SetCoord(dungeon.SetItemPos(player1.num));
            dungeon.PlaceObject(player1.GetCoord(), player1.num);

            if (players == 1) //if two players, set up other player
            {
                player2.SetCoord(dungeon.SetItemPos(player2.num));
                dungeon.PlaceObject(player2.GetCoord(), player2.num);
            }

            //key coordinates generated, placed on board//

            //if difficulty = -1, no keys are placed (debug)
            switch (difficulty)
            {
                case >= 2: //difficulty high or extreme:
                    keys.CreateKeysList(key6, key7);
                    break;
                case >= 1: //difficulty medium or higher:
                    keys.CreateKeysList(key4, key5);
                    break;
                case >= 0: //difficulty easy or higher:
                    keys.CreateKeysList(key1, key2, key3);
                    break;
            }

            enemy.SetCoord(dungeon.SetItemPos(enemy.num));
            dungeon.PlaceObject(enemy.GetCoord(), enemy.num);
        }
        //

        //deals with movement//
        //
        private Tuple<char, int> GetMove()
        {
            //takes user input, ouputs direction and which player(1,2) or invalid input(0)//
            ConsoleKey key = Console.ReadKey(true).Key;

            char direction;
            int type;

            direction = key switch
            {
                ConsoleKey.W or ConsoleKey.UpArrow => 'N',
                ConsoleKey.D or ConsoleKey.RightArrow => 'E',
                ConsoleKey.S or ConsoleKey.DownArrow => 'S',
                ConsoleKey.A or ConsoleKey.LeftArrow => 'W',
                ConsoleKey.C or ConsoleKey.Insert => 'P',
                ConsoleKey.V or ConsoleKey.Delete => 'D',
                _ => ' '
            };

            type = key switch
            {
                ConsoleKey.Escape => 0,
                ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D => 1,
                ConsoleKey.UpArrow or ConsoleKey.RightArrow or ConsoleKey.DownArrow or ConsoleKey.LeftArrow => 2,
                ConsoleKey.C or ConsoleKey.V => 3, //plyr 1 pick and drop
                ConsoleKey.Insert or ConsoleKey.Delete => 4, //plyr 2 pick and drop
                _ => 0
            };

            return Tuple.Create(direction, type);
        }
        //
    }
}
