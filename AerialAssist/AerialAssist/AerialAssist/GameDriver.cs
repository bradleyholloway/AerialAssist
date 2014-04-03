using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BradleyXboxUtils;

namespace AerialAssist
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameDriver : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont timesNewRoman;
        double startTime, remainingTime;
        bool keyboard1, keyboard2;
        MenuInput menuInput;
        List<MenuItem> menuItems;
        List<MenuItem> strategyMenu;
        List<MenuItem> zoneMenu;
        List<MenuItem> driveMenu;
        int menuIndex, bmenuIndex, rmenuIndex;
        bool firstCycle, pressed;

        int gameState = 0;
        Texture2D field;
        Texture2D truss;
        float widthScale, heightScale;
        float trussHeight;
        int redScore, blueScore;
        int redPenalty, bluePenalty;
        int redTruss, blueTruss;
        int redCatch, blueCatch;
        int red1Strat = -1, red2Strat = -1, red3Strat = -1;
        int blue1Strat = -1, blue2Strat = -1, blue3Strat = -1;
        int red1Zone = -1, red2Zone = -1, red3Zone = -1;
        int blue1Zone = -1, blue2Zone = -1, blue3Zone = -1;
        int red1Drive, red2Drive, red3Drive;
        int blue1Drive, blue2Drive, blue3Drive;
        MenuInput redTeamInput = null;
        MenuInput blueTeamInput = null;

        List<Robot> robots;
        List<Ball> balls;

        
        //SoundEffect goal;
        
        SoundEffect endGame;
        SoundEffect score;
        
        SoundEffect buzzer;
        SoundEffect teleOp;
        SoundEffect start;
        SoundEffect titleScreen;
        SoundEffect gamePlay;
        SoundEffectInstance gamePlayInstance;
        SoundEffectInstance endGameInstance;
        SoundEffectInstance titleScreenInstance;

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            
            graphics.IsFullScreen = true;
            keyboard1 = true;
            keyboard2 = false;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem("No Keyboard", new Vector2(100f, 200f), Color.White, 4));
            menuItems.Add(new MenuItem("1  Keyboard", new Vector2(100f, 250f), Color.Red, 2));
            menuItems.Add(new MenuItem("2  Keyboard", new Vector2(100f, 300f), Color.Red, 3));
            menuItems.Add(new MenuItem("Quit", new Vector2(100f, 350f), Color.Black, 99));

            driveMenu = new List<MenuItem>();
            driveMenu.Add(new MenuItem("Arcade Drive", new Vector2(0, 0), Color.White, AerialRobot.ArcadeDrive));
            driveMenu.Add(new MenuItem("McCannum Drive", new Vector2(0,100), Color.White, AerialRobot.McCannumDrive));
            driveMenu.Add(new MenuItem("Field Centric Drive", new Vector2(0, 200), Color.White, AerialRobot.FieldCentric));
            driveMenu.Add(new MenuItem("Unicorn Drive", new Vector2(0, 300), Color.White, AerialRobot.McCannumDrive));
            
            strategyMenu = new List<MenuItem>();
            strategyMenu.Add(new MenuItem("Standard", new Vector2(0,0), Color.White, AerialRobot.StandardAI));
            strategyMenu.Add(new MenuItem("Recieve and Shoot" , new Vector2(0,100), Color.White, AerialRobot.RecieveAndShootAI));
            strategyMenu.Add(new MenuItem("Follow and Shoot" , new Vector2(0, 200), Color.White, AerialRobot.FollowAndShootAI));

            zoneMenu = new List<MenuItem>();
            zoneMenu.Add(new MenuItem("Red Zone", new Vector2(0, 0), Color.Red, AerialRobot.RedPrimary));
            zoneMenu.Add(new MenuItem("White Zone", new Vector2(0, 100), Color.White, AerialRobot.WhitePrimary));
            zoneMenu.Add(new MenuItem("Blue Zone", new Vector2(0, 200), Color.Blue, AerialRobot.BluePrimary));

            AerialRobot.ArcadeDriveConstant = 4f;
            AerialRobot.McCannumDriveConstant = 3f;
            AerialRobot.UnicornDriveConstant = 4f;
            AerialRobot.turnConst = .1f;

            AerialRobot.minXPosition = 40f;
            AerialRobot.maxXPosition = 550f;
            AerialRobot.minYPosition = 36f;
            AerialRobot.maxYPosition = 260f;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            timesNewRoman = Content.Load<SpriteFont>("TimesNewRoman");
            field = Content.Load<Texture2D>("Field");
            truss = Content.Load<Texture2D>("Truss");
            redScore = blueScore = 0;
            widthScale = (float)graphics.GraphicsDevice.Viewport.Width / field.Width;
            heightScale = (float)graphics.GraphicsDevice.Viewport.Height / field.Height;

            if(titleScreenInstance == null)
            {
                AerialRobot.launch = Content.Load<SoundEffect>("Piston");
                score = Content.Load<SoundEffect>("Score");
                AerialRobot.feed = Content.Load<SoundEffect>("Feed");
                endGame = Content.Load<SoundEffect>("battlescars");
                AerialRobot.driveSound = Content.Load<SoundEffect>("Driving");
                buzzer = Content.Load<SoundEffect>("Buzzer");
                teleOp = Content.Load<SoundEffect>("TeleOp");
                start = Content.Load<SoundEffect>("TrumpetStart");
                titleScreen = endGame;//Content.Load<SoundEffect>("CantHoldUs");
                gamePlay = Content.Load<SoundEffect>("pompeii");
                gamePlayInstance = gamePlay.CreateInstance();
                titleScreenInstance = titleScreen.CreateInstance();
                endGameInstance = endGame.CreateInstance();
                endGameInstance.IsLooped = false;
                titleScreenInstance.IsLooped = true;
            }


            Ball.trussHeight = trussHeight = 12;
            Ball.image = Content.Load<Texture2D>("sphere");
            Ball.radius = widthScale * 47f;
            Ball.growthConstant = .02f;
            Ball.decayConstant = .012f;
            Ball.ballAcceleration = 0.08f;
            Ball.bounceDecay = .58f;
            Ball.onePointTop = 60 * heightScale;
            Ball.onePointBottom = GraphicsDevice.Viewport.Height - Ball.onePointTop;
            AerialRobot.launchPower = 1.5f * (float)Math.Sqrt(widthScale * widthScale * heightScale * heightScale);
            AerialRobot.widthScale = widthScale;
            AerialRobot.heightScale = heightScale;
            ScoreMatrix.blueWhiteZone = .35f * widthScale * field.Width;
            ScoreMatrix.redWhiteZone = .67f * widthScale * field.Width;
            AerialRobot.redZone = .67 * field.Width;
            AerialRobot.blueZone = .35f * field.Width;
            AerialRobot.zoneBleed = 32f;

            Texture2D sonic = Content.Load<Texture2D>("robot2");
            float sonicScale = .12f;
            Texture2D diamondBack = Content.Load<Texture2D>("top_view");
            float dBScale = .045f;

            robots = new List<Robot>();
            robots.Add(new AerialRobot(diamondBack, new Vector2(dBScale * widthScale, dBScale * heightScale), new Vector2(widthScale * 320f, heightScale * 100f), Color.Red, .00001f, 0f, new KeyboardInput(PlayerIndex.One), red1Drive, !keyboard1, red1Strat, red1Zone));
            robots.Add(new AerialRobot(sonic, new Vector2(sonicScale * widthScale, sonicScale * heightScale), new Vector2(widthScale * 260f, heightScale * 100f), Color.Blue, (float)Math.PI+.00001f, 0f, new KeyboardInput(PlayerIndex.Two), blue1Drive, !keyboard2, blue1Strat, blue1Zone));
            robots.Add(new AerialRobot(diamondBack, new Vector2(dBScale * widthScale, dBScale * heightScale), new Vector2(widthScale * 320f, heightScale * 150f), Color.Red, .00001f, 0f, new ControllerInput(PlayerIndex.Two), red2Drive, !GamePad.GetState(PlayerIndex.Two).IsConnected, red2Strat, red2Zone));
            robots.Add(new AerialRobot(sonic, new Vector2(sonicScale * widthScale, sonicScale * heightScale), new Vector2(widthScale * 260f, heightScale * 150f), Color.Blue, (float)Math.PI + .00001f, 0f, new ControllerInput(PlayerIndex.One), blue2Drive, !GamePad.GetState(PlayerIndex.One).IsConnected, blue2Strat, blue2Zone));
            robots.Add(new AerialRobot(diamondBack, new Vector2(dBScale * widthScale, dBScale * heightScale), new Vector2(widthScale * 320f, heightScale * 200f), Color.Red, .00001f, 0f, new ControllerInput(PlayerIndex.Four),red3Drive, !GamePad.GetState(PlayerIndex.Four).IsConnected, red3Strat, red3Zone));
            robots.Add(new AerialRobot(sonic, new Vector2(sonicScale * widthScale, sonicScale * heightScale), new Vector2(widthScale * 260f, heightScale * 200f), Color.Blue, (float)Math.PI + .00001f, 0f, new ControllerInput(PlayerIndex.Three), blue3Drive, !GamePad.GetState(PlayerIndex.Three).IsConnected, blue3Strat, blue3Zone));

            balls = new List<Ball>();
            balls.Add(new Ball(robots.ElementAt<Robot>(0), Color.Red, robots.ElementAt<Robot>(2), robots.ElementAt<Robot>(4)));
            balls.Add(new Ball(robots.ElementAt<Robot>(1), Color.Blue, robots.ElementAt<Robot>(3), robots.ElementAt<Robot>(5)));
            balls.Add(new Ball(robots.ElementAt<Robot>(2), Color.Red, robots.ElementAt<Robot>(0), robots.ElementAt<Robot>(4)));
            balls.Add(new Ball(robots.ElementAt<Robot>(3), Color.Blue, robots.ElementAt<Robot>(1), robots.ElementAt<Robot>(5)));
            balls.Add(new Ball(robots.ElementAt<Robot>(4), Color.Red, robots.ElementAt<Robot>(0), robots.ElementAt<Robot>(2)));
            balls.Add(new Ball(robots.ElementAt<Robot>(5), Color.Blue, robots.ElementAt<Robot>(1), robots.ElementAt<Robot>(3)));

            menuInput = new MenuInput(((GamePad.GetState(PlayerIndex.One).IsConnected) ? (Input)new ControllerInput(PlayerIndex.One) : (Input)new KeyboardInput()));
            menuIndex = 0;
            firstCycle = true;
            redScore = blueScore = redPenalty = bluePenalty = redCatch = blueCatch = redTruss = blueTruss = 0;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (gameState == 0 && !pressed)
                {
                    this.Exit();
                }
                else
                {
                    gameState = 0;
                    pressed = true;
                }
            }
            else
            {
                pressed = false;
            }
                
            if (gameState == 0)
            {
                titleScreenInstance.Play();
                endGameInstance.Stop();
                gamePlayInstance.Stop();
                foreach (Robot r in robots)
                {
                    r.stopSound();
                }
                menuInput.run();
                int act = menuInput.getAction();
                if (act == 1)
                {
                    menuIndex--;
                    if (menuIndex == -1)
                    {
                        menuIndex = menuItems.Count - 1;
                    }
                }
                else if (act == -1)
                {
                    menuIndex++;
                    if (menuIndex == menuItems.Count)
                    {
                        menuIndex = 0;
                    }
                }
                else if (act == 3)
                {
                    gameState = menuItems.ElementAt<MenuItem>(menuIndex).point();
                    LoadContent();
                }
            }
            else if (gameState == 1)
            {
                if (firstCycle)
                {
                    startTime = gameTime.TotalGameTime.TotalSeconds;
                    start.Play();
                    gamePlayInstance.Play();
                    titleScreenInstance.Stop();
                    
                }
                firstCycle = false;
                remainingTime =  Math.Round((120 - (gameTime.TotalGameTime.TotalSeconds - startTime)), 0);

                if (remainingTime == 0)
                {
                    gameState = 5;
                    buzzer.Play();
                    endGameInstance.Play();
                    gamePlayInstance.Stop();
                    foreach (Robot r in robots)
                    {
                        r.stopSound();
                    }

                }

                foreach (Robot r in robots)
                {
                    r.run(robots, balls, widthScale, heightScale);
                }
                for (int ball = 0; ball < balls.Count; ball++)
                {
                    Ball b = balls.ElementAt<Ball>(ball);
                    int result = b.run(balls, widthScale, heightScale);
                    if (result > 0)
                    {
                        score.Play();
                    }
                    int penalty = b.getPenaltyPoints();
                    int catchPoints = b.getCatchPoints();
                    int trussPoints = b.getTrussPoints();
                    if (b.getColor().Equals(Color.Red))
                    {
                        redScore += catchPoints + trussPoints;
                        blueScore -= penalty;
                        if (catchPoints != 0)
                        {

                            redCatch += catchPoints;
                        }
                        
                        if (penalty != 0)
                        {
                            bluePenalty -= penalty;
                        }
                        if (trussPoints != 0)
                        {
                            redTruss += trussPoints;
                        }
                    }
                    else
                    {
                        blueScore += catchPoints + trussPoints;
                        redScore -= penalty;
                        if (catchPoints != 0)
                        {
                            blueCatch += catchPoints;
                        }
                        if (penalty != 0)
                        {
                            redPenalty -= penalty;
                        }
                        if (trussPoints != 0)
                        {
                            blueTruss += trussPoints;
                        }
                    }
                    
                    if (result != -1)
                    {
                        balls.Remove(b);
                        if (b.getColor().Equals(Color.Red))
                        {
                            redScore += result;
                            bool redBall = false;
                            foreach (Ball bb in balls)
                            {
                                if (bb.getColor().Equals(Color.Red))
                                {
                                    redBall = true;
                                }
                            }
                            if (!redBall)
                            {
                                balls.Add(new Ball(new Vector2(120 * widthScale, 140 * heightScale), Color.Red, robots.ElementAt<Robot>(0), robots.ElementAt<Robot>(2), robots.ElementAt<Robot>(4)));
                            }
                        }
                        else
                        {
                            blueScore += result;
                            bool blueBall = false;
                            foreach (Ball bb in balls)
                            {
                                if (bb.getColor().Equals(Color.Blue))
                                {
                                    blueBall = true;
                                }
                            }
                            if (!blueBall)
                            {
                                balls.Add(new Ball(new Vector2(480 * widthScale, 140 * heightScale), Color.Blue, robots.ElementAt<Robot>(1), robots.ElementAt<Robot>(3), robots.ElementAt<Robot>(5)));
                            }
                        }
                        ball--;
                    }
                }
            }
            else if (gameState == 2)
            {
                keyboard1 = true;
                keyboard2 = false;
                gameState = 7;
                LoadContent();
            }
            else if (gameState == 3)
            {
                keyboard1 = true;
                keyboard2 = true;
                gameState = 7;
                LoadContent();
            }
            else if (gameState == 4)
            {
                keyboard1 = false;
                keyboard2 = false;
                gameState = 7;
                LoadContent();
            }
            else if (gameState == 5)
            {
                menuInput.run();
                if (menuInput.getAction() != 0)
                {
                    endGameInstance.Stop();
                    gameState = 0;
                }
                for (int ball = 0; ball < balls.Count; ball++)
                {
                    Ball b = balls.ElementAt<Ball>(ball);
                    int result = b.run(balls, widthScale, heightScale);
                    if (result > 0)
                    {
                        score.Play();
                    }
                    int penalty = b.getPenaltyPoints();
                    int catchPoints = b.getCatchPoints();
                    int trussPoints = b.getTrussPoints();
                    if (b.getColor().Equals(Color.Red))
                    {
                        redScore += penalty + catchPoints + trussPoints;
                        redCatch += catchPoints;
                        if (penalty != 0)
                        {

                            bluePenalty -= penalty;
                        }
                        redTruss += trussPoints;
                    }
                    else
                    {
                        blueScore += penalty + catchPoints + trussPoints;
                        blueCatch += catchPoints;
                        if (penalty != 0)
                        {
                            redPenalty -= penalty;
                        }
                        blueTruss += trussPoints;
                    }

                    if (result != -1)
                    {
                        if (result != 0)
                        {
                            score.Play();
                        }

                        balls.Remove(b);
                        if (b.getColor().Equals(Color.Red))
                        {
                            redScore += result;
                            bool redBall = false;
                            foreach (Ball bb in balls)
                            {
                                if (bb.getColor().Equals(Color.Red))
                                {
                                    redBall = true;
                                }
                            }
                            if (!redBall)
                            {
                                balls.Add(new Ball(new Vector2(120 * widthScale, 140 * heightScale), Color.Red, robots.ElementAt<Robot>(0), robots.ElementAt<Robot>(2), robots.ElementAt<Robot>(4)));
                            }
                        }
                        else
                        {
                            blueScore += result;
                            bool blueBall = false;
                            foreach (Ball bb in balls)
                            {
                                if (bb.getColor().Equals(Color.Blue))
                                {
                                    blueBall = true;
                                }
                            }
                            if (!blueBall)
                            {
                                balls.Add(new Ball(new Vector2(480 * widthScale, 140 * heightScale), Color.Blue, robots.ElementAt<Robot>(1), robots.ElementAt<Robot>(3), robots.ElementAt<Robot>(5)));
                            }
                        }
                        ball--;
                    }
                }
                
            }
            else if (gameState == 7) //Select Roles
            {
                red1Strat = -1;// AerialRobot.FollowAndShootAI;
                red1Zone = -1;//AerialRobot.BluePrimary;
                red1Drive = 0;

                red2Strat = -1;//AerialRobot.RecieveAndShootAI;
                red2Zone = -1;//AerialRobot.WhitePrimary;
                red2Drive = 0;

                red3Strat = -1;//AerialRobot.RecieveAndShootAI;
                red3Zone = -1;//AerialRobot.RedPrimary;
                red3Drive = 0;

                blue1Strat = -1;//AerialRobot.FollowAndShootAI;
                blue1Zone = -1;//AerialRobot.RedPrimary;
                blue1Drive = 0;

                blue2Strat = -1;//AerialRobot.RecieveAndShootAI;
                blue2Zone = -1;//AerialRobot.WhitePrimary;
                blue2Drive = 0;

                blue3Strat = -1;//AerialRobot.RecieveAndShootAI;
                blue3Zone = -1;//AerialRobot.BluePrimary;
                blue3Drive = 0;

                Input blueTeamI = null;
                Input redTeamI = null;

                if (keyboard2)
                {
                    blueTeamI = new KeyboardInput(PlayerIndex.Two);
                    blue1Strat = 0;
                    blue1Zone = 0;
                    blue1Drive = -1;
                }

                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    blueTeamI = new ControllerInput(PlayerIndex.One);
                    blue2Strat = 0;
                    blue2Zone = 0;
                    blue2Drive = -1;
                }

                if (GamePad.GetState(PlayerIndex.Three).IsConnected)
                {
                    blue3Strat = 0;
                    blue3Zone = 0;
                    blue3Drive = -1;
                }


                if (keyboard1)
                {
                    redTeamI = new KeyboardInput(PlayerIndex.One);
                    red1Strat = 0;
                    red1Zone = 0;
                    red1Drive = -1;
                }

                if (GamePad.GetState(PlayerIndex.Two).IsConnected)
                {
                    redTeamI = new ControllerInput(PlayerIndex.Two);
                    red2Zone = 0;
                    red2Zone = 0;
                    red2Drive = -1;
                }

                if (GamePad.GetState(PlayerIndex.Four).IsConnected)
                {
                    red3Zone = 0;
                    red3Strat = 0;
                    red3Drive = -1;
                }
                if (blueTeamI != null)
                {
                    blueTeamInput = new MenuInput(blueTeamI);
                }
                else
                {
                    blueTeamInput = null;
                }
                if (redTeamI != null)
                {
                    redTeamInput = new MenuInput(redTeamI);
                }
                else
                {
                    redTeamInput = null;
                }
                
                gameState = 8;
                
            }
            else if (gameState == 8)
            {
                titleScreenInstance.Play();

                if (redTeamInput != null)
                {

                    redTeamInput.run();
                    int act = redTeamInput.getAction();
                    if (red1Drive == -1 || red2Drive == -1 || red3Drive == -1)
                    {
                        if (act == 1)
                        {
                            rmenuIndex--;
                            if (rmenuIndex <= -1)
                            {
                                rmenuIndex = driveMenu.Count - 1;
                            }
                        }
                        else if (act == -1)
                        {
                            rmenuIndex++;
                            if (rmenuIndex >= driveMenu.Count)
                            {
                                rmenuIndex = 0;
                            }
                        }
                        else if (act == 3)
                        {
                            int temp = driveMenu.ElementAt<MenuItem>(rmenuIndex).point();
                            if (red1Drive == -1)
                            {
                                red1Drive = temp;
                            }
                            else if (red2Drive == -1)
                            {
                                red2Drive = temp;
                            }
                            else if (red3Drive == -1)
                            {
                                red3Drive = temp;
                            }

                        }
                    }
                    else if (red1Zone == -1 || red2Zone == -1 || red3Zone == -1)
                    {
                        if (act == 1)
                        {
                            rmenuIndex--;
                            if (rmenuIndex <= -1)
                            {
                                rmenuIndex = zoneMenu.Count - 1;
                            }
                        }
                        else if (act == -1)
                        {
                            rmenuIndex++;
                            if (rmenuIndex >= zoneMenu.Count)
                            {
                                rmenuIndex = 0;
                            }
                        }
                        else if (act == 3)
                        {
                            int temp = zoneMenu.ElementAt<MenuItem>(rmenuIndex).point();
                            if (red1Zone == -1)
                            {
                                red1Zone = temp;
                            }
                            else if (red2Zone == -1)
                            {
                                red2Zone = temp;
                            }
                            else if (red3Zone == -1)
                            {
                                red3Zone = temp;
                            }

                        }
                    }// If zones
                    else
                    {
                        if (red1Strat == -1 || red2Strat == -1 || red3Strat == -1)
                        {
                            if (act == 1)
                            {
                                rmenuIndex--;
                                if (rmenuIndex <= -1)
                                {
                                    rmenuIndex = strategyMenu.Count - 1;
                                }
                            }
                            else if (act == -1)
                            {
                                rmenuIndex++;
                                if (rmenuIndex >= strategyMenu.Count)
                                {
                                    rmenuIndex = 0;
                                }
                            }
                            else if (act == 3)
                            {
                                int temp = strategyMenu.ElementAt<MenuItem>(rmenuIndex).point();
                                if (red1Strat == -1)
                                {
                                    red1Strat = temp;
                                }
                                else if (red2Strat == -1)
                                {
                                    red2Strat = temp;
                                }
                                else if (red3Strat == -1)
                                {
                                    red3Strat = temp;
                                }

                            }
                        }//If strategies not done
                    }//If zone done
                }//red Input != null
                else
                {
                    red1Zone = AerialRobot.BluePrimary;
                    red1Strat = AerialRobot.FollowAndShootAI;
                    red2Zone = AerialRobot.WhitePrimary;
                    red2Strat = AerialRobot.RecieveAndShootAI;
                    red3Zone = AerialRobot.RedPrimary;
                    red3Strat = AerialRobot.RecieveAndShootAI;
                }

                //BLUE
                if (blueTeamInput != null)
                {

                    blueTeamInput.run();
                    int act = blueTeamInput.getAction();
                    if (blue1Drive == -1 || blue2Drive == -1 || blue3Drive == -1)
                    {
                        if (act == 1)
                        {
                            bmenuIndex--;
                            if (bmenuIndex <= -1)
                            {
                                bmenuIndex = driveMenu.Count - 1;
                            }
                        }
                        else if (act == -1)
                        {
                            bmenuIndex++;
                            if (bmenuIndex >= driveMenu.Count)
                            {
                                bmenuIndex = 0;
                            }
                        }
                        else if (act == 3)
                        {
                            int temp = driveMenu.ElementAt<MenuItem>(bmenuIndex).point();
                            if (blue1Drive == -1)
                            {
                                blue1Drive = temp;
                            }
                            else if (blue2Drive == -1)
                            {
                                blue2Drive = temp;
                            }
                            else if (blue3Drive == -1)
                            {
                                blue3Drive = temp;
                            }

                        }
                    }
                    else if (blue1Zone == -1 || blue2Zone == -1 || blue3Zone == -1)
                    {
                        if (act == 1)
                        {
                            bmenuIndex--;
                            if (bmenuIndex <= -1)
                            {
                                bmenuIndex = zoneMenu.Count - 1;
                            }
                        }
                        else if (act == -1)
                        {
                            bmenuIndex++;
                            if (bmenuIndex >= zoneMenu.Count)
                            {
                                bmenuIndex = 0;
                            }
                        }
                        else if (act == 3)
                        {
                            int temp = zoneMenu.ElementAt<MenuItem>(bmenuIndex).point();
                            if (blue1Zone == -1)
                            {
                                blue1Zone = temp;
                            }
                            else if (blue2Zone == -1)
                            {
                                blue2Zone = temp;
                            }
                            else if (blue3Zone == -1)
                            {
                                blue3Zone = temp;
                            }

                        }
                    }// If zones
                    else
                    {
                        if (blue1Strat == -1 || blue2Strat == -1 || blue3Strat == -1)
                        {
                            if (act == 1)
                            {
                                bmenuIndex--;
                                if (bmenuIndex <= -1)
                                {
                                    bmenuIndex = strategyMenu.Count - 1;
                                }
                            }
                            else if (act == -1)
                            {
                                bmenuIndex++;
                                if (bmenuIndex >= strategyMenu.Count)
                                {
                                    bmenuIndex = 0;
                                }
                            }
                            else if (act == 3)
                            {
                                int temp = strategyMenu.ElementAt<MenuItem>(bmenuIndex).point();
                                if (blue1Strat == -1)
                                {
                                    blue1Strat = temp;
                                }
                                else if (blue2Strat == -1)
                                {
                                    blue2Strat = temp;
                                }
                                else if (blue3Strat == -1)
                                {
                                    blue3Strat = temp;
                                }

                            }
                        }//If strategies not done
                    }//If zone done
                }//Blue Input != null
                else
                {
                    blue1Zone = AerialRobot.RedPrimary;
                    blue1Strat = AerialRobot.FollowAndShootAI;
                    blue2Zone = AerialRobot.WhitePrimary;
                    blue2Strat = AerialRobot.RecieveAndShootAI;
                    blue3Zone = AerialRobot.BluePrimary;
                    blue3Strat = AerialRobot.RecieveAndShootAI;
                }

                if (blue1Drive != -1 && blue2Drive != -1 && blue3Drive != -1 && red1Drive != -1 && red2Drive != -1 && red3Drive != -1 && blue1Strat != -1 && blue2Strat != -1 && blue3Strat != -1 && red1Strat != -1 && red2Strat != -1 && red3Strat != -1)
                {
                    gameState = 1;
                    LoadContent();
                }
            }
            else if (gameState == 99)
            {
                this.Exit();
            }
            //redScore = balls.ElementAt<Ball>(0).getAssistBonus()+"";
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();


            //Draw the Field
            spriteBatch.Draw(field, new Vector2(0,0), null, Color.White, 0f, Vector2.Zero, new Vector2(widthScale, heightScale) , SpriteEffects.None, 0f);
            foreach (Robot r in robots)
            {
                spriteBatch.Draw(r.getImage(), r.getLocation(), null, r.getColor(), r.getRotation(), r.getOrigin(), r.getScale(), SpriteEffects.None, 0f);
            }
            foreach (Ball b in balls)
            {
                spriteBatch.Draw(b.getImage(), b.getLocation(), null, b.getColor(), b.getRotation(), b.getOrigin(), b.getScale(), SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(truss, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(widthScale, heightScale), SpriteEffects.None, 0f);
            foreach (Ball b in balls)
            {
                if (b.getHeight() > trussHeight)
                {
                    spriteBatch.Draw(b.getImage(), b.getLocation(), null, b.getColor(), b.getRotation(), b.getOrigin(), b.getScale(), SpriteEffects.None, 0f);
                }
            }

            if (gameState == 1)
            {
                //Draw the Robots
                

                //Draw the Balls


                

                //Draw the Score and Time
                spriteBatch.DrawString(timesNewRoman, blueScore + "", new Vector2(widthScale * 20f, heightScale * 20f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, redScore + "", new Vector2(graphics.GraphicsDevice.Viewport.Width - widthScale * 60f, heightScale * 20f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, remainingTime + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 30f * widthScale, heightScale * 20f), Color.White);

            }

            if (gameState == 0)
            {
                spriteBatch.DrawString(timesNewRoman, "Aerial Assist", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 70f * widthScale, heightScale * 25f), Color.Black);
                spriteBatch.DrawString(timesNewRoman, "Team 1477 Texas Torque", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 140f * widthScale, heightScale * 55f), Color.Black);
                foreach (MenuItem m in menuItems)
                {
                    spriteBatch.DrawString(timesNewRoman, m.text(), m.location(), m.color());
                }
                spriteBatch.Draw(Ball.image, menuItems.ElementAt<MenuItem>(menuIndex).location() + new Vector2(-45,5), null, Color.Red, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
            }
            if (gameState == 5)
            {
                spriteBatch.DrawString(timesNewRoman, "Match Results", new Vector2(graphics.GraphicsDevice.Viewport.Width/2 - 80f * widthScale, heightScale * 25f), Color.White);
                spriteBatch.DrawString(timesNewRoman, "Red Alliance", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3 - 50f * widthScale, heightScale * 50f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, "Blue Alliance", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 - 95f * widthScale, heightScale * 50f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, "Truss "+redTruss + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3 - 35f * widthScale, heightScale * 75f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, "Truss "+blueTruss + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 - 60f * widthScale, heightScale * 75f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, "Catch "+redCatch + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3 - 35f * widthScale, heightScale * 100f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, "Catch "+blueCatch + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 - 60f * widthScale, heightScale * 100f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, "Penalty "+redPenalty + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3 - 35f * widthScale, heightScale * 125f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, "Penalty "+bluePenalty + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 - 60f * widthScale, heightScale * 125f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, "Teleop " +(redScore - redTruss - redCatch - redPenalty) + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3 - 35f * widthScale, heightScale * 150f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, "Teleop "+(blueScore - blueTruss - blueCatch - bluePenalty) + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 - 60f * widthScale, heightScale * 150f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, "Total "+redScore + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3 - 35f * widthScale, heightScale * 175f), Color.Red);
                spriteBatch.DrawString(timesNewRoman, "Total "+blueScore + "", new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 - 60f * widthScale, heightScale * 175f), Color.Blue);
            }

            if (gameState == 8)
            {
                if (redTeamInput != null)
                {
                    Vector2 red = new Vector2(GraphicsDevice.Viewport.Width / 2 + 100, 50);
                    if (red1Drive == -1 || red2Drive == -1 || red3Drive == -1)
                    {
                        foreach (MenuItem m in driveMenu)
                        {
                            spriteBatch.DrawString(timesNewRoman, m.text(), m.location() + red, m.color());
                        }
                        spriteBatch.Draw(Ball.image, driveMenu.ElementAt<MenuItem>(rmenuIndex).location() + new Vector2(-45, 5) + red, null, Color.Red, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
                    }
                    else if (red1Zone == -1 || red2Zone == -1 || red3Zone == -1)
                    {
                        foreach (MenuItem m in zoneMenu)
                        {
                            spriteBatch.DrawString(timesNewRoman, m.text(), m.location() + red, m.color());
                        }
                        if (rmenuIndex < zoneMenu.Count)
                        {
                            spriteBatch.Draw(Ball.image, zoneMenu.ElementAt<MenuItem>(rmenuIndex).location() + new Vector2(-45, 5) + red, null, Color.Red, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
                        }
                    }
                    else if (red1Strat == -1 || red2Strat == -1 || red3Strat == -1)
                    {
                        foreach (MenuItem m in strategyMenu)
                        {
                            spriteBatch.DrawString(timesNewRoman, m.text(), m.location() + red, m.color());
                        }
                        if (rmenuIndex < strategyMenu.Count)
                        {
                            spriteBatch.Draw(Ball.image, strategyMenu.ElementAt<MenuItem>(rmenuIndex).location() + new Vector2(-45, 5) + red, null, Color.Red, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
                        }
                    }
                }
                if (blueTeamInput != null)
                {
                    Vector2 blue = new Vector2(150, 50);
                    if (blue1Zone == -1 || blue2Zone == -1 || blue3Zone == -1)
                    {
                        foreach (MenuItem m in driveMenu)
                        {
                            spriteBatch.DrawString(timesNewRoman, m.text(), m.location() + blue, m.color());
                        }
                        spriteBatch.Draw(Ball.image, driveMenu.ElementAt<MenuItem>(bmenuIndex).location() + new Vector2(-45, 5) + blue, null, Color.Blue, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
                    }
                    else if (blue1Zone == -1 || blue2Zone == -1 || blue3Zone == -1)
                    {
                        foreach (MenuItem m in zoneMenu)
                        {
                            spriteBatch.DrawString(timesNewRoman, m.text(), m.location() + blue, m.color());
                        }
                        if (bmenuIndex < zoneMenu.Count)
                        {
                            spriteBatch.Draw(Ball.image, zoneMenu.ElementAt<MenuItem>(bmenuIndex).location() + new Vector2(-45, 5) + blue, null, Color.Blue, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
                        }
                    }
                    else if (blue1Strat == -1 || blue2Strat == -1 || blue3Strat == -1)
                    {
                        foreach (MenuItem m in strategyMenu)
                        {
                            spriteBatch.DrawString(timesNewRoman, m.text(), m.location() + blue, m.color());
                        }
                        if (bmenuIndex < strategyMenu.Count)
                        {
                            spriteBatch.Draw(Ball.image, strategyMenu.ElementAt<MenuItem>(bmenuIndex).location() + new Vector2(-45, 5) + blue, null, Color.Blue, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
                        }
                    }
                }



            }
            

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
