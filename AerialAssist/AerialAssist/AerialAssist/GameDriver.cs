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

        Texture2D field;
        Texture2D truss;
        float widthScale, heightScale;
        float trussHeight;

        List<Robot> robots;
        List<Ball> balls;

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
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
            field = Content.Load<Texture2D>("Field");
            truss = Content.Load<Texture2D>("Truss");
            widthScale = (float)graphics.GraphicsDevice.Viewport.Width / field.Width;
            heightScale = (float)graphics.GraphicsDevice.Viewport.Height / field.Height;

            Ball.trussHeight = trussHeight = 12;
            Ball.image = Content.Load<Texture2D>("sphere");
            Ball.radius = widthScale * 47f;
            Ball.growthConstant = .02f;
            Ball.decayConstant = .006f;
            Ball.ballAcceleration = 0.08f;
            Ball.bounceDecay = .32f;
            AerialRobot.launchPower = 2f;
            ScoreMatrix.blueWhiteZone = .33f * widthScale * field.Width;
            ScoreMatrix.redWhiteZone = .66f * widthScale * field.Width;

            Texture2D sonic = Content.Load<Texture2D>("robot2");

            robots = new List<Robot>();
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 100f, heightScale * 100f), Color.Red, 0f, 0f, new KeyboardInput(PlayerIndex.One), AerialRobot.ArcadeDrive));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 200f, heightScale * 100f), Color.Blue, (float)Math.PI, 0f, new KeyboardInput(PlayerIndex.Two), AerialRobot.ArcadeDrive));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 100f, heightScale * 150f), Color.Red, 0f, 0f, new KeyboardInput(PlayerIndex.One), AerialRobot.ArcadeDrive));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 200f, heightScale * 150f), Color.Blue, (float)Math.PI, 0f, new KeyboardInput(PlayerIndex.Two), AerialRobot.ArcadeDrive));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 100f, heightScale * 200f), Color.Red, 0f, 0f, new KeyboardInput(PlayerIndex.One), AerialRobot.ArcadeDrive));
            robots.Add(new AerialRobot(sonic, new Vector2(.12f * widthScale, .12f * heightScale), new Vector2(widthScale * 200f, heightScale * 200f), Color.Blue, (float)Math.PI, 0f, new KeyboardInput(PlayerIndex.Two), AerialRobot.ArcadeDrive));

            balls = new List<Ball>();
            balls.Add(new Ball(robots.ElementAt<Robot>(0), Color.Red, robots.ElementAt<Robot>(2), robots.ElementAt<Robot>(4)));
            balls.Add(new Ball(robots.ElementAt<Robot>(1), Color.Blue, robots.ElementAt<Robot>(3), robots.ElementAt<Robot>(5)));
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

            foreach (Robot r in robots)
            {
                r.run(robots, balls, widthScale, heightScale);
            }
            foreach (Ball b in balls)
            {
                b.run(balls, widthScale, heightScale);
            }
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

            //Draw the Robots
            foreach (Robot r in robots) {
                spriteBatch.Draw(r.getImage(), r.getLocation(), null, r.getColor(), r.getRotation(), r.getOrigin(), r.getScale(), SpriteEffects.None, 0f);
            }

            //Draw the Balls
            foreach (Ball b in balls) {
                spriteBatch.Draw(b.getImage(), b.getLocation(), null, b.getColor(), b.getRotation(), b.getOrigin(), b.getScale(), SpriteEffects.None, 0f);
            }

            //Draw the Score and Time
            //
            //
            //

            spriteBatch.Draw(truss, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(widthScale, heightScale), SpriteEffects.None, 0f);

            foreach (Ball b in balls)
            {
                if (b.getHeight() > trussHeight)
                {
                    spriteBatch.Draw(b.getImage(), b.getLocation(), null, b.getColor(), b.getRotation(), b.getOrigin(), b.getScale(), SpriteEffects.None, 0f);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
