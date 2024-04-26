using System;

namespace Dungeon_Escape
{
    class Program
    {
        static void Main(string[] _)
        {
            // hide cursor
            Console.CursorVisible = false;


            // display menu until exit is chosen
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }
        private static bool MainMenu()
        {
            int option = MenuSetup.MenuDisplay(0); // main menu displayed and index of option returned
            int difficulty,
                players = 0;

            switch (option)
            {

                case 0:
                    difficulty = MenuSetup.MenuDisplay(1);

                    if (difficulty != 1)
                        players = MenuSetup.MenuDisplay(2); //if tutorial is not selected, user can choose to play with one or two players

                    Game newGame = new(difficulty, players); //creates instance of game
                    return true;

                case 1:
                    Game tutorial = new(-1, 0); //creates instance of game with tutorial settings
                    return true;

                case 2:
                    return false; //if user chooses to exit from main menu, this option is 

                default:
                    return true;
            }
        }
        //


    }
}