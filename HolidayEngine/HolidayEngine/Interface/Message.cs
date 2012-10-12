using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayEngine.Interface
{
    public class Message : Screen
    {
        public Message(Engine engine, String Message, bool Priority)
            : base("Message")
        {
            this.AddText(Message, engine.FontMain);
            this.AddCloseButton(engine);
            this.Size.Y = GetTotalElementHeight();
            this.Center(engine);
            this.DemandPriority = Priority;
        }
    }
}
