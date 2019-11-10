using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect_Four
{
    class AI2
    {
        public enum PlayerValue : int
        {
            Player1 = -1,
            AI = 1,
            Nobody = 0
        }
        
        public class Column
        {
            public int column { get; set; }
            public int score { get; set; }
        }
        
        Board myBoard = new Board();
        public const int width = 7;
        public const int height = 6;
        public const int AIWin = 10;
        public const int instantWin = int.MaxValue;
        public const int avoidOppConnect = instantWin - 1;
        public const int PlayerWin = -AIWin;
        int[] emptyCellsCoordonates = new int[3];
        short[,] virtualShortMoves = new short[width, height];
        short[] singleLineCheck = new short[5];
        short[,] smallBoard = new short[4, 4];
        List<Column> sortedColumnsList = new List<Column>();
        List<int> AILastMoves = new List<int>();
        List<int> playerLastMoves = new List<int>();
        
        
        public int SetMove(int AIDifficulty, Brush AIBrush, Brush playerBrush, Brush justPlayedBrush)
        {
            TranslateActualEllipseBoard(AIBrush, playerBrush, justPlayedBrush);
            AlphaBeta(AIDifficulty, int.MinValue, int.MaxValue, (int)PlayerValue.AI, AIDifficulty);
        
            sortedColumnsList = sortedColumnsList.OrderByDescending(x => x.score).ToList();
        
            int bestAIMove, y;
        
            do
            {
                bestAIMove = sortedColumnsList[0].column;
                sortedColumnsList.RemoveAt(0);
                y = myBoard.FindCoinSpot(bestAIMove) - 1;
            } while (y == -1);
        
            return bestAIMove;
        
            //int y = -1;
            //int randomColumn;
            //do
            //{
            //    Random rnd = new Random();
            //    randomColumn = rnd.Next(0, 6);
            //
            //    y = myBoard.FindCoinSpot(randomColumn) - 1;
            //} while (y == -1);
            //myBoard.FillBoard(randomColumn, y, AIBrush);
            //
            //return randomColumn;
        }
        
        private int AlphaBeta(int depth, int alpha, int beta, int currentVirtualPlayer, int AIDifficulty)
        {
            //Alpha : Meilleur coup pour l'AI
            //Beta : Meilleur coup pour l'adversaire
            int xAI, xPlayer, yAI, yPlayer, x, y;
        
            int didWin = CheckForWin();
            int score = EvaluateBoard(depth, currentVirtualPlayer);
            if (didWin == Math.Abs(AIWin) || depth <= 0)
            {
                return score;
            }
        
            if (currentVirtualPlayer == (int)PlayerValue.Player1)
            {
                for (xPlayer = 0; xPlayer < 7; xPlayer++)
                {
                    yPlayer = FindYSpot(xPlayer) - 1;
                    if (yPlayer >= 0)
                    {
                        virtualShortMoves[xPlayer, yPlayer] = -1;
                        playerLastMoves.Add(yPlayer);
                        playerLastMoves.Add(xPlayer);
        
                        score = AlphaBeta(depth - 1, alpha, beta, (int)PlayerValue.AI, AIDifficulty);
        
                        if (playerLastMoves.Count != 0)
                        {
                            x = playerLastMoves[playerLastMoves.Count - 1];
                            playerLastMoves.RemoveAt(playerLastMoves.Count - 1);
                            y = playerLastMoves[playerLastMoves.Count - 1];
                            playerLastMoves.RemoveAt(playerLastMoves.Count - 1);
        
                            virtualShortMoves[xPlayer, yPlayer] = 0;
                        }
        
                        beta = Math.Min(beta, score);
                        if (alpha >= beta) { return beta; }
                    }
                }
        
                return beta;
            }
            else if (currentVirtualPlayer == (int)PlayerValue.AI)
            {
                int canWinColumn = CheckSelfConnect();
                int columnAvoidOppConnect = CheckOppConnect(AIDifficulty);
                for (xAI = 0; xAI < 7; xAI++)
                {
                    if (canWinColumn > -1 && xAI == canWinColumn)
                    {
                        if (depth == AIDifficulty)
                        {
                            sortedColumnsList.Add(new Column() { column = canWinColumn, score = instantWin });
                            xAI++;
                        }
                    }
                    else if (columnAvoidOppConnect > -1 && xAI == columnAvoidOppConnect)
                    {
                        if (depth == AIDifficulty)
                        {
                            sortedColumnsList.Add(new Column() { column = columnAvoidOppConnect, score = avoidOppConnect });
                            xAI++;
                        }
                    }
        
                    yAI = FindYSpot(xAI) - 1;
                    if (yAI >= 0)
                    {
                        virtualShortMoves[xAI, yAI] = 1;
                        AILastMoves.Add(yAI);
                        AILastMoves.Add(xAI);
        
                        score = AlphaBeta(depth - 1, alpha, beta, (int)PlayerValue.Player1, AIDifficulty);
        
                        if (AILastMoves.Count != 0)
                        {
                            x = AILastMoves[AILastMoves.Count - 1];
                            AILastMoves.RemoveAt(AILastMoves.Count - 1);
                            y = AILastMoves[AILastMoves.Count - 1];
                            AILastMoves.RemoveAt(AILastMoves.Count - 1);
        
                            virtualShortMoves[xAI, yAI] = 0;
                        }
        
                        alpha = Math.Max(alpha, score);
                        if (depth == AIDifficulty)
                        {
                            sortedColumnsList.Add(new Column() { column = xAI, score = alpha });
                        }
                        if (alpha >= beta)
                        {
                            return alpha;
                        }
                    }
                }
                return alpha;
            }
            return score;
        }
        
        private int CheckSelfConnect()
        {
            int x, y, canAVoid = -1;
            for (x = 0; x <= 3; x++)
            {
                for (y = 0; y <= 2; y++)
                {
                    FillSmallBoard(x, y);
                    canAVoid = EvaluateSmallBoardForOppConnect((int)PlayerValue.AI, x, y);
                    if (canAVoid > -1)
                    {
                        return canAVoid;
                    }
                }
            }
        
            return canAVoid;
        }
        
        private int CheckOppConnect(int difficulty)
        {
            int x, y, canAVoid = -1;
            if (difficulty > 4)
            {
                for (x = 0; x <= 2; x++)
                {
                    for (y = 0; y < 6; y++)
                    {
                        FillSingleLineBoard(x, y);
                        canAVoid = EvaluateSingleLineBaordForThreat(x, y);
                        if (canAVoid > -1)
                        {
                            return canAVoid;
                        }
                    }
                }
            }
        
            canAVoid = -1;
            for (x = 0; x <= 3; x++)
            {
                for (y = 0; y <= 2; y++)
                {
                    FillSmallBoard(x, y);
                    canAVoid = EvaluateSmallBoardForOppConnect((int)PlayerValue.Player1, x, y);
                    if (canAVoid > -1)
                    {
                        return canAVoid;
                    }
                }
            }
        
            return canAVoid;
        }
        
        private int EvaluateSingleLineBaordForThreat(int xLoop, int yLoop)
        {
            int x, lineScore = 0, emptyCellsCount = 0, columnToPlay = -1, spaceBetween = 0;
        
            for (x = 0; x < 2; x++)
            {
                emptyCellsCoordonates[x] = -1;
            }
        
            for (x = 0; x < 5; x++)
            {
                if (singleLineCheck[x] == -1)
                {
                    lineScore++;
                    spaceBetween = 0;
                    if (lineScore > 2) { return columnToPlay; }
                }
                else if (singleLineCheck[x] == 0)
                {
                    emptyCellsCount++;
                    spaceBetween++;
                    if (emptyCellsCount > 3 || spaceBetween > 2) { return columnToPlay; }
                    emptyCellsCoordonates[emptyCellsCount - 1] = x + xLoop;
                }
                else if (singleLineCheck[x] == 1)
                {
                    break;
                }
            }
            if (emptyCellsCount == 3 && lineScore == 2)
            {
                columnToPlay = ReturnToStopThreatColumn(yLoop);
                if (columnToPlay >= 0)
                {
                    return columnToPlay;
                }
            }
        
            return columnToPlay;
        }
        
        private int ReturnToStopThreatColumn(int actualY)
        {
            for (int x = 0; x < 3; x++)
            {
                int xEmptyCellOnFullBoard = emptyCellsCoordonates[x];
                if (xEmptyCellOnFullBoard == 6 || virtualShortMoves[xEmptyCellOnFullBoard + 1, actualY] != 0)
                {
                    if (actualY < 5 && actualY > -1)
                    {
                        if (virtualShortMoves[xEmptyCellOnFullBoard, actualY + 1] != 0)
                        {
                            return xEmptyCellOnFullBoard;
                        }
                    }
                    else if (actualY == 5)
                    {
                        return xEmptyCellOnFullBoard;
                    }
                }
            }
        
            return -1;
        }
        
        private int EvaluateSmallBoardForOppConnect(int playerNumber, int xLoop, int yLoop)
        {
            int x, y, lineScore = 0, emptyCellsCount = 0, xEmptyCell = -1, yEmptyCell = -1, columnToPlay = -1;
        
            //Vertical lines
            for (x = 0; x < 4; x++)
            {
                for (y = 0; y < 4; y++)
                {
                    if (smallBoard[x, y] == playerNumber)
                    {
                        lineScore++;
                    }
                    else if (smallBoard[x, y] == 0)
                    {
                        emptyCellsCount++;
                        if (emptyCellsCount > 1) { break; }
                        xEmptyCell = x;
                        yEmptyCell = y;
                    }
                    else if (smallBoard[x, y] == -playerNumber)
                    {
                        break;
                    }
                }
                if (emptyCellsCount == 1 && lineScore == 3)
                {
                    columnToPlay = ReturnToBlockColumn(xEmptyCell + xLoop, yEmptyCell + yLoop);
                    if (columnToPlay >= 0)
                    {
                        return columnToPlay;
                    }
                }
        
                lineScore = 0;
                emptyCellsCount = 0;
            }
        
            //Horizontal lines
            for (y = 0; y < 4; y++)
            {
                for (x = 0; x < 4; x++)
                {
                    if (smallBoard[x, y] == playerNumber)
                    {
                        lineScore++;
                    }
                    else if (smallBoard[x, y] == 0)
                    {
                        emptyCellsCount++;
                        if (emptyCellsCount > 1) { break; }
                        xEmptyCell = x;
                        yEmptyCell = y;
                    }
                    else if (smallBoard[x, y] == -playerNumber)
                    {
                        break;
                    }
                }
                if (emptyCellsCount == 1 && lineScore == 3)
                {
                    columnToPlay = ReturnToBlockColumn(xEmptyCell + xLoop, yEmptyCell + yLoop);
                    if (columnToPlay >= 0)
                    {
                        return columnToPlay;
                    }
                }
        
                lineScore = 0;
                emptyCellsCount = 0;
            }
        
            //First diagonal
            for (x = 0; x < 4; x++)
            {
                y = x;
                if (smallBoard[x, y] == playerNumber)
                {
                    lineScore++;
                }
                else if (smallBoard[x, y] == 0)
                {
                    emptyCellsCount++;
                    if (emptyCellsCount > 1) { break; }
                    xEmptyCell = x;
                    yEmptyCell = y;
                }
                else if (smallBoard[x, y] == -playerNumber)
                {
                    break;
                }
            }
            if (emptyCellsCount == 1 && lineScore == 3)
            {
                columnToPlay = ReturnToBlockColumn(xEmptyCell + xLoop, yEmptyCell + yLoop);
                if (columnToPlay >= 0)
                {
                    return columnToPlay;
                }
            }
            lineScore = 0;
            emptyCellsCount = 0;
        
            //Second diagonal
            for (x = 0; x < 4; x++)
            {
                y = 3 - x;
                if (smallBoard[x, y] == playerNumber)
                {
                    lineScore++;
                }
                else if (smallBoard[x, y] == 0)
                {
                    emptyCellsCount++;
                    if (emptyCellsCount > 1) { break; }
                    xEmptyCell = x;
                    yEmptyCell = y;
                }
                else if (smallBoard[x, y] == -playerNumber)
                {
                    break;
                }
            }
            if (emptyCellsCount == 1 && lineScore == 3)
            {
                columnToPlay = ReturnToBlockColumn(xEmptyCell + xLoop, yEmptyCell + yLoop);
                if (columnToPlay >= 0)
                {
                    return columnToPlay;
                }
            }
        
            return columnToPlay;
        }
        
        private int ReturnToBlockColumn(int xEmptyCellOnFullBoard, int yEmptyCellOnFullBoard)
        {
            if (yEmptyCellOnFullBoard < 5 && yEmptyCellOnFullBoard > -1)
            {
                if (virtualShortMoves[xEmptyCellOnFullBoard, yEmptyCellOnFullBoard + 1] != 0)
                {
                    return xEmptyCellOnFullBoard;
                }
            }
            else if (yEmptyCellOnFullBoard == 5)
            {
                return xEmptyCellOnFullBoard;
            }
            return -1;
        }
        
        private int EvaluateBoard(int turn, int whosTurn)
        {
            int overAllScore = 0;
            int winScore = CheckForWin();
            if (winScore < 0)
            {
                overAllScore += -int.Parse(Math.Pow(Math.Abs(winScore), turn + 1).ToString());
            }
            else if (winScore > 0)
            {
                overAllScore += int.Parse(Math.Pow(Math.Abs(winScore), turn).ToString());
            }
            overAllScore += CheckForStreaks();
        
            return overAllScore;
        }
        
        private int CheckForStreaks()
        {
            int score = 0;
        
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    FillSmallBoard(x, y);
                    score += EvaluateSmallBoardForStreak();
                }
            }
            return score;
        }
        
        private int EvaluateSmallBoardForStreak()
        {
            int x, y, lineScore = 0, emptyCells = 0, totalScore = 0;
        
            //Vertical lines
            for (x = 0; x < 4; x++)
            {
                for (y = 0; y < 4; y++)
                {
                    lineScore += smallBoard[x, y];
                    if (smallBoard[x, y] == 0)
                    {
                        emptyCells++;
                    }
                }
                if (Math.Abs(lineScore) == 4 - emptyCells && Math.Abs(lineScore) < 4 && Math.Abs(lineScore) > 1)
                {
                    totalScore += lineScore;
                }
        
                lineScore = 0;
                emptyCells = 0;
            }
        
            //Horizontal lines
            for (y = 0; y < 4; y++)
            {
                for (x = 0; x < 4; x++)
                {
                    lineScore += smallBoard[x, y];
                    if (smallBoard[x, y] == 0)
                    {
                        emptyCells++;
                    }
                }
                if (Math.Abs(lineScore) == 4 - emptyCells && Math.Abs(lineScore) < 4 && Math.Abs(lineScore) > 1)
                {
                    totalScore += lineScore;
                }
        
                lineScore = 0;
                emptyCells = 0;
            }
        
            //First diagonal
            for (x = 0; x < 4; x++)
            {
                y = x;
                lineScore += smallBoard[x, y];
                if (smallBoard[x, y] == 0)
                {
                    emptyCells++;
                }
                if (Math.Abs(lineScore) == 4 - emptyCells && Math.Abs(lineScore) < 4 && Math.Abs(lineScore) > 1)
                {
                    totalScore += lineScore;
                }
        
                lineScore = 0;
                emptyCells = 0;
            }
        
            //Second diagonal
            for (x = 0; x < 4; x++)
            {
                y = 3 - x;
                lineScore += smallBoard[x, y];
                if (smallBoard[x, y] == 0)
                {
                    emptyCells++;
                }
                if (Math.Abs(lineScore) == 4 - emptyCells && Math.Abs(lineScore) < 4 && Math.Abs(lineScore) > 1)
                {
                    totalScore += lineScore;
                }
        
                lineScore = 0;
                emptyCells = 0;
            }
        
            return totalScore;
        }
        
        private int CheckForWin()
        {
            int score = 0;
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    FillSmallBoard(x, y);
                    score = EvaluateSmallBoardForWin();
                    if (Math.Abs(score) == Math.Abs(AIWin)) { return score; }
                }
            }
            return score;
        }
        
        private int EvaluateSmallBoardForWin()
        {
            int x, y, totalOfLine = 0;
        
            //Check Vertical lines
            for (x = 0; x < 4; x++)
            {
                for (y = 0; y < 4; y++)
                {
                    totalOfLine += smallBoard[x, y];
                }
                if (Math.Abs(totalOfLine) == 4)
                {
                    if (totalOfLine < 0)
                    {
                        return PlayerWin;
                    }
                    else
                    {
                        return AIWin;
                    }
                }
                totalOfLine = 0;
            }
        
            //Check Horizontal lines
            for (y = 0; y < 4; y++)
            {
                for (x = 0; x < 4; x++)
                {
                    totalOfLine += smallBoard[x, y];
                }
                if (Math.Abs(totalOfLine) == 4)
                {
                    if (totalOfLine < 0)
                    {
                        return PlayerWin;
                    }
                    else
                    {
                        return AIWin;
                    }
                }
                totalOfLine = 0;
            }
        
            //Check Diagonal lines
            for (x = 0; x < 4; x++)
            {
                y = x;
                totalOfLine += smallBoard[x, y];
            }
            if (Math.Abs(totalOfLine) == 4)
            {
                if (totalOfLine < 0)
                {
                    return PlayerWin;
                }
                else
                {
                    return AIWin;
                }
            }
            totalOfLine = 0;
        
            for (x = 0; x < 4; x++)
            {
                y = 3 - x;
                totalOfLine += smallBoard[x, y];
            }
            if (Math.Abs(totalOfLine) == 4)
            {
                if (totalOfLine < 0)
                {
                    return PlayerWin;
                }
                else
                {
                    return AIWin;
                }
            }
        
            return 0;
        }
        
        private void FillSmallBoard(int xBegin, int yBegin)
        {
            for (int x = xBegin; x < xBegin + 4; x++)
            {
                for (int y = yBegin; y < yBegin + 4; y++)
                {
                    smallBoard[(x - xBegin), (y - yBegin)] = virtualShortMoves[x, y];
                }
            }
        }
        
        private void FillSingleLineBoard(int xBegin, int yPosition)
        {
            for (int x = xBegin; x < xBegin + 5; x++)
            {
                singleLineCheck[(x - xBegin)] = virtualShortMoves[x, yPosition];
            }
        }
        
        public int FindYSpot(int column)
        {
            if (column >= 7)
            {
                return -1;
            }
            if (virtualShortMoves[column, 0] != 0)
            {
                return 0;
            }
        
            for (int y = 0; y <= 5; y++)
            {
                if (virtualShortMoves[column, y] != 0)
                {
                    return y;
                }
            }
            return 6;
        }
        
        private int FindDisponibility(int column)
        {
            int y = FindYSpot(column);
            if (y == 0)
            {
                do
                {
                    column++;
                    if (column >= 7)
                    {
                        break;
                    }
                    y = FindYSpot(column);
                } while (y == 0);
            }
            return column;
        }
        
        private bool TranslateActualEllipseBoard(Brush AIBrush, Brush playerBrush, Brush justPlayedBrush)
        {
            sortedColumnsList.Clear();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Brush coinBrush = myBoard.GetCoinBrush(x, y);
                    if (coinBrush == AIBrush || coinBrush == justPlayedBrush)
                    {
                        virtualShortMoves[x, y] = (int)PlayerValue.AI;
                    }
                    else if (coinBrush == Brushes.LightGray)
                    {
                        virtualShortMoves[x, y] = (int)PlayerValue.Nobody;
                    }
                    else if (coinBrush == playerBrush)
                    {
                        virtualShortMoves[x, y] = (int)PlayerValue.Player1;
                    }
                }
            }
            return true;
        }
    }
}
