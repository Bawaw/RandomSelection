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
    class Vegitatie : GameObject
    {
        //instellingen
        const int TIME_TO_CREATE = 6; //om hoeveeltijd zaden lossen (niet static!)

        #region declareren list, field
        static public List<Vegitatie> planten = new List<Vegitatie>();
        private static double CreationTimer;
        #endregion

        public Vegitatie(Vector2 position, Texture2D texture)
            : base(position, texture) 
        {
            CreationTimer = TIME_TO_CREATE;
            planten.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            SpawnTimer(gameTime);
        }

        #region Methodes
        private void SpawnTimer(GameTime gameTime)
        {
            if (CreationTimer >= 0)
                CreationTimer -= gameTime.ElapsedGameTime.TotalSeconds / (planten.Count) ;


            if (CreationTimer < 0)
            {
                CreateNewPlant();
                CreationTimer = TIME_TO_CREATE;
            }
        }

        private void CreateNewPlant()
        { 
            int x = Game1.random.Next(Texture.Width,Game1.SCHERM_BREEDTE - (Texture.Width*2));
            int y = Game1.random.Next(Texture.Width, Game1.SCHERM_HOOGTE - (Texture.Width * 2));
            

            new Vegitatie(new Vector2(x, y), this.Texture);
        }
        public override void Dead() //verwijder object
        {
            planten.Remove(this);
            GameObjecten.Remove(this);
        }
        #endregion 
    }
}
