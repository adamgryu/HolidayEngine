using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using HolidayEngine.Level;
using Microsoft.Xna.Framework;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{
    public class LightSettings : Screen
    {
        LevelEditor ParentScreen;
        int PictureTop;
        ScreenInput LightIntensityBox;
        ScreenInput AmbienceBox;

        public LightSettings(Engine engine, LevelEditor ParentScreen)
            : base("Light Settings")
        {
            this.Size = new Vector2(256, 256);
            this.ParentScreen = ParentScreen;

            ParentScreen.RestrictZ = false;
            AddCloseButton(engine);
            ParentScreen.StackBlocks = false;
            ParentScreen.StackButton.Tex = engine.textureManager.Dic["stack_off_button"];
            AddText("Click in 3D space to place sun.", engine.FontMain);
            LightIntensityBox = new ScreenInput(this, engine.FontMain, "IntensityInput");
            AddElement(LightIntensityBox);
            AmbienceBox = new ScreenInput(this, engine.FontMain, "AmbienceInput");
            AddElement(AmbienceBox);
            PictureTop = this.GetTotalElementHeight();
            this.Size = new Vector2(256, PictureTop + 256);

            LightIntensityBox.InputString = engine.primManager.myEffect.LightIntensity.ToString();
            AmbienceBox.InputString = engine.primManager.myEffect.Ambience.ToString();
        }

        public override void Update(Engine engine, bool Selected)
        {
            if (engine.screenManager.IsSelectedScreen(this))
                if (engine.screenManager.NoFocus)
                {
                    if (engine.inputManager.MouseLeftButtonTapped)
                    {
                        engine.primManager.myEffect.LightPosition = ParentScreen.Cursor3D * Block.Size + new Vector3(1, 1, 1) * Block.Size / 2;
                        Vector3 roomCenter = Vector3.UnitX * (engine.room.Width / 2) * Block.Size
                                             + Vector3.UnitZ * (engine.room.Height / 2) * Block.Size
                                             + Vector3.UnitY * (engine.room.Depth / 2) * Block.Size;
                        engine.primManager.myEffect.LightDirection = roomCenter - engine.primManager.myEffect.LightPosition;
                        engine.primManager.myEffect.UpdateShadowMap(engine);
                    }
                }

            base.Update(engine, Selected);
        }

        public override void PreformAction(Engine engine, string ActionName, params string[] Arguments)
        {
            switch (ActionName)
            {
                case "IntensityInput":
                    float _f;
                    if (float.TryParse(LightIntensityBox.InputString, out _f))
                        engine.primManager.myEffect.LightIntensity = _f;
                    LightIntensityBox.InputString = engine.primManager.myEffect.LightIntensity.ToString();
                    break;
                case "AmbienceInput":
                    float _a;
                    if (float.TryParse(AmbienceBox.InputString, out _a))
                        engine.primManager.myEffect.Ambience = _a;
                    AmbienceBox.InputString = engine.primManager.myEffect.Ambience.ToString();
                    break;
                case "Close":
                    ParentScreen.RestrictZ = false;
                    break;
            }

            base.PreformAction(engine, ActionName, Arguments);
        }

        public override void Draw2D(Engine engine, float alpha)
        {
            base.Draw2D(engine, alpha);
            if (!Minimized)
                engine.spriteBatch.Draw(engine.primManager.myEffect.shadowMap, new Rectangle((int)this.Position.X, (int)this.Position.Y + PictureTop, (int)this.Size.X, (int)this.Size.Y - PictureTop), Color.White);
        }
    }
}
