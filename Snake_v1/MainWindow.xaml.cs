using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty,Images.Empty },
            {GridValue.Snake,Images.Body },
            {GridValue.Food,Images.Food }
        };

        private readonly Dictionary<Directions, int> dirToRoation = new()
        {
            {Directions.Up,0 },
            {Directions.Right,90 },
            {Directions.Down,180 },
            {Directions.Left,270 }
            

        };

        private readonly int rows = 20,cols = 20;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunnig;
        private int highScore = 0;





        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            highScore = Properties.Settings.Default.HighScore;
            gameState = new GameState(rows,cols);

           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            
            Properties.Settings.Default.HighScore = highScore;
            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);

        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if(!gameRunnig)
            {
                gameRunnig = true;
                await RunGame();
                gameRunnig = false;
            }

        }



        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(gameState.Gameover)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Directions.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Directions.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Directions.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Directions.Down);
                    break;
                   /* case Key.Space :
                    gameState.TogglePause();
                    e.Handled = true;    
                    break;*/
                     
            }
        }

        private async Task GameLoop() 
        {
            while(!gameState.Gameover ) 
            {
               if (Keyboard.IsKeyDown(Key.Space)) 
                {
                    gameState.TogglePause(); 
                }

                if (!gameState.IsPaused) 
                { 
                    gameState.Move();
                    Draw();
                    Resume();
                }
                else 
                {
                Pause();
                }

                 await Task.Delay(100);
            }
        }            


        private Image[,] SetupGrid()    
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image()
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)

                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);

                }
            }

            return images;

        }
        
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            if (gameState.Score > highScore)
            {
                highScore = gameState.Score;
            }
            ScoreText.Text = $" Score: {gameState.Score}\nHigh Score: {highScore}";

        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRoation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(70);
                
            }

        }
        


        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for( int c = 0; c < cols;c++)
                {
                    GridValue gridval = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridval];
                    gridImages[r, c].RenderTransform = Transform.Identity;

                }
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async void Pause()
        {
            
                Overlay.Visibility = Visibility.Visible;
                Pause_Button.Content = "RESUME";
                OverlayText.Text = "PAUSED";
                await Task.Delay(600);


        }

        private async Task Resume() 
        {
           
            Overlay.Visibility = Visibility.Hidden;
            Pause_Button.Content = "PAUSE";
           

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!gameRunnig || gameState.Gameover)
           {
                return; 
            }

            gameState.TogglePause();
            if (!gameState.IsPaused)
            {
                Resume(); 
            }
            else
                Pause();


        }
        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "GAME OVER";
            await Task.Delay(1550);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press ANY Key TO START";
        }

        private async void ResetHighScore_Click(object sender, RoutedEventArgs e)
        {
            if (!gameRunnig || gameState.Gameover || gameState.IsPaused)
            {
                highScore = gameState.Score;

                Properties.Settings.Default.HighScore = highScore;
                Properties.Settings.Default.Save();



                await Task.Delay(200);
                ScoreText.Text = $" Score: {gameState.Score}\nHigh Score: {highScore}";
            }
        }

    }
}
