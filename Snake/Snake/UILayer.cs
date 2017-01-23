﻿using System;
using BusinessLayer;
using System.Collections.Generic;

namespace Snake
{
    class Program
    {
        private const int GameStepMilliseconds = 100;

        private static KeyListner keyListner = new KeyListner();
        private static bool ExitGame = false;
        private static gameMode currentGameMode;
        static bool gameSelected = false;

        static void Main(string[] args)
        {
            initialMenuLoad();

            currentGameMode = gameMode.basic;

            MazeLevel mazeMode = ChooseMazeMode();

            using (Engine gameEngine = new Engine(gameMode.basic, mazeMode))
            {
                Elements[,] Maze = gameEngine.initializeGame();

                Elements[,] updateMaze = Maze;

                Draw(Maze);

                do
                {
                    int score = gameEngine.getScore();
                    drawScore(score);

                    ConsoleKeyInfo keyInfo = keyListner.ReadKey(GameStepMilliseconds);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            updateMaze = gameEngine.updateGame(Direction.Up);
                            break;
                        case ConsoleKey.DownArrow:
                            updateMaze = gameEngine.updateGame(Direction.Down);
                            break;
                        case ConsoleKey.RightArrow:
                            updateMaze = gameEngine.updateGame(Direction.Right);
                            break;
                        case ConsoleKey.LeftArrow:
                            updateMaze = gameEngine.updateGame(Direction.Left);
                            break;
                        case ConsoleKey.Q:
                            ExitGame = true;
                            break;
                        default:
                            updateMaze = gameEngine.updateGame(Direction.Unchanged);
                            break;
                    }

                    if (updateMaze[0, 0] == Elements.snakeDeath)
                    {
                        ExitGame = true;
                    }
                    System.Console.Clear();
                    Draw(updateMaze);
                }
                while (ExitGame == false);

                endGame(gameEngine.getScore());
            }
        }

        public static void Draw(Elements[,] DynamicMaze)
        {
            int rowLength = DynamicMaze.GetLength(0);
            int colLength = DynamicMaze.GetLength(1);
            Style style = new Style();
            for (int i = 0; i < rowLength; i++)
            {
                string row = "";
                for (int j = 0; j < colLength; j++)
                {
                    row += style.StyleMazeElement(DynamicMaze[i, j]);

                }
                if (row.Contains(" "))
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.Write(row);
                Console.Write(Environment.NewLine);
            }
        }

        public static void drawScore(int score)
        {
            System.Console.WriteLine("Score: " + score);
        }

        static void endGame(int score) // and high score)
        {
            Console.WriteLine("Your final score is " + score);
            // Console.WriteLine("The high score is " + Score.getHighScore());
            throw new SystemException("Display high score unimplemented in UI");
            Console.WriteLine("Enter r to replay. q key to quit.");

            ConsoleKeyInfo keyInfo = keyListner.ReadKey(Int32.MaxValue);
            if (keyInfo.KeyChar.ToString().Equals("r", StringComparison.OrdinalIgnoreCase))
            {
                ExitGame = false;
                Console.Clear();
                Main(null);
            }
            else if (keyInfo.KeyChar.ToString().Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(0);
            }
     
            for (int i = 1; i < 3; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - i);
                ClearCurrentConsoleLine();
            }

            endGame(score);
        }

        static void initialMenuLoad()
        {
            do
            {

                Style.menuImage();
                try
                {
                    currentGameMode = (gameMode)Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine();
                }

                if (currentGameMode == gameMode.basic)
                {
                    gameSelected = true;
                }
            }
            while (gameSelected == false);
        }




        public static MazeLevel ChooseMazeMode()
        {
            MazeLevel mazeMode;

            Console.WriteLine("1. Beginner Mode : Simple Maze");
            Console.WriteLine("2. Easy Mode : Line Maze");
            Console.WriteLine("3. Medium Mode : Cross Maze");
            Console.WriteLine("4. Hard Mode : Grid Maze");

            do
            {
                Console.WriteLine();
                Console.Write("Please enter 1,2,3 or 4 : ");
                
            }
            while (!Enum.TryParse(keyListner.ReadKey(Int32.MaxValue).KeyChar.ToString(), out mazeMode) || !Enum.IsDefined(typeof(MazeLevel),mazeMode));


            Console.Clear();
            return mazeMode;

        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
