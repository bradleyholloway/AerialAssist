using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BradleyXboxUtils;
using Microsoft.Xna.Framework.Audio;

namespace AerialAssist
{
    class AerialRobot : Robot
    {
        public static SoundEffect driveSound;
        public static SoundEffect feed;
        public static SoundEffect launch;
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
        public static float zoneBleed = 30f;
        public static float widthScale, heightScale;
        public const int StandardAI = 0;
        public const int FollowAndShootAI = 1;
        public const int PassAI = 2;
        public const int RecieveAndShootAI = 3;
        public const int DefenseAI = 4;
        public const int RedPrimary = 0;
        public const int WhitePrimary = 1;
        public const int BluePrimary = 2;
        public static double redZone = 600;
        public static double blueZone = 300;

        private static Random r = new Random();

        private SoundEffectInstance driveSoundInstance;
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
        private int stuckCount;
        private double power;

        private AIHandler aiHandler;
        private AICommand previous;
        private double cycles;
        private PID aiDrivePID;
        private PID aiTurnPID;
        private float angularMomentum;
        private int primaryZone;
        private Vector2 previousPosition;

        public AerialRobot(Texture2D robotImage, Vector2 scale, Vector2 location, Color color, float rotation, float ballH, Input driverInput, int driveMode, bool CPU, int AImode, int primaryZone)
        {
            this.previousPosition = Vector2.Zero;
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
            this.drivePID = new PID(.05,.0005,.1, 1);
            this.turnPID = new PID(.09, .005, 0, .2);
            this.primaryZone = primaryZone;
            this.driveSoundInstance = driveSound.CreateInstance();
            driveSoundInstance.IsLooped = true;
            driveSoundInstance.Volume = .310f;
            if (CPU)
            {
                aiTurnPID = new PID(.05, .05, .05, .01);
                aiTurnPID.setMinDoneCycles(20);
                aiTurnPID.setDoneRange(.1);
                aiDrivePID = new PID(.01, 0, .05, .01);
                aiHandler = new AIHandler();
                bool high1;
                if (primaryZone == RedPrimary)
                {
                    high1 = r.Next(2) == 1;
                    Vector2 corner = new Vector2(500 * widthScale, ((high1) ? 1 : 0) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 10));
                    
                }
                else if (primaryZone == BluePrimary)
                {
                    high1 = r.Next(2) == 1;
                    Vector2 corner = new Vector2(100 * widthScale, ((high1) ? 1 : 0) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 10));
                    
                }
                else
                {
                    high1 = r.Next(2) == 1;
                    Vector2 corner = new Vector2((300 + r.Next(200) - 100) * widthScale, ((high1) ? 1 : 0) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 10));
                    
                }

                if (AImode == StandardAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((float)redZone + 5f * widthScale, heightScale * 200f), 300));
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((float)blueZone - 5f * widthScale, heightScale * 200f), 300));
                    
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                    }
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(200 * ((color.Equals(Color.Red)) ? 1 : -1), 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 100 * (r.Next(2) * 2 - 1)), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));

                    aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                        aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    }
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    bool high = r.Next(2) == 1;
                    Vector2 corner = new Vector2((300 + r.Next(200) - 100) * widthScale, ((high) ? 1:0) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 200 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                        aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    }
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    corner = new Vector2((300 + r.Next(200) - 100) * widthScale, (r.Next(2)) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 200 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300));
                        aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    }

                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    corner = new Vector2((300 + r.Next(200) - 100) * widthScale, (((!high) ? 1 : 0)) * 220 * heightScale + 100);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 200));
                    //aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2( r.Next(8)*50+50,r.Next(5) * 50 + 50), 200));
                }
                if (AImode == RecieveAndShootAI)
                {

                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(200 * widthScale * ((color.Equals(Color.Red)) ? 1 : -1), 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));


                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((color.Equals(Color.Red)) ? 700 * widthScale : 100 * widthScale, heightScale * 150), 30));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));


                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((color.Equals(Color.Red)) ? 700 * widthScale : 100 * widthScale, heightScale * 150), 30));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));


                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((color.Equals(Color.Red)) ? 700 * widthScale : 100 * widthScale, heightScale * 150), 30));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));


                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30));


                }
                if (AImode == FollowAndShootAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                }
                if (AImode == PassAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 30.0));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 60));
                }
                if (AImode == DefenseAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, Vector2.Zero, 100));
                }
                //aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30));
                if (primaryZone == RedPrimary)
                {
                    
                    Vector2 corner = new Vector2(500 * widthScale, (((!high1) ? 1 : 0)) * 220 * heightScale + 100);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 30));
                }
                else if (primaryZone == BluePrimary)
                {
                    
                    Vector2 corner = new Vector2(100 * widthScale, (((!high1) ? 1 : 0)) * 220 * heightScale + 100);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 30));
                }
                else
                {
                    
                    Vector2 corner = new Vector2((300 + r.Next(200) - 100) * widthScale, (((!high1) ? 1 : 0)) * 220 * heightScale + 100);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 30));
                }
                if (AImode == StandardAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((float)redZone + 5f * widthScale, heightScale * 200f), 300));
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((float)blueZone - 5f * widthScale, heightScale * 200f), 300));

                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                    }
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(200 * ((color.Equals(Color.Red)) ? 1 : -1), 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 100 * (r.Next(2) * 2 - 1)), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));

                    aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                        aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    }
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    bool high = r.Next(2) == 1;
                    Vector2 corner = new Vector2((300 + r.Next(200) - 100) * widthScale, ((high) ? 1 : 0) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 200 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300 + r.Next(100)));
                        aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    }
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    corner = new Vector2((300 + r.Next(200) - 100) * widthScale, (r.Next(2)) * 220 * heightScale + 50);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 200 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0 + r.Next(100)));
                    if (r.Next(2) == 0)
                    {

                        aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    }
                    else
                    {
                        aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300));
                        aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    }

                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300 + r.Next(100)));
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30 + r.Next(100)));
                    corner = new Vector2((300 + r.Next(200) - 100) * widthScale, (((!high) ? 1 : 0)) * 220 * heightScale + 100);
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, corner, 200));
                    //aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2( r.Next(8)*50+50,r.Next(5) * 50 + 50), 200));
                }
                if (AImode == RecieveAndShootAI)
                {

                    //aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(200 * ((color.Equals(Color.Red)) ? 1 : -1), 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));


                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.driveCommand, new Vector2((color.Equals(Color.Red)) ? 550 * widthScale : 100 * widthScale, heightScale * 150), 30));

                }
                if (AImode == FollowAndShootAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                }
                if (AImode == PassAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 300.0));
                    aiHandler.putCommand(new AICommand(AICommand.passCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, new Vector2(0, 0), 30.0));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                }
                if (AImode == DefenseAI)
                {
                    aiHandler.putCommand(new AICommand(AICommand.defenseCommand, null, 30));
                    aiHandler.putCommand(new AICommand(AICommand.fireCommand, null, 300));
                    aiHandler.putCommand(new AICommand(AICommand.positionCommand, Vector2.Zero, 100));
                }
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

                if (driverInput.getRightTrigger() > .5)
                {
                    if (activeBall != null)
                    {
                        launch.Play();
                        activeBall.launch(new Vector3(launchPower * (float)Math.Cos(rotation) + velocity.X, launchPower * (float)Math.Sin(rotation) + velocity.Y, 2f));
                        activeBall = null;
                    }
                }
            }
            else
            {
                aiDrive(robots, balls);
            }

        }
        void Robot.stopSound()
        {
            driveSoundInstance.Stop();
        }

        private void aiDrive(List<Robot> robots, List<Ball> balls)
        {
            if (this.previousPosition.Equals(location))
            {
                stuckCount++;
            }
            else
            {
                stuckCount = 0;
            }
            bool moved = false;
            rotation = (float)UTIL.normalizeDirection(rotation);

            float turnAxis, strafeAxis, powerAxis;
            bool fire = false;
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
                moved = true;
                command = aiHandler.get();
                cycles = 0;
            }
            Vector2 target = Vector2.Zero;
            if (command.getType() == AICommand.driveCommand)
            {
                target = (Vector2)command.getValue();
                powerAxis = (float)aiDrivePID.calcPID(UTIL.distance(location, target));
                double goal = UTIL.normalizeDirection(UTIL.getDirectionTward(location, target));
                if (Math.Abs(goal + 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal += 2 * Math.PI;
                }
                if (Math.Abs(goal - 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal -= 2 * Math.PI;
                }

                aiTurnPID.setDesiredValue(goal);
                turnAxis = (float)aiTurnPID.calcPID(rotation);
                if (UTIL.distance(target, location) < 35 && !moved)
                {
                    moved = true;
                    aiHandler.move();
                }

            }
            else if (command.getType() == AICommand.fireCommand)
            {
                double goal = 0.0;
                if (color.Equals(Color.Blue))
                {
                    goal = Math.PI;
                }
                if (Math.Abs(goal + 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal += 2 * Math.PI;
                }
                if (Math.Abs(goal - 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal -= 2 * Math.PI;
                }
                aiTurnPID.setDesiredValue(goal);
                
                turnAxis = (float)aiTurnPID.calcPID(rotation);
                powerAxis = (aiTurnPID.isDone()) ? -0.3f : 0.25f;
                fire = aiTurnPID.isDone();

                if (activeBall == null && !moved)
                {
                    aiHandler.move();
                    moved = true;
                }

            }
            else if (command.getType() == AICommand.passCommand)
            {
                double goal = 0.0;
                double minDistance = double.MaxValue;
                foreach (Robot r in robots)
                {
                    if (!r.Equals(this) && r.getColor().Equals(color))
                    {
                        if (UTIL.distance(location, r.getLocation()) < minDistance)
                        {
                            minDistance = UTIL.distance(location, r.getLocation());
                            goal = UTIL.getDirectionTward(location, r.getLocation());
                        }
                    }
                }
                
                
                if (Math.Abs(goal + 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal += 2 * Math.PI;
                }
                if (Math.Abs(goal - 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal -= 2 * Math.PI;
                }
                aiTurnPID.setDesiredValue(goal);

                turnAxis = (float)aiTurnPID.calcPID(rotation);
                fire = aiTurnPID.isDone();

                if (!moved && (activeBall == null || activeBall.getAssistBonus() >= 10))
                {
                    aiHandler.move();
                }

            }
            else if (command.getType() == AICommand.positionCommand)
            {
                Vector2 offSet = (Vector2)command.getValue();
                Vector2 ballCoordinate = Vector2.Zero;
                
                double minDistance = double.MaxValue;

                

                
                
                foreach (Ball b in balls)
                {
                    if (b.getColor().Equals(color))
                    {
                        if (UTIL.distance(location, b.getLocation()) < minDistance && ((offSet.Equals(Vector2.Zero)) ? b.getIsFree() : !b.getIsFree()))
                        {
                            target = b.getLocation() + offSet;
                        
                            if (primaryZone == RedPrimary)
                            {
                                if (target.X > redZone * widthScale - zoneBleed * widthScale)
                                {
                                    ballCoordinate = b.getLocation();
                                    minDistance = UTIL.distance(ballCoordinate, location);
                                }
                            }
                            else if (primaryZone == WhitePrimary)
                            {
                                if (target.X > blueZone * widthScale - zoneBleed * widthScale && target.X < redZone * widthScale + zoneBleed * widthScale)
                                {
                                    ballCoordinate = b.getLocation();
                                    minDistance = UTIL.distance(ballCoordinate, location);
                                }
                            }
                            else if (primaryZone == BluePrimary)
                            {
                                if (target.X < blueZone * widthScale + zoneBleed * widthScale)
                                {
                                    ballCoordinate = b.getLocation();
                                    minDistance = UTIL.distance(ballCoordinate, location);
                                }
                            }
                        

                        
                            
                        }
                    }
                }
               

                target = ballCoordinate + offSet;

                powerAxis = (float)aiDrivePID.calcPID(UTIL.distance(location, target));

                double goal = UTIL.getDirectionTward(location, target);
                if (Math.Abs(goal + 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal += 2 * Math.PI;
                }
                if (Math.Abs(goal - 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal -= 2 * Math.PI;
                }
                aiTurnPID.setDesiredValue(goal);
                
                turnAxis = (float)aiTurnPID.calcPID(rotation);

                

                if (!moved && activeBall != null || ballCoordinate.Equals(Vector2.Zero))
                {
                    moved = true;
                    aiHandler.move();
                }

            }
            else if (command.getType() == AICommand.defenseCommand)
            {
                Vector2 offSet = Vector2.Zero;
                Vector2 ballCoordinate = Vector2.Zero;
                double minDistance = double.MaxValue;
                foreach (Ball b in balls)
                {
                    if (!b.getColor().Equals(color))
                    {
                        if (UTIL.distance(location, b.getLocation()) < minDistance)
                        {
                            ballCoordinate = b.getLocation();
                            minDistance = UTIL.distance(ballCoordinate, location);
                        }
                    }
                }
                target = ballCoordinate + offSet;

                powerAxis = (float)aiDrivePID.calcPID(UTIL.distance(location, target));

                double goal = UTIL.getDirectionTward(location, target);
                if (Math.Abs(goal + 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal += 2 * Math.PI;
                }
                if (Math.Abs(goal - 2 * Math.PI - rotation) < Math.Abs(goal - rotation))
                {
                    goal -= 2 * Math.PI;
                }
                aiTurnPID.setDesiredValue(goal);
                
                turnAxis = (float)aiTurnPID.calcPID(rotation);

                if (activeBall != null && !moved)
                {
                    moved = true;
                    aiHandler.move();
                }

            }
            

            if (!target.Equals(Vector2.Zero) && !moved)
            {
                if (primaryZone == RedPrimary)
                {
                    if (target.X < redZone * widthScale - zoneBleed * widthScale)
                    {
                        aiHandler.move();
                        moved = true;
                    }
                }
                else if (primaryZone == WhitePrimary)
                {
                    if (target.X < blueZone * widthScale - zoneBleed * widthScale || target.X > redZone * widthScale + zoneBleed * widthScale)
                    {
                        aiHandler.move();
                        moved = true;
                    }
                }
                else if (primaryZone == BluePrimary)
                {
                    if (target.X > blueZone * widthScale + zoneBleed * widthScale)
                    {
                        aiHandler.move();
                        moved = true;
                    }
                }
            }
            if (!moved || fire)
            {
                drive(robots, balls, turnAxis, powerAxis, strafeAxis);
            }
            else
            {
                drive(robots, balls, 0, 0, 0);
            }
            if (stuckCount >= 200)
            {
                turnAxis = -0.5f;
                powerAxis = -1.0f;
                strafeAxis = 0.0f;
                drive(robots, balls, turnAxis, powerAxis, strafeAxis);
            }

            if (activeBall != null && fire)
            {
                activeBall.launch(new Vector3(launchPower * (float)Math.Cos(rotation) + velocity.X, launchPower * (float)Math.Sin(rotation) + velocity.Y, 2f));
                activeBall = null;
                launch.Play();
            }

            previous = command;
        }

        
        private void drive(List<Robot> robots, List<Ball> balls, float turnAxis, float powerAxis, float strafeAxis)
        {
            double tempH = Math.Sqrt(powerAxis * powerAxis + strafeAxis * strafeAxis);
            
            drivePID.setDesiredValue(tempH);
            power += drivePID.calcPID(power);
            if (tempH == 0)
            {
                double theta = UTIL.normalizeDirection(-rotation + Math.Atan2(velocity.Y, velocity.X));

                //powerAxis = (velocity.Y > 0 && UTIL.normalizeDirection(rotation) < Math.PI || velocity.Y < 0 && UTIL.normalizeDirection( rotation) > Math.PI) ? -1 : 1;
                powerAxis = (float)(-velocity.Length() * Math.Cos(theta));
                strafeAxis = (float)(velocity.Length() * Math.Sin(theta));
                //strafeAxis = (float)(velocity.X * Math.Sin(theta));
                tempH = velocity.Length();
            }
            powerAxis = (float)(power * powerAxis / tempH);
            strafeAxis = (float)(power * strafeAxis / tempH);

            this.previousPosition = location;
            powerAxis *= -1;
            Vector2 tempLocation = location;

            float tempRotation = rotation;
            turnPID.setDesiredValue(turnAxis);
            angularMomentum += (float)turnPID.calcPID(angularMomentum);
            tempRotation = rotation + angularMomentum * turnConst;// (float)turnPID.calcPID(rotation);
            

            if (driveMode == ArcadeDrive)
            {
                
                tempLocation = location + new Vector2(powerAxis * ArcadeDriveConstant * (float)Math.Cos(rotation), powerAxis * ArcadeDriveConstant * (float)Math.Sin(rotation));
                
            }
            else if (driveMode == FieldCentric)
            {
                
                tempLocation = location + new Vector2(strafeAxis * McCannumDriveConstant, -powerAxis * McCannumDriveConstant);

                
            }
            else if (driveMode == McCannumDrive)
            {
                
                tempLocation = location + new Vector2(-strafeAxis * (float)Math.Cos(rotation - Math.PI/2) * McCannumDriveConstant, -strafeAxis * (float)Math.Sin(rotation - Math.PI/2) * McCannumDriveConstant);
                tempLocation+= new Vector2(powerAxis * McCannumDriveConstant * (float)Math.Cos(rotation), powerAxis * McCannumDriveConstant * (float)Math.Sin(rotation));

                
            }
            else if (driveMode == UnicornDrive)
            {

                tempLocation = location + new Vector2(-strafeAxis * (float)Math.Cos(rotation - Math.PI / 2) * UnicornDriveConstant, -strafeAxis * (float)Math.Sin(rotation - Math.PI / 2) * UnicornDriveConstant);
                tempLocation += new Vector2(powerAxis * UnicornDriveConstant * (float)Math.Cos(rotation), powerAxis * UnicornDriveConstant * (float)Math.Sin(rotation));

                

                
            }
            if (tempLocation.X > minXPosition * widthScale && tempLocation.X < maxXPosition * widthScale && tempLocation.Y > minYPosition * heightScale && tempLocation.Y < maxYPosition * heightScale)
            {
                velocity = tempLocation - location;

                foreach (Robot r in robots)
                {
                    if (!r.Equals(this))
                    {
                        if (UTIL.distance(r.getLocation(), location) <= Math.Sqrt(robotImage.Width*robotImage.Width * scale.X*scale.X + robotImage.Height*robotImage.Height*scale.Y*scale.Y) * .8f) 
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
                if (velocity.Length() != 0)
                {
                    driveSoundInstance.Play();
                }
                else
                {
                    driveSoundInstance.Stop();
                }
                
                location += velocity;
            }
            rotation = tempRotation;

            foreach (Ball b in balls)
            {
                if(!b.Equals(activeBall) && UTIL.distance(location, b.getLocation()) < Ball.radius/2 && b.getHeight() < 1f && b.getIsFree()) 
                {
                    if ((driverInput.getLeftTrigger() > .5 || (CPU && aiHandler.get().getType() != AICommand.defenseCommand && aiHandler.get().getType() != AICommand.driveCommand)) && Math.Abs(UTIL.normalizeDirection(rotation) - UTIL.normalizeDirection(UTIL.getDirectionTward(location, b.getLocation()))) < .6 && activeBall== null)
                    {
                        feed.Play();
                        b.linkRobot(this);
                        activeBall = b;
                    }
                    else
                    {
                        b.pushBall(UTIL.magD(velocity.Length() * 2.5f + 1, UTIL.getDirectionTward(Vector2.Zero, velocity)));
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
