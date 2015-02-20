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

namespace RandomSelection
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        //instellingen Scherm
        public const int SCHERM_BREEDTE = 1300;
        public const int SCHERM_HOOGTE = 700;

        public static Random random = new Random();
        private static int round;

        #region Declare props,enums,fields
        enum GameState
        {
            Title,
            Round,
            Playing
        }


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont MainFont;

        private Texture2D carnivoorM;
        private Texture2D herbivoorM;
        private Texture2D carnivoorV;
        private Texture2D herbivoorV;
        private Texture2D Plant;
        private Texture2D BackGround;
        private Texture2D Round;
        private GameState gameState;
        private float counter;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Initialiseren Scherm Breedte en hoogte
            graphics.PreferredBackBufferHeight = SCHERM_HOOGTE;
            graphics.PreferredBackBufferWidth = SCHERM_BREEDTE;

            round = 1;
            counter = 5;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            gameState = GameState.Title;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //Initializseren Tetures
            carnivoorM = Content.Load<Texture2D>(@"Textures\carnivoorM");
            herbivoorM = Content.Load<Texture2D>(@"Textures\herbivoorM");
            carnivoorV = Content.Load<Texture2D>(@"Textures\carnivoorV");
            herbivoorV = Content.Load<Texture2D>(@"Textures\herbivoorV");
            Plant = Content.Load<Texture2D>(@"Textures\Plant");
            BackGround = Content.Load<Texture2D>(@"Textures\Start");
            Round = Content.Load<Texture2D>(@"Textures\Round");
            MainFont = Content.Load<SpriteFont>(@"Textures\MainFont");

            //maken eerste objecten
            new Vegitatie(new Vector2 (600,350) , Plant);
            
            new Carnivoor(1,-1, 1.0f, 1.0f, new Vector2(100,300), carnivoorV);
            new Carnivoor(1, 1, 1.0f, 1.0f, new Vector2(100,400), carnivoorM);

            new Herbivoor(1, -1, 1.0f, 1.5f, new Vector2(1200, 400), herbivoorV);
            new Herbivoor(1, -1, 1.0f, 1.5f, new Vector2(1200, 350), herbivoorV);
            new Herbivoor(1, 1, 1.0f, 1.5f, new Vector2(1200, 300), herbivoorM);

            
            //new Herbivoor(1, 1, 1.0f, 1.5f, new Vector2(1000, 350), herbivoorM);      
            
            
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
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (gameState == GameState.Title)       //Titel Scherm
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                    gameState = GameState.Round;
            }

            else if (gameState == GameState.Round)
            {
                counter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (counter <= 0) 
                    gameState = GameState.Playing;
            }


            else if (gameState == GameState.Playing)    //simulatie draait
            {
                for (int i = 0; i < GameObject.GameObjecten.Count; i++)
                {
                    GameObject.GameObjecten[i].Update(gameTime);
                }
                
                if (Creature.Beesten.Count <= 0 || Creature.Beesten.Count > 100)
                {
                    gameState = GameState.Round;
                    int amount = Creature.Beesten.Count;

                    for (int i = amount-1; i != 0; i--)
                    {
                        Creature.Beesten[i].Dead();
                    }
                    
                    addRandomCreature(1, 2);
                    addRandomCreature(0, 3);
                    round++;
                    counter = 5;
                }
                
                else if (Carnivoor.Carnivoren.Count <= 0)
                {
                    gameState = GameState.Round;

                    int amount = Herbivoor.Herbivoren.Count;
                    if (amount > 2)
                    {
                        for (int i = amount - 1; i != 2; i--)
                        {
                            Herbivoor.Herbivoren[i].Dead();
                        }
                    }
                    else
                    {
                        amount = 3 - Herbivoor.Herbivoren.Count;
                        addRandomCreature(0, amount);
                    }

                    addRandomCreature(1,2);

                    round++;
                    counter = 5;
                }
                
                else if (Herbivoor.Herbivoren.Count <= 0)
                {
                    gameState = GameState.Round;

                    int amount = Carnivoor.Carnivoren.Count;
                    if (amount > 1)
                    {
                        for (int i = amount - 1; i != 1; i--)
                        {
                            Carnivoor.Carnivoren[i].Dead();
                        }
                    }
                    else
                    {
                        amount = 2 - Carnivoor.Carnivoren.Count;
                        addRandomCreature(1, amount);
                    }

                    addRandomCreature(0, 3);
                    round++;
                    counter = 5;
                }
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            // 0.1f
            if (gameState == GameState.Title) //Titel Scherm 
            {
                spriteBatch.Draw(BackGround, new Vector2(0, 0), Color.White);
                GraphicsDevice.Clear(Color.White);
            }


            if (gameState == GameState.Round)
            {
                //spriteBatch.Draw(Round, new Vector2(0, 0), Color.White);
                
                GraphicsDevice.Clear(Color.White);

                spriteBatch.DrawString(MainFont, "Round: " + round, new Vector2(100, 100), Color.Black);
            }


            if (gameState == GameState.Playing) //Simulatie draait
            {
                GraphicsDevice.Clear(Color.LawnGreen);
                foreach (GameObject gameObject in GameObject.GameObjecten)
                {
                      gameObject.Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        private void addRandomCreature(byte Ind,int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                float speed, size, offspring;
                int manOfVrouw;
                Texture2D text;

                Vector2 pos;

                speed = Game1.random.Next(0,80000)/10000;

                size = Game1.random.Next(10000,30000)/10000;

                offspring = Game1.random.Next(0,80000)/10000;

                manOfVrouw = (int)Game1.random.Next(0, 100);


                if (manOfVrouw < 50)
                    manOfVrouw = 1;
                else
                    manOfVrouw = -1;

                if (Ind == 0) // herbivoor
                {
                    if (manOfVrouw == 1)
                        text = herbivoorM;
                    else
                        text = herbivoorV;

                    pos = new Vector2(SCHERM_BREEDTE - 400, SCHERM_HOOGTE - 500 / amount * (i + 1));
                    new Herbivoor(speed, manOfVrouw, size, offspring, pos, text);
                }
                else //carnivoor
                {
                    if (manOfVrouw == 1)
                        text = carnivoorM;
                    else
                        text = carnivoorV;
                    pos = new Vector2(200, SCHERM_HOOGTE - 500 / amount * (i + 1));
                    new Carnivoor(speed, manOfVrouw, size, offspring, pos, text);
                }
            }
          }
    }
}
