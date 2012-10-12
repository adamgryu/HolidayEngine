using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Interface.ScreenElements;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Interface
{
    public class RoomSettings : Screen
    {
        Vector3 NewSize;
        Vector3 NewShift = Vector3.Zero;

        ScreenText DisplayText;

        public RoomSettings(Engine engine)
            : base("Room Settings")
        {
            NewSize = new Vector3(engine.room.Width, engine.room.Depth, engine.room.Height);

            AddCloseButton(engine);

            AddText("Use these buttons to resize the map. Left click extends a border, right click retracts the border.", engine.FontMain);
            DisplayText = new ScreenText(this, "#", engine.FontMain);
            UpdateText();
            AddElement(DisplayText);
            AddElement(new ScreenButton(this, "Update Room", engine.FontMain));


            Vector2 _tl = new Vector2(Screen.boarderSize, GetTotalElementHeight());
            Vector2 _sz = new Vector2(32, 32);
            Vector2 _left = new Vector2(32, 0);
            Vector2 _down = new Vector2(0, 32);
            ScreenPictureButton BTop = new ScreenPictureButton(this, "Up Border", engine.textureManager.Dic["uparrow"], _tl + _left, _sz);
            ScreenPictureButton BLeft = new ScreenPictureButton(this, "Left Border", engine.textureManager.Dic["leftarrow"], _tl + _down, _sz);
            ScreenPictureButton BRight = new ScreenPictureButton(this, "Right Border", engine.textureManager.Dic["rightarrow"], _tl + _down + _left * 2, _sz);
            ScreenPictureButton BFront = new ScreenPictureButton(this, "Down Border", engine.textureManager.Dic["downarrow"], _tl + _down + _left, _sz);
            this.AddElement(BTop);
            this.AddElement(BLeft);
            this.AddElement(BRight);
            this.AddElement(BFront);
            ScreenPictureButton BBack = new ScreenPictureButton(this, "ZUp", engine.textureManager.Dic["uparrow"], _tl + _left * 4, _sz);
            ScreenPictureButton BBottom = new ScreenPictureButton(this, "ZDown", engine.textureManager.Dic["downarrow"], _tl + _down + _left * 4, _sz);
            this.AddElement(BBack);
            this.AddElement(BBottom);

            AddCloseButton(engine);
        }

        public void UpdateText()
        {
            DisplayText.UpdateText("Size: " + NewSize.ToString() + "#Shift: " + NewShift.ToString());
        }

        public override void PreformAction(Engine engine, string ActionName, params string[] Arguments)
        {
            switch (ActionName)
            {
                case "Update Room":
                    engine.room.RebuildArray(engine, NewSize, NewShift);
                    engine.room.UpdateRoomVertices();
                    UpdateText();
                    NewShift = Vector3.Zero;
                    break;
                case "Up Border":
                    NewSize.Y++;
                    UpdateText();
                    break;
                case "Down Border":
                    NewSize.Y++;
                    NewShift.Y++;
                    UpdateText();
                    break;
                case "Right Border":
                    NewSize.X++;
                    UpdateText();
                    break;
                case "Left Border":
                    NewSize.X++;
                    NewShift.X++;
                    UpdateText();
                    break;
                case "ZUp":
                    NewSize.Z++;
                    UpdateText();
                    break;
                case "ZDown":
                    NewSize.Z++;
                    NewShift.Z++;
                    UpdateText();
                    break;
                case "#Up Border":
                    NewSize.Y--;
                    UpdateText();
                    break;
                case "#Down Border":
                    NewSize.Y--;
                    NewShift.Y--;
                    UpdateText();
                    break;
                case "#Right Border":
                    NewSize.X--;
                    UpdateText();
                    break;
                case "#Left Border":
                    NewSize.X--;
                    NewShift.X--;
                    UpdateText();
                    break;
                case "#ZUp":
                    NewSize.Z--;
                    UpdateText();
                    break;
                case "#ZDown":
                    NewSize.Z--;
                    NewShift.Z--;
                    UpdateText();
                    break;
            }

            base.PreformAction(engine, ActionName, Arguments);
        }
    }
}
