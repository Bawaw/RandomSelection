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
    class Creature : GameObject
    {       
        //instellingen
        private const double ONVRUCHTBAAR_TIJD = 10; //tijd onvruchtbaar na paring
        private const double BEZIG_TIJD = 3; //tijd bezig iets te doen
        private const int GENEOFFSET = 15;  //procent gene afwijking bij geboorte 
        private const int JONGONVRUCHTBAAR = 15; // tijd jong onvruchtbaar bij geboorte

        # region declareren properties,fields
        public static List<Creature> Beesten = new List<Creature>();
        public enum geslacht
        {
            Mannelijk = 1,
            Vrouwelijk = -1,
        }

        public double DNAscore { get; protected set; }

        public double vruchtbaar { get; private set;}
        public double bezig { get; protected set; }

        public float snelheid { get; protected set; }
        public float grootte { get; protected set; }
        public float scale { get; private set; }
        public float nakomelingen { get; protected set; }
        public double hongerniveau { get; protected set; }
        public geslacht sex { get; protected set; }
        public float geRot { get; protected set; }
        

        Color deadColor;

        protected Vector2 versnelling;
        #endregion

        protected Creature(float snelheid,int sex, float grootte, float nakomelingen, Vector2 position, Texture2D texture)
        :base (position, texture)
        {
            this.snelheid = snelheid;
            this.grootte = 0.1f;
            this.scale = grootte;
            this.sex = (geslacht)sex;
            this.nakomelingen = nakomelingen;

            this.vruchtbaar = JONGONVRUCHTBAAR;
            this.bezig = BEZIG_TIJD;
            this.versnelling = new Vector2(0, 0);
            
            this.hongerniveau = 100;
            deadColor = new Color (30,30,30,0);
            geRot = 1.0f;

            Beesten.Add(this);
        }

       


        #region methods

        protected virtual void checkPartner() { }

        protected void move()
        {
            if(!(bezig > 0))
                checkWallCollision();
            this.position += versnelling *snelheid;
        }

        protected void moveRandom()
        { 
            int x = Game1.random.Next(-1000, 1000);
            int y = Game1.random.Next(-1000, 1000);

            this.versnelling = new Vector2(x,y);

            resetPath();
            adjustPath(); 
        }

        private void checkWallCollision()              
        {
            int Xver, Yver;

                if (this.position.X - this.origin.Y * this.grootte < 0)     //linker muur
                {
                    this.position.X = 0 + this.origin.Y * this.grootte;
                    Xver = Game1.random.Next(0, 1000);
                    Yver = Game1.random.Next(-1000, 1000);


                    this.versnelling.X = Xver;
                    this.versnelling.Y = Yver;
                    resetPath();
                    adjustPath();
                }
                else if (this.position.X + this.origin.Y * this.grootte > Game1.SCHERM_BREEDTE)  //rechter muur
                {
                    this.position.X = Game1.SCHERM_BREEDTE - this.origin.Y * this.grootte;

                    Xver = Game1.random.Next(-1000, 1000);
                    Yver = Game1.random.Next(-100, 0);

                    this.versnelling.X = Xver;
                    this.versnelling.Y = Yver;
                    resetPath();
                    adjustPath();
                }

                else if (this.position.Y - this.origin.Y * this.grootte < 0)  //boven muur
                {
                    this.position.Y = 0 + this.origin.Y * this.grootte;

                    Xver = Game1.random.Next(-1000, 1000);
                    Yver = Game1.random.Next(0, 1000);

                    this.versnelling.X = Xver;
                    this.versnelling.Y = Yver;
                    resetPath();
                    adjustPath();
                }

                else if (this.position.Y + this.origin.Y * this.grootte > Game1.SCHERM_HOOGTE) //onder muur
                {
                    this.position.Y = Game1.SCHERM_HOOGTE - this.origin.Y * this.grootte;

                    Xver = Game1.random.Next(-1000, 1000);
                    Yver = Game1.random.Next(-100, 0);

                    this.versnelling.X = Xver;
                    this.versnelling.Y = Yver;
                    resetPath();
                    adjustPath();
                }
        }

        private void resetPath()    // Stop jagen
        {
            if (this.target != null)
            {
                this.target.target = null;
                this.target = null;
            }
        }

        private void adjustPath()
        {
            if (versnelling.Length() > 0.0f)
               this.versnelling.Normalize();
        }

        protected void moveTowards(Vector2 pos2)    //beweeg richting parameter
        {
            Vector2 direction = Vector2.Normalize(pos2 - this.position);
            versnelling = direction * 1.2f;
        }

        protected float RangeCheck(float a, float b)    //berken afstand tussen a en b
        {
            float c = (float)Math.Sqrt(a * a + b * b);

            return c;
        }

        protected void checkPartner(Creature ally, int visionDistance)  
        {
            Vector2 pos = new Vector2();

            if (ally != this && ((int)ally.sex == (int)this.sex * -1) && (ally.vruchtbaar <= 0) )
            {
                float b = (ally.origin.X + ally.position.X) - (this.origin.X + this.position.X);
                float a = (ally.origin.Y + ally.position.Y) - (this.origin.Y + this.position.Y);
                float c = RangeCheck(a, b);

                if (ally.target == null && c < visionDistance* this.grootte)
                {
                        pos = ally.origin + ally.position;
                        moveTowards(pos);
                        pos = this.origin + this.position;
                        ally.moveTowards(pos);

                        this.target = ally;
                        ally.target = this;
                }
                else if (ally.target == this && c < ((this.Texture.Width * this.grootte / 2) + (ally.Texture.Width * ally.grootte / 2)/2))
                {
                    this.position = ally.position;
                    friendlyCollission(ally);
                }
            }
        }

        protected void friendlyCollission(Creature partner) //collision met partner
        {
            partner.versnelling = Vector2.Zero;
            this.versnelling = Vector2.Zero;

            vruchtbaar = ONVRUCHTBAAR_TIJD;
            partner.vruchtbaar = ONVRUCHTBAAR_TIJD;

            this.bezig = BEZIG_TIJD;
            partner.bezig = BEZIG_TIJD;

            creatureCreëren(partner);   //nieuwe beesten maken
        }

        protected void HostileCollission(GameObject prooi) //collision met prooi
        {
            this.versnelling = Vector2.Zero;
            this.bezig = BEZIG_TIJD;
            prooi.Dead();
        }

        protected void timers(GameTime gameTime)
        {
            if(this.vruchtbaar >= 0)
                this.vruchtbaar -= gameTime.ElapsedGameTime.TotalSeconds;

            if (this.bezig >= 0)
                this.bezig -= gameTime.ElapsedGameTime.TotalSeconds;

            if (this.bezig < 0 && (this.versnelling == Vector2.Zero))
                moveRandom();
            if (this.scale > this.grootte)
            {
                if (this.grootte + gameTime.ElapsedGameTime.TotalSeconds / JONGONVRUCHTBAAR > this.scale)
                    this.grootte = this.scale;

                else
                    this.grootte += (float) (gameTime.ElapsedGameTime.TotalSeconds*this.scale) / JONGONVRUCHTBAAR;
            }
            if (this.hongerniveau >= 0)
                this.hongerniveau -= gameTime.ElapsedGameTime.TotalSeconds * this.DNAscore;

            if (hongerniveau < 0)   // dood, rot
            {
                versnelling = Vector2.Zero;
                geRot -= 0.0001f;
            }
                
        }
         
        private void creatureCreëren(Creature ouder2)   //nieuw beest maken
        {
            for (int i = 0; i < (int)((this.nakomelingen + ouder2.nakomelingen) / 2); i++)
            {
                float speed, size, offspring;
                int manOfVrouw;
                int offSet;

                Vector2 pos;
                Texture2D text = this.Texture;

                speed = (this.snelheid + ouder2.snelheid) / 2;
                offSet = Game1.random.Next(-GENEOFFSET, GENEOFFSET);
                speed = speed + (speed / 100) * offSet;

                size = (this.grootte + ouder2.grootte) / 2;
                offSet = Game1.random.Next(-GENEOFFSET, GENEOFFSET);
                size = size + (size / 100) * offSet;

                offspring = (this.nakomelingen + ouder2.nakomelingen) / 2;
                offSet = Game1.random.Next(-GENEOFFSET, GENEOFFSET);
                offspring = offspring + (offspring / 100) * offSet;
     
                pos = new Vector2(this.position.X, this.position.Y);

                manOfVrouw = (int)Game1.random.Next(0, 100);

                if (manOfVrouw < 50)
                    manOfVrouw = 1;
                else
                    manOfVrouw = -1;

                if (this.sex == geslacht.Mannelijk)
                {
                    if (manOfVrouw == 1)
                        text = this.Texture;
                    else
                        text = ouder2.Texture;
                }

                else if (ouder2.sex == geslacht.Mannelijk)
                {
                    if (manOfVrouw == 1)
                        text = ouder2.Texture;

                    else
                        text = this.Texture;
                }

                if (this is Carnivoor)
                {
                       new Carnivoor(speed, manOfVrouw, size, offspring, pos, text); 
                }

                else if (this is Herbivoor)
                {
                       new Herbivoor(speed, manOfVrouw, size, offspring, pos, text);
                }
            }
        }

        protected Color DarkenColor(Color HealthyColor)
        {
            byte r = (byte) MathHelper.Lerp(deadColor.R, HealthyColor.R, geRot);
            byte g = (byte) MathHelper.Lerp(deadColor.G, HealthyColor.G, geRot);
            byte b = (byte) MathHelper.Lerp(deadColor.B, HealthyColor.B, geRot);
            byte a = (byte) MathHelper.Lerp(deadColor.A, HealthyColor.A, geRot);

            return new Color(r, g, b,a);
        }

        public override void Dead() 
        {
            GameObject.GameObjecten.Remove(this);
            Beesten.Remove(this);
        }

        #endregion
 
    }
}



