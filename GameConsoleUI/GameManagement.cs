using System;
using GameLogic;
using static GameLogic.Enums;

namespace GameConsoleUI
{
    internal class GameManagement
    {
        internal string Player1Name { get; }
        internal string Player2Name { get; }
        internal int BoardSize { get; }
        internal int NumOfPlayers { get; }
        internal VisualGame VisualizedGame { get; set; }
        internal Round Round { get; set; }
        internal bool QuitByUser { get; set; }

        internal GameManagement(string i_Player1Name, string i_Player2Name, int i_BoardSize, int i_NumOfPlayers)
        {
            this.Player1Name = i_Player1Name;
            this.Player2Name = i_Player2Name;
            this.BoardSize = i_BoardSize;
            this.NumOfPlayers = i_NumOfPlayers;
            this.Round = new Round(this.BoardSize, this.NumOfPlayers);
            
        }

        internal void StartNewRound()
        {
            this.Round.InitRound();
            this.VisualizedGame = new VisualGame(Player1Name, Player2Name, BoardSize, this.Round.RoundNumber, this.Round.BoardAsMatrix, this.NumOfPlayers);
            Console.Clear();;
            this.VisualizedGame.showRoundDetails(this.Round.LastTurnPlayer);
            playGame();
        }

        private void playGame()
        {
            string[] positionsAsChars;
            int fromCol, fromRow, toCol, toRow, errorCode;
            string moveAsString;

            while (!this.Round.RoundIsOver)
            {
                moveAsString = getValidTurnFromInput(out positionsAsChars);
                if (this.Round.RoundIsOver = checkIfQuit(moveAsString))
                {
                    break;
                }
                
                createPositionsFromString(positionsAsChars, out fromCol, out fromRow, out toCol, out toRow);
                while (!this.Round.IsValidTurn(fromCol, fromRow, toCol, toRow, out errorCode))
                {
                    Console.Write(String.Format("{0} Try again: ", errorMessage(errorCode)));
                    moveAsString = getValidTurnFromInput(out positionsAsChars);
                    if (this.Round.RoundIsOver = checkIfQuit(moveAsString))
                    {
                        break;
                    }

                    createPositionsFromString(positionsAsChars, out fromCol, out fromRow, out toCol, out toRow);
                }

                if(checkIfQuit(moveAsString))
                {
                    break;
                }

                this.VisualizedGame.MoveAsString = moveAsString;
                this.Round.MakeTurn(fromCol, fromRow, toCol, toRow);
                Console.Clear();;
                this.VisualizedGame.showRoundDetails(this.Round.LastTurnPlayer);
            }

            endRound();
        }

        private string getValidTurnFromInput(out string[] i_PositionsChars)
        {
            string moveAsString = Console.ReadLine();
            i_PositionsChars = moveAsString.Split('>');

            while (i_PositionsChars.Length != 2 || !(isValidPosition(i_PositionsChars[0]) && isValidPosition(i_PositionsChars[1])))
            {
                if (checkIfQuit(moveAsString))
                {
                    break;
                }

                Console.Write("Invalid input. please enter move as 'COLrow>COLrow': ");
                moveAsString = Console.ReadLine();
                i_PositionsChars = moveAsString.Split('>');
            }

            return moveAsString;
        }

        private bool isValidPosition(string i_Position)
        {
            return (i_Position.Length == 2) && (i_Position[0] >= 'A') && (i_Position[0] <= 'A' + this.BoardSize - 1) && (i_Position[1] >= 'a') && (i_Position[1] <= 'a' + this.BoardSize - 1);
        }

        private static void createPositionsFromString(string[] i_PositionsChars, out int i_FromCol, out int i_FromRow, out int i_ToCol, out int i_ToRow)
        {
            i_FromCol = (int)(i_PositionsChars[0][0] - 'A');
            i_FromRow = (int)(i_PositionsChars[0][1] - 'a');
            i_ToCol = (int)(i_PositionsChars[1][0] - 'A');
            i_ToRow = (int)(i_PositionsChars[1][1] - 'a');
        }

        private bool checkIfQuit(string i_InputFromUser)
        {
            if (QuitByUser = Equals(i_InputFromUser, 'Q'.ToString()))
            {
                this.Round.EndWithWinning = QuitByUser;
                this.Round.Winner = this.Round.CurrentTurnPlayer;
            }

            return QuitByUser;
        }

        private void endRound()
        {
            string endChoice;
            string winnerName;

            if (this.Round.EndWithWinning)
            {
                if (QuitByUser)
                {
                    if (this.Round.CurrentTurnPlayer == ePlayerNumber.Player1)
                    {
                        winnerName = this.Player2Name;
                        Round.ComputeScore(Round.Player2, Round.Player1);
                    }
                    else
                    {
                        winnerName = this.Player1Name;
                        Round.ComputeScore(Round.Player1, Round.Player2);
                    }
                }
                else
                {
                    winnerName = this.Round.Winner == ePlayerNumber.Player1 ? this.Player1Name : this.Player2Name;
                }
                
                Console.WriteLine(String.Format("\nThe round is over and {0} is the winner!", winnerName));
            }
            else
            {
                Console.WriteLine("\nThe round is over with a tie!");
            }

            Console.WriteLine(String.Format("{0} have {2} points, and {1} have {3} points ", this.Player1Name, this.Player2Name, this.Round.Player1.PlayerScore, this.Round.Player2.PlayerScore));
            Console.WriteLine("Do you want to play another round? Press 'y'/'n': ");
            endChoice = Console.ReadLine();
            while (!Equals(endChoice, 'y'.ToString()) && !Equals(endChoice, 'n'.ToString()))
            {
                Console.WriteLine("Invalid input. Please Press 'y'/'n': ");
                endChoice = Console.ReadLine();
            }

            if (Equals(endChoice, 'y'.ToString()))
            {
                StartNewRound();
            }
            else
            {
                endGame();
            }
        }

        private void endGame()
        {
            Console.WriteLine("It was a pleasure! Goodbye");
        }

        private static string errorMessage(int i_ErrorCode)
        {
            string message = "";

            if (i_ErrorCode == 1)
            {
                message = "Please choose a valid checker.";
            } 
            else if (i_ErrorCode == 2)
            {
                message = "Please make a legal step.";
            }
            else if (i_ErrorCode == 3)
            {
                message = "You must make a capture if you can.";
            }
            else if (i_ErrorCode == 4)
            {
                message = "You made a capture and you have another one available, you must do so!";
            }
            else if(i_ErrorCode == 5)
            {
                message = "Touch-Move rule: You need to move with the last one you mistaked with!";
            }

            return message;
        }
    }
}
