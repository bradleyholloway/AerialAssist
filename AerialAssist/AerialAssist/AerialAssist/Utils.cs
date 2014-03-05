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

namespace BradleyXboxUtils
{
    public abstract class Input
    {
        public abstract double getLeftX();
        public abstract double getRightX();
        public abstract double getLeftY();
        public abstract double getRightY();
        public abstract Boolean getBottomActionButton();
        public abstract Boolean getTopActionButton();
        public abstract Boolean getLeftActionButton();
        public abstract Boolean getRightActionButton();
        public abstract double getRightTrigger();
        public abstract double getLeftTrigger();
        public abstract Boolean getRightBumper();
        public abstract Boolean getLeftBumper();
        public abstract Boolean getUpDPad();
        public abstract Boolean getDownDPad();
        public abstract Boolean getLeftDPad();
        public abstract Boolean getRightDPad();
        public abstract Boolean getStart();
        public abstract Boolean getBack();
        public abstract Boolean getRightStickDown();
        public abstract Boolean getLeftStickDown();
        public abstract Boolean getBigButton();
        public abstract Vector2 getLeftJoystick();
        public abstract Vector2 getRightJoystick();
    }

    public class ControllerInput : Input
    {
        PlayerIndex p;

        public ControllerInput()
        {
            p = PlayerIndex.One;
        }

        public ControllerInput(PlayerIndex player)
        {
            p = player;
        }
        
        public override double getLeftX()
        {
            return GamePad.GetState(p).ThumbSticks.Left.X;
        }
        public override double getRightX()
        {
            return GamePad.GetState(p).ThumbSticks.Right.X;
        }
        public override double getLeftY()
        {
            return -1 * GamePad.GetState(p).ThumbSticks.Left.Y;
        }
        public override double getRightY()
        {
            return -1 * GamePad.GetState(p).ThumbSticks.Right.Y;
        }
        public override Boolean getBottomActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.A);
        }
        public override Boolean getLeftActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.X);
        }
        public override Boolean getTopActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.Y);
        }
        public override Boolean getRightActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.B);
        }
        public override Boolean getUpDPad()
        {
            return (GamePad.GetState(p).DPad.Up == ButtonState.Pressed);
        }
        public override Boolean getRightDPad()
        {
            return (GamePad.GetState(p).DPad.Right == ButtonState.Pressed);
        }
        public override Boolean getDownDPad()
        {
            return (GamePad.GetState(p).DPad.Down == ButtonState.Pressed);
        }
        public override Boolean getLeftDPad()
        {
            return (GamePad.GetState(p).DPad.Left == ButtonState.Pressed);
        }
        public override Boolean getStart()
        {
            return (GamePad.GetState(p).IsButtonDown(Buttons.Start));
        }
        public override Boolean getBack()
        {
            return (GamePad.GetState(p).IsButtonDown(Buttons.Back));
        }
        public override Boolean getBigButton()
        {
            return (GamePad.GetState(p).IsButtonDown(Buttons.BigButton));
        }
        public override Boolean getRightBumper()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.RightShoulder);
        }
        public override Boolean getLeftBumper()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.LeftShoulder);
        }
        public override Boolean getRightStickDown()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.RightStick);
        }
        public override Boolean getLeftStickDown()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.LeftStick);
        }
        public override double getRightTrigger()
        {
            return GamePad.GetState(p).Triggers.Right;
        }
        public override double getLeftTrigger()
        {
            return GamePad.GetState(p).Triggers.Left;
        }
        public override Vector2 getLeftJoystick()
        {
            return GamePad.GetState(p).ThumbSticks.Left * new Vector2(1,-1);
        }
        public override Vector2 getRightJoystick()
        {
            return GamePad.GetState(p).ThumbSticks.Right * new Vector2(1, -1);
        }
    }//Controller Input

    public class KeyboardInput : Input
    {
        PlayerIndex p;
        Boolean secondPlayer;

        public KeyboardInput()
        {
            p = PlayerIndex.One;
        }

        public KeyboardInput(PlayerIndex player)
        {
            p = player;
            secondPlayer = (p.Equals(PlayerIndex.Two));
        }

        private double boolToDouble(Boolean state)
        {
            return (state) ? 1.00 : 0.00;
        }

        public override double getLeftX()
        {
            return boolToDouble(false);
        }
        public override double getRightX()
        {
            if (secondPlayer)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    return 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                    return -1;
                else
                    return 0;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    return 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    return -1;
                else
                    return 0;
            }
        }

        public override double getLeftY()
        {
            if (secondPlayer)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    return -1;
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                    return 1;
                else
                    return 0;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    return -1;
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    return 1;
                else
                    return 0;
            }
        }
        public override double getRightY()
        {
            return 0;
        }
        public override Boolean getBottomActionButton()
        {
            if (secondPlayer)
                return Keyboard.GetState().IsKeyDown(Keys.LeftControl);
            else
                return Keyboard.GetState().IsKeyDown(Keys.RightControl);
            //return GamePad.GetState(p).IsButtonDown(Buttons.A);
        }
        public override Boolean getLeftActionButton()
        {
            if (secondPlayer)
                return Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            else
                return Keyboard.GetState().IsKeyDown(Keys.RightShift);
            //return GamePad.GetState(p).IsButtonDown(Buttons.X);
        }
        public override Boolean getTopActionButton()
        {
            return false;
            //return GamePad.GetState(p).IsButtonDown(Buttons.Y);
        }
        public override Boolean getRightActionButton()
        {
            if (secondPlayer)
                return Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            else
                return Keyboard.GetState().IsKeyDown(Keys.RightAlt);
        }
        public override Boolean getUpDPad()
        {
            return Keyboard.GetState().IsKeyDown(Keys.Space);
            //return (GamePad.GetState(p).DPad.Up == ButtonState.Pressed);
        }
        public override Boolean getRightDPad()
        {
            return false;
            //return (GamePad.GetState(p).DPad.Right == ButtonState.Pressed);
        }
        public override Boolean getDownDPad()
        {
            if (secondPlayer)
                return Keyboard.GetState().IsKeyDown(Keys.Tab);
            else
                return Keyboard.GetState().IsKeyDown(Keys.End);
            //return (GamePad.GetState(p).DPad.Down == ButtonState.Pressed);
        }
        public override Boolean getLeftDPad()
        {
            return false;
            //return (GamePad.GetState(p).DPad.Left == ButtonState.Pressed);
        }
        public override Boolean getStart()
        {
            return Keyboard.GetState().IsKeyDown(Keys.Enter);
            //return (GamePad.GetState(p).IsButtonDown(Buttons.Start));
        }
        public override Boolean getBack()
        {
            return getDownDPad();
            //return (GamePad.GetState(p).IsButtonDown(Buttons.Back));
        }
        public override Boolean getBigButton()
        {
            return false;
            //return (GamePad.GetState(p).IsButtonDown(Buttons.BigButton));
        }
        public override Boolean getRightBumper()
        {
            return Keyboard.GetState().IsKeyDown(Keys.F);
            //return GamePad.GetState(p).IsButtonDown(Buttons.RightShoulder);
        }
        public override Boolean getLeftBumper()
        {
            return false;
            //return GamePad.GetState(p).IsButtonDown(Buttons.LeftShoulder);
        }
        public override Boolean getRightStickDown()
        {
            return false;
            //return GamePad.GetState(p).IsButtonDown(Buttons.RightStick);
        }
        public override Boolean getLeftStickDown()
        {
            return false;
            //return GamePad.GetState(p).IsButtonDown(Buttons.LeftStick);
        }
        public override double getRightTrigger()
        {
            return boolToDouble(getRightBumper());
            //return GamePad.GetState(p).Triggers.Right;
        }
        public override double getLeftTrigger()
        {
            return 0.0;
            //return GamePad.GetState(p).Triggers.Left;
        }
        public override Vector2 getLeftJoystick()
        {
            return new Vector2((float)getLeftX(), (float)getLeftY());
        }
        public override Vector2 getRightJoystick()
        {
            return getLeftJoystick();
            //return new Vector2((float)getRightX(), (float)getRightY());
        }
    }//Keyboard Input

    public class MenuInput
    {
        Input input;
        ControlButton up;
        ControlButton down;
        ControlButton ok;
        int action;
        
        public MenuInput(Input i)
        {
            input = i;
            up = new ControlButton();
            down = new ControlButton();
            ok = new ControlButton();
            action = 0;
        }
        public void run()
        {
            action = 0;
            if (up.update(input.getUpDPad() || input.getRightY() > .7 || input.getLeftY() > .7))
                action -= 1;
            if (down.update(input.getDownDPad() || input.getRightY() < -.7 || input.getLeftY() < -.7))
                action += 1;
            if (ok.update(input.getBottomActionButton() || input.getRightTrigger() > .7))
                action = 3;
        }
        public int getAction()
        {
            return action;
        }
    }

    public class Toggle
    {
        Boolean value;
        Boolean lastValue;

        public Toggle()
        {
            value = false;
            lastValue = true;
        }
        public Toggle(Boolean a)
        {
            value = a;
            lastValue = true;
        }

        public Boolean update(Boolean a)
        {
            if (a != lastValue)
            {
                if (a)
                {
                    value = !value;
                    lastValue = true;
                }
                else if (!a)
                {
                    lastValue = false;
                }
            }
            return value;
        }
        public Boolean get()
        {
            return value;
        }
    }//Toggle

    public class ControlButton
    {
        Boolean last;
        Boolean state;
        int maxDelay;
        int delay;

        public ControlButton()
        {
            last = true;
            state = false;
        }
        public ControlButton(Boolean start)
        {
            state = start;
            last = true;
        }
        public ControlButton(int maxDelay)
        {
            this.maxDelay = maxDelay;
            delay = maxDelay;
        }

        /*public void update(Boolean current)
        {
            if (!last && current)
                state = true;
            last = current;
        }*/

        public Boolean update(Boolean current)
        {
            if (delay > 0)
                delay--;
            else
            {
                if (!last && current)
                    state = true;
                last = current;
                Boolean temp = state;
                if (temp)
                {
                    if (delay != 0)
                    {
                        temp = false;
                    }
                    else
                    {
                        state = false;
                        delay = maxDelay;
                    }
                }
                return temp;
            }
            
            return false;
        }

        public Boolean get()
        {
            Boolean temp = state;
            state = false;
            return temp;
        }
    }// ControlButton

    public class MenuItem
    {
        private Color colord;
        private Vector2 locationd;
        private String textd;
        private int pointd;

        public MenuItem(String t, Vector2 l, Color c)
        {
            colord = c;
            textd = t;
            locationd = l;
            pointd = 0;
        }
        public MenuItem(String t, Vector2 l, Color c, int p)
        {
            colord = c;
            textd = t;
            locationd = l;
            pointd = p;
        }
        public Color color()
        {
            return colord;
        }
        public Vector2 location()
        {
            return locationd;
        }
        public String text()
        {
            return textd;
        }
        public int point()
        {
            return pointd;
        }
    }//MenuItem

    public class PID
    {
        private double pConst;
        private double iConst;
        private double dConst;

        private double desiredVal;
        private double previousVal;
        private double errorSum;
        private double errorIncrement;
        private double errorEpsilon;
        private double doneRange;

        private Boolean firstCycle;
        private double maxOutput;
        private int minCycleCount;
        private int cycleCount;

        public PID(double p, double i, double d, double eps)
        {
            pConst = p;
            iConst = i;
            dConst = d;
            errorEpsilon = eps;
            doneRange = eps;

            desiredVal = 0.0;
            firstCycle = true;
            maxOutput = 1.0;
            errorIncrement = 1.0;

            cycleCount = 0;
            minCycleCount = 0;
        }
        public void setConstants(double p, double i, double d)
        {
            pConst = p;
            iConst = i;
            dConst = d;
        }
        public void setDoneRange(double range)
        {
            doneRange = range;
        }
        public void setErrorEpsilon(double eps)
        {
            errorEpsilon = eps;
        }
        public void setDesiredValue(double val)
        {
            desiredVal = val;
        }
        public void setMaxOutput(double max)
        {
            if (max < 0.0)
            {
                maxOutput = 0.0;
            }
            else if (max > 1.0)
            {
                maxOutput = 1.0;
            }
            else
            {
                maxOutput = max;
            }
        }
        public void setMinDoneCycles(int num)
        {
            minCycleCount = num;
        }
        public void resetErrorSum()
        {
            errorSum = 0.0;
        }
        public double getDesiredVal()
        {
            return desiredVal;
        }
        public double getPreviousVal()
        {
            return previousVal;
        }
        public double calcPID(double currentVal)
        {
            double pVal = 0.0;
            double iVal = 0.0;
            double dVal = 0.0;

            if (firstCycle)
            {
                previousVal = currentVal;
                firstCycle = false;
            }

            //P Calculation
            double error = desiredVal - currentVal;
            pVal = pConst * error;

            //I Calculation
            if (error > errorEpsilon)
            {
                if (errorSum < 0.0)
                {
                    errorSum = 0.0;
                }
                errorSum += Math.Min(error, errorIncrement);
            }
            else if (error < -1 * errorEpsilon)
            {
                if (errorSum > 0.0)
                {
                    errorSum = 0.0;
                }
                errorSum += Math.Max(error, -1 * errorIncrement);
            }
            else
            {
                errorSum = 0.0;
            }
            iVal = iConst * errorSum;

            //D Calculation
            double deriv = currentVal - previousVal;
            dVal = dConst * deriv;

            //PID calculation
            double output = pVal + iVal - dVal;

            output = UTIL.limitValue(output, maxOutput);

            previousVal = currentVal;
            return output;
        }
        public Boolean isDone()
        {
            double currError = Math.Abs(desiredVal - previousVal);
            if (currError <= doneRange)
            {
                cycleCount++;
            }
            else
            {
                cycleCount = 0;
            }
            return cycleCount > minCycleCount;
        }
        public void resetPreviousVal()
        {
            firstCycle = true;
        }
    }//PID

    public static class UTIL
    {
        public static double limitValue(double input, double limit)
        {
            if (input > limit)
                return limit;
            else if (input < -limit)
                return -limit;
            else
                return input;
        }
        public static double distance(Vector2 point1, Vector2 point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }
        public static double distance(Point point1, Point point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }
        public static Vector2 magD(double mag, double d)
        {
            return new Vector2((float)(mag * Math.Cos(d)), (float)(mag * Math.Sin(d)));
        }
        public static Point midpoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }
        public static Vector2 pointToVector(Point a)
        {
            return new Vector2(a.X, a.Y);
        }
        public static Point vectorToPoint(Vector2 a)
        {
            return new Point((int)a.X, (int)a.Y);
        }
        public static double getDirectionTward(Point start, Point goal)
        {
            return Math.Atan2((goal.Y - start.Y), (goal.X - start.X));
        }
        public static double getDirectionTward(Vector2 start, Vector2 goal)
        {
            return Math.Atan2((goal.Y - start.Y), (goal.X - start.X));
        }
        public static double normalizeDirection(double startDir)
        {
            while (startDir > Math.PI * 2)
                startDir -= (Math.PI * 2);
            while (startDir < 0)
                startDir += (Math.PI * 2);
            return startDir;
        }
        public static Boolean tolerant(double actual, double goal, double tollerance)
        {
            return tollerance > Math.Abs(actual - goal);
        }
    }//static Util

}
