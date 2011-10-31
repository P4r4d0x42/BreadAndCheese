#define testdata // Using to pull debug and test data out at compile time. Just comment it out when you don't need it
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
using System.Diagnostics;



namespace BreadAndCheese
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BreadAndCheeseGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

       
        
        #region Object Structures
        
        struct BarSpriteStruct
        {
            private Texture2D spriteTexture;
            private Rectangle spriteRectangle;
            private float x;
            private float y;
            private float xSpeed;
            private float ySpeed;

            public void LoadTexture(Texture2D inSpriteTexture)
            {
                spriteTexture = inSpriteTexture;
            }

            public void StartGame(
                float widthFactor,
                float ticksToCrossScreen,
                float inDisplayWidth,
                float initialX,
                float initialY)
            {
                spriteRectangle.Width = (int)((inDisplayWidth * widthFactor) + 0.5f);
                float aspectRatio =
                    (float)spriteTexture.Width / spriteTexture.Height;
                spriteRectangle.Height =
                    (int)((spriteRectangle.Width / aspectRatio) + 0.5f);
                x = initialX;
                y = initialY;
                xSpeed = inDisplayWidth / ticksToCrossScreen;
                ySpeed = xSpeed;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(spriteTexture, spriteRectangle, Color.White);
            }

            public void Update()
            {/*
            // Should have a better way to do this? Maybe not since it's supposed to be completely self contained (encapsulated)
               GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
               KeyboardState keys = Keyboard.GetState();
            // Keyboard controls as well
            // Moving the bar
            // TODO: Figure out how clamp works after converting/using bar.XY as a vector2. Also, this seems like a bad place to put it
            // Keep in mind it's not the center it's measuring from but the top left.
            if (pad1.DPad.Left == ButtonState.Pressed || keys.IsKeyDown(Keys.Left) && spriteRectangle.X > minDisplayX)
            {
                spriteRectangle.X += (spriteRectangle.xSpeed * -1);
            }
            if (pad1.DPad.Right == ButtonState.Pressed || keys.IsKeyDown(Keys.Right) && spriteRectangle.X < (maxDisplayX - spriteRectangle.SpriteRectangle.Width))
            {
                spriteRectangle.X += (spriteRectangle.xSpeed * 1);
            }
            if (pad1.DPad.Up == ButtonState.Pressed || keys.IsKeyDown(Keys.Up) && spriteRectangle.Y > getPercentage(60, maxDisplayY))
            {
                spriteRectangle.Y += (spriteRectangle.ySpeed * -1);
            }
            if (pad1.DPad.Down == ButtonState.Pressed || keys.IsKeyDown(Keys.Down) && spriteRectangle.Y < (maxDisplayY - spriteRectangle.SpriteRectangle.Height))
            {
                spriteRectangle.Y += (spriteRectangle.ySpeed * 1);
            }
            // TODO: Need to update this code so that the bread is bound when you use a controller
            if (pad1.IsConnected)
            {
                x += (xSpeed * pad1.ThumbSticks.Left.X);
                y -= (ySpeed * pad1.ThumbSticks.Left.Y);
            }
             */

                // From Book
                GamePadState gamePad1 = GamePad.GetState(PlayerIndex.One);
                x = x + (xSpeed * gamePad1.ThumbSticks.Left.X);
                y = y - (ySpeed * gamePad1.ThumbSticks.Left.Y);
                spriteRectangle.X = (int)x;
                spriteRectangle.Y = (int)y;
                 
            }   
        }

        #endregion



        // The Game World
        BarSpriteStruct BreadBat;


        #region Display dimension values
        float
            displayWidth,
            displayHeight,
            overScanPercentage = 10.0f,
            minDisplayX,
            maxDisplayX,
            minDisplayY, 
            maxDisplayY;


        private void setScreenSizes()
        {
            displayWidth = graphics.GraphicsDevice.Viewport.Width;
            displayHeight = graphics.GraphicsDevice.Viewport.Height;
            float xOverscanMargin = getPercentage(overScanPercentage, displayWidth) / 2.0f;
            float yOverscanMargin = getPercentage(overScanPercentage, displayHeight) / 2.0f;


            minDisplayX = xOverscanMargin;
            minDisplayY = yOverscanMargin;

            maxDisplayX = displayWidth - xOverscanMargin;
            maxDisplayY = displayHeight - yOverscanMargin;

        }



        #endregion 

        #region Utility methods

        /// <summary>
        /// Calculates percentages
        /// </summary>
        /// <param name="percentage">the percentage to be calculated</param>
        /// <param name="inputValue">the value to be worked on</param>
        /// <returns>the resulting value</returns>
        float getPercentage(float percentage, float inputValue)
        {
            return (inputValue * percentage) / 100;
        }
        #endregion

        #region Update Methods


        private void updateDebugDisplay()
        {
            // Debug Output            
            /*
             *  
             * 1  For console.Write/WriteLine, your app must be a console application. (right-click the project in Solution Explorer, choose Properties, 
             *     and look at the "Output Type" combo in the Application Tab -- should be "Console Application" (note, if you really need a windows application 
             *     or a class library, don't change this to Console App just to get the Console.WriteLine).
             *  
             * 2  You could use System.Diagnostics.Debug.WriteLine to write to the output window (to show the output window in VS, got to View | Output) 
             *     Note that these writes will only occur in a build where the DEBUG conditional is defined (by default, debug builds define this, and release builds do not)
             *  
             * 3  You could use System.Diagnostics.Trace.Writeline if you want to be able to write to configurable "listeners" in non-debug builds. 
             *     (by default, this writes to the Output Window in Visual Studio, just like Debug.Writeline)
            */

            // TODO: Shorten the text need to write Console.Writeline and add COLOUR!!!!!!!
            // TODO: Fix this code to reflect current changes
            Console.Clear();

            //switch (state)
            //{
            //case GameState.titleScreen:
            // Console.WriteLine("High Score: " + highScore);
            //Console.WriteLine("Press the ENTER key to Start");
            //  break;
            //case GameState.playingGame:

            // Cheese
            //Console.WriteLine("CX: " + blueCheese.X);
            //Console.WriteLine("CY: " + blueCheese.Y);
            //Console.WriteLine("CXSpeed: " + blueCheese.XSpeed);
            //Console.WriteLine("CYSpeed: " + blueCheese.YSpeed);
            // Bread
            //Console.WriteLine("BX: " + frenchBread.X);
            //Console.WriteLine("BY: " + frenchBread.Y);
            //Console.WriteLine("BXSpeed: " + frenchBread.XSpeed);
            //Console.WriteLine("BYSpeed: " + frenchBread.YSpeed);
            // Score
            //Console.WriteLine("Score: " + score);
            //Console.WriteLine("High Score: " + highScore);
            //break;
        }


        #endregion


        // Controls setup
       public KeyboardState keys;
       public GamePadState pad1;
        
        
        // Constructor
        public BreadAndCheeseGame()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // Initialize anything here that is not specific to a game state or specific object
            
            setScreenSizes();
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
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pad1 = GamePad.GetState(PlayerIndex.One);
            keys = Keyboard.GetState();


            // Allows the game to exit
            if (pad1.Buttons.Back == ButtonState.Pressed || keys.IsKeyDown(Keys.Escape))
                this.Exit();



            // Does what it says it does
            updateDebugDisplay();



            base.Update(gameTime);
        }
        
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BurlyWood);

            spriteBatch.Begin();
           

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
