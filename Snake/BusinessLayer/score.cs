﻿using DatabaseLayer;

namespace BusinessLayer
{
    public class Score
    {
        private int currentScore;
        Database db = new Database();

        public Score()
        {
            currentScore = 0;
        }
       
        public int getScore()
        {
            return currentScore;
        }

        public void incrementScore(int increment)
        {
            currentScore += increment;
        }

        public int getHighScore()
        {
            return db.getHighSCore();
        }

        public void setHighScore(int highScore)
        {
            if (db.setHighScore(highScore) == false)
            {
                throw new System.Exception("Database fail");
            }
        }
    }
}
