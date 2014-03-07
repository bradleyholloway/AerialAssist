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
        public static float launchPower = 1f;
        private static float widthScale, heightScale;

        private Texture2D robotImage;
        private Vector2 scale;
        private Vector2 location;
        private Vector2 velocity;
        private Color color;
        private float rotation;
        private float ballH;
        private Input driverInput;
        private int driveMode;
        private Ball activeBall;

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
            this.activeBall = null;
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
            AerialRobot.widthScale = widthScale;
            AerialRobot.heightScale = heightScale;
            drive(robots, balls);

            if (driverInput.getRightActionButton())
            {
                if (activeBall != null)
                {
                    activeBall.launch(new Vector3(launchPower * (float)Math.Cos(rotation) + velocity.X, launchPower * (float)Math.Sin(rotation) + velocity.Y,2f));
                    activeBall = null;
                }
            }

        }

        private void drive(List<Robot> robots, List<Ball> balls)
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

                tempLocation = location + new Vector2(strafeAxis * (float)Math.Sin(rotation) * McCannumDriveConstant, -strafeAxis * (float)Math.Cos(rotation) * McCannumDriveConstant);
                tempLocation+= new Vector2(powerAxis * McCannumDriveConstant * (float)Math.Cos(rotation), powerAxis * McCannumDriveConstant * (float)Math.Sin(rotation));

                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == UnicornDrive)
            {
                float turnAxis = (float)driverInput.getRightX();
                float powerAxis = (float)-driverInput.getLeftY();
                float strafeAxis = (float)driverInput.getLeftX();

                tempLocation = location + new Vector2(strafeAxis * (float)Math.Sin(rotation) * UnicornDriveConstant, -strafeAxis * (float)Math.Cos(rotation) * UnicornDriveConstant);
                tempLocation += new Vector2(powerAxis * UnicornDriveConstant * (float)Math.Cos(rotation), powerAxis * UnicornDriveConstant * (float)Math.Sin(rotation));

                tempRotation = rotation + turnAxis * turnConst;

                
            }
            if (tempLocation.X > minXPosition * widthScale && tempLocation.X < maxXPosition * widthScale && tempLocation.Y > minYPosition * heightScale && tempLocation.Y < maxYPosition * heightScale)
            {
                velocity = tempLocation - location;

                foreach (Robot r in robots)
                {
                    if (!r.Equals(this))
                    {
                        if (UTIL.distance(r.getLocation(), location) <= Math.Sqrt(robotImage.Width*robotImage.Width * scale.X*scale.X + robotImage.Height*robotImage.Height*scale.Y*scale.Y))
                        {
                            if (r.pushRobot(location, velocity) == 1)
                            {
                                velocity /= 2f;
                            }
                            else if (r.pushRobot(location, velocity) == 0)
                            {
                                velocity *= 0f;
                            }
                            
                        }
                    }
                }
                location += velocity;
            }
            rotation = tempRotation;

            foreach (Ball b in balls)
            {
                if(!b.Equals(activeBall) && UTIL.distance(location, b.getLocation()) < Ball.radius/2 && b.getHeight() < .1f && b.getIsFree()) 
                {
                    if (driverInput.getBottomActionButton() && Math.Abs(UTIL.normalizeDirection(rotation) - UTIL.normalizeDirection(UTIL.getDirectionTward(location, b.getLocation()))) < .35 && activeBall== null)
                    {
                        b.linkRobot(this);
                        activeBall = b;
                    }
                    else
                    {
                        b.pushBall(UTIL.magD(velocity.Length() * 1f + 1, UTIL.getDirectionTward(location, b.getLocation())));
                    }
                }
            }
        }

        int Robot.pushRobot(Vector2 locationStart, Vector2 velocityOfImpact)
        {
            Vector2 tempLocation = location + velocityOfImpact / 2;

            if (!(tempLocation.X > minXPosition * widthScale && tempLocation.X < maxXPosition * widthScale && tempLocation.Y > minYPosition * heightScale && tempLocation.Y < maxYPosition * heightScale))
            {
                return 0;
            }
            if (Math.Abs(UTIL.getDirectionTward(locationStart, location) - UTIL.getDirectionTward(Vector2.Zero, velocityOfImpact)) < Math.PI)
            {
                return -1;
            }

            location += velocityOfImpact / 2;

            return 1;
        }

        void Robot.linkBall(Ball b)
        {
            this.activeBall = b;
        }
    }
}
