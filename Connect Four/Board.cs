using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading;

namespace Connect_Four
{
    public class Board
    {
        public static Ellipse[,] virtualEllipseBoard = new Ellipse[7, 6];
        private int[,] winCells = new int[3, 2];
        int xLastMove = -1;
        int yLastMove = -1;
        Brush winBrush = Brushes.HotPink;
        
        public Ellipse[,] GetActualBoard()
        {
            return virtualEllipseBoard;
        }

        public Ellipse GetOneSpot(int x, int y)
        {
            return virtualEllipseBoard[x, y];
        }

        public void Add(int x, int y, Ellipse myEllipse)
        {
            virtualEllipseBoard[x, y] = myEllipse;
        }

        public void FillCoin(int x, int y, Brush playerBrush)
        {
            virtualEllipseBoard[x, y].Fill = playerBrush;
        }
        
        public Brush GetCoinBrush(int x, int y)
        {
            return virtualEllipseBoard[x, y].Fill;
        }
        
        public int FindCoinSpot(int column)
        {
            if(GetCoinBrush(column, 0) != Brushes.LightGray)
            {
                return 0;
            }
            
            int y = 6;
            for (int a = 0; a <= 5; a++)
            {
                if (GetCoinBrush(column, a) != Brushes.LightGray)
                {
                    y = a;
                    break;
                }
            }
            return y;
        }

        public int CheckForSameBrush(int x, int y, Brush playerBrush)
        {
            int inARow = 0;

            Brush coinBrush = GetCoinBrush(x, y);
            if (coinBrush == playerBrush)
            {
                inARow++;
            }
            else
            {
                return inARow;
            }
            return inARow;
        }

        public Brush CheckForWinningMove(int xLastCoin, int yLastCoin)
        {
            for (int a = 0; a < 3; a++)
            {
                for (int b = 0; b < 2; b++)
                {
                    winCells[a, b] = -1;
                }
            }
        
            xLastMove = xLastCoin;
            yLastMove = yLastCoin;

            Brush playerBrush = GetCoinBrush(xLastMove, yLastMove);

            bool winYet1 = CheckHorizontal(playerBrush);
            bool winYet2 = CheckVertical(playerBrush);
            bool winYet3 = CheckCross1(playerBrush);
            bool winYet4 = CheckCross2(playerBrush);
            if (winYet1 == true || winYet2 == true || winYet3 == true || winYet4 == true)
            {
                FillCoin(xLastMove, yLastMove, winBrush);
                for (int a = 0; a < 3; a++)
                {
                    FillCoin(winCells[a, 0], winCells[a, 1], winBrush);
                }

                return playerBrush;
            }
            else
            {
                return Brushes.LightGray;
            }
        }

        private bool CheckHorizontal(Brush playerBrush)
        {
            int inARow = 0;
            ClearWinCells();

            for (int x = xLastMove + 1; x < xLastMove + 4; x++)
            {
                if (x <= 6 && CheckForSameBrush(x, yLastMove, playerBrush) != 0)
                {
                    AddToWinCells(inARow, x, yLastMove);
                    inARow += CheckForSameBrush(x, yLastMove, playerBrush);
                }
                else
                {
                    break;
                }
            }

            for (int x = xLastMove - 1; x > xLastMove - 4; x--)
            {
                if (x >= 0 && CheckForSameBrush(x, yLastMove, playerBrush) != 0)
                {
                    AddToWinCells(inARow, x, yLastMove);
                    inARow += CheckForSameBrush(x, yLastMove, playerBrush);
                }
                else
                {
                    break;
                }
            }

            return CountConnectFour(inARow);
        }

        private bool CheckVertical(Brush playerBrush)
        {
            int inARow = 0;
            ClearWinCells();

            for (int y = yLastMove + 1; y < yLastMove + 4; y++)
            {
                if (y <= 5 && CheckForSameBrush(xLastMove, y, playerBrush) != 0)
                {
                    AddToWinCells(inARow, xLastMove, y);
                    inARow += CheckForSameBrush(xLastMove, y, playerBrush);
                }
                else
                {
                    break;
                }
            }

            return CountConnectFour(inARow);
        }

        private bool CheckCross1(Brush playerBrush)
        {
            int inARow = 0;
            ClearWinCells();

            int y = yLastMove;
            for (int x = xLastMove + 1; x < xLastMove + 4; x++)
            {
                y++;
                if (x <= 6 && y <= 5 && CheckForSameBrush(x, y, playerBrush) != 0)
                {
                    AddToWinCells(inARow, x, y);
                    inARow += CheckForSameBrush(x, y, playerBrush);
                }
                else
                {
                    break;
                }
            }

            y = yLastMove;
            for (int x = xLastMove - 1; x > xLastMove - 4; x--)
            {
                y--;
                if (x >= 0 && y >= 0 && CheckForSameBrush(x, y, playerBrush) != 0)
                {
                    AddToWinCells(inARow, x, y);
                    inARow += CheckForSameBrush(x, y, playerBrush);
                }
                else
                {
                    break;
                }
            }

            return CountConnectFour(inARow);
        }

        private bool CheckCross2(Brush playerBrush)
        {
            int inARow = 0;
            ClearWinCells();

            int y = yLastMove;
            for (int x = xLastMove + 1; x < xLastMove + 4; x++)
            {
                y--;
                if (x <= 6 && y >= 0 && CheckForSameBrush(x, y, playerBrush) != 0)
                {
                    AddToWinCells(inARow, x, y);
                    inARow += CheckForSameBrush(x, y, playerBrush);
                }
                else
                {
                    break;
                }
            }

            y = yLastMove;
            for (int x = xLastMove - 1; x > xLastMove - 4; x--)
            {
                y++;
                if (x >= 0 && y <= 5 && CheckForSameBrush(x, y, playerBrush) != 0)
                {
                    AddToWinCells(inARow, x, y);
                    inARow += CheckForSameBrush(x, y, playerBrush);
                }
                else
                {
                    break;
                }
            }
            return CountConnectFour(inARow);
        }

        public bool CountConnectFour(int numberOfAlignCoins)
        {
            if (numberOfAlignCoins >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddToWinCells(int a, int x, int y)
        {
            if (winCells[2, 0] == -1)
            {
                winCells[a, 0] = x;
                winCells[a, 1] = y;
            }
        }

        private void ClearWinCells()
        {
            if(winCells[2, 0] == -1 && winCells[2, 1] == -1)
            {
                for(int a = 0; a < 2; a++)
                {
                    for(int b = 0; b < 2; b++)
                    {
                        winCells[a, b] = -1;
                    }
                }
            }
        }
    }
}
