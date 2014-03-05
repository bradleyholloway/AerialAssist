using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AerialAssist
{
    class AerialRobot : Robot
    {
        Vector2 Robot.getLocation()
        {
            return Vector2.Zero;
        }

        Vector2 Robot.getScale()
        {
            return Vector2.UnitX + Vector2.UnitY;
        }

        float Robot.getRotation()
        {
            return 0f;
        }

        float Robot.getBallHeight()
        {
            return 0f;
        }

        Color Robot.getColor()
        {
            return Color.White;
        }

        Texture2D Robot.getImage()
        {
            return null;
        }

    }
}
