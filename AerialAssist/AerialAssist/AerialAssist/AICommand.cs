using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AerialAssist
{
    class AICommand
    {
        public const int driveCommand = 1;
        public const int fireCommand = 2;
        public const int positionCommand = 3;
        public const int passCommand = 5;
        public const int defenseCommand = 6;

        Object value;
        int commandType;
        double timeout;

        public AICommand(int commandType, Object value, double timeout)
        {
            this.commandType = commandType;
            this.value = value;
            this.timeout = timeout;
        }

        public int getType()
        {
            return commandType;
        }

        public Object getValue()
        {
            return value;
        }

        public double getTimeout()
        {
            return timeout;
        }
    }
}
