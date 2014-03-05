using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BradleyXboxUtils;

namespace AerialAssist
{
    class AerialRobot : Robot
    {
        private Texture2D robotImage;
        private Vector2 scale;
        private Vector2 location;
        private Color color;
        private float rotation;
        private float ballH;
        private Input driverInput;

        public AerialRobot(Texture2D robotImage, Vector2 scale, Vector2 location, Color color, float rotation, float ballH, Input driverInput)
        {
            this.robotImage = robotImage;
            this.scale = scale;
            this.location = location;
            this.color = color;
            this.rotation = rotation;
            this.ballH = ballH;
            this.driverInput = driverInput;
        }

        Vector2 Robot.getLocation()
        {
            return location;
        }

        Vector2 Robot.getScale()
        {
            return scale;
        }

        float Robot.getRotation()
        {
            return rotation;
        }

        float Robot.getBallHeight()
        {
            return ballH;
        }

        Color Robot.getColor()
        {
            return color;
        }

        Texture2D Robot.getImage()
        {
            return robotImage;
        }

        void Robot.run(List<Robot> robots, List<Ball> balls)
        {
            float turnAxis = (float)driverInput.getRightX();
            float powerAxis = (float)driverInput.getLeftY();

            Vector2 tempLocation = location + new Vector2(powerAxis * (float)Math.Cos(rotation), powerAxis * (float)Math.Sin(rotation));
            float tempRotation = rotation + turnAxis * .01f;

            location = tempLocation;
            rotation = tempRotation;
        }
    }
}
