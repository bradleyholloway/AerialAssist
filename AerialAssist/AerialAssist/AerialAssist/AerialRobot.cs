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
        public static int ArcadeDrive = 0;
        public static int McCannumDrive = 1;
        public static int FieldCentric = 2;
        public static int UnicornDrive = 3;
        public static float McCannumDriveConstant = 1f;
        public static float ArcadeDriveConstant = 1f;
        public static float UnicornDriveConstant = 1f;
        public static float turnConst = .1f;
        public static float minXPosition = 0f;
        public static float maxXPosition = 1f;
        public static float minYPosition = 0f;
        public static float maxYPosition = 1f;

        private Texture2D robotImage;
        private Vector2 scale;
        private Vector2 location;
        private Color color;
        private float rotation;
        private float ballH;
        private Input driverInput;
        private int driveMode;

        public AerialRobot(Texture2D robotImage, Vector2 scale, Vector2 location, Color color, float rotation, float ballH, Input driverInput, int driveMode)
        {
            this.robotImage = robotImage;
            this.scale = scale;
            this.location = location;
            this.color = color;
            this.rotation = rotation;
            this.ballH = ballH;
            this.driverInput = driverInput;
            this.driveMode = driveMode;
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

        Vector2 Robot.getOrigin()
        {
            return new Vector2(robotImage.Width / 2, robotImage.Height / 2);
        }

        void Robot.run(List<Robot> robots, List<Ball> balls, float widthScale, float heightScale)
        {
            drive(robots, balls, widthScale, heightScale);
            
        }

        private void drive(List<Robot> robots, List<Ball> balls, float widthScale, float heightScale)
        {
            Vector2 tempLocation = location;
            float tempRotation = rotation;
            if (driveMode == ArcadeDrive)
            {
                float turnAxis = (float)driverInput.getRightX();
                float powerAxis = (float)driverInput.getLeftY() * -ArcadeDriveConstant;

                tempLocation = location + new Vector2(powerAxis * (float)Math.Cos(rotation), powerAxis * (float)Math.Sin(rotation));
                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == FieldCentric)
            {
                float turnAxis = (float)driverInput.getRightX();
                float powerAxis = (float)driverInput.getLeftY();
                float strafeAxis = (float)driverInput.getLeftX();

                tempLocation = location + new Vector2(strafeAxis * McCannumDriveConstant, powerAxis * McCannumDriveConstant);

                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == McCannumDrive)
            {
                float turnAxis = (float)driverInput.getRightX();
                float powerAxis = (float)-driverInput.getLeftY();
                float strafeAxis = (float)driverInput.getLeftX();

                tempLocation = location + new Vector2(strafeAxis * McCannumDriveConstant * (float)Math.Sin(rotation) + powerAxis * McCannumDriveConstant * (float)Math.Cos(rotation), powerAxis * McCannumDriveConstant * (float)Math.Sin(rotation) + strafeAxis * McCannumDriveConstant * (float)Math.Cos(rotation));

                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == UnicornDrive)
            {
                float turnAxis = (float)driverInput.getRightX();
                float powerAxis = (float)-driverInput.getLeftY();
                float strafeAxis = (float)driverInput.getLeftX();

                tempLocation = location + new Vector2(strafeAxis * UnicornDriveConstant * (float)Math.Sin(rotation) + powerAxis * UnicornDriveConstant * (float)Math.Cos(rotation), powerAxis * UnicornDriveConstant * (float)Math.Sin(rotation) + strafeAxis * UnicornDriveConstant * (float)Math.Cos(rotation));

                tempRotation = rotation + turnAxis * turnConst;

                
            }
            if (tempLocation.X > minXPosition * widthScale && tempLocation.X < maxXPosition * widthScale && tempLocation.Y > minYPosition * heightScale && tempLocation.Y < maxYPosition * heightScale)
            {
                location = tempLocation;
            }
            rotation = tempRotation;
        }
    }
}
