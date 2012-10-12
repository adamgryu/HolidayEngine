using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Level;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{
    class BlockSettings : Screen
    {
        Block block;

        ScreenButton CullingButton;
        ScreenButton SolidButton;

        public BlockSettings(Engine engine, Block block)
            : base("Block Settings")
        {
            this.block = block;
            this.DemandPriority = true;
            CullingButton = new ScreenButton(this, "Toggle Culling", "Culling: False", engine.FontMain);
            AddElement(CullingButton);
            SolidButton = new ScreenButton(this, "Toggle Solid", "Solid: False", engine.FontMain);
            AddElement(SolidButton);
            AddCloseButton(engine);
            Center(engine);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            CullingButton.Text = "Culling: " + block.Culling.ToString();
            SolidButton.Text = "Solid: " + block.Solid.ToString();
        }

        public override void PreformAction(Engine engine, string ActionName, params string[] Arguments)
        {
            switch (ActionName)
            {
                case "Toggle Culling":
                    block.Culling = !block.Culling;
                    UpdateButtons();
                    break;
                case "Toggle Solid":
                    block.Solid = !block.Solid;
                    UpdateButtons();
                    break;
            }
            base.PreformAction(engine, ActionName, Arguments);
        }

    }
}
