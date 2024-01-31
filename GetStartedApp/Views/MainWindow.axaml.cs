using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace GetStartedApp.Views;

public partial class MainWindow : Window
{
    private readonly List<Rectangle> _itemsToRemove = new List<Rectangle>();
    // Bind the TextBox Score in the MainWindow.axaml file to this property
    private int _enemyLimit;
    private ConsoleColor _defaultConsoleColor = Console.ForegroundColor;
    public MainWindow()
    {
        InitializeComponent();
        
        _enemyLimit = 7;
        Score.Text = $"Score: {_enemyLimit}";
        // Start the game with the player in the middle of the canvas(horizontally) and at the bottom of the canvas(vertically)
        Player.Margin = new Thickness(295, 525, 0, 0);
        Player.Tag = "player";
        
        //Create a timer move the enemy from right to left every 100 milliseconds
        var timer = new DispatcherTimer();
        timer.Tick += GameLoop;
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Start();

        // Focus the canvas so that the key events are captured
        Screen.Focus();
        
        CreateEnemy();
    }
    
    private void CreateEnemy()
    {
        var myEnemy = new Rectangle
        {
            // Set the tag of the rectangle to enemy, this is used to differentiate the enemy from the player
            Tag = "enemy",
            Width = 50,
            Height = 50,
            Fill = Brushes.Red,
            // Start the enemy at the right of the canvas and at the bottom of the canvas
            // 700 is the size of the canvas, so the enemy starts at the right of the canvas
            Margin = new Thickness(700, 500, 0, 0)
        };
       
        Screen.Children.Add(myEnemy);
    }

    private void GameLoop(object? sender, EventArgs args)
    {
        // If there is no enemy on the canvas, create an enemy
        if (Screen.Children.OfType<Rectangle>().FirstOrDefault(x => x.Tag == "enemy") is null)
        {
            CreateEnemy();
        }
        
        // Loop through all the rectangles on the canvas (player and enemy are rectangles)
        foreach (var x in  Screen.Children.OfType<Rectangle>())
        {
            // If the rectangle is the player
            if (x is Rectangle && (string)x.Tag == "player")
            {
                // Create a rect that represents the player, the rect contains the position and size of the player
                Rect playerHitBox = new Rect(x.Margin.Left, 0, x.Width, x.Height);

                // Loop through all the rectangles on the canvas
                foreach (var y in Screen.Children.OfType<Rectangle>())
                {
                    // If the rectangle is an enemy
                    if (y is Rectangle && (string)y.Tag == "enemy")
                    {
                        // Move the enemy to the left
                        y.Margin = new Thickness(y.Margin.Left - 15, y.Margin.Top, 0, 0);
                        
                        // If the enemy is out of the canvas, add it to the list of items to remove
                        if (y.Margin.Left < -50)
                        {
                            _itemsToRemove.Add(y);
                        }
                        
                        // Create a rect that represents the enemy, the rectangle is used to detect if the enemy is hit by the player
                        Rect enemyHitBox = new Rect(y.Margin.Left, 0, y.Width, y.Height);
                
                        // Print the position of the player and the enemy to the console
                        Console.WriteLine($"Player: {playerHitBox.Left}, Enemy: {enemyHitBox.Left}");
                        
                        //If the player is hit by the enemy
                        //If the player left position in canvas is equal to the enemy left position, the player is hit by the enemy
                        if (Math.Abs(playerHitBox.Left - enemyHitBox.Left) < 5)
                        {
                            // Decrease the enemy limit by 1
                            _enemyLimit--;
                            Score.Text = $"Score: {_enemyLimit}";
                            
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Player hit by enemy");
                            Console.ForegroundColor = _defaultConsoleColor;
                           
                            if (_enemyLimit <= 0)
                            {
                                // Stop the game loop and stop moving the enemy
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Game Over");
                                Console.ForegroundColor = _defaultConsoleColor;
                                (sender as DispatcherTimer)?.Stop();
                            }
                        }
                        
                    }
                }
            }
        }

        // Remove all the items in the itemsToRemove list from the canvas (removes the enemy from the canvas)
        foreach (var item in _itemsToRemove)
        {
            Screen.Children.Remove(item);
        }
    }

    private void Screen_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left)
        {
            // Move rectangle (player) to the left
            
            /* Player.Margin.Left - 25 is the new position of the player, the greater the number, the more the player moves to the left
                ** Player.Margin.Top is the position of the player from the top
                ** 0, 0 is the position of the player from the right and bottom
                ** The Thickness class is used to set the margin of the rectangle, it stretches the rectangle to the left and right, top and bottom
                ** In this case, we are only interested in the left margin, so we only change the left margin
                ** That way, the player moves to the left ( the thickness of the left margin increases, the player moves to the left)
            */
            Player.Margin = new Thickness(Player.Margin.Left - 25, Player.Margin.Top, 0, 0);
            
        }
        else if (e.Key == Key.Right)
        {
            // Move rectangle (player) to the right
            Player.Margin = new Thickness(Player.Margin.Left + 25, Player.Margin.Top, 0, 0);
        }
        if (e.Key == Key.Space)
        {
            /* Jump
                ** Move the player up, the greater the number, the higher the player goes
                ** Subtracting from the top margin moves the player up
            */
            Player.Margin = new Thickness(Player.Margin.Left, Player.Margin.Top - 40, 0, 0);
        }
    }

    private void Screen_OnKeyUp(object? sender, KeyEventArgs e)
    {
        // When the space key is released, move the player down
        if (e.Key == Key.Space)
        {
            /* Move the player down
                ** Moving the player down proportional to the number that was subtracted from the top margin to move the player up
                ** That makes the player go back to the original position
            */
            Player.Margin = new Thickness(Player.Margin.Left, Player.Margin.Top + 40, 0, 0);
        }
    }
}