using System;
using static GameLogic.Enums;

namespace GameConsoleUI
{
    internal class VisualGame
    {
        internal string Player1Name { get; }
        internal string Player2Name { get; }
        internal int Boardsize { get; }
        internal int RoundNumber { get; }
        internal eCheckerType[,] BoardAsMatrix { get; set; }
        internal int NumOfPlayers { get; }
        internal string MoveAsString { get; set; }

        internal enum eCheckerSigns
        {
            X = 1,
            O,
            Z,
            Q
        }

        internal VisualGame(string i_Player1Name, string i_Player2Name, int i_Boardsize, int i_RoundNumber, eCheckerType[,] i_BoardAsMatrix, int i_NumOfPlayers)
        {
            this.Player1Name = i_Player1Name;
            this.Player2Name = i_Player2Name;
            this.Boardsize = i_Boardsize;
            this.RoundNumber = i_RoundNumber;
            this.BoardAsMatrix = i_BoardAsMatrix;
            this.NumOfPlayers = i_NumOfPlayers;
        }

        internal void showRoundDetails(ePlayerNumber i_LastTurnPlayer)
        {
            Console.WriteLine();
            Console.WriteLine(String.Format("Round number: {0}", this.RoundNumber));
            showBoard();
            showTurnsDetails(i_LastTurnPlayer);
        }

        private void showBoard()
        {
            string seperatorLine = "    " + new String('=', 1 + 6 * this.Boardsize);

            Console.Write("  ");
            for (int i = 0; i < this.Boardsize; i++)
            {
                Console.Write("     ");
                Console.Write((char)(65 + i));
            }
            Console.WriteLine();
            Console.WriteLine(seperatorLine);

            for (int i = 0; i < this.Boardsize; i++)
            {
                Console.Write(" " + (char)(97 + i));
                for (int j = 0; j < this.Boardsize; j++)
                {
                    if (BoardAsMatrix[i, j] == 0)
                    {
                        Console.Write("  |   ");
                    }
                    else
                    {
                        Console.Write("  |  " + (eCheckerSigns)BoardAsMatrix[i, j]);
                    }
                }
                Console.WriteLine("  |");
                Console.WriteLine(seperatorLine);
            }
        }

        private void showTurnsDetails(ePlayerNumber i_LastTurnPlayer)
        {
            string lastPlayerName = Player2Name;
            string currentPlayerName = Player1Name;

            if (this.NumOfPlayers == 2 && i_LastTurnPlayer == ePlayerNumber.Player1)
            {
                lastPlayerName = this.Player1Name;
                currentPlayerName = this.Player2Name;
            }

            if (i_LastTurnPlayer != 0 && NumOfPlayers == 2)
            {
                Console.WriteLine(String.Format("{0}'s move was: {1}", lastPlayerName, this.MoveAsString));
            }

            Console.Write(String.Format("{0}'s turn: ", currentPlayerName));
        }
    }
}
