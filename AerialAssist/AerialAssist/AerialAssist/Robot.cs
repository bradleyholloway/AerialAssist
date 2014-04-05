using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AerialAssist
{
    interface Robot
    {
        Texture2D getImage();
        float getRotation();
        Vector2 getLocation();
        Vector2 getScale();
        Color getColor();
        Vector2 getOrigin();
        float getBallHeight();
        void run(List<Robot> robots, List<Ball> balls, float widthScale, float heightScale, double time);
        void linkBall(Ball ball);
        void stopSound();
        int pushRobot(Vector2 location, Vector2 velocityOfImpact, List<Robot> robots);
    }
}
