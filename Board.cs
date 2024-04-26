using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Escape
{
    public struct Coord
    {
        public int x, y;
    }
    class Board
    {
        //ratio 3x : 2y//
        public const int
                    maxX = 15,  //from left to right
                    maxY = 10,  //from top to bottom
                    width = 6,  //space between cell values (x)
                    height = 2; //space between rows (y)

        private const char
                    NESW = '\u256c',    //╬
                    NES = '\u2560',     //╠
                    NSW = '\u2563',     //╣
                    NEW = '\u2569',     //╩
                    ESW = '\u2566',     //╦
                    NE = '\u255a',      //╚
                    SW = '\u2557',      //╗
                    NW = '\u255d',      //╝
                    ES = '\u2554',      //╔
                    NS = '\u2551',      //║
                    EW = '\u2550',      //═

                    EMPT = '\u00b7',    //·
                    KEY = '\u00b6',     //¶   u00d7 ×
                    DOOR = '\u00a6',    //¦

                    PLYR = '\u2588',    //█ u2588       //■ u25a0
                    ENMY0 = ' ',        //_
                    ENMY1 = '\u2591',   //░
                    ENMY2 = '\u2592',   //▒
                    ENMY3 = '\u2593';   //▓

        private readonly static char[]
                    ENMY = { ENMY0, ENMY1, ENMY2, ENMY3 };   //_,░,▒,▓

        /* random board gen
        //checks cells to north//
        private static bool EqualsUp(int i, int j, int[,] matrix, int value)
            => i >= 0 && matrix[i, j] == value;

        //checks cells to west//
        private static bool EqualsLeft(int i, int j, int[,] matrix, int value)
            => j >= 0 && matrix[i, j] == value;

        public static int[,] GenerateEmptySpaces(int y, int x)
        {
            var matrix = new int[y, x];
            var random = new Random();

            for (int i = 0; i < y; ++i)
                for (int j = 0; j < x; ++j)
                    while (true)
                    {
                        int cellValue = random.Next(0, 3); // generates 0,1,2
                        if (EqualsUp(i - 1, j, matrix, cellValue) && EqualsUp(i - 2, j, matrix, cellValue))
                            continue; // need to regenerate cellValue
                        if (EqualsLeft(i, j - 1, matrix, cellValue) && EqualsLeft(i, j - 2, matrix, cellValue))
                            continue; // need to regenerate cellValue

                        matrix[i, j] = cellValue;
                        break;
                    }
            return matrix;
        }
        */
        private static int[,] boardLayout = new int[maxY, maxX];


        public int[,] BoardLayout
        {
            get
            {
                return boardLayout;
            }
        }
        public void SetBoardLayout(int difficulty)
        {
            if (difficulty == 0 || difficulty == -1)
                //0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 
                boardLayout = new int[maxY, maxX]
                {{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}, // 0 = empty
                {1,0,1,0,0,0,0,1,0,0,0,1,0,0,1}, // 1 = wall
                {1,0,1,1,0,0,0,1,1,1,0,1,1,0,1}, // 2 = key
                {1,0,0,0,0,1,0,1,0,0,0,0,1,0,1}, // 3 = door
                {1,1,1,0,0,1,0,1,0,1,1,0,1,0,1}, // 4 = player(s)
                {1,0,0,0,0,1,0,0,0,0,1,0,0,0,1}, // 5 = enemy(s)
                {1,0,1,1,0,1,1,1,1,0,1,0,1,1,1},
                {1,0,0,1,1,1,0,0,1,0,1,0,1,0,1},
                {1,0,0,1,0,0,0,0,0,0,1,0,0,0,3},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};
        }

        //deals with object positions//

        //

        public Coord GetRandomCoord()
        {
            Coord position;
            Random rnd = new();
            do
            {
                //generates random coordinate within the limits of the board//
                position.y = rnd.Next(0, maxY - 2) + 1; //between 1 to maxY -1
                position.x = rnd.Next(0, maxX - 2) + 1; //between 1 to maxX -1
            } while (!Empty(position));

            return position;
        }
        public Coord GetRandomCoordNear(Coord coord)
        {
            Coord position;
            Random rnd = new();

            Coord N = coord, E = coord, S = coord, W = coord;
            N.y--;
            E.x++;
            S.y++;
            W.x--;

            if (Empty(N) | Empty(E) | Empty(S) | Empty(W))
            {
                do
                {
                    //generates random coordinate within the limits of the board//
                    position.y = rnd.Next(0, 1); //between -1 to 1
                    position.x = rnd.Next(0, 1); //between -1 to 1


                    if (position.y == 0)
                        coord.y--;
                    else
                        coord.y++;

                    if (position.x == 0)
                        coord.x--;
                    else
                        coord.x++;

                } while (!Empty(coord));
            }

            return coord; //if somehow no spaces around, will be placed under character. otherwise randomly around character
            
        }
        public Coord SetItemPos(int obj)
        {
            //generate random position until an empty one is found, place specified object//
            Coord position;
            position = GetRandomCoord();
            PlaceObject(position, obj);
            return position;
        }

        //
        public bool Empty(Coord position)
        {
            //check if the position is empty (0)//
            if (boardLayout[position.y, position.x] == 0)
                return true;
            if (boardLayout[position.y, position.x] == 2)
                return true;
            else
                return false;
        }
        public void PlaceObject(Coord newPosition, int obj)
        {
            boardLayout[newPosition.y, newPosition.x] = obj;
        }
        public void ReplaceItem(Coord oldPosition, Coord newPosition, int obj, bool wasOnKey, Keys keys)
        {
            //replaces the object with empty in array//
            boardLayout[oldPosition.y, oldPosition.x] = 0;

            //replaces the object with empty on screen//
            Console.SetCursorPosition(((width + 1) * oldPosition.x), (height + 1) * oldPosition.y);
            if (!wasOnKey)
                Console.Write(EMPT);
            else
            {
                if (keys.FindKeyIndex(oldPosition) != -1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(KEY);
                    Console.ResetColor();
                }
            }


            boardLayout[newPosition.y, newPosition.x] = obj;
            Console.SetCursorPosition(((width + 1) * newPosition.x), (height + 1) * newPosition.y);
            switch (obj)
            {
                case 2:
                    Console.Write(KEY);
                    break;
                case 4:
                    Console.Write(PLYR);
                    break;
                case 5:
                    Console.Write(ENMY[3]);
                    break;
                default:
                    break;
            }
        }
        public void ReplaceItem(Coord oldPosition, Coord newPosition, Enemy enmy, bool wasOnKey, Keys keys)
        {
            //replaces the object with empty in array//
            boardLayout[oldPosition.y, oldPosition.x] = 0;

            //replaces the object with empty on screen//
            Console.SetCursorPosition(((width + 1) * oldPosition.x), (height + 1) * oldPosition.y);
            if (!wasOnKey)
                Console.Write(EMPT); //if wasn't on key
            else
            {
                if (keys.FindKeyIndex(oldPosition) != -1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(KEY);
                    Console.ResetColor();
                }
            }

            boardLayout[newPosition.y, newPosition.x] = 5;
            Console.SetCursorPosition((width + 1) * newPosition.x, (height + 1) * newPosition.y);

            Console.Write(ENMY[enmy.Visibility]);
        }
        //

        //displays gameboard//

        //
        public void DisplayAll(Enemy enmy)
        {
            Console.SetCursorPosition(0, 0);
            //sets position at (0,0)//
            Coord checkedCell;
            checkedCell.x = 0;
            checkedCell.y = 0;
            int[] neswWeights = new int[4];
            int neswTotal,
                fillers,
                o,
                n,
                e,
                s,
                w;

            //go through cells, drawing each value after checking surrounding cell values//
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    o = boardLayout[checkedCell.y, checkedCell.x];
                    fillers = width;

                    switch (o)
                    {
                        case 0:
                            //display empty character//
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(EMPT);
                            break;
                        case 1:
                            //find n,e,s,w values//
                            if (checkedCell.y != 0)
                                n = boardLayout[checkedCell.y - 1, checkedCell.x];
                            else
                                n = 0;

                            if (checkedCell.x < maxX - 1)
                                e = boardLayout[checkedCell.y, checkedCell.x + 1];
                            else
                                e = 0;

                            if (checkedCell.y < maxY - 1)
                                s = boardLayout[checkedCell.y + 1, checkedCell.x];
                            else
                                s = 0;

                            if (checkedCell.x != 0)
                                w = boardLayout[checkedCell.y, checkedCell.x - 1];
                            else
                                w = 0;

                            //check N//
                            if (n == 1 || n == 3)
                                neswWeights[0] = 1;
                            else
                                neswWeights[0] = 0;
                            //check E//
                            if (e == 1)
                                neswWeights[1] = 2;
                            else
                                neswWeights[1] = 0;
                            //check S//
                            if (s == 1 || s == 3)
                                neswWeights[2] = 4;
                            else
                                neswWeights[2] = 0;
                            //check W//
                            if (w == 1)
                                neswWeights[3] = 8;
                            else
                                neswWeights[3] = 0;

                            neswTotal = neswWeights.Sum();

                            Console.Write(ChooseWall(neswTotal));

                            //if a wall in east, write 'width' amount of filler walls//
                            if (neswWeights[1] == 2)
                            {
                                for (int i = 0; i < fillers; i++)
                                    Console.Write(EW);
                                fillers = 0;
                            }

                            break;
                        case 2:
                            //display key//
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(KEY);
                            break;
                        case 3:
                            //display door//
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(DOOR);
                            break;
                        case 4:
                            //display player//
                            Console.Write(PLYR);
                            break;
                        case 5:
                            //display enemy//
                            Console.Write(ENMY[enmy.Visibility]);
                            break;
                    }
                    Console.ResetColor();

                    for (int i = 0; i < fillers; i++)
                        Console.Write(" ");
                    checkedCell.x++;
                }

                if (checkedCell.y <= maxY - 2)
                    FillerChars(checkedCell.y);

                checkedCell.x = 0;
                checkedCell.y++;
                Console.WriteLine();
            }
        }
        private static void FillerChars(int y)
        {
            Coord origin;
            int valBelow,
                valOrigin,
                spaces;
            bool door = false;

            origin.y = y;

            for (int h = 0; h < height; h++)
            {
                Console.WriteLine();
                for (int x = 0; x < maxX; x++)
                {
                    valBelow = boardLayout[origin.y + 1, x];
                    valOrigin = boardLayout[origin.y, x];

                    spaces = width;

                    switch (boardLayout[origin.y, x])
                    {
                        case 1:
                            if (valBelow == 1 && valOrigin == 1)
                                Console.Write(NS);
                            else if (valBelow == 3)
                            {
                                if (!door)
                                    Console.Write(NS);
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.Write(DOOR);
                                    Console.ResetColor();
                                }

                                door = true;
                            }
                            else
                                spaces++;
                            break;
                        case 3:
                            if (valBelow == 1)
                            {
                                if (!door)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.Write(DOOR);
                                    Console.ResetColor();
                                }
                                else
                                    Console.Write(NS);
                                door = true;
                            }
                            else
                                spaces++;
                            break;
                        default:
                            spaces++;
                            break;
                    }

                    Console.SetCursorPosition(Console.CursorLeft + spaces, Console.CursorTop);
                }
            }
        }
        private static char ChooseWall(int neswTotal)
        {
            return neswTotal switch
            {
                1 or 4 or 5 => NS,
                2 or 8 or 10 => EW,
                3 => NE,
                6 => ES,
                7 => NES,
                9 => NW,
                11 => NEW,
                12 => SW,
                13 => NSW,
                14 => ESW,
                15 => NESW,
                _ => 'e',
            };
        }

        private static List<int> keyStatuses = new();

        // all keys displayed from y(3 +keys*2), x(maxX+2 * width + 3) //
        public static void DisplaySideMenu(int keysCount, bool initial) //initial side menu creation
        {
            Console.SetCursorPosition((maxX + 2) * width + 3, 3);
            Console.Write("Keys:");

            if (initial)            
                for (int i = 0; i < keysCount; i++)
                    keyStatuses.Add(-1);
                

            // displayed 3 spaces right of board, 3 spaces from top, written with a space between each key //
            for (int i = 0; i < keysCount; i++)
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop + 2); //writes every other line
                if (keyStatuses[i] == -1)
                    Console.ForegroundColor = ConsoleColor.Gray;
                if (keyStatuses[i] == 0)
                    Console.ForegroundColor = ConsoleColor.Red; //held by enemy
                if (keyStatuses[i] == 1)
                    Console.ForegroundColor = ConsoleColor.DarkBlue; //held by player

                Console.Write(KEY);
            }
            Console.CursorVisible = false;
        }
        public static void AlterSideMenu(List<int> keys)
        {
            keyStatuses = keys;
            DisplaySideMenu(keyStatuses.Count, false);
        }

        /// <summary>
        /// Writes message, returns the length of the message written
        /// </summary>
        /// <param name="player">An int, 1 or 2, for which player the message is for.</param>
        /// <param name="pick">A bool, true for if the key is being picked rather than dropped.</param>
        /// <param name="remove">A bool for if the message length and line is found and them removed.</param>
        /// <param name="completed">A bool for if the message is to express completion of action.</param>
        public void DisplayMessage(int player, bool pick, bool remove, bool completed)
        {
            string message = null;
            int line;

            if (player == 1)
                line = 0; //0, 1 for player 1
            else
                line = 1; //4, 5 for player 2

            if (pick) //if the player can pick key
                switch (player)
                {
                    case 1:
                        message = "Player 1: Press <C> to pick up key";
                        break;
                    case 2:
                        message = "Player 2: Press <insert> to pick up key";
                        break;
                }
            else //if the player can drop key
            {
                line++;
                switch (player)
                {
                    case 1:
                        message = "Player 1: Press <V> to drop key";
                        break;
                    case 2:
                        message = "Player 2: Press <delete> to drop key";
                        break;
                }
            }

            if (!remove) //if message is being removed rather than written
            {
                int y = keyStatuses.Count * 2 + 5 + line,
                x = (maxX + 2) * width + 3;

                Console.SetCursorPosition(x, y);
                Console.Write(message);
            }

            else
                RemoveMessage(Tuple.Create(message.Length, line));
            Console.CursorVisible = false;
        }
        private void RemoveMessage(Tuple<int, int>length_Line)
        {
            int y = keyStatuses.Count * 2 + 5 + length_Line.Item2, //y is 5 below where keys are displayed, plus the line it was displayed upon
                x = (maxX + 2) * width + 3; //x is 3 from where the board is displayed

            Console.SetCursorPosition(x, y);
            for(int i = 0; i < length_Line.Item1; i++)
                Console.Write(" "); //overwrite where message was written
        }
        //
    }
}
