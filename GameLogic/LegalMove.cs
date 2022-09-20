namespace GameLogic
{
    internal class LegalMove
    {
        internal int FromCol { get; set; }
        internal int FromRow { get; set; }
        internal int ToCol { get; set; }
        internal int ToRow { get; set; }

        internal LegalMove(int i_FromCellCol, int i_FromCellRow, int i_ToCellCol, int i_ToCellRow)
        {
            this.FromCol = i_FromCellCol;
            this.FromRow = i_FromCellRow;
            this.ToCol = i_ToCellCol;
            this.ToRow = i_ToCellRow;
        }

        internal bool CompareBySourceChecker(int i_Col, int i_Row)
        {
            return this.FromCol == i_Col && this.FromRow == i_Row;
        }

        internal bool CompareByBothCheckers(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow)
        {
            return this.FromCol == i_FromCol && this.FromRow == i_FromRow && this.ToCol == i_ToCol && this.ToRow == i_ToRow;
        }
    }
}
