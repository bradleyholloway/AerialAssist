using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AerialAssist
{
    interface Robot
    {
        public Texture2D getImage();
        public float getRotation();
        public Vector2 getLocation();
        public Vector2 getScale();
        public Color getColor();
        public float getBallHeight();
        public void run(List<Robot> robots, List<Ball> balls);
    }
}
