using System.Collections.Generic;
using static GameLogic.Enums;

namespace GameLogic
{
    public class Player
    {
        internal ePlayerNumber PlayerNumber { get; }
        public int PlayerScore { get; set; }
        internal List<Checker> PlayerCheckers { get; set; }
        internal List<LegalMove> LegalPlayerSteps { get; set; }
        internal List<LegalMove> LegalPlayerCaptures { get; set; }
        private eCheckerType[,] BoardAsMatrix { get; set; }
        
        internal Player(ePlayerNumber i_playerNumber, eCheckerType[,] i_BoardAsMatrix)
        {
            this.PlayerNumber = i_playerNumber;
            this.PlayerScore = 0;
            this.PlayerCheckers = new List<Checker>();
            this.LegalPlayerSteps = new List<LegalMove>();
            this.LegalPlayerCaptures = new List<LegalMove>();
            this.BoardAsMatrix = i_BoardAsMatrix;
        }

        internal void UpdateLegalMoves(int i_Distance)
        {
            int verticalDirection;
            bool verticalCheck;

            foreach (Checker checker in this.PlayerCheckers)
            {
                if (!checker.IsKing)
                {
                    if (this.PlayerNumber == ePlayerNumber.Player1)
                    {
                        verticalDirection = -i_Distance;
                        verticalCheck = checker.CanMoveUp(i_Distance);
                    }
                    else
                    {
                        verticalDirection = i_Distance;
                        verticalCheck = checker.CanMoveDown(i_Distance);
                    }

                    if (verticalCheck)
                    {
                        if (checker.CanMoveRight(i_Distance) && cellIsEmpty(checker.Row + verticalDirection, checker.Column + i_Distance))
                        {
                            if (i_Distance == 2 && cellIsOccupiedByEnemy(checker.Row + verticalDirection / 2, checker.Column + 1))
                            {
                                addCapture(checker.Column, checker.Row, checker.Column + i_Distance, checker.Row + verticalDirection);
                            }
                            else if (i_Distance == 1)
                            {
                                addStep(checker.Column, checker.Row, checker.Column + i_Distance, checker.Row + verticalDirection);
                            }
                        }
                        if (checker.CanMoveLeft(i_Distance) && cellIsEmpty(checker.Row + verticalDirection, checker.Column - i_Distance))
                        {
                            if (i_Distance == 2 && cellIsOccupiedByEnemy(checker.Row + verticalDirection / 2, checker.Column - 1))
                            {
                                addCapture(checker.Column, checker.Row, checker.Column - i_Distance, checker.Row + verticalDirection);
                            }
                            else if (i_Distance == 1)
                            {
                                addStep(checker.Column, checker.Row, checker.Column - i_Distance, checker.Row + verticalDirection);
                            }

                        }
                    }
                }
                else
                {
                    UpdateKingMoves(checker, i_Distance);
                }
            }
        }

        internal void UpdateKingMoves(Checker i_Checker, int i_Distance)
        {
            if (i_Checker.CanMoveUp(i_Distance) && i_Checker.CanMoveRight(i_Distance) && cellIsEmpty(i_Checker.Row - i_Distance, i_Checker.Column + i_Distance))
            {
                if (i_Distance == 2 && cellIsOccupiedByEnemy(i_Checker.Row - 1, i_Checker.Column + 1))
                {
                    addCapture(i_Checker.Column, i_Checker.Row, i_Checker.Column + i_Distance, i_Checker.Row - i_Distance);
                }
                else if (i_Distance == 1)
                {
                    addStep(i_Checker.Column, i_Checker.Row, i_Checker.Column + i_Distance, i_Checker.Row - i_Distance);
                }
            }
            if (i_Checker.CanMoveUp(i_Distance) && i_Checker.CanMoveLeft(i_Distance) && cellIsEmpty(i_Checker.Row - i_Distance, i_Checker.Column - i_Distance))
            {
                if (i_Distance == 2 && cellIsOccupiedByEnemy(i_Checker.Row - 1, i_Checker.Column - 1))
                {
                    addCapture(i_Checker.Column, i_Checker.Row, i_Checker.Column - i_Distance, i_Checker.Row - i_Distance);
                }
                else if (i_Distance == 1)
                {
                    addStep(i_Checker.Column, i_Checker.Row, i_Checker.Column - i_Distance, i_Checker.Row - i_Distance);
                }
            }
            if (i_Checker.CanMoveDown(i_Distance) && i_Checker.CanMoveLeft(i_Distance) && cellIsEmpty(i_Checker.Row + i_Distance, i_Checker.Column - i_Distance))
            {
                if (i_Distance == 2 && cellIsOccupiedByEnemy(i_Checker.Row + 1, i_Checker.Column - 1))
                {
                    addCapture(i_Checker.Column, i_Checker.Row, i_Checker.Column - i_Distance, i_Checker.Row + i_Distance);
                }
                else if (i_Distance == 1)
                {
                    addStep(i_Checker.Column, i_Checker.Row, i_Checker.Column - i_Distance, i_Checker.Row + i_Distance);
                }
            }
            if (i_Checker.CanMoveDown(i_Distance) && i_Checker.CanMoveRight(i_Distance) && cellIsEmpty(i_Checker.Row + i_Distance, i_Checker.Column + i_Distance))
            {
                if (i_Distance == 2 && cellIsOccupiedByEnemy(i_Checker.Row + 1, i_Checker.Column + 1))
                {
                    addCapture(i_Checker.Column, i_Checker.Row, i_Checker.Column + i_Distance, i_Checker.Row + i_Distance);
                }
                else if (i_Distance == 1)
                {
                    addStep(i_Checker.Column, i_Checker.Row, i_Checker.Column + i_Distance, i_Checker.Row + i_Distance);
                }
            }
        }

        private void addStep(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow)
        {
            this.LegalPlayerSteps.Add(new LegalMove(i_FromCol, i_FromRow, i_ToCol, i_ToRow));
        }

        private void addCapture(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow)
        {
            this.LegalPlayerCaptures.Add(new LegalMove(i_FromCol, i_FromRow, i_ToCol, i_ToRow));
        }

        private bool cellIsEmpty(int i_Row, int i_Column)
        {
            return this.BoardAsMatrix[i_Row, i_Column] == eCheckerType.BlankCell;
        }

        private bool cellIsOccupiedByEnemy(int i_Row, int i_Column)
        {
            return this.BoardAsMatrix[i_Row, i_Column] == (eCheckerType)(3 - PlayerNumber) || this.BoardAsMatrix[i_Row, i_Column] == (eCheckerType)(5 - PlayerNumber);
        }
    }
}
