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
            if (!matrix[robot, zone])
            {
                matrix[robot, zone] = true;
            }
        }

        public int getAssistBonus()
        {
            return 0;
        }
    }
}
