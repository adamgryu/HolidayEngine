using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Level;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{
    public class SaveLoadBlockset : Screen
    {
        ScreenText text;
        BlockPlacer parentScreen;
        ScreenInput input;

        public SaveLoadBlockset(Engine engine, BlockPlacer parentScreen)
            : base("Save or Load Prompt")
        {
            DemandPriority = true;
            text = new ScreenText(this, "Type in the name of this set below.", engine.FontMain);
            AddElement(text);
            input = new ScreenInput(this, engine.FontMain);
            input.InputString = engine.room.BlockSet.Name;
            AddElement(input);
            AddElement(new ScreenButton(this, "Save", engine.FontMain));
            AddElement(new ScreenButton(this, "Load", engine.FontMain));
            AddCloseButton(engine);
            this.parentScreen = parentScreen;
        }

        public override void PreformAction(Engine engine, string ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Save":
                    engine.room.BlockSet.SaveBlockSet(engine, input.InputString);
                    this.PreformAction(engine, "Close");
                    break;
                case "Load":
                    Blockset _b = Blockset.LoadBlockSet(engine, input.InputString);
                    if (_b != null)
                    {
                        engine.room.BlockSet = _b;
                        engine.room.ResetInvalidBlocks();
                        parentScreen.FilterBlocks(engine);
                    }
                    else
                        engine.screenManager.AddScreen(new Message(engine, "The file was not found.",true));
                    this.PreformAction(engine, "Close");
                    break;
            }
            base.PreformAction(engine, ActionName);
        }
    }
}
