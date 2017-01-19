﻿using BusinessLayer.FoodFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Engine
    {
        private const int snakeInitialLength = 4;
        private const int step = 1;
        private const int mazeRenderWidth = 70;
        private const int mazeRenderLength = 20;
        private const int AINumber = 7;
        private int mazeLength { get; set; }
        private int mazeWidth { get; set; }
        private int[,] mazeArray { get; set; }
        private GameSound gameSound;
        private Maze gameMaze;

        AI newAI = new AI();
       

       

        private GameSnake gameSnake1;
        // For future use, 2 player game mode
        //private GameSnake gameSnake2;
        private Food food;
        private FoodGenerator foodGenerator;
        private gameMode currentGameMode;

      

        gameMode currentMode = gameMode.basic;

        public Engine(gameMode mode, int length = mazeRenderLength, int width = mazeRenderWidth)
        {
            mazeLength = length;
            mazeWidth = width;
            currentGameMode = mode;
            Score.resetScore();
            foodGenerator = new FoodGenerator();
        }

        public Elements[,] initializeGame()
        {

            switch (currentMode)
            {
                case gameMode.basic:


                    // Create a New Maze and initialize it
                    gameMaze = new Maze(mazeWidth, mazeLength);
                    mazeArray = gameMaze.CreateMaze();
                    gameSound = new GameSound();
                    // Add the Snake
                    gameSnake1 = new GameSnake();
                    //List<Point> snakeBody = new List<Point>();
                    List<Point> snakeCurrentBody = gameSnake1.createFirstSnake(mazeLength, mazeWidth, snakeInitialLength);

                    // Make the whole snake as body first
                    foreach (Point value in snakeCurrentBody)
                    {
                        mazeArray[value.returnX(), value.returnY()] = (int)Elements.snakeBody;
                    }
                    // Identify snake head
                    Point head = snakeCurrentBody[0];
                    mazeArray[head.returnX(), head.returnY()] = (int)Elements.snakeHead;


                    bool isAIValid = true;
                    do
                    {
                        
                        newAI.SpawnAI(mazeWidth, mazeLength);
                        isAIValid = validateNewAILocation(newAI);
                    } while (!isAIValid);
                    mazeArray[newAI.XCoordinate, newAI.YCoordinate] = AINumber;
                    

                    // Add the Food
                    bool isValid = true;
                    do
                    {
                        food = foodGenerator.generateFood(mazeLength, mazeWidth);
                        isValid = validateNewFoodLocation(food);
                    } while (!isValid);
                    mazeArray[food.xLocation, food.yLocation] = Food;
                    break;

                default:
                    throw new System.Exception("Invalid Game Mode!");
            }

            return mazeArray;
        }

        public int[,] updateGame(Direction snakeDirection)
        {
            if (snakeDirection == Direction.Unchanged)
            {
                snakeDirection = gameSnake1.directionFacing;
            }

            Point newSnakeHead = getNewHead(snakeDirection);
            List<Point> snakesNewLocation;

            switch (mazeArray[newSnakeHead.returnX(), newSnakeHead.returnY()])
            {

                case (int)Elements.mazeBody:
                    gameSound.SnakeDiesSound();
                    if (Score.getScore() > Score.getHighScore())
                    {
                        gameSound.SnakeGetsHighScore();
                        Score.setHighScore(Score.getScore());
                    }
                    mazeArray[0, 0] = snakeHitsMaze;
                    return mazeArray;


                case Food:  // snake hits the food
                    snakesNewLocation = gameSnake1.snakeMove(snakeDirection, true);
                    
                    gameSound.SnakeEatsSound();
                    foreach (Point value in snakesNewLocation)
                    {
                        mazeArray[value.returnX(), value.returnY()] = snakeBody;
                    }
                    // Identify snake head
                    Point head = snakesNewLocation[0];
                    mazeArray[head.returnX(), head.returnY()] = snakeHead;

                    if ((newSnakeHead.returnX() == food.xLocation) && (newSnakeHead.returnY() == food.yLocation))
                    {
                        Score.incrementScore(food.pointsWorth);

                    }

                    food = null;
                    bool isValid = true;
                    do
                    {
                        food = foodGenerator.generateFood(mazeLength, mazeWidth);
                        isValid = validateNewFoodLocation(food);
                    } while (!isValid);

                    mazeArray[food.xLocation, food.yLocation] = Food;
                    break;

                default:   // snake moves
                    List<Point> SnakeCurrentPosition = gameSnake1.returnCurrentSnakePosition();
                    mazeArray[SnakeCurrentPosition.Last().returnX(), SnakeCurrentPosition.Last().returnY()] = (int)Elements.blank;
                    mazeArray[SnakeCurrentPosition.First().returnX(), SnakeCurrentPosition.First().returnY()] = (int)Elements.snakeBody;
                    snakesNewLocation = gameSnake1.snakeMove(snakeDirection, false);
                    mazeArray[snakesNewLocation.First().returnX(), snakesNewLocation.First().returnY()] = (int)Elements.snakeHead;

                    mazeArray[food.xLocation, food.yLocation] = Food;

                    mazeArray[newAI.XCoordinate, newAI.YCoordinate] = 0;

                    bool isAIValid = true;
                    int previousX = newAI.XCoordinate;
                    int previousY = newAI.YCoordinate;
                    do
                    {
                        newAI.XCoordinate = previousX;
                        newAI.YCoordinate = previousY;
                        newAI.MoveAI();
                        isAIValid = validateNewAILocation(newAI);
                    } while (!isAIValid);
                    mazeArray[newAI.XCoordinate, newAI.YCoordinate] = AINumber;
                    break;
            }
            return mazeArray;
        }

        private Point getNewHead(Direction snakeDirection)
        {
            List<Point> snakeBody = gameSnake1.returnCurrentSnakePosition();

            // Check current snake head location.
            Point snakeHead = snakeBody.First();

            // Cross check with new snake head location.
            Point newSnakeHead;
            int x = snakeHead.returnX();
            int y = snakeHead.returnY();
            switch (snakeDirection)
            {

                case Direction.Right:
                    newSnakeHead = new Point(x, y + step);
                    break;

                case Direction.Left:
                    newSnakeHead = new Point(x, y - step);
                    break;

                case Direction.Up:
                    newSnakeHead = new Point(x - step, y);
                    break;

                case Direction.Down:
                    newSnakeHead = new Point(x + step, y);
                    break;

                default:
                    throw new System.Exception("Invalid direction.");
            }
            return newSnakeHead;
        }
        public bool validateNewFoodLocation(Food newFood)
        {
            int x = newFood.xLocation;
            int y = newFood.yLocation;

            if ((x > mazeLength) || (y > mazeWidth))
            {
                return false;
            }
            if (mazeArray[x, y] == (int)Elements.mazeBody)
            {
                return false;
            }
            if (mazeArray[x, y] == (int)Elements.snakeBody)
            {
                return false;
            }
            if (mazeArray[x, y] == (int)Elements.snakeHead)
            {
                return false;
            }
            return true;
        }

        public bool validateNewAILocation(AI newAI)
        {
            int x = newAI.XCoordinate;
            int y = newAI.YCoordinate;

            if ((x > mazeLength) || (y > mazeWidth))
            {
                return false;
            }
            if ((x < 0) || (y < 0))
            {
                return false;
            }
            if (mazeArray[x, y] == (int)Elements.mazeBody)
            {
                return false;
            }
            return true;
        }
    }

}