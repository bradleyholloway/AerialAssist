using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AerialAssist
{
    class ScoreMatrix
    {
        public static float blueWhiteZone;
        public static float redWhiteZone;

        private bool[,] matrix;
        private List<Robot> robots;
        
        public ScoreMatrix(Robot r1, Robot r2, Robot r3)
        {
            matrix = new bool[3,3];
            robots = new List<Robot>();
            robots.Add(r1);
            robots.Add(r2);
            robots.Add(r3);
        }

        public void update(Robot r)
        {
            int zone = 2;
            if (r.getLocation().X < blueWhiteZone)
            {
                zone = 0;
            }
            else if (r.getLocation().X < redWhiteZone)
            {
                zone = 1;
            }
            int robot = robots.IndexOf(r);
            if (robot != -1 && !matrix[robot, zone])
            {
                matrix[robot, zone] = true;
            }
        }

        public int getAssistBonus()
        {
            int maxCount = 0;
            int levelOne = 0;
            int levelTwo = 0;
            int levelThree = 0;
            for (int i = 0; i < 3; i++)
            {
                levelOne = 0;
                levelOne += (matrix[0, i]) ? 1 : 0;
                for (int j = 0; j < 3; j++)
                {
                    levelTwo = 0;
                    levelTwo += (matrix[1, j]) ? 1 : 0;
                    if (j != i)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            if (k != i && k != j)
                            {
                                levelThree = 0;
                                levelThree += (matrix[2, k]) ? 1 : 0;
                                if (levelOne + levelTwo + levelThree > maxCount)
                                {
                                    maxCount = levelOne + levelTwo + levelThree;
                                }
                            }
                        }
                    }
                }

            }
            int assistScore = 0;
            switch (maxCount)
            {
                case 0: assistScore = 0; break;
                case 1: assistScore = 0; break;
                case 2: assistScore = 10; break;
                case 3: assistScore = 30; break;
            }
            return assistScore;
        }

        public string ToString()
        {
            String s = "";
            for (int i = 0; i < 3; i++)
            {
                for (int a = 0; a < 3; a++)
                {
                    s += (matrix[i, a]) ? "Y" : "N" ;
                }
            }
            return s;
        }
    }
}
