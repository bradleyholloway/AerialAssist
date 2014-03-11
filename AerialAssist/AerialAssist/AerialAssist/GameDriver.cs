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
        bool keyboard1, keyboard2;
        MenuInput menuInput;
        List<MenuItem> menuItems;
        int menuIndex;

        int gameState = 0;
        Texture2D field;
        Texture2D truss;
        float widthScale, heightScale;
        float trussHeight;
        int redScore, blueScore;

        List<Robot> robots;
        List<Ball> balls;

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
            menuItems.Add(new MenuItem("No Keyboard", new Vector2(100f, 100f), Color.White, 4));
            menuItems.Add(new MenuItem("1  Keyboard", new Vector2(100f, 150f), Color.Red, 2));
            menuItems.Add(new MenuItem("2  Keyboard", new Vector2(100f, 200f), Color.Red, 3));

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

            Ball.trussHeight = trussHeight = 12;
            Ball.image = Content.Load<Texture2D>("sphere");
            Ball.radius = widthScale * 47f;
            Ball.growthConstant = .02f;
            Ball.decayConstant = .012f;
            Ball.ballAcceleration = 0.08f;
            Ball.bounceDecay = .58f;
            AerialRobot.launchPower = 4f;
            AerialRobot.widthScale = widthScale;
            AerialRobot.heightScale = heightScale;
            ScoreMatrix.blueWhiteZone = .33f * widthScale * field.Width;
            ScoreMatrix.redWhiteZone = .66f * widthScale * field.Width;

            Texture2D sonic = Content.Load<Texture2D>("robot2");

            robots = new List<Robot>();
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 500f, heightScale * 100f), Color.Red, .00001f, 0f, new KeyboardInput(PlayerIndex.One), AerialRobot.ArcadeDrive, !keyboard1, AerialRobot.StandardAI));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 200f, heightScale * 100f), Color.Blue, (float)Math.PI+.00001f, 0f, new KeyboardInput(PlayerIndex.Two), AerialRobot.ArcadeDrive, !keyboard2, AerialRobot.DefenseAI));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 500f, heightScale * 150f), Color.Red, .00001f, 0f, new ControllerInput(PlayerIndex.One), AerialRobot.ArcadeDrive, !GamePad.GetState(PlayerIndex.One).IsConnected, AerialRobot.StandardAI));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 200f, heightScale * 150f), Color.Blue, (float)Math.PI + .00001f, 0f, new ControllerInput(PlayerIndex.Two), AerialRobot.McCannumDrive, !GamePad.GetState(PlayerIndex.Two).IsConnected, AerialRobot.StandardAI));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 500f, heightScale * 200f), Color.Red, .00001f, 0f, new ControllerInput(PlayerIndex.Three), AerialRobot.McCannumDrive, !GamePad.GetState(PlayerIndex.Three).IsConnected, AerialRobot.DefenseAI));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 200f, heightScale * 200f), Color.Blue, (float)Math.PI + .00001f, 0f, new ControllerInput(PlayerIndex.Four), AerialRobot.McCannumDrive, !GamePad.GetState(PlayerIndex.Four).IsConnected, AerialRobot.StandardAI));

            balls = new List<Ball>();
            balls.Add(new Ball(robots.ElementAt<Robot>(0), Color.Red, robots.ElementAt<Robot>(2), robots.ElementAt<Robot>(4)));
            balls.Add(new Ball(robots.ElementAt<Robot>(1), Color.Blue, robots.ElementAt<Robot>(3), robots.ElementAt<Robot>(5)));

            menuInput = new MenuInput(((GamePad.GetState(PlayerIndex.One).IsConnected) ? (Input)new ControllerInput(PlayerIndex.One) : (Input)new KeyboardInput()));
            menuIndex = 0;

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
                this.Exit();
            if (gameState == 0)
            {
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
                foreach (Robot r in robots)
                {
                    r.run(robots, balls, widthScale, heightScale);
                }
                for (int ball = 0; ball < balls.Count; ball++)
                {
                    Ball b = balls.ElementAt<Ball>(ball);
                    int result = b.run(balls, widthScale, heightScale);
                    if (result != -1)
                    {
                        balls.Remove(b);
                        if (b.getColor().Equals(Color.Red))
                        {
                            redScore += result;
                            balls.Add(new Ball(new Vector2(200 * widthScale, 140 * heightScale), Color.Red, robots.ElementAt<Robot>(0), robots.ElementAt<Robot>(2), robots.ElementAt<Robot>(4)));
                        }
                        else
                        {
                            blueScore += result;
                            balls.Add(new Ball(new Vector2(400 * widthScale, 140 * heightScale), Color.Blue, robots.ElementAt<Robot>(1), robots.ElementAt<Robot>(3), robots.ElementAt<Robot>(5)));
                        }
                        ball--;
                    }
                }
            }
            else if (gameState == 2)
            {
                keyboard1 = true;
                keyboard2 = false;
                gameState = 1;
                LoadContent();
            }
            else if (gameState == 3)
            {
                keyboard1 = true;
                keyboard2 = true;
                gameState = 1;
                LoadContent();
            }
            else if (gameState == 4)
            {
                keyboard1 = false;
                keyboard2 = false;
                gameState = 1;
                LoadContent();
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

            if (gameState == 1)
            {
                //Draw the Robots
                foreach (Robot r in robots)
                {
                    spriteBatch.Draw(r.getImage(), r.getLocation(), null, r.getColor(), r.getRotation(), r.getOrigin(), r.getScale(), SpriteEffects.None, 0f);
                }

                //Draw the Balls
                foreach (Ball b in balls)
                {
                    spriteBatch.Draw(b.getImage(), b.getLocation(), null, b.getColor(), b.getRotation(), b.getOrigin(), b.getScale(), SpriteEffects.None, 0f);
                }

                //Draw the Score and Time
                spriteBatch.DrawString(timesNewRoman, blueScore + "", new Vector2(widthScale * 20f, heightScale * 20f), Color.Blue);
                spriteBatch.DrawString(timesNewRoman, redScore + "", new Vector2(graphics.GraphicsDevice.Viewport.Width - widthScale * 60f, heightScale * 20f), Color.Red);


                spriteBatch.Draw(truss, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(widthScale, heightScale), SpriteEffects.None, 0f);

                foreach (Ball b in balls)
                {
                    if (b.getHeight() > trussHeight)
                    {
                        spriteBatch.Draw(b.getImage(), b.getLocation(), null, b.getColor(), b.getRotation(), b.getOrigin(), b.getScale(), SpriteEffects.None, 0f);
                    }
                }
            }

            if (gameState == 0)
            {
                foreach (MenuItem m in menuItems)
                {
                    spriteBatch.DrawString(timesNewRoman, m.text(), m.location(), m.color());
                }
                spriteBatch.Draw(Ball.image, menuItems.ElementAt<MenuItem>(menuIndex).location() + new Vector2(-45,5), null, Color.Red, 0f, Vector2.Zero, new Vector2(.2f, .2f), SpriteEffects.None, 0f);
            }
            

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
