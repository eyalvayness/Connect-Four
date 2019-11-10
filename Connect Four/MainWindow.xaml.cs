using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Connect_Four
{
    public partial class MainWindow : Window
    {
        public int AIDifficulty = 1;
        public int descendingY = 0;
        static int time = 0;
        static int maxTurnTime = 10;
        static int xLastCoin = -1;
        static int yLastCoin = -1;
        static int coinColumn = -1;
        public int currentPlayer = (int)whichPlayerTurn.Player1;
        static bool AIvsEnable = false;
        static bool firstTime = true;
        static bool firstPass = true;
        static bool timerModeEnable = false;
        static bool AIModeEnable = false;
        static int winnerPlayer = 0;
        const string timeLeft = " sec is left";
        const string playerTurn = "It is the turn of ";
        static string namePlayer1 = "Player 1";
        static string namePlayer2 = "Player 2";
        public static int numberOfCoins = 1;
        Brush player1Brush = Brushes.Red;
        Brush player2Brush = Brushes.Yellow;
        Brush emptyBrush = Brushes.LightGray;
        Brush justPlayedBrush = Brushes.YellowGreen;
        AI2 myAI2 = new AI2();
        AI myAI = new AI();
        Board myBoard = new Board();
        List<int> undoList = new List<int>();
        Window SettingsWindow = new Window();
        Window passTurn = new Window();
        Label player1Lbl = new Label();
        Label player2Lbl = new Label();
        TextBox player1TxtBx = new TextBox();
        TextBox player2TxtBx = new TextBox();
        CheckBox timerMode = new CheckBox();
        Slider sliderTurnDuration = new Slider();
        CheckBox AIMode = new CheckBox();
        CheckBox AIvs = new CheckBox();
        RadioButton easyDifficulty = new RadioButton();
        RadioButton mediumDiificulty = new RadioButton();
        RadioButton hardDifficulty = new RadioButton();
        DispatcherTimer turnTimer = new DispatcherTimer();

        public enum whichPlayerTurn : int
        {
            Player1 = -1,
            Player2 = 1,
            Nobody = 0
        }

        public MainWindow()
        {
            InitializeComponent();

            SetGrid();
            
            TimeSpan timerTimeSpan = new TimeSpan(0, 0, 1);
            turnTimer.Interval = timerTimeSpan;
            turnTimer.Tick += new EventHandler(TurnTimerTick);
            

            string downArrow = " |\n\\|/";
            button_Column1.Content = downArrow;
            button_Column2.Content = downArrow;
            button_Column3.Content = downArrow;
            button_Column4.Content = downArrow;
            button_Column5.Content = downArrow;
            button_Column6.Content = downArrow;
            button_Column7.Content = downArrow;

            if (firstTime)
            {
                SetSettings();
                //namePlayer1 = "Eyal";
                //namePlayer2 = "Robin";
                //maxTurnTime = 10;
                //timerModeEnable = false;
                //AIModeEnable = true;
                //AIvsEnable = true;
            }
            
            turnHeader.Content = playerTurn + namePlayer1;
            currentPlayer = (int)whichPlayerTurn.Player1;
        }
        
        private void ResetGridChildren()
        {
            ConnectFourGrid.Children.Clear();
            ConnectFourGrid.Children.Add(button_Column1);
            ConnectFourGrid.Children.Add(button_Column2);
            ConnectFourGrid.Children.Add(button_Column3);
            ConnectFourGrid.Children.Add(button_Column4);
            ConnectFourGrid.Children.Add(button_Column5);
            ConnectFourGrid.Children.Add(button_Column6);
            ConnectFourGrid.Children.Add(button_Column7);

            for(int x = 0; x < 7; x++)
            {
                for(int y = 0; y < 6; y++)
                {
                    Grid.SetColumn(myBoard.GetOneSpot(x,y), x);
                    Grid.SetRow(myBoard.GetOneSpot(x, y), y+1);
                    ConnectFourGrid.Children.Add(myBoard.GetOneSpot(x, y));
                }
            }
        }

        private void SetSettings()
        {
            if (firstTime)
            {
                SettingsWindow .Width = 320;
                SettingsWindow .Height = 305;
                SettingsWindow.Title = "Settings";

                StackPanel inputPanel = new StackPanel();

                Grid myGrid = new Grid();
                ColumnDefinition columns = new ColumnDefinition();
                columns.MinWidth = 100;
                myGrid.ColumnDefinitions.Add(columns);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();
                RowDefinition row4 = new RowDefinition();
                RowDefinition row5 = new RowDefinition();
                RowDefinition row6 = new RowDefinition();
                row1.Height = new GridLength(45);
                row2.Height = new GridLength(45);
                row3.Height = new GridLength(45);
                row4.Height = new GridLength(45);
                row5.Height = new GridLength(45);
                row6.Height = new GridLength(45);
                myGrid.RowDefinitions.Add(row1);
                myGrid.RowDefinitions.Add(row2);
                myGrid.RowDefinitions.Add(row3);
                myGrid.RowDefinitions.Add(row4);
                myGrid.RowDefinitions.Add(row5);
                myGrid.RowDefinitions.Add(row6);

                player1Lbl.Content = "Player 1 name :";
                player2Lbl.Content = "Player 2 name :";
                player1Lbl.Width = 100;
                player1Lbl.Width = 100;
                player1Lbl.HorizontalAlignment = HorizontalAlignment.Left;
                player2Lbl.HorizontalAlignment = HorizontalAlignment.Left;
                player1Lbl.VerticalAlignment = VerticalAlignment.Center;
                player2Lbl.VerticalAlignment = VerticalAlignment.Center;

                player1TxtBx.Width = 150;
                player2TxtBx.Width = 150;
                player1TxtBx.Background = Brushes.LightGray;
                player2TxtBx.Background = Brushes.LightGray;
                player1TxtBx.HorizontalAlignment = HorizontalAlignment.Right;
                player2TxtBx.HorizontalAlignment = HorizontalAlignment.Right;
                player1TxtBx.VerticalAlignment = VerticalAlignment.Center;
                player2TxtBx.VerticalAlignment = VerticalAlignment.Center;
                player1TxtBx.Margin = new Thickness(0, 0, 2, 0);
                player2TxtBx.Margin = new Thickness(0, 0, 2, 0);

                timerMode.HorizontalAlignment = HorizontalAlignment.Left;
                timerMode.VerticalAlignment = VerticalAlignment.Center;
                timerMode.Width = 150;
                timerMode.Content = "Timer Mode ("+ maxTurnTime.ToString()+" sec)";
                timerMode.IsChecked = false;
                timerMode.Checked += ChangeSliderEnable;
                timerMode.Unchecked += ChangeSliderDisable;
                timerMode.Margin = new Thickness(2, 0, 0, 0);

                sliderTurnDuration.IsEnabled = false;
                sliderTurnDuration.Maximum = 20;
                sliderTurnDuration.Minimum = 5;
                sliderTurnDuration.Value = 10;
                sliderTurnDuration.TickFrequency = 1;
                sliderTurnDuration.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;
                sliderTurnDuration.Width = 100;
                sliderTurnDuration.Height = 20;
                sliderTurnDuration.HorizontalAlignment = HorizontalAlignment.Right;
                sliderTurnDuration.VerticalAlignment = VerticalAlignment.Center;
                sliderTurnDuration.IsSnapToTickEnabled = true;
                sliderTurnDuration.ValueChanged += ChangeSlideValue;
                sliderTurnDuration.Margin = new Thickness(0, 0, 2, 0);

                AIMode.HorizontalAlignment = HorizontalAlignment.Left;
                AIMode.VerticalAlignment = VerticalAlignment.Center;
                AIMode.Width = 200;
                AIMode.Content = "Against the computer";
                AIMode.IsChecked = false;
                AIMode.Checked += ChangePlayer2Disable;
                AIMode.Unchecked += ChangePlayer2Enable;
                AIMode.Margin = new Thickness(2, 0, 0, 0);

                //AIvs.HorizontalAlignment = HorizontalAlignment.Right;
                //AIvs.VerticalAlignment = VerticalAlignment.Center;
                //AIvs.Width = 75;
                //AIvs.Content = "AI vs AI";
                //AIvs.IsChecked = false;
                //AIvs.IsEnabled = false;
                //AIvs.Margin = new Thickness(0, 0, 2, 0);

                easyDifficulty.HorizontalAlignment = HorizontalAlignment.Left;
                easyDifficulty.VerticalAlignment = VerticalAlignment.Center;
                easyDifficulty.Content = "Easy";
                easyDifficulty.IsEnabled = false;
                easyDifficulty.Margin = new Thickness(2, 0, 0, 0);
                mediumDiificulty.HorizontalAlignment = HorizontalAlignment.Center;
                mediumDiificulty.VerticalAlignment = VerticalAlignment.Center;
                mediumDiificulty.Content = "Medium";
                mediumDiificulty.IsEnabled = false;
                hardDifficulty.HorizontalAlignment = HorizontalAlignment.Right;
                hardDifficulty.VerticalAlignment = VerticalAlignment.Center;
                hardDifficulty.Content = "Hard";
                hardDifficulty.IsEnabled = false;
                hardDifficulty.Margin = new Thickness(0, 0, 2, 0);

                Button okButton = new Button();
                okButton.Content = "Apply settings";
                okButton.HorizontalAlignment = HorizontalAlignment.Center;
                okButton.VerticalAlignment = VerticalAlignment.Center;
                okButton.Click += CloseSettingBox;
                
                Grid.SetRow(player1Lbl ,0);
                Grid.SetColumn(player1Lbl, 0);
                myGrid.Children.Add(player1Lbl);
                Grid.SetRow(player1TxtBx, 0);
                Grid.SetColumn(player1TxtBx, 1);
                myGrid.Children.Add(player1TxtBx);
                Grid.SetRow(player2Lbl, 1);
                Grid.SetColumn(player2Lbl, 0);
                myGrid.Children.Add(player2Lbl);
                Grid.SetRow(player2TxtBx, 1);
                Grid.SetColumn(player2TxtBx, 1);
                myGrid.Children.Add(player2TxtBx);
                Grid.SetRow(timerMode, 2);
                Grid.SetColumn(timerMode, 0);
                myGrid.Children.Add(timerMode);
                Grid.SetRow(sliderTurnDuration, 2);
                Grid.SetColumn(sliderTurnDuration, 1);
                myGrid.Children.Add(sliderTurnDuration);
                Grid.SetRow(AIMode, 3);
                Grid.SetColumn(AIMode, 0);
                myGrid.Children.Add(AIMode);
                //Grid.SetRow(AIvs, 3);
                //Grid.SetColumn(AIvs, 1);
                //myGrid.Children.Add(AIvs);
                Grid.SetRow(easyDifficulty, 4);
                myGrid.Children.Add(easyDifficulty);
                Grid.SetRow(mediumDiificulty, 4);
                myGrid.Children.Add(mediumDiificulty);
                Grid.SetRow(hardDifficulty, 4);
                myGrid.Children.Add(hardDifficulty);
                Grid.SetRow(okButton, 5);
                myGrid.Children.Add(okButton);

                inputPanel.Children.Add(myGrid);
                SettingsWindow.Content = inputPanel;
                SettingsWindow.ShowDialog();
            }
            else
            {
                MainPanel.Visibility = Visibility.Hidden;
                MainPanel.Visibility = Visibility.Collapsed;
                player1TxtBx.Text = "";
                player2TxtBx.Text = "";
                SettingsWindow.Visibility = Visibility.Visible;

                turnTimer.Stop();
            }
        }

        private void CloseSettingBox(object sender, RoutedEventArgs args)
        {
            string oldPlayer2Name = namePlayer2;
            bool AIModeWasEnabled = AIModeEnable;
            AIvsEnable = AIvs.IsChecked.Value;
            AIModeEnable = AIMode.IsChecked.Value;
            timerModeEnable = timerMode.IsChecked.Value;

            if (AIModeEnable)
            {
                if (!AIModeWasEnabled)
                {
                    for(int x = 0; x < 7; x++)
                    {
                        for(int y = 0; y < 6; y++)
                        {
                            myBoard.FillCoin(x, y, emptyBrush);
                        }
                    }
                }

                turnHeader.Content = playerTurn + namePlayer1;
                currentPlayer = (int)whichPlayerTurn.Player1;

                if (player1TxtBx.Text != "" && player1TxtBx != null)
                {
                    namePlayer1 = player1TxtBx.Text;
                }
                
                

                if (easyDifficulty.IsChecked == true)
                {
                    AIDifficulty = 3;
                }
                if (mediumDiificulty.IsChecked == true)
                {
                    AIDifficulty = 5;
                }
                if (hardDifficulty.IsChecked == true)
                {
                    AIDifficulty = 8;
                }
            }
            else
            {
                if (AIModeWasEnabled)
                {
                    for (int x = 0; x < 7; x++)
                    {
                        for (int y = 0; y < 6; y++)
                        {
                            myBoard.FillCoin(x, y, emptyBrush);
                        }
                    }
                }

                if (player1TxtBx.Text != "" && player1TxtBx != null)
                {
                    namePlayer1 = player1TxtBx.Text;
                }
                if (player2TxtBx.Text != "" && player2TxtBx != null)
                {
                    namePlayer2 = player2TxtBx.Text;    
                }

                if(turnHeader.Content.ToString() == (playerTurn + oldPlayer2Name))
                {
                    turnHeader.Content = playerTurn + namePlayer2;
                }
                else
                {
                    turnHeader.Content = playerTurn + namePlayer1;
                }
            }

            if (timerModeEnable)
            {
                maxTurnTime = int.Parse(sliderTurnDuration.Value.ToString());
                timeLeftLbl.Content = maxTurnTime.ToString() + timeLeft;
            }
            else
            {
                timeLeftLbl.Content = "";
            }
            enableButtons();
            SettingsWindow.Visibility = Visibility.Hidden;
            MainPanel.Visibility = Visibility.Visible;
            firstTime = false;
        }

        private void SetSettingsButton(object sender, RoutedEventArgs e)
        {
            SetSettings();
        }

        public void SetGrid()
        {
            ConnectFourGrid.Background = Brushes.Blue;

            for (int a = 0; a < ConnectFourGrid.ColumnDefinitions.Count; a++)
            {
                for (int b = 1; b < ConnectFourGrid.RowDefinitions.Count; b++)
                {
                    int c = b - 1;
                    Ellipse myEllipse = new Ellipse();
                    myEllipse.Name = "Tile_" + c.ToString() + a.ToString();
                    myEllipse.Fill = emptyBrush;
                    myEllipse.HorizontalAlignment = HorizontalAlignment.Stretch;
                    myEllipse.VerticalAlignment = VerticalAlignment.Stretch;
                    myEllipse.Margin = new Thickness(10, 10, 0, 0);


                    myBoard.Add(a, c, myEllipse);
                    
                    Grid.SetColumn(myEllipse, a);
                    Grid.SetRow(myEllipse, b);
                    ConnectFourGrid.Children.Add(myEllipse);
                }
            }
        }

        private void Column1Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 0;
            MakeMove();
        }

        private void Column2Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 1;
            MakeMove();
        }

        private void Column3Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 2;
            MakeMove();
        }

        private void Column4Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 3;
            MakeMove();
        }

        private void Column5Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 4;
            MakeMove();
        }

        private void Column6Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 5;
            MakeMove();
        }

        private void Column7Coin(object sender, RoutedEventArgs e)
        {
            coinColumn = 6;
            MakeMove();
        }

        public void MakeMove()
        {
            if(timerModeEnable == true)
            {
                if(turnTimer.IsEnabled == true)
                {
                    turnTimer.Stop();
                    turnTimer.Start();
                    time = 0;
                    timeLeftLbl.Content = maxTurnTime + timeLeft;
                }
                else
                {
                    turnTimer.Start();
                    timeLeftLbl.Content = maxTurnTime + timeLeft;
                }
            }
            bool notFullColumn = InsertCoin();
            if (!notFullColumn)
            {
                return;
            }
            winnerPlayer = HaveWinGame();
            if (winnerPlayer != (int)whichPlayerTurn.Nobody)
            {
                EndOfGame();
                return;
            }
            else if (numberOfCoins == 42)
            {
                numberOfCoins = 0;
                ExAequo();
                return;
            }
            else if (notFullColumn && winnerPlayer == (int)whichPlayerTurn.Nobody)
            {
                if (AIModeEnable)
                {
                    MakeAIMove();
                }
                else
                {
                    ChangePlayerTurn();
                }
            }
        }

        private void MakeAIMove()
        {
            ChangePlayerTurn();
            xLastCoin = myAI2.SetMove(AIDifficulty, player2Brush, player1Brush, justPlayedBrush);
            yLastCoin = myBoard.FindCoinSpot(xLastCoin) - 1;
            myBoard.FillCoin(xLastCoin, yLastCoin, player2Brush);
            winnerPlayer = HaveWinGame();
            if (winnerPlayer != (int)whichPlayerTurn.Nobody)
            {
                EndOfGame();
                return;
            }
            else if (numberOfCoins == 42)
            {
                numberOfCoins = 0;
                ExAequo();
                return;
            }
            myBoard.FillCoin(xLastCoin, yLastCoin, justPlayedBrush);
            undoList.Add(xLastCoin);
            numberOfCoins++;
            ChangePlayerTurn();
        }

        public void ExAequo()
        {
            turnHeader.Content = "Nobody has won the game.\nPress 'Reset Grid' to restart (bunch of assholes..)";
            disableButtons();
            if(timerMode.IsEnabled == true)
            {
                turnTimer.Stop();
                time = 0;
                timeLeftLbl.Content = maxTurnTime + timeLeft;
            }
        }

        public void EndOfGame()
        {
            if (currentPlayer == (int)whichPlayerTurn.Player1)
            {
                turnHeader.Content = namePlayer1 + " has won the game.\nPress 'Reset Grid' to restart";
                currentPlayer = (int)whichPlayerTurn.Player2;
            }
            else if (currentPlayer == (int)whichPlayerTurn.Player2)
            {
                if (AIModeEnable)
                {
                    turnHeader.Content = "The computer has won the game.\nPress 'Reset Grid' to restart";
                }
                else
                {
                    turnHeader.Content = namePlayer2 + " has won the game.\nPress 'Reset Grid' to restart";
                }
                currentPlayer = (int)whichPlayerTurn.Player1;
            }
            if (timerModeEnable == true)
            {
                turnTimer.Stop();
                time = 0;
                timeLeftLbl.Content = maxTurnTime + timeLeft;
            }
        }

        public int HaveWinGame()
        {
            Brush myBrush = myBoard.CheckForWinningMove(xLastCoin, yLastCoin);
            if(myBrush == player1Brush) { return (int)whichPlayerTurn.Player1; }
            else if(myBrush == player2Brush) { return (int)whichPlayerTurn.Player2; }
            else { return (int)whichPlayerTurn.Nobody; }
        }

        public bool InsertCoin()
        {
            if (myBoard.GetCoinBrush(coinColumn, 0) != emptyBrush)
            {
                return false;
            }

            int y = myBoard.FindCoinSpot(coinColumn)-1;

            if(undoList.Count > 0)
            {
                if(myBoard.GetCoinBrush(xLastCoin, yLastCoin) == justPlayedBrush)
                {
                    myBoard.FillCoin(xLastCoin, yLastCoin, player2Brush);
                }
            }

            xLastCoin = coinColumn;
            yLastCoin = y;
            undoList.Add(coinColumn);
            numberOfCoins++;

            if (currentPlayer == (int)whichPlayerTurn.Player1)
            {
                myBoard.FillCoin(xLastCoin, yLastCoin, player1Brush);
            }
            else if (currentPlayer == (int)whichPlayerTurn.Player2)
            {
                myBoard.FillCoin(xLastCoin, yLastCoin, player2Brush);
            }



            return true;
        }

        public void ChangePlayerTurn()
        {
            if (currentPlayer == (int)whichPlayerTurn.Player1)
            {
                turnHeader.Content = playerTurn + namePlayer2;
                currentPlayer = (int)whichPlayerTurn.Player2;
            }
            else if (currentPlayer == (int)whichPlayerTurn.Player2)
            {
                turnHeader.Content = playerTurn + namePlayer1;
                currentPlayer = (int)whichPlayerTurn.Player1;
            }
            if (timerModeEnable)
            {
                time = 0;
                if(turnTimer.IsEnabled)
                {
                    turnTimer.Stop();
                }
                turnTimer.Start();
            }
        }

        private void ResetGrid(object sender, RoutedEventArgs e)
        {
            numberOfCoins = 0;
            undoList.Clear();
            enableButtons();
            for (int x = 0; x<7; x++)
            {
                for(int y = 0; y < 6; y++)
                {
                    myBoard.FillCoin(x, y, emptyBrush);
                }
            }
            
            if (timerModeEnable == true)
            {
                turnTimer.Stop();
                time = 0;
                timeLeftLbl.Content = maxTurnTime + timeLeft;
            }

            if (currentPlayer == (int)whichPlayerTurn.Player1)
            {
                turnHeader.Content = playerTurn + namePlayer1;
            }
            else if (currentPlayer == (int)whichPlayerTurn.Player2)
            {
                if (AIModeEnable)
                {
                    MakeAIMove();
                    if (timerModeEnable == true)
                    {
                        turnTimer.Start();
                    }
                    ChangePlayerTurn();
                }
                else
                {
                    turnHeader.Content = playerTurn + namePlayer2;
                }
            }
        }

        private void TurnTimerTick(object sender, EventArgs e)
        {
            time++;
            timeLeftLbl.Content = (maxTurnTime - time).ToString() + timeLeft;
            if (time == maxTurnTime )
            {
                time = 0;
                turnTimer.Stop();
                if (firstPass)
                {
                    firstPass = false;
                    StackPanel myStackPanel = new StackPanel();
                    Label myLabel = new Label();
                    Button myButton = new Button();

                    passTurn.Height = 150;
                    passTurn.Width = 300;

                    myStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    myStackPanel.VerticalAlignment = VerticalAlignment.Stretch;

                    myLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    myLabel.VerticalAlignment = VerticalAlignment.Center;

                    myButton.HorizontalAlignment = HorizontalAlignment.Center;
                    myButton.VerticalAlignment = VerticalAlignment.Bottom;
                    myButton.Height = 20;
                    myButton.Width = 150;
                    myButton.Click += CloseDialogBox;
                    myButton.Margin = new Thickness(0, 0, 0, passTurn.Height / 3 - 20);

                    passTurn.Title = "Yout took too much time, idiot..";
                    if(currentPlayer == (int)whichPlayerTurn.Player1)
                    {
                        myLabel.Content = "You took too much time by thinking. \nIt is now te turn of "+ namePlayer2;
                        myButton.Content = "Begin turn of " + namePlayer2;
                    }
                    else if (currentPlayer == (int)whichPlayerTurn.Player2)
                    {
                        myLabel.Content = "You took too much time by thinking. \nIt is now te turn of " + namePlayer1;
                        myButton.Content = "Begin turn of " + namePlayer1;
                    }

                    myStackPanel.Children.Add(myLabel);
                    myStackPanel.Children.Add(myButton);
                    passTurn.Content = myStackPanel;
                    disableButtons();
                    passTurn.ShowDialog();

                }
                else
                {
                    passTurn.Visibility = Visibility.Visible;
                    passTurn.Activate();
                    turnTimer.Stop();
                }

            }
            timeLeftLbl.Content = (maxTurnTime-time).ToString() + timeLeft;

        }

        private void UndoMove(object sender, RoutedEventArgs e)
        {
            if(undoList.Count > 0)
            {
                int x = undoList[undoList.Count - 1];
                undoList.RemoveAt(undoList.Count - 1);
                int y = myBoard.FindCoinSpot(x);

                myBoard.FillCoin(x, y, emptyBrush);
                ChangePlayerTurn();
                numberOfCoins--;
                if (AIModeEnable && undoList.Count > 0)
                {
                    x = undoList[undoList.Count - 1];
                    undoList.RemoveAt(undoList.Count - 1);
                    y = myBoard.FindCoinSpot(x);

                    myBoard.FillCoin(x, y, emptyBrush);
                    ChangePlayerTurn();
                    numberOfCoins--;
                }
            }
        }

        public void enableButtons()
        {
            button_Column1.IsEnabled = true;
            button_Column2.IsEnabled = true;
            button_Column3.IsEnabled = true;
            button_Column4.IsEnabled = true;
            button_Column5.IsEnabled = true;
            button_Column6.IsEnabled = true;
            button_Column7.IsEnabled = true;
            undoButton.IsEnabled = true;
        }

        public void disableButtons()
        {
            button_Column1.IsEnabled = false;
            button_Column2.IsEnabled = false;
            button_Column3.IsEnabled = false;
            button_Column4.IsEnabled = false;
            button_Column5.IsEnabled = false;
            button_Column6.IsEnabled = false;
            button_Column7.IsEnabled = false;
            undoButton.IsEnabled = false;
        }
        
        private void ChangePlayer2Disable(object sender, EventArgs e)
        {
            player2Lbl.IsEnabled = false;
            player2TxtBx.IsEnabled = false;

            easyDifficulty.IsEnabled = true;
            mediumDiificulty.IsEnabled = true;
            hardDifficulty.IsEnabled = true;
            AIvs.IsEnabled = true;
        }

        private void ChangePlayer2Enable(object sender, EventArgs e)
        {
            player2Lbl.IsEnabled = true;
            player2TxtBx.IsEnabled = true;

            easyDifficulty.IsEnabled = false;
            mediumDiificulty.IsEnabled = false;
            hardDifficulty.IsEnabled = false;
            AIvs.IsEnabled = false;
        }

        private void ChangeSliderEnable(object sender, EventArgs e)
        {
            sliderTurnDuration.IsEnabled = true;
        }

        private void ChangeSliderDisable(object sender, EventArgs e)
        {
            sliderTurnDuration.IsEnabled = false;
        }

        private void ChangeSlideValue(object sender, EventArgs e)
        {
            timerMode.Content = "Timer Mode (" + sliderTurnDuration.Value.ToString() + " sec)";
        }

        private void CloseDialogBox (object sender, EventArgs e)
        {
            passTurn.Visibility = Visibility.Hidden;
            MainPanel.Visibility = Visibility.Visible;
            MainPanel.Activate();
            if (AIModeEnable)
            {
                MakeAIMove();
            }
            else
            {
                ChangePlayerTurn();
            }
            enableButtons();
        }

        private void FinishApp(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
