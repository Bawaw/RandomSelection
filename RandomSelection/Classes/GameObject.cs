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
    class GameObject
    {
        #region declareren fields, props
        static public List<GameObject> GameObjecten = new List<GameObject>();

        public Vector2 position;
        protected Texture2D Texture;
        public Vector2 origin { get { return new Vector2(this.Texture.Width / 2, this.Texture.Height / 2); } }
        public GameObject target;

        #endregion

        protected GameObject(Vector2 position, Texture2D texture)
        {
            GameObjecten.Add(this);
            this.position = position;
            this.Texture = texture;
            this.target = null;
        }

        public virtual void Draw(SpriteBatch spritebatch) 
        {
            spritebatch.Draw(this.Texture,this.position,null,Color.White,0.0f,new Vector2(0,0),1f,SpriteEffects.None,0.0f);
        }
        public virtual void Update(GameTime gameTime) { }

        public virtual void Dead () 
        {
            GameObjecten.Remove(this);
        }
    }
}
