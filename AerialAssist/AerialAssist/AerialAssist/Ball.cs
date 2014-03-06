using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AerialAssist
{
    class Ball
    {
        public static float radius;
        public static Texture2D image;
        public static float growthConstant;
        public static float decayConstant;
        public static float ballAcceleration;
        public static float bounceDecay;
        public static float trussHeight;
        
        private Vector3 location;
        private Color color;
        private Vector2 scale;
        private bool isFree;
        private Vector3 launchPosition;
        private Vector3 launchVelocity;
        private float timeSinceLaunch;
        private Robot linked;
        private bool inAir;

        public Ball(Robot startLink, Color color)
        {
            this.linked = startLink;
            this.color = color;
            this.isFree = false;
            this.linked.linkBall(this);
        }

        public void run(List<Ball> balls, float widthScale, float heightScale)
        {
            float x, y, z;
            if (isFree || linked == null)
            {
                timeSinceLaunch++;
                
                
                z = Math.Max(-ballAcceleration/2 * timeSinceLaunch * timeSinceLaunch + launchVelocity.Z * timeSinceLaunch + launchPosition.Z, 0f);
                
                if (inAir && z == 0)
                {
                    float velocity = launchVelocity.Z - ballAcceleration * timeSinceLaunch;
                    launchVelocity = new Vector3(launchVelocity.X * (1 - timeSinceLaunch * decayConstant), launchVelocity.Y * (1 - timeSinceLaunch * decayConstant), Math.Max(0, -(velocity + bounceDecay)));
                    timeSinceLaunch = 0f;
                    launchPosition = location;
                    inAir = false;
                }

                if (z > 0)
                {
                    inAir = true;
                }
                
                if (decayConstant * timeSinceLaunch >= 1)
                {
                    x = launchVelocity.X / (2 * decayConstant) + launchPosition.X;
                    y = launchVelocity.Y / (2 * decayConstant) + launchPosition.Y;
                }
                else
                {
                    x = launchVelocity.X * timeSinceLaunch - (decayConstant * timeSinceLaunch * timeSinceLaunch * launchVelocity.X) / 2 + launchPosition.X;
                    y = launchVelocity.Y * timeSinceLaunch - (decayConstant * timeSinceLaunch * timeSinceLaunch * launchVelocity.Y) / 2 + launchPosition.Y;
                }
                location = new Vector3(x, y, z);

                if (location.X < AerialRobot.minXPosition * widthScale || location.X > AerialRobot.maxXPosition * widthScale || Math.Abs(location.X * widthScale - 893.6f) < 10 * widthScale && Math.Abs(location.Z - trussHeight) < 4)
                {
                    launchPosition = location;
                    float velocity = launchVelocity.Z - ballAcceleration * timeSinceLaunch;
                    if (location.Z == 0)
                    {
                        velocity = 0f;
                    }
                    launchVelocity = new Vector3(-launchVelocity.X * (1 - decayConstant * timeSinceLaunch), launchVelocity.Y * (1 - decayConstant * timeSinceLaunch), -(velocity + bounceDecay));
                    timeSinceLaunch = 0f;
                }
                if (location.Y < AerialRobot.minYPosition * heightScale || location.Y > AerialRobot.maxYPosition * heightScale)
                {
                    launchPosition = location;
                    float velocity = launchVelocity.Z - ballAcceleration * timeSinceLaunch;
                    if (location.Z == 0)
                    {
                        velocity = 0f;
                    }
                    launchVelocity = new Vector3(launchVelocity.X * (1 - decayConstant * timeSinceLaunch), -launchVelocity.Y * (1 - decayConstant * timeSinceLaunch), -(velocity + bounceDecay));
                    timeSinceLaunch = 0f;
                }
            }
            else
            {
                inAir = true;
                location = new Vector3(linked.getLocation().X, linked.getLocation().Y, linked.getBallHeight());
            }
        }

        public void pushBall(Vector2 contact)
        {
            float velocity = launchVelocity.Z - ballAcceleration * timeSinceLaunch;
            if (getHeight() == 0f)
            {
                velocity = -bounceDecay*2;
            }
            launchPosition = location;
            launchVelocity = new Vector3(contact.X + launchVelocity.X * (1 - timeSinceLaunch * decayConstant), contact.Y + launchVelocity.Y * (1 - timeSinceLaunch * decayConstant), -(velocity + bounceDecay));
            timeSinceLaunch = 0f;
        }

        public void launch(Vector3 launchVelocity)
        {
            launchPosition = new Vector3(location.X, location.Y, location.Z);
            this.launchVelocity = launchVelocity;
            this.isFree = true;
            this.linked = null;
            timeSinceLaunch = 0f;
        }

        public Vector2 getScale()
        {
            float x = radius / 2 / image.Width;
            float y = radius / 2 / image.Height;

            x *= (1 + location.Z * growthConstant);
            y *= (1 + location.Z * growthConstant);

            scale = new Vector2(x, y);
            return scale;
        }

        public void linkRobot(Robot r)
        {
            isFree = false;
            linked = r;
        }

        public Texture2D getImage()
        {
            return image;
        }

        public Vector2 getLocation()
        {
            return new Vector2(location.X, location.Y);
        }

        public Color getColor()
        {
            return color;
        }

        public float getRotation()
        {
            return 0f;
        }

        public Vector2 getOrigin()
        {
            return new Vector2(image.Width / 2, image.Height / 2);
        }

        public float getHeight()
        {
            return location.Z;
        }
    }
}
