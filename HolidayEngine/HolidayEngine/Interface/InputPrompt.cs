using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{
    public class InputPrompt : Screen
    {
        Screen parentScreen;
        ScreenInput input;

        public InputPrompt(Engine engine, Screen parentScreen, String title, String text, String startText, bool CloseButton)
            : base(title)
        {
            DemandPriority = true;
            AddText(text, engine.FontMain);
            input = new ScreenInput(this, engine.FontMain);
            input.InputString = startText;
            AddElement(input);
            AddElement(new ScreenButton(this, "OK", engine.FontMain));
            this.parentScreen = parentScreen;
            if (CloseButton)
                AddCloseButton(engine);
            Center(engine);
        }

        public override void PreformAction(Engine engine, string ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Input":
                    PreformAction(engine, "OK");
                    break;
                case "OK":
                    parentScreen.PreformAction(engine, Name, input.InputString);
                    this.PreformAction(engine, "Close");
                    break;
            }
            base.PreformAction(engine, ActionName);
        }
    }
}
