using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using HolidayEngine.Drawing;
using HolidayEngine.Level;
using HolidayEngine.Interface;
using HolidayEngine.Level.Objects;
using HolidayEngine.Level.Objects.PlayerAI;

namespace HolidayEngine
{
    /// <summary>
    /// This is the main type for the game.
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        // ---- GRAPHIC CONTROLLERS ----

        /// <summary>
        /// The graphics device manager for the game.
        /// </summary>
        public GraphicsDeviceManager graphics;

        /// <summary>
        /// The spritebatch for this game.
        /// It is used to draw all 2D sprites.
        /// </summary>
        public SpriteBatch spriteBatch;

        /// <summary>
        /// The primitivate manager for this game controls all 3D drawing.
        /// </summary>
        public PrimManager primManager;


        // ---- CONTENT REFERENCES ----

        /// <summary>
        /// The texture manager holds all the textures for this game.
        /// </summary>
        public TextureManager textureManager = new TextureManager();

        /// <summary>
        /// A dictionary of all blocksets in the game.
        /// </summary>
        public Dictionary<String, Blockset> blockSetList = new Dictionary<string, Blockset>();

        /// <summary>
        /// The standard debug font.
        /// </summary>
        public SpriteFont FontMain;

        /// <summary>
        /// The smaller debug font.
        /// </summary>
        public SpriteFont FontTiny;


        // ---- CURRENT ROOM FUNCTIONALITY ----

        /// <summary>
        /// The current room instance of the game.
        /// </summary>
        public Room room;

        /// <summary>
        /// A list of all the objects in the room.
        /// </summary>
        public CubeManager cubeManager = new CubeManager();

        /// <summary>
        /// The camera instance which controls the viewing information.
        /// </summary>
        public Camera camera = new Camera();


        // ---- DEBUG, CONTROL, AND INTERFACE ----

        /// <summary>
        /// The screen manager controls the screens, making sure they run in the correct order
        /// and disables and enables screens as needed.
        /// </summary>
        public ScreenManager screenManager = new ScreenManager();

        /// <summary>
        /// Controls all input in an easy to use package.
        /// </summary>
        public InputManager inputManager = new InputManager();

        /// <summary>
        /// At launch, if this variable is true it will resize the window as well.
        /// </summary>
        private bool fullscreen = true;

        /// <summary>
        /// The current windowsize.
        /// </summary>
        public Vector2 windowSize = new Vector2(1024, 768);


        /// <summary>
        /// The constructor for the game class.
        /// </summary>
        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = false;
            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Internal initalizations made here.
        /// </summary>
        protected override void Initialize()
        {
            // Sets graphics to fullscreen if necessary.
            this.IsMouseVisible = true;
            if (fullscreen)
            {
                windowSize.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                windowSize.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }
            graphics.PreferredBackBufferWidth = (int)windowSize.X;
            graphics.PreferredBackBufferHeight = (int)windowSize.Y;
            if (fullscreen)
                //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            base.Initialize();
        }


        /// <summary>
        /// Loads content and and game logic.
        /// </summary>
        protected override void LoadContent()
        {
            // The primitive manager and spritebatch is loaded.
            primManager = new PrimManager(this, Content.Load<Effect>("Texts/WorldEffect"));
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loads textures and fonts.
            textureManager.LoadTexturesFromFile("textureinfo", Content);
            FontMain = Content.Load<SpriteFont>("Fonts/FontMain");
            FontTiny = Content.Load<SpriteFont>("Fonts/FontTiny");

            // ---- GAME SPECIFIC INITALIZATIONS ----
            room = new Room(this, "blockset1", "plateau1");
            screenManager.AddScreen(new LevelEditor(this));
            cubeManager.AddCube(new Player(new Vector3(3, 3, 5), new PlayerAI(), textureManager.AniDic["player"]));
            cubeManager.AddCube(new PushCube(new Vector3(4,3,5), textureManager.TilesetList[0].Tiles[1]));

            // Places the light.
            primManager.myEffect.PlaceLightUsingDirection(this, 16);
            primManager.myEffect.UpdateShadowMap(this);
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Updates all the game elements.
        /// It allows quitting, updates all input, updates all cubes,
        /// updates the camera, updates all the screens.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Allows for exiting the game.
            if (inputManager.KeyboardTapped(Keys.Escape))
                this.Exit();

            // Gets input.
            inputManager.Update();

            // Updates all the objects in the room.
            cubeManager.Update(this);

            // Updates camera.
            camera.Update(this);

            // Updates screens.
            screenManager.Update(this);

            // Updates XNA framework.
            base.Update(gameTime);
        }


        /// <summary>
        /// Draws all elements of the game.
        /// First the room, then cube manager.
        /// Lastly, the screens currently open.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            // Draws static room elements.
            room.Draw(this);

            // Draws dynamic elements.
            cubeManager.Draw(this);

            // Draws the screens.
            screenManager.Draw(this);
        
            base.Draw(gameTime);
        }
    }
}
