using System;

namespace Dungeon_Escape
{
    class MenuSetup
    {
        private static int MultipleChoice(string menuName, bool canCancel, params string[] options)
        {
            //spacing and starting position of options//
            const int
                startX = 8,
                startY = 5,
                spacingPerLine = 12;

            int currentSelection = 0;
            ConsoleKey key;
            int optionsAmount = options.Length;

            //loops through until an option is selected//
            do
            {
                Console.Clear();
                Console.ResetColor();

                //display menu name and instruction text//
                Console.WriteLine(menuName + "\nSelect option with the arrow keys and enter");

                //writes each option, colouring red if selected//
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsAmount) * spacingPerLine, startY + i / optionsAmount);

                    if (i == currentSelection)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(options[i]);
                    Console.ResetColor();
                }

                //waits for input, reads key//
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        //checks if option on left, moves selection if so//
                        if (currentSelection > 0)
                            currentSelection--;
                        break;

                    case ConsoleKey.RightArrow:
                        //checks if option on right, moves selection if so//
                        if (currentSelection < optionsAmount - 1)
                            currentSelection++;
                        break;

                    case ConsoleKey.Escape:
                        //checks if exit from menu is permitted, exits if so//
                        if (canCancel)
                            return -1;
                        break;
                }
            } while (key != ConsoleKey.Enter);

            Console.Clear();
            return currentSelection;
        }

        /// <summary>
        /// 0 for main menu, 1 for difficulty select, 2 for player count, 3 for pause menu
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int MenuDisplay(int type)
        {
            switch (type)
            {
                case 0:
                    return MultipleChoice("DUNGEON ESCAPE", false, "PLAY", "DEBUG", "EXIT");
                case 1:
                    return MultipleChoice("DIFFICULTY SELECT", true, "EASY", "MEDIUM", "HARD", "EXTREME");
                case 2:
                    return MultipleChoice("PLAYER COUNT", true, "ONE", "TWO");
                case 3:
                    return MultipleChoice("PAUSE MENU", true, "CONTINUE", "INSTRUCTIONS", "KEYS", "EXIT");
                default:
                    return 0;
            }
        }
    }
}
