using static GameLogic.Enums;

namespace GameLogic
{
    internal class Checker
    {
        internal int Column { get; set; }
        internal int Row { get; set; }
        private ePlayerNumber PlayerNumber { get; set; }
        private int BoardSize { get; }
        internal bool IsKing { get; set; }
        internal eCheckerType CheckerType { get; set; }

        internal Checker(int i_Column, int i_Row, ePlayerNumber i_PlayerNumber,int i_BoardSize)
        {
            this.Column = i_Column;
            this.Row = i_Row;
            this.PlayerNumber = i_PlayerNumber;
            this.BoardSize = i_BoardSize;
            this.CheckerType = (eCheckerType)PlayerNumber;
        }

        internal bool CanMoveRight(int i_Distance)
        {
            return this.Column < this.BoardSize - i_Distance;
        }

        internal bool CanMoveLeft(int i_Distance)
        {
            return this.Column >= i_Distance;
        }

        internal bool CanMoveUp(int i_Distance)
        {
            return this.Row >= i_Distance;
        }

        internal bool CanMoveDown(int i_Distance)
        {
            return this.Row < this.BoardSize - i_Distance;
        }

        internal void CheckIfBecameKing()
        {
            if (!this.IsKing)
            {
                if ((this.PlayerNumber == ePlayerNumber.Player1 && this.Row == 0) || (this.PlayerNumber == ePlayerNumber.Player2 && this.Row == this.BoardSize - 1))
                {
                    this.CheckerType += 2;
                    this.IsKing = true;
                }
            }
        }

        internal bool CompareByColAndRow(int i_Col, int i_Row)
        {
            return this.Column == i_Col && this.Row == i_Row;
        }
    }
}
