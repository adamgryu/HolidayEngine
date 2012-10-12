using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{
    public class Screen
    {
        /// <summary>
        /// Pixel height of window header.
        /// </summary>
        public const int headerSize = 24;

        /// <summary>
        /// The boarder width to use for windows.
        /// </summary>
        public const int boarderSize = 8;

        /// <summary>
        /// The name of the window that will appear in the header.
        /// </summary>
        public String Name;

        /// <summary>
        /// The vector position of the top-left of the window.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The vector size of the window.
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// If the window is minimized or not.
        /// </summary>
        public bool Minimized = false;

        /// <summary>
        /// If the window pauses all other windows while active.
        /// </summary>
        public bool DemandPriority = false;


        /// <summary>
        /// A list of all special window elements in the window.
        /// </summary>
        public List<ScreenElement> ElementList = new List<ScreenElement>();


        /// <summary>
        /// Tracks the starting point of when the window is grabbed.
        /// </summary>
        Vector2 GrabbedPos;

        /// <summary>
        /// If the window grabbed or not.
        /// </summary>
        bool Grabbed = false;




        /// <summary>
        /// Constructs a default screen.
        /// </summary>
        /// <param name="Name"></param>
        public Screen(String Name)
        {
            this.Name = Name;
            this.Position = new Vector2(32f, 32f);
            this.Size = new Vector2(256, 128);
        }


        /// <summary>
        /// Preforms the default updating of a window.
        /// Allows it to be grabbed and minimized.
        /// </summary>
        public virtual void Update(Engine engine, bool Selected)
        {
            if (Selected)
            {
                // Allows the user to drag the window around by its header.
                if (engine.inputManager.mouse.X > Position.X && engine.inputManager.mouse.X < Position.X + Size.X
                        && engine.inputManager.mouse.Y > Position.Y && engine.inputManager.mouse.Y < Position.Y + headerSize)
                {
                    if (engine.inputManager.MouseLeftButtonTapped)
                    {
                        GrabbedPos = engine.inputManager.MousePosition;
                        Grabbed = true;
                    }
                }
                if (Grabbed)
                {
                    this.Position += engine.inputManager.MousePosition - engine.inputManager.MousePrevPosition;
                    if (this.Position.X + this.Size.X / 2 < 0)
                        this.Position.X = -this.Size.X / 2;
                    if (this.Position.Y < 0)
                        this.Position.Y = 0;
                    if (this.Position.X + this.Size.X / 2 > engine.windowSize.X)
                        this.Position.X = engine.windowSize.X - this.Size.X / 2;
                    if (this.Position.Y + headerSize > engine.windowSize.Y)
                        this.Position.Y = engine.windowSize.Y - headerSize;
                    if (engine.inputManager.mouse.LeftButton == ButtonState.Released)
                    {
                        Grabbed = false;
                        if (GrabbedPos == engine.inputManager.MousePosition)
                            Minimized = !Minimized;
                    }
                }

                if (!Minimized)
                {
                    // Updates all the elements.
                    foreach (ScreenElement element in ElementList)
                        element.Update(engine);
                }
            }
        }


        /// <summary>
        /// Preforms an action based on the current override for the window.
        /// </summary>
        /// <param name="engine">The current game engine state.</param>
        /// <param name="ActionName">The action to preform.</param>
        public virtual void PreformAction(Engine engine, String ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Close":
                    engine.screenManager.RemoveScreen(this);
                    break;
            }
        }


        /// <summary>
        /// Centers the window in the screen.
        /// </summary>
        public void Center(Engine engine)
        {
            this.Position = new Vector2((int)(engine.windowSize.X / 2 - this.Size.X / 2), (int)(engine.windowSize.Y / 2 - this.Size.Y / 2));
        }


        /// <summary>
        /// Adds a screen-element to the window and resizes the window if necessary.
        /// </summary>
        public void AddElement(ScreenElement element)
        {
            ElementList.Add(element);
            int _height = GetTotalElementHeight();
            if (_height > this.Size.Y)
                this.Size.Y = _height;
        }


        /// <summary>
        /// Adds a close button to the window.
        /// </summary>
        public void AddCloseButton(Engine engine)
        {
            ScreenButton _button = new ScreenButton(this, "Close", "X", engine.FontMain);
            _button.Position = new Vector2(this.Size.X - headerSize, 0);
            _button.Size = new Vector2(headerSize, headerSize);
            ElementList.Add(_button);
        }


        /// <summary>
        /// Add text to the window.
        /// </summary>
        /// <param name="Text">The text to add.</param>
        /// <param name="Font">The font to display the text in.</param>
        public void AddText(String Text, SpriteFont Font)
        {
            AddElement(new ScreenText(this, Text, Font));
        }
        

        /// <summary>
        /// Gets the total height of all the elements in the window.
        /// </summary>
        public virtual int GetTotalElementHeight()
        {
            int _ret = headerSize + boarderSize;
            foreach (ScreenElement element in ElementList)
                if (element.Position.Y + element.Size.Y + boarderSize > _ret)
                    _ret = (int)element.Position.Y + (int)element.Size.Y + boarderSize;
            return _ret;
        }


        /// <summary>
        /// Any 3D drawing that must be done in the world.
        /// </summary>
        public virtual void Draw3D(Engine engine)
        {
            // Do nothing.
        }


        /// <summary>
        /// Draws the 2D portions of the window.
        /// </summary>
        /// <param name="engine">The current engine state.</param>
        /// <param name="alpha">The alpha for which to draw the window at.</param>
        public virtual void Draw2D(Engine engine, float alpha)
        {
            Texture2D _blank = engine.textureManager.Dic["blank"].TextureMain;
            if (!Minimized)
                engine.spriteBatch.Draw(_blank, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.Black * alpha * 0.8f);

            engine.spriteBatch.Draw(_blank, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, headerSize), Color.Black * alpha);
            engine.spriteBatch.DrawString(engine.FontMain, this.Name, Position + new Vector2(4, 4), Color.White * alpha);

            if (!Minimized)
                foreach (ScreenElement element in ElementList)
                    element.Draw(engine, alpha);
        }


        /// <summary>
        /// Any 3D drawing to be done ontop of the windows.
        /// </summary>
        public virtual void Draw3DOrtho(Engine engine)
        {
            // Do nothing.
        }
    }
}
