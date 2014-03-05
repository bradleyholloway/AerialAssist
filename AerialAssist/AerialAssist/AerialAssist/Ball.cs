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
        
        private Vector3 location;
        private Color color;
        private Vector2 scale;
        private Boolean isFree;
        private Vector3 launchPosition;
        private Vector3 launchVelocity;
        private float timeSinceLaunch;
        private Robot linked;

        public Ball(Vector2 location, Color color)
        {
            this.location = new Vector3(location.X, location.Y, 0f);
            this.color = color;
        }

        public void run()
        {
            float x, y, z;
            if (isFree)
            {
                timeSinceLaunch++;
                z = Math.Max(-4.9f * timeSinceLaunch * timeSinceLaunch + launchVelocity.Z * timeSinceLaunch + launchPosition.Z, 0f);
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
            }
            else
            {
                location = new Vector3(linked.getLocation().X, linked.getLocation().Y, linked.getBallHeight());
            }
        }

        public void launch(Vector3 launchVelocity)
        {
            launchPosition = new Vector3(location.X, location.Y, location.Z);
            this.launchVelocity = launchVelocity;
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

        public Texture2D getImage()
        {
            return image;
        }
    }
}
