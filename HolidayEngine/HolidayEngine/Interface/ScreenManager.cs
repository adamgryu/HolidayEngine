using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Interface
{
    public class ScreenManager
    {
        public List<Screen> screenStack = new List<Screen>();
        List<Screen> screenRemoveBuffer = new List<Screen>();
        List<Screen> screenAddBuffer = new List<Screen>();

        public bool NoFocus = false;

        public ScreenManager()
        {
            // Do nothing.
        }

        public bool IsSelectedScreen(Screen screen)
        {
            return screen == screenStack[0];
        }

        public void Update(Engine engine)
        {
            // Updates any screens that need to be updated.
            if (screenStack.Count > 0)
            {
                // If this is triggered, then a new screen is selected this step.
                bool _newScreen = false;

                // Chooses which parts should be selected.
                if (engine.inputManager.MouseLeftButtonTapped)
                {
                    if (!screenStack[0].DemandPriority)
                    {
                        Screen _selectedScreen = null;
                        foreach (Screen screen in screenStack)
                        {
                            float _length = screen.Size.Y;
                            if (screen.Minimized)
                                _length = Screen.headerSize;

                            if (engine.inputManager.mouse.X > screen.Position.X && engine.inputManager.mouse.X < screen.Position.X + screen.Size.X
                                && engine.inputManager.mouse.Y > screen.Position.Y && engine.inputManager.mouse.Y < screen.Position.Y + _length)
                            {
                                _selectedScreen = screen;
                                break;
                            }
                        }
                        NoFocus = (_selectedScreen == null);
                        if (!NoFocus)
                        {
                            _newScreen = (screenStack[0] != _selectedScreen);
                            int _idx = screenStack.FindIndex(_selectedScreen.Equals);
                            screenStack[_idx] = screenStack[0];
                            screenStack[0] = _selectedScreen;
                        }

                    }
                    else
                        NoFocus = false;
                }

                // It will not update a screen if a new one is selected in this step.
                if (!_newScreen)
                {
                    // Updates first screen on the list
                    screenStack[0].Update(engine, true);

                    // Updates the other screens if necessary.
                    if (!screenStack[0].DemandPriority)
                    {
                        for (int i = 1; i < screenStack.Count; i++)
                        {
                            screenStack[i].Update(engine, false);
                        }
                    }
                }

                // Updates the remove list buffer.
                foreach (Screen screen in screenRemoveBuffer)
                {
                    screenStack.Remove(screen);
                }
                screenRemoveBuffer.Clear();
            }

            // Updates the add list buffer.
            foreach (Screen screen in screenAddBuffer)
            {
                screenStack.Insert(0, screen);
            }
            screenAddBuffer.Clear();
        }

        /// <summary>
        /// Draws all the screens.
        /// This will set a spriteBatch and use orthographic settings during the function.
        /// The mode should be in standard 3D when called, and is returned to that
        /// state when finished.
        /// </summary>
        public void Draw(Engine engine)
        {
            // Draws all the 3D portions of the windows.
            if (screenStack.Count > 0)
            {
                screenStack[0].Draw3D(engine);
                if (!screenStack[0].DemandPriority)
                    for (int i = 1; i < screenStack.Count; i++)
                        screenStack[i].Draw3D(engine);
            }

            // Draws all 2D portions of windows.
            engine.spriteBatch.Begin();

            if (screenStack.Count > 0)
            {
                float _alpha = 0.5f;
                if (screenStack[0].DemandPriority)
                    _alpha = 0.25f;

                for (int i = screenStack.Count-1; i > 0; i--)
                    screenStack[i].Draw2D(engine, _alpha);

                screenStack[0].Draw2D(engine, 1);
            }

            engine.spriteBatch.End();

            // Draws the 3D orthographic portions of windows.
            engine.primManager.Reset3D(engine);

            if (screenStack.Count > 0)
            {
                screenStack[0].Draw3DOrtho(engine);
                if (!screenStack[0].DemandPriority)
                    for (int i = 1; i < screenStack.Count; i++)
                        screenStack[i].Draw3DOrtho(engine);
            }
        }


        public void AddScreen(Screen screen)
        {
            screenAddBuffer.Add(screen);
        }


        public void RemoveScreen(Screen screen)
        {
            screenRemoveBuffer.Add(screen);
        }
    }
}
