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
        public static float onePointBottom;
        public static float onePointTop;
        
        private Vector3 location;
        private Color color;
        private Vector2 scale;
        private bool isFree;
        private Vector3 launchPosition;
        private Vector3 launchVelocity;
        private float timeSinceLaunch;
        private Robot linked;
        private bool inAir;
        private ScoreMatrix matrix;
        private int penaltyPoints;
        private bool fouled;
        private int trussPoints;
        private int catchPoints;
        private bool trussed;
        private bool catched;
        private bool catchable;

        public Ball(Robot startLink, Color color, Robot r2, Robot r3)
        {
            this.linked = startLink;
            this.color = color;
            this.isFree = false;
            this.linked.linkBall(this);
            this.matrix = new ScoreMatrix(startLink, r2, r3);
        }

        public Ball(Vector2 startPosition, Color color, Robot r1, Robot r2, Robot r3)
        {
            this.linked = null;
            this.color = color;
            this.isFree = true;
            this.location = new Vector3(startPosition.X, startPosition.Y, 0);
            this.launchPosition = location;
            this.launchVelocity = Vector3.Zero;
            this.matrix = new ScoreMatrix(r1, r2, r3);
        }

        public int run(List<Ball> balls, float widthScale, float heightScale)
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
                    catchable = false;
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

                foreach (Ball b in balls)
                {
                    if (!b.Equals(this))
                    {
                        if (BradleyXboxUtils.UTIL.distance(b.getLocation(), this.getLocation()) < Ball.radius / (widthScale))
                        {

                            //Sphere Collision
                            Vector2 forceVector = BradleyXboxUtils.UTIL.magD(Ball.radius / (widthScale) - BradleyXboxUtils.UTIL.distance(this.getLocation(), b.getLocation()), BradleyXboxUtils.UTIL.getDirectionTward(this.getLocation(), b.getLocation()));
                            b.pushBall(forceVector * .4f);
                            pushBall(-1 * forceVector * .4f);
                        }
                    }
                }
                if ((location.X < AerialRobot.minXPosition * widthScale && color.Equals(Color.Blue) || location.X > AerialRobot.maxXPosition * widthScale && color.Equals(Color.Red)) && location.Z >= trussHeight * 3/2 && location.Z <= trussHeight * 6/2)
                {
                    return matrix.getAssistBonus() + 10;
                }
                if ((location.X < (AerialRobot.minXPosition + 20) * widthScale && color.Equals(Color.Blue) || location.X > (AerialRobot.maxXPosition * widthScale - 20) && color.Equals(Color.Red)) && location.Z < 3 && (location.Y < onePointTop || location.Y > onePointBottom))
                {
                    return matrix.getAssistBonus() + 1;
                }

                if (location.X < AerialRobot.minXPosition * widthScale || location.X > AerialRobot.maxXPosition * widthScale || Math.Abs(location.X - 304.278f * widthScale) < 8 * widthScale && Math.Abs(location.Z - trussHeight) < 4)
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

                if (location.X < AerialRobot.minXPosition * widthScale - Ball.radius/3 || location.X > AerialRobot.maxXPosition * widthScale + Ball.radius/3)
                {
                    return 0;
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

                if (location.Y < AerialRobot.minYPosition * heightScale - Ball.radius/3 || location.Y > AerialRobot.maxYPosition * heightScale + Ball.radius/3)
                {
                    return 0;
                }

            }
            else
            {
                inAir = true;
                location = new Vector3(linked.getLocation().X, linked.getLocation().Y, linked.getBallHeight());
                matrix.update(linked);
            }
            if (linked != null && !linked.getColor().Equals(color))
            {
                if (!fouled)
                {
                    fouled = true;
                    penaltyPoints = 50;
                }
            }
            else
            {
                fouled = false;
            }
            
            if(!trussed && (((launchPosition.X < 304.278f * widthScale) && (location.X > 304.278f * widthScale)) ||((launchPosition.X > 304.278f * widthScale) && (location.X < 304.278f * widthScale))) && (location.Z > trussHeight))
            {
                trussed = true;
                trussPoints = 10;
            }
            if (!catched && (((launchPosition.X < 304.278f * widthScale) && (location.X > 304.278f * widthScale)) || ((launchPosition.X > 304.278f * widthScale) && (location.X < 304.278f * widthScale))) && (location.Z > trussHeight))
            {
                catchable = true;
            }
            

            return -1;
        }

        public int getPenaltyPoints()
        {
            int temp = penaltyPoints;
            penaltyPoints = 0;
            return temp;
        }
        public int getTrussPoints()
        {
            int temp = trussPoints;
            trussPoints = 0;
            return temp;
        }
        public int getCatchPoints()
        {
            int temp = catchPoints;
            catchPoints = 0;
            return temp;
        }

        public void pushBall(Vector2 contact)
        {
            if (contact.Length() > 5)
            {
                contact = BradleyXboxUtils.UTIL.magD(5, BradleyXboxUtils.UTIL.getDirectionTward(Vector2.Zero, contact));
            }
            if (contact.Length() < 1)
            {
                contact = BradleyXboxUtils.UTIL.magD(1, BradleyXboxUtils.UTIL.getDirectionTward(Vector2.Zero, contact));
            }
            
            float velocity = launchVelocity.Z - ballAcceleration * timeSinceLaunch;
            if (getHeight() == 0f)
            {
                velocity = -bounceDecay*2;
            }
            launchPosition = location;
            launchVelocity = new Vector3(contact.X + launchVelocity.X * Math.Min((1 - timeSinceLaunch * decayConstant),0), contact.Y + launchVelocity.Y * Math.Min((1 - timeSinceLaunch * decayConstant),0), -(velocity + bounceDecay));
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
            if (!catched && catchable && location.Z > .1)
            {
                catched = true;
                catchPoints = 10;

            }
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

        public bool getIsFree()
        {
            return isFree;
        }

        public int getAssistBonus()
        {
            return matrix.getAssistBonus();
        }
    }
}
