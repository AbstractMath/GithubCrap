using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngineProjectRevive.Objects;
using GameEngineProjectRevive.Physics;
using System;
using System.Collections.Generic;
using GameEngineProjectRevive.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace GameEngineProjectRevive
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWindow : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tileSet;
        Scene testScene;
        SpriteLayer layer1;
        SpriteLayer layer2;
        TileMap testMap;
        TileMap testMap1;
        Camera activeCamera;
        public static Texture2D Line;
        MouseState state;

        //Test of physics objects
        PolygonCollider testPoly;
        PolygonCollider testPoly2;

        AlignedBoundingBox b1;
        AlignedBoundingBox b2;
        Quadtree physicsTree;

        StaticObject obj1;
        StaticObject obj2;

        GameObject container1;
        GameObject container2;

        TileCollider TestCollider;

        Vector2 tp = new Vector2(100, 20);
        Vector2 tp2 = new Vector2(0, 0);

        AudioManager audioManager;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 2 * 480;
            graphics.PreferredBackBufferWidth = 2 * 1024;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            activeCamera = new Camera();

            audioManager = new AudioManager();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //testImage = Content.Load<Texture2D>("Stupid");

            testScene = new Scene(GraphicsDevice, spriteBatch);
            layer1 = new SpriteLayer();
            layer2 = new SpriteLayer();

            Line = Content.Load<Texture2D>("Line");
            testMap = new TileMap("TileMaps/Tramtunnel1.tmx", 1, Content);
            testMap1 = new TileMap("TileMaps/Tramtunnel1.tmx", 3, Content);

            testMap1.Parent = layer2;
            testMap.Parent = layer1;
            testMap.Scale = new Vector2(1, 1);
            testMap1.Scale = new Vector2(1, 1);

            testScene.Layers.AddLast(layer1);
            testScene.Layers.AddLast(layer2);

            Polygon testPolygon = new Polygon();
            testPolygon.addPoint(new Vector2(0, 100));
            testPolygon.addPoint(new Vector2(-50, -2));
            testPolygon.addPoint(new Vector2(50, -2));

            Polygon testPolygon2 = new Polygon();
            testPolygon2.addPoint(new Vector2(-40, -20));
            testPolygon2.addPoint(new Vector2(30, -10));
            testPolygon2.addPoint(new Vector2(30, 50));
            testPolygon2.addPoint(new Vector2(-40, 35));

            testPolygon.updateBounds();
            testPolygon2.updateBounds();

            testPoly = new PolygonCollider(testPolygon);
            testPoly2 = new PolygonCollider(testPolygon2);

            container1 = new GameObject();
            container2 = new GameObject();

            obj1 = new StaticObject(testPoly, container1);
            b1 = obj1.BoundingShape;

            obj2 = new StaticObject(testPoly2, container2);
            b2 = obj2.BoundingShape;

            physicsTree = new Quadtree(Vector2.Zero, 1000, 7);

            physicsTree.InsertBound(b2);
            physicsTree.InsertBound(b1);

            container1.Collider = obj1;
            container2.Collider = obj2;

            TestCollider = new TileCollider();
            TestCollider.Map = testMap;
            TestCollider.TileSize = testMap.TileSets[0].TileHeight;

            audioManager.LoadContent(Content);
            SoundEffect windHowl = audioManager.LoadSoundEffect("wind_howl_1", Content);
            SoundEffectInstance windInstance = audioManager.MakeEmitter(windHowl, new Vector3(699f,312f,0f));
            windInstance.IsLooped = true;
            windInstance.Play();
            //test spatial audio emmiters
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public static void DrawLine(Vector2 a, Vector2 b, Color c, Camera cam, SpriteBatch batch)
        {
            float length = (a - b).Length();
            float angle = (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
            batch.Draw(Line, new Rectangle((int)(a.X - cam.Translation.X), (int)(a.Y - cam.Translation.Y), (int)length, 1), null, c, angle, Vector2.Zero, SpriteEffects.None, 1);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                activeCamera.Translation += new Vector2(2, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                activeCamera.Translation -= new Vector2(2, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                activeCamera.Translation -= new Vector2(0, 2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                activeCamera.Translation += new Vector2(0, 2);
            }
            state = Mouse.GetState();

            // Audio Test
            if (GetSingleKeyboardPress(Keys.K))
            {
                audioManager.PlaySystemSound(SystemSound.OK);
            }

            if (GetSingleKeyboardPress(Keys.L))
            {
                audioManager.PlaySystemSound(SystemSound.CANCEL);
            }

            // Spatial audio test
            if (GetSingleKeyboardPress(Keys.N))
            {
                Vector3 currentListenerPosition = audioManager.GetListenerPosition();
                audioManager.SetListenerPosition(new Vector2(currentListenerPosition.X - 0.01f, currentListenerPosition.Y));
            }

            if (GetSingleKeyboardPress(Keys.M))
            {
                Vector3 currentListenerPosition = audioManager.GetListenerPosition();
                audioManager.SetListenerPosition(new Vector2(currentListenerPosition.X + 0.01f, currentListenerPosition.Y));
            }

            tp2 = new Vector2(state.Position.X, state.Position.Y) + activeCamera.GlobalTranslation;
            container2.Translation = tp2;
            Debug.WriteLine ("cursor at: " + tp2.X + ", " + tp2.Y);

            audioManager.SetListenerPosition(tp2);
            // TODO: Add your update logic here


            OldState = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            testScene.Render(activeCamera);

            //physicsTree.Draw(Color.Blue, spriteBatch, activeCamera);
            LinkedList<Tuple<PhysicsObject, Vector2>> Colliding = obj1.getColliding();
            //Tuple<bool, Vector2> CollidingTile = TestCollider.CheckForCollision(obj2.Collider, testMap.Translation, obj2.Position, new Vector2(1, 1), new Vector2(1, 1));
            Tuple<bool, Vector2> CollidingTile = obj2.Collider.CheckForCollision(TestCollider, obj2.Position, testMap.Translation, new Vector2(1, 1), new Vector2(1, 1));

            Color drawColor = CollidingTile.Item1 ? Color.Red : Color.Black;//Colliding.Count != 0 ? Color.Red : Color.Black;

            //b2.Draw(drawColor, spriteBatch, activeCamera);
            //b1.Draw(drawColor, spriteBatch, activeCamera);
            /*spriteBatch.Begin();
            GameWindow.DrawLine(TestCollider.Corner0, TestCollider.Corner1, Color.Red, activeCamera, spriteBatch);
            spriteBatch.End();*/
            //The upper three lines are for debugging tile collisions

            obj2.BoundingShape.Draw(Color.Wheat, spriteBatch, activeCamera);
            testPoly.Draw(Color.White, b1.RelativeColliderPosition + b1.Position, spriteBatch, activeCamera, new Vector2(1, 1));
            testPoly2.Draw(drawColor, b2.RelativeColliderPosition + b2.Position, spriteBatch, activeCamera, new Vector2(1, 1));

            //GameWindow.DrawLine(new Vector2(40, 70), new Vector2(1000, 2000), Color.White, activeCamera, spriteBatch);

            base.Draw(gameTime);
        }

        KeyboardState OldState;

        private bool GetSingleKeyboardPress (Keys key) {

            KeyboardState newState = Keyboard.GetState();
            bool result = false;

            if(newState.IsKeyDown(key) && !OldState.IsKeyDown (key)) {
                result =  true;
            } 

            return result;
        }
    }
}
