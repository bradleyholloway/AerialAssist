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
        private static Random r = new Random();

        private Texture2D robotImage;
        private Vector2 scale;
        private Vector2 location;
        private Vector2 velocity;
        private Color color;
        private PID drivePID;
        private PID turnPID;
        private float rotation;
        private float ballH;
        private Input driverInput;
        private int driveMode;
        private Ball activeBall;
        private bool CPU;

        private AIHandler aiHandler;
        private AICommand previous;
        private double cycles;
        private PID aiDrivePID;
        private PID aiTurnPID;

        public AerialRobot(Texture2D robotImage, Vector2 scale, Vector2 location, Color color, float rotation, float ballH, Input driverInput, int driveMode, bool CPU)
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
            this.CPU = CPU;
            this.drivePID = new PID(.1,.05,.1,3);

            if (CPU)
            {
                aiTurnPID = new PID(.05, .05, .05, .01);
                aiDrivePID = new PID(.05, .05, .05, .01);
                aiHandler = new AIHandler();
                
                aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2(100f * (r.Next(8)+1) , 100f * (r.Next(3)+1)), 300.0));
                aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2(100f * (r.Next(8)+1) , 100f * (r.Next(3)+1)), 300.0));

            }
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
            if (!CPU)
            {
                float turnAxis = (float)driverInput.getRightX();
                float powerAxis = (float)driverInput.getLeftY();
                float strafeAxis = (float)driverInput.getLeftX();

                drive(robots, balls, turnAxis, powerAxis, strafeAxis);

                if (driverInput.getRightActionButton())
                {
                    if (activeBall != null)
                    {
                        activeBall.launch(new Vector3(launchPower * (float)Math.Cos(rotation) + velocity.X, launchPower * (float)Math.Sin(rotation) + velocity.Y, 2f));
                        activeBall = null;
                    }
                }
            }
            else
            {
                aiDrive(robots, balls);
                aiLaunch();
            }

        }
        private void aiDrive(List<Robot> robots, List<Ball> balls)
        {
            float turnAxis, strafeAxis, powerAxis;
            turnAxis = strafeAxis = powerAxis = 0f;
            AICommand command = aiHandler.get();
            if (!command.Equals(previous))
            {
                cycles = 0;
            }
            cycles++;
            if (cycles > command.getTimeout())
            {
                aiHandler.move();
                command = aiHandler.get();
                cycles = 0;
            }

            if (command.getType() == AICommand.driveCommand)
            {
                Vector2 target = (Vector2)command.getValue();
                powerAxis = (float)aiDrivePID.calcPID(UTIL.distance(location, target));
                aiTurnPID.setDesiredValue(UTIL.getDirectionTward(location, target));
                turnAxis = (float)aiTurnPID.calcPID(rotation);
                if (UTIL.distance(target, location) < 35)
                {
                    aiHandler.move();
                }

            }
            else if (command.getType() == AICommand.fireCommand)
            {
                Boolean fire = (Boolean)command.getValue();
            }
            else if (command.getType() == AICommand.positionCommand)
            {
                Vector2 offSet = (Vector2)command.getValue();
                Vector2 ballCoordinate = Vector2.Zero;
                foreach (Ball b in balls)
                {
                    if (b.getColor().Equals(color))
                    {
                        ballCoordinate = b.getLocation();
                    }
                }
                Vector2 target = ballCoordinate + offSet;

                powerAxis = (float)aiDrivePID.calcPID(UTIL.distance(location, target));
                aiTurnPID.setDesiredValue(UTIL.getDirectionTward(location, target));
                turnAxis = (float)aiTurnPID.calcPID(rotation);

            }
            drive(robots, balls, turnAxis, powerAxis, strafeAxis);
            previous = command;
        }

        private void aiLaunch()
        {

        }

        private void drive(List<Robot> robots, List<Ball> balls, float turnAxis, float powerAxis, float strafeAxis)
        {
            powerAxis *= -1;
            Vector2 tempLocation = location;
            float tempRotation = rotation;
            if (driveMode == ArcadeDrive)
            {
                
                tempLocation = location + new Vector2(powerAxis * ArcadeDriveConstant * (float)Math.Cos(rotation), powerAxis * ArcadeDriveConstant * (float)Math.Sin(rotation));
                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == FieldCentric)
            {
                
                tempLocation = location + new Vector2(strafeAxis * McCannumDriveConstant, powerAxis * McCannumDriveConstant);

                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == McCannumDrive)
            {
                
                tempLocation = location + new Vector2(strafeAxis * (float)Math.Sin(rotation) * McCannumDriveConstant, -strafeAxis * (float)Math.Cos(rotation) * McCannumDriveConstant);
                tempLocation+= new Vector2(powerAxis * McCannumDriveConstant * (float)Math.Cos(rotation), powerAxis * McCannumDriveConstant * (float)Math.Sin(rotation));

                tempRotation = rotation + turnAxis * turnConst;
            }
            else if (driveMode == UnicornDrive)
            {
                
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
                            if (r.pushRobot(location, velocity, robots) == 1)
                            {
                                velocity /= 2f;
                            }
                            else if (r.pushRobot(location, velocity, robots) == 0)
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

        int Robot.pushRobot(Vector2 locationStart, Vector2 velocityOfImpact, List<Robot> robots)
        {
            Vector2 tempLocation = location + velocityOfImpact / 2;

            if (!(tempLocation.X > minXPosition * widthScale && tempLocation.X < maxXPosition * widthScale && tempLocation.Y > minYPosition * heightScale && tempLocation.Y < maxYPosition * heightScale))
            {
                
                //color = Color.Red;
                return 0;
            }
            if (Math.Abs(UTIL.getDirectionTward(locationStart, location) - UTIL.getDirectionTward(Vector2.Zero, velocityOfImpact)) > Math.PI * 2 / 3)
            {

                //color = Color.Aqua;
                return -1;
            }

            foreach (Robot r in robots)
            {
                if (!r.Equals(this) && !r.getLocation().Equals(locationStart))
                {
                    if (UTIL.distance(r.getLocation(), location) <= Math.Sqrt(robotImage.Width * robotImage.Width * scale.X * scale.X + robotImage.Height * robotImage.Height * scale.Y * scale.Y))
                    {
                        return 0;
                    }
                }
            }

            

            location += velocityOfImpact / 2;
            velocity += velocityOfImpact / 2;

            
            //color = Color.Green;
            return 1;
        }

        void Robot.linkBall(Ball b)
        {
            this.activeBall = b;
        }
    }
}
