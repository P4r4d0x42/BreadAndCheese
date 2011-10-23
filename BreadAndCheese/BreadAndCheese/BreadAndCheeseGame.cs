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

        // Game state setup
        /// <summary>
        /// Many games show different displays during their "attract mode." It's easy to get your game to do this by making the program change from one state to another over time. 
        /// The game code can do this by counting the number of times that titleUpdate has been called and then moving to another state when the counter reaches a particular value.
        /// </summary>
        enum GameState
        {
            titleScreen,
            playingGame
        }

        GameState state = GameState.titleScreen;


       // Structure that i can re-use for all my sprites
        struct GameSpriteStruct
        {
            public Texture2D SpriteTexture;
            public Rectangle SpriteRectangle;
            public float X, XSpeed, Y, YSpeed, WidthFactor, TicksToCrossScreen;
            public bool Visible;
        }
        
        // Sprites and other texture setups
        GameSpriteStruct
            frenchBread,
            blueCheese,
            title, 
            background;

        // Texture 2d shit 
        Texture2D
            pimentoPepperTexture;
        GameSpriteStruct[] pimentoPeppers;
        int numberOfPimentoPeppers = 20;
        // Player stuff, should go in it's own Player Class
        // Score
        int score,
            lives,
            highScore;

        // Fonts
        SpriteFont font;


        // Display settings
        float
            displayWidth,
            displayHeight,
            overScanPercentage = 10.0f,
            minDisplayX,
            maxDisplayX,
            minDisplayY,
            maxDisplayY;

        // Extra
        float pimentoPepperHeight, pimentoPepperHeightLimit, pimentoPepperStepFactor;
        


        // Controls
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
            // Initialize anything here that is not game state specific
            displayWidth = GraphicsDevice.Viewport.Width;
            displayHeight = GraphicsDevice.Viewport.Height;
            setupScreen();
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
            // Load textures into sprites
            blueCheese.SpriteTexture = Content.Load<Texture2D>("Images/Blue_Cheese");
            frenchBread.SpriteTexture= Content.Load<Texture2D>("Images/French_Bread");
            // For array use
            pimentoPepperTexture = Content.Load<Texture2D>("Images/Pimento_Pepper");
            // For Score use
            font = Content.Load<SpriteFont>("Fonts/Score");
            loadBackgroundContent();
            loadTitleContent();
            // TODO: Add some mother fucking sound!
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

            switch (state)
            {
                case GameState.titleScreen:
                    updateTitle();  // changes state to playingGame when A pressed
#if testdata                    
                    updateDebugDisplay();
                    Console.WriteLine("Press the ENTER key to Start");
#endif

                    break;
                
                case GameState.playingGame:
                    updateBlueCheese(); // changes state to titleScreen when game ends
                    updateControls();
#if testdata
                     updateDebugDisplay(); // Shows debug data on screen. Comment out testdata at the top of this class to disable this feature
#endif
                     // PermaDeath
                     if (lives <= 0)
                     {
                         gameOver();
                         return; // If this gets called nothing below this in this method will get called.
                     }
                     updateFrenchBread();
                     updatePimentoPepper();
                     break;
            }

            base.Update(gameTime);
        }
        #region Update Methods
        private void updateControls()
        {

            
            
            
            // Keyboard controls as well
            // Moving the Bread
            if (pad1.DPad.Left == ButtonState.Pressed || keys.IsKeyDown(Keys.Left))
            {
                frenchBread.X = frenchBread.X + (frenchBread.XSpeed * -1);
            }
            if (pad1.DPad.Right == ButtonState.Pressed || keys.IsKeyDown(Keys.Right))
            {
                frenchBread.X = frenchBread.X + (frenchBread.XSpeed * 1);
            }
            if (pad1.DPad.Up == ButtonState.Pressed || keys.IsKeyDown(Keys.Up))
            {
                frenchBread.Y = frenchBread.Y + (frenchBread.YSpeed * -1);
            }
            if (pad1.DPad.Down == ButtonState.Pressed || keys.IsKeyDown(Keys.Down))
            {
                frenchBread.Y = frenchBread.Y + (frenchBread.YSpeed * 1);
            }
            if (pad1.IsConnected)
            {
                frenchBread.X = frenchBread.X + (frenchBread.XSpeed * pad1.ThumbSticks.Left.X);
                frenchBread.Y = frenchBread.Y - (frenchBread.YSpeed * pad1.ThumbSticks.Left.Y);
            }


            // Allows the game to exit
            if (pad1.Buttons.Back == ButtonState.Pressed || keys.IsKeyDown(Keys.Escape))
                this.Exit();



            
            
        }
        private void updateBlueCheese()
        {
            // Cheese movement
            blueCheese.X += blueCheese.XSpeed;
            blueCheese.Y += blueCheese.YSpeed;
            blueCheese.SpriteRectangle.X = (int)(blueCheese.X + 0.5f);
            blueCheese.SpriteRectangle.Y = (int)(blueCheese.Y + 0.5f);

            // Bounce sprite if it reaches the edge of the screen
            if (blueCheese.X + blueCheese.SpriteRectangle.Width >= maxDisplayX)
            {
                blueCheese.XSpeed = blueCheese.XSpeed * -1;
            }
            if (blueCheese.X <= minDisplayX)
            {
                blueCheese.XSpeed = blueCheese.XSpeed * -1;
            }

            // Bottom of screen
            if (blueCheese.Y + blueCheese.SpriteRectangle.Height >= maxDisplayY)
            {
                blueCheese.YSpeed = Math.Abs(blueCheese.YSpeed) * -1;
                if (lives > 0)
                {            // TODO: Kill Death
                    lives--; // Death lives here
                }
            }
            // top of screen
            if (blueCheese.Y <= minDisplayY)
            {
                blueCheese.YSpeed = Math.Abs(blueCheese.YSpeed);
            }
        }
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

            // TODO: Shorten the text need to write Console.Writeline
            // TODO: Setup state switching here so it shows data pertinate to what you would want to see for each screen
            Console.Clear();
            // Cheese
            Console.WriteLine("CX: " + blueCheese.X);
            Console.WriteLine("CY: " + blueCheese.Y);
            Console.WriteLine("CXSpeed: " + blueCheese.XSpeed);
            Console.WriteLine("CYSpeed: " + blueCheese.YSpeed);
            // Bread
            Console.WriteLine("BX: " + frenchBread.X);
            Console.WriteLine("BY: " + frenchBread.Y);
            Console.WriteLine("BXSpeed: " + frenchBread.XSpeed);
            Console.WriteLine("BYSpeed: " + frenchBread.YSpeed);
            // Score
            Console.WriteLine("Score: " + score);
            Console.WriteLine("High Score: " + highScore);
           
        }
        private void updateFrenchBread()
        {
            frenchBread.SpriteRectangle.X = (int)frenchBread.X;
            frenchBread.SpriteRectangle.Y = (int)frenchBread.Y;

            // Keeps the Bread in the box
            // Not great but it does work
            // Would like to use the clamp()
            if (frenchBread.X >= maxDisplayX - 80)
            {
                frenchBread.X = frenchBread.X - 6.666667f;
            }
            if (frenchBread.X <= minDisplayX - 40)
            {
                frenchBread.X = frenchBread.X + 6.666667f;
            }
            if (frenchBread.Y >= maxDisplayY - 30)
            {
                frenchBread.Y = frenchBread.Y - 6.666667f;
            }
            if (frenchBread.Y <= minDisplayY - 20)
            {
                frenchBread.Y = frenchBread.Y + 6.666667f;
            }
            // Collision detection  
            if (blueCheese.SpriteRectangle.Intersects(frenchBread.SpriteRectangle))
            {
#if testdata
                Console.WriteLine("Is BlueCheese is collided with FrenchBread");
#endif
                blueCheese.YSpeed = blueCheese.YSpeed * -1;

            }
        }
        private void updatePimentoPepper()
        {
            for (int i = 0; i < numberOfPimentoPeppers; i++)
            {
                pimentoPeppers[i].SpriteRectangle.X = (int)pimentoPeppers[i].X;
                pimentoPeppers[i].SpriteRectangle.Y = (int)pimentoPeppers[i].Y;
            }
            // Sprite removal and very basic "level" progression
            bool noPimentoPeppers = true;
            for (int i = 0; i < numberOfPimentoPeppers; i++)
            {
                if (pimentoPeppers[i].Visible)
                {
                    noPimentoPeppers = false;
                    if (blueCheese.SpriteRectangle.Intersects(pimentoPeppers[i].SpriteRectangle))
                    {
                        pimentoPeppers[i].Visible = false;
                        // Score: Same as score = score + 10;
                        score += 10;
                        blueCheese.YSpeed = blueCheese.YSpeed * -1;
                        break;
                    }
                }

            }
            if (noPimentoPeppers)
            {
                resetPimentoPepperDisplay();
            }
        }
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BurlyWood);

            spriteBatch.Begin();
            switch (state)
            {
                case GameState.titleScreen:
                    drawTitle();
                    break;
                case GameState.playingGame:
                    drawBackground();
                    drawBlueCheese();
                    drawFrenchBread();
                    drawPimentoPeppers();
                    drawScore_Lives();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Draw Methods
        private void drawScore_Lives()
        {
            // Calls the draw text method to draw said text on screen
            // Did not need the ToString() conversion .... not really sure why 
            drawText("Score: " + score, Color.Gold, minDisplayX, maxDisplayY - 40);
            drawText("Lives: " + lives, Color.Gold, maxDisplayX - 105, maxDisplayY - 40);
        }

        private void drawPimentoPeppers()
        {
            // Array for drawing up pimentoPepper
            for (int i = 0; i < numberOfPimentoPeppers; i++)
            {
                if (pimentoPeppers[i].Visible)
                {
                    spriteBatch.Draw(pimentoPeppers[i].SpriteTexture, pimentoPeppers[i].SpriteRectangle, Color.White);
                }
            }
        }

        private void drawFrenchBread()
        {
            spriteBatch.Draw(frenchBread.SpriteTexture, frenchBread.SpriteRectangle, Color.White);
        }

        private void drawBlueCheese()
        {
            spriteBatch.Draw(blueCheese.SpriteTexture, blueCheese.SpriteRectangle, Color.White);
        }

        void drawText(string text, Color textColor, float x, float y)
        {
            int layer;
            Vector2 textVector = new Vector2(x, y);

            // Draw the shadow
            Color backColor = new Color(0, 0, 0, 20);
            for (layer = 0; layer < 5; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X++;
                textVector.Y++;
            }

            // Draw the solid part of the characters
            backColor = new Color(190, 190, 190);
            for (layer = 0; layer < 2; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, textColor);
                textVector.X++;
                textVector.Y++;
            }

            // Draw the top of the characters
            spriteBatch.DrawString(font, text, textVector, textColor);


        }
        #endregion

        #region Game Setup Methods
       /*
        void setupSprites()
        {   //This is how to use references so that methods can change the content of variables passed as parameters
            //setupSprite(ref blueCheese, 0.05f, 200.0f, 200, 100, true);
            //setupSprite(ref frenchBread, 0.15f, 120.0f, (displayWidth / 2) - 75, displayHeight / 1.3f, true);
        }
        */
        /// <summary>
        /// Scaling for different screens, sets initial sprite position and sets movement based on screen size etc.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="widthFactor"></param>
        /// <param name="ticksToCrossScreen"></param>
        /// <param name="initialX"></param>
        /// <param name="initialY"></param>
        /// <param name="initialVisibility">Set item visibility</param>
        void setupSprite(ref GameSpriteStruct sprite, float widthFactor, float ticksToCrossScreen, float initialX, float initialY, bool initialVisibility)
        {
            sprite.WidthFactor = widthFactor;
            sprite.TicksToCrossScreen = ticksToCrossScreen;
            sprite.SpriteRectangle.Width = (int)((displayWidth * widthFactor) + 0.5f);
            float aspectRatio = (float)sprite.SpriteTexture.Width / sprite.SpriteTexture.Height;
            sprite.SpriteRectangle.Height = (int)((sprite.SpriteRectangle.Width / aspectRatio) + 0.5f);
            sprite.X = initialX;
            sprite.Y = initialY;
            sprite.XSpeed = displayWidth / ticksToCrossScreen;
            sprite.YSpeed = sprite.XSpeed;
            sprite.Visible = initialVisibility;
        }
        void setupPimentoPepper()
        {
            pimentoPepperHeight = minDisplayY;
            pimentoPepperHeightLimit = minDisplayY + ((maxDisplayY - minDisplayY) / 2);
            pimentoPeppers = new GameSpriteStruct[numberOfPimentoPeppers];
            float pimentoPepperSpacing = (maxDisplayX - minDisplayX) / numberOfPimentoPeppers;

            for (int i = 0; i < numberOfPimentoPeppers; i++)
            {
                pimentoPeppers[i].SpriteTexture = pimentoPepperTexture;
                setupSprite(
                    ref pimentoPeppers[i],
                    0.05f,  // 20 pimentoPeppers across the screen
                    1000,   // 1000 ticks to move across the screen
                    minDisplayX + (i * pimentoPepperSpacing), minDisplayY,
                    true  // initially visible
                );
            }
        }
        void resetPimentoPepperDisplay()
        {
            pimentoPepperHeight = pimentoPepperHeight + (displayHeight * pimentoPepperStepFactor); // the real issue is the step factor was not set...fixed it

            if (pimentoPepperHeight > pimentoPepperHeightLimit)
            {
                pimentoPepperHeight = minDisplayY;
            }

            for (int i = 0; i < numberOfPimentoPeppers; i++)
            {
                pimentoPeppers[i].Visible = true;
                pimentoPeppers[i].Y = pimentoPepperHeight;
            }
        }
        private void setupScreen()// Display setting 
        {
            displayWidth = graphics.GraphicsDevice.Viewport.Width;
            displayHeight = graphics.GraphicsDevice.Viewport.Height;
            float xOverscanMargin =
                getPercentage(overScanPercentage, displayWidth) / 2.0f;
            float yOverscanMargin =
                getPercentage(overScanPercentage, displayHeight) / 2.0f;


            minDisplayX = xOverscanMargin;
            minDisplayY = yOverscanMargin;

            maxDisplayX = displayWidth - xOverscanMargin;
            maxDisplayY = displayHeight - yOverscanMargin;

        }
        #endregion
        
        #region Background code and data
        private void loadBackgroundContent()
        {
            background.SpriteTexture = Content.Load<Texture2D>("Images/catfish-cuttingboard-detail");

            background.SpriteRectangle =
                new Rectangle(
                    (int)minDisplayX, (int)minDisplayY,
                    (int)(maxDisplayX - minDisplayX),
                    (int)(maxDisplayY - minDisplayY)
                 );
        }
        private void updateBackground()
        {
        }
        private void drawBackground()
        {
            spriteBatch.Draw(background.SpriteTexture,background.SpriteRectangle, Color.White);
        }
        #endregion

        #region Title code and data
        private void loadTitleContent()
        {
            title.SpriteTexture = Content.Load<Texture2D>("Images/Title");

            title.SpriteRectangle =
                new Rectangle(
                    (int)minDisplayX, (int)minDisplayY,
                    (int)(maxDisplayX - minDisplayX),
                    (int)(maxDisplayY - minDisplayY)
                 );
        }
        private void updateTitle()
        {
            if (pad1.Buttons.Start == ButtonState.Pressed || keys.IsKeyDown(Keys.Enter))
            {
                startGame();
            }
        }
        private void drawTitle()
        {
            spriteBatch.Draw(title.SpriteTexture, title.SpriteRectangle, Color.White);
        }
        #endregion

        #region Game state management

        void startGame()
        {
            // Looks like this is sort of taking the place of init. Well at least this segment
            score = 0;
            lives = 3;
            pimentoPepperStepFactor = .10f; // Seems like this needs to be done in percentages and not whole numbers
            startBlueCheese();
            startFrenchBread();
            startPimentoPeppers();
            state = GameState.playingGame;
        }

        private void startBlueCheese()
        {
            setupSprite(ref blueCheese, 0.05f, 200.0f, 200, 100, true);
        }

        private void startPimentoPeppers()
        {
            setupPimentoPepper();
        }
         
        private void startFrenchBread()
        {
            setupSprite(ref frenchBread, 0.15f, 120.0f, (displayWidth / 2) - 75, displayHeight / 1.3f, true);
        }

        void gameOver()
        {
            if (score > highScore)
            {
                highScore = score;
            }
            state = GameState.titleScreen;
        }

        #endregion

        float getPercentage(float percentage, float inputValue)
        {
            return (inputValue * percentage) / 100;
        }
    }
}
