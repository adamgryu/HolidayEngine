using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{
    public class ConfirmPrompt : Screen
    {
        Screen parent;

        public ConfirmPrompt(Engine engine, Screen parentScreen, String Name, String Text)
            : base (Name)
        {
            AddText(Text, engine.FontMain);
            AddElement(new ScreenButton(this, "Yes", engine.FontMain));
            AddElement(new ScreenButton(this, "No", engine.FontMain));
            this.parent = parentScreen;
            Center(engine);
        }

        public override void PreformAction(Engine engine, string ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Yes":
                    parent.PreformAction(engine, Name + " Yes");
                    this.PreformAction(engine, "Close");
                    break;
                case "No":
                    parent.PreformAction(engine, Name + " No");
                    this.PreformAction(engine, "Close");
                    break;
            }

            base.PreformAction(engine, ActionName);
        }
    }
}
