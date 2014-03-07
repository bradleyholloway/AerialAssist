using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AerialAssist
{
    class AIHandler
    {
        int index = 0;
        List<AICommand> aiCommands;

        public AIHandler()
        {
            index = 0;
            aiCommands = new List<AICommand>();
        }
        public void putCommand(AICommand command)
        {
            aiCommands.Add(command);
        }
        public AICommand get()
        {
            return aiCommands.ElementAt<AICommand>(index);
        }
        public void move()
        {
            index++;
            if (index == aiCommands.Count)
            {
                index = 0;
            }
        }
    }
}
