using System;

namespace GameConsoleUI
{
    internal class GameIntro
    {
        internal enum eBoardSize
        {
            Small = 6,
            Medium = 8,
            Large = 10
        }

        internal enum eNumOfPlayers
        {
            One = 1,
            Two
        }

        internal void Run()
        {
            string player1Name, player2Name;
            int boardSize, numOfPlayers;

            getGameData(out player1Name, out player2Name, out boardSize, out numOfPlayers);
            GameManagement gameManager = new GameManagement(player1Name, player2Name, boardSize, numOfPlayers);
            gameManager.StartNewRound();
        }

        private static void getGameData(out string o_Player1Name, out string o_Player2Name, out int o_BoardSize, out int o_NumOfPlayers)
        {
            Console.WriteLine("Welcome to Checkers game! Please enter your name - up to 10 characters and with no spaces:");
            getNameUntilValid(out o_Player1Name);

            Console.WriteLine("Please enter the requested board size (6, 8 or 10):");
            while (!int.TryParse(Console.ReadLine(), out o_BoardSize) || (o_BoardSize != (int)eBoardSize.Small && o_BoardSize != (int)eBoardSize.Medium && o_BoardSize != (int)eBoardSize.Large))
            {
                Console.WriteLine("Invalid size. Please Try again.");
            }

            Console.WriteLine("Press 1 if you want to play against the computer, or press 2 if you want to play against another player:");
            while (!int.TryParse(Console.ReadLine(), out o_NumOfPlayers) || (o_NumOfPlayers != (int)eNumOfPlayers.One &&  o_NumOfPlayers != (int)eNumOfPlayers.Two))
            {
                Console.WriteLine("Invalid input. Please Try again.");
            }

            if (o_NumOfPlayers == (int)eNumOfPlayers.Two)
            {
                Console.WriteLine("Please enter the second player's name - up to 10 characters and with no spaces:");
                getNameUntilValid(out o_Player2Name);
            }
            else
            {
                o_Player2Name = "Computer";
            }
        }

        private static void getNameUntilValid(out string o_PlayerName)
        {
            o_PlayerName = Console.ReadLine();

            while (o_PlayerName != null && (o_PlayerName.Split(' ').Length > 1 || o_PlayerName.Length > 10 || o_PlayerName.Length == 0))
            {
                Console.WriteLine("Invalid name. Please Try again.");
                o_PlayerName = Console.ReadLine();
            }
        }
    }
}
