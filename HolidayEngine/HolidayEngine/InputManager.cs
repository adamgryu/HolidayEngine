using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace HolidayEngine
{
    public class InputManager
    {
        public MouseState mouse;
        private MouseState mousePrev;
        public KeyboardState keyboard;
        public KeyboardState keyboardPrev;

        public static Keys KeyForward = Keys.W;
        public static Keys KeyBackward = Keys.S;
        public static Keys KeyLeft = Keys.A;
        public static Keys KeyRight = Keys.D;
        public static Keys KeyFall = Keys.Space;

        public InputManager()
        {
            mouse = Mouse.GetState();
            mousePrev = Mouse.GetState();
            keyboard = Keyboard.GetState();
            keyboardPrev = Keyboard.GetState();
        }

        public void Update()
        {
            mousePrev = mouse;
            keyboardPrev = keyboard;
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
        }

        public bool KeyboardTapped(Keys key)
        {
            return (keyboardPrev.IsKeyDown(key) && keyboard.IsKeyUp(key));
        }

        public bool MouseScrollDown
        {
            get { return (mouse.ScrollWheelValue < mousePrev.ScrollWheelValue); }
        }

        public bool MouseScrollUp
        {
            get { return (mouse.ScrollWheelValue > mousePrev.ScrollWheelValue); }
        }

        public bool MouseLeftButtonTapped
        {
            get { return (mouse.LeftButton == ButtonState.Pressed && mousePrev.LeftButton == ButtonState.Released); }
        }

        public bool MouseRightButtonTapped
        {
            get { return (mouse.RightButton == ButtonState.Pressed && mousePrev.RightButton == ButtonState.Released); }
        }

        public Vector2 MousePosition
        {
            get { return new Vector2(mouse.X, mouse.Y); }
        }

        public Vector2 MousePrevPosition
        {
            get { return new Vector2(mousePrev.X, mousePrev.Y); }
        }
    }
}
