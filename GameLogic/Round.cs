using System;
using System.Collections.Generic;
using static GameLogic.Enums;

namespace GameLogic
{
    public class Round
    {
        public int RoundNumber { get; set; }
        public eCheckerType[,] BoardAsMatrix { get; set; }
        private int BoardSize { get; }
        private int NumOfPlayers { get; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public ePlayerNumber CurrentTurnPlayer { get; set; }
        public ePlayerNumber LastTurnPlayer { get; set; }
        public ePlayerNumber Winner { get; set; }
        public bool RoundIsOver { get; set; }
        public bool EndWithWinning { get; set; }
        private bool NeedToMakeCapture { get; set; }
        private Checker CheckerToCapture { get; set; }
        private bool NeedToMoveTheLastTouched { get; set; }
        private Checker LastTouchedChecker { get; set; }

        public Round(int i_BoardSize, int i_NumOfPlayers)
        {
            this.BoardSize = i_BoardSize;
            this.BoardAsMatrix = new eCheckerType[BoardSize, BoardSize];
            this.Player1 = new Player(ePlayerNumber.Player1, BoardAsMatrix);
            this.Player2 = new Player(ePlayerNumber.Player2, BoardAsMatrix);
            this.RoundNumber = 0;
            this.NumOfPlayers = i_NumOfPlayers;
        }

        public void InitRound()
        {
            this.RoundIsOver = false;
            this.EndWithWinning = false;
            this.NeedToMakeCapture = false;
            this.NeedToMoveTheLastTouched = false; 
            this.RoundNumber++;
            this.LastTurnPlayer = 0;
            this.CurrentTurnPlayer = this.Player1.PlayerNumber;
            initCells();
            Player1.LegalPlayerSteps = new List<LegalMove>();
            Player1.LegalPlayerCaptures = new List<LegalMove>();
            Player1.UpdateLegalMoves(1);
            Player2.LegalPlayerSteps = new List<LegalMove>();
            Player2.LegalPlayerCaptures = new List<LegalMove>();
            Player2.UpdateLegalMoves(1);
        }

        private void initCells()
        {
            int player2AreaBorder = (this.BoardSize / 2) - 1;
            int player1AreaBorder = player2AreaBorder + 2;
            int row, col;

            this.Player1.PlayerCheckers = new List<Checker>();
            this.Player2.PlayerCheckers = new List<Checker>();
            for (int i = 0; i < this.BoardSize * this.BoardSize; i++)
            {
                row = i / this.BoardSize;
                col = i % this.BoardSize;

                if (((row % 2 == 0) && (col % 2 != 0)) || ((row % 2 != 0) && (col % 2 == 0)))
                {
                    if (row < player2AreaBorder)
                    {
                        this.Player2.PlayerCheckers.Add(new Checker(col, row, ePlayerNumber.Player2,this.BoardSize));
                        this.BoardAsMatrix[row, col] = eCheckerType.Man2;
                    }
                    else if (row >= player1AreaBorder)
                    {
                        this.Player1.PlayerCheckers.Add(new Checker(col, row, ePlayerNumber.Player1, this.BoardSize));
                        this.BoardAsMatrix[row, col] = eCheckerType.Man1;
                    }
                    else
                    {
                        this.BoardAsMatrix[row, col] = eCheckerType.BlankCell;
                    }
                }
            }
        }

        public bool IsValidTurn(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow, out int o_ErrorCode)
        {
            Player currentPlayer = findCurrentPlayer();
            bool isCapture = false;
            bool isStep = false;
            bool isContinuedCapture = false;
            bool isTouchMoveRuleNeeded = false;
            bool isLegalSourceChecker = isCheckerExists(i_FromCol, i_FromRow);

            if (this.NeedToMakeCapture)
            {
                isContinuedCapture = this.CheckerToCapture.CompareByColAndRow(i_FromCol, i_FromRow);
            }

            if (currentPlayer.LegalPlayerCaptures.Count != 0)
            {
                if (isContinuedCapture || !this.NeedToMakeCapture)
                {
                    isCapture = isMoveInList(i_FromCol, i_FromRow, i_ToCol, i_ToRow, currentPlayer.LegalPlayerCaptures);
                }
            }
            else
            {
                isStep = isMoveInList(i_FromCol, i_FromRow, i_ToCol, i_ToRow, currentPlayer.LegalPlayerSteps);
            }

            if (!this.NeedToMoveTheLastTouched && !(isCapture || isStep) && !isCheckerBlocked(i_FromCol, i_FromRow) && currentPlayer.LegalPlayerCaptures.Count == 0)
            {
                isTouchMoveRuleNeeded = true;
                this.NeedToMoveTheLastTouched = isTouchMoveRuleNeeded;
                this.LastTouchedChecker = findCheckerInList(i_FromCol, i_FromRow);
            }

            if (this.NeedToMoveTheLastTouched && (isCapture || isStep))
            {
                if (!(isTouchMoveRuleNeeded = !this.LastTouchedChecker.CompareByColAndRow(i_FromCol, i_FromRow)))
                {
                    this.NeedToMoveTheLastTouched = !this.NeedToMoveTheLastTouched;
                }
            }

            o_ErrorCode = getErrorCode(isLegalSourceChecker, isStep, isContinuedCapture, isTouchMoveRuleNeeded);

            return isLegalSourceChecker && (isCapture || isStep) && !isTouchMoveRuleNeeded;
        }

        public void MakeTurn(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow)
        {
            bool captureWasMade;
            
            makeMove(i_FromCol, i_FromRow, i_ToCol, i_ToRow);
            captureWasMade = Math.Abs(i_ToRow - i_FromRow) == 2;
            if (captureWasMade)
            {
                makeCapture((i_FromCol + i_ToCol) / 2, (i_FromRow + i_ToRow) / 2);
            }
            updateMoves();
            if (!checkIfRoundIsOver())
            {
                if (captureWasMade)
                {
                    if (this.NeedToMakeCapture = isCanCapture(i_ToCol, i_ToRow))
                    {
                        this.CheckerToCapture = findCheckerInList(i_ToCol, i_ToRow);
                    }
                }
                if (NumOfPlayers == 1 && ((this.CurrentTurnPlayer == ePlayerNumber.Player1 && !this.NeedToMakeCapture)
                                         || (this.CurrentTurnPlayer == ePlayerNumber.Player2 && this.NeedToMakeCapture)))
                {
                    makeRandomTurn();
                }
                else if (!this.NeedToMakeCapture)
                {
                    swapTurns();
                }
            }
        }

        private void makeMove(int i_FromCellCol, int i_FromCellRow, int i_ToCellCol, int i_ToCellRow)
        {
            Checker checkerToMove = findCheckerInList(i_FromCellCol, i_FromCellRow);
            checkerToMove.Column = i_ToCellCol;
            checkerToMove.Row = i_ToCellRow;
            this.BoardAsMatrix[i_FromCellRow, i_FromCellCol] = eCheckerType.BlankCell;
            checkerToMove.CheckIfBecameKing();
            this.BoardAsMatrix[i_ToCellRow, i_ToCellCol] = checkerToMove.CheckerType;
        }

        private void makeCapture(int i_Col, int i_Row)
        {
            findOppositePlayer().PlayerCheckers.RemoveAll(checker => (checker.CompareByColAndRow(i_Col, i_Row)));
            this.BoardAsMatrix[i_Row, i_Col] = eCheckerType.BlankCell;
        }

        private void updateMoves()
        {
            this.Player1.LegalPlayerSteps = new List<LegalMove>();
            this.Player1.LegalPlayerCaptures = new List<LegalMove>();
            Player1.UpdateLegalMoves(1);
            Player1.UpdateLegalMoves(2);
            this.Player2.LegalPlayerSteps = new List<LegalMove>();
            this.Player2.LegalPlayerCaptures = new List<LegalMove>();
            Player2.UpdateLegalMoves(1);
            Player2.UpdateLegalMoves(2);
        }

        private void makeRandomTurn()
        {
            LegalMove randomMove;
            Random random = new Random();

            if (Player2.LegalPlayerCaptures.Count > 0)
            {
                randomMove = Player2.LegalPlayerCaptures[random.Next(Player2.LegalPlayerCaptures.Count)];
            }
            else
            {
                randomMove = Player2.LegalPlayerSteps[random.Next(Player2.LegalPlayerSteps.Count)];
            }

            this.LastTurnPlayer = ePlayerNumber.Player1;
            this.CurrentTurnPlayer = ePlayerNumber.Player2;
            MakeTurn(randomMove.FromCol, randomMove.FromRow, randomMove.ToCol, randomMove.ToRow);
        }

        private void swapTurns()
        {
            this.LastTurnPlayer = this.CurrentTurnPlayer;
            this.CurrentTurnPlayer = 3 - this.CurrentTurnPlayer;
        }

        private bool checkIfRoundIsOver()
        {
            bool lostByComputer = false;

            if(this.NumOfPlayers == 1)
            {
                if (lostByComputer = checkIfOutOfMoves(findCurrentPlayer()))
                {
                    ComputeScore(Player2, Player1);
                }
            }
            
            return this.RoundIsOver = lostByComputer || checkIfWon(findCurrentPlayer(), findOppositePlayer()) || checkIfTie(findCurrentPlayer(), findOppositePlayer());
        }

        private bool checkIfWon(Player i_CurrentPlayer, Player i_OppositePlayer)
        {
            if (this.EndWithWinning = checkIfOutOfMoves(i_OppositePlayer))
            {
                ComputeScore(i_CurrentPlayer, i_OppositePlayer);
                this.Winner = i_CurrentPlayer == this.Player1 ? ePlayerNumber.Player1 : ePlayerNumber.Player2;
            }
            return this.EndWithWinning;
        }

        private static bool checkIfTie(Player i_currentPlayer, Player i_OppositePlayer)
        {
            return checkIfOutOfMoves(i_currentPlayer) && checkIfOutOfMoves(i_OppositePlayer);
        }

        private static bool checkIfOutOfMoves(Player i_Player)
        {
            return i_Player.LegalPlayerSteps.Count == 0 && i_Player.LegalPlayerCaptures.Count == 0;
        }

        public static void ComputeScore(Player i_Winner, Player i_Loser)
        {
            i_Winner.PlayerScore += Math.Abs(countCheckers(i_Winner) - countCheckers(i_Loser));
        }

        private static int countCheckers(Player i_Player)
        {
            int count = 0;

            foreach (Checker checker in i_Player.PlayerCheckers)
            {
                if (!checker.IsKing)
                {
                    count++;
                }
                else
                {
                    count += 4;
                }
            }

            return count;
        }

        private int getErrorCode(bool i_LegalSourceChecker, bool i_LegalStep, bool i_ContinuedCapture, bool i_TouchMoveRule)
        {
            int errorCode = 0;
            bool hasCaptures = findCurrentPlayer().LegalPlayerCaptures.Count > 0;

            if (!i_LegalSourceChecker)
            {
                errorCode = 1;
            } 
            else if (!i_LegalStep && !hasCaptures)
            {
                errorCode = 2;
            }
            else if (hasCaptures)
            {
                errorCode = 3;
            }
            else if (i_ContinuedCapture)
            {
                errorCode = 4;
            } 
            else if (i_TouchMoveRule)
            {
                errorCode = 5;
            }

            return errorCode;
        }

        private Checker findCheckerInList(int i_Col, int i_Row)
        {          
            return findCurrentPlayer().PlayerCheckers.Find(checker => (checker.CompareByColAndRow(i_Col, i_Row)));
        }

        private bool isCheckerExists(int i_Col, int i_Row)
        {
            return findCurrentPlayer().PlayerCheckers.Exists(checker => (checker.CompareByColAndRow(i_Col, i_Row)));
        }

        private bool isCheckerBlocked(int i_FromCol, int i_FromRow)
        {
            return !(findCurrentPlayer().LegalPlayerSteps.Exists(move => (move.CompareBySourceChecker(i_FromCol, i_FromRow))) || findCurrentPlayer().LegalPlayerCaptures.Exists(move => (move.CompareBySourceChecker(i_FromCol, i_FromRow))));
        }

        private static bool isMoveInList(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow, List<LegalMove> i_ListOfMoves)
        {
            return i_ListOfMoves.Exists(move => (move.CompareByBothCheckers(i_FromCol, i_FromRow, i_ToCol, i_ToRow)));
        }

        private bool isCanCapture(int i_FromCol, int i_FromRow)
        {
            return findCurrentPlayer().LegalPlayerCaptures.Exists(move => (move.CompareBySourceChecker(i_FromCol, i_FromRow)));
        }

        private Player findCurrentPlayer()
        {
            Player currentPlayer = this.CurrentTurnPlayer == ePlayerNumber.Player1 ? this.Player1 : this.Player2;

            return currentPlayer;
        }

        private Player findOppositePlayer()
        {
            Player oppositePlayer = this.CurrentTurnPlayer == ePlayerNumber.Player1 ? this.Player2 : this.Player1;

            return oppositePlayer;
        }
    }
}
