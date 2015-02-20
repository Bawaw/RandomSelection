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
    class Carnivoor : Creature
    {
        // Instellingen
        private const int VISION_DISTANCE = 100;  //afstand van waar een Carnivoor andere objecten kan vinden
        private const float DNA_OFFSET = 0.10f; //Percent verschil voor DNA voor nieuwe soort
        private const float ENERGY_GAIN = 40f;  //energie percent bij gekregen per eet beurt

        #region declareren/initialiseren props,fields,list

        static public List<Carnivoor> Carnivoren = new List<Carnivoor>();
        

        Color color;

        public static int AantalSoorten = 0;
        public int soort { get; private set; }

        #endregion

        public Carnivoor(float snelheid, int sex, float grootte, float nakomelingen,Vector2 position,Texture2D texture)
            : base(snelheid, sex, grootte, nakomelingen,position, texture)
        {
            DNAscore = this.snelheid*0.45 + this.grootte*0.55 + this.nakomelingen*0.1; 
            Carnivoren.Add(this);
            calculateSoort();
        }

        public override void Draw (SpriteBatch spritebatch) // tekenen van carnivoor
        {
            float angleInRadians = (float)Math.Atan2(versnelling.X, versnelling.Y); // Berekenen loop hoek
            spritebatch.Draw(this.Texture, position, null, this.color, angleInRadians * -1, origin , this.grootte, SpriteEffects.None,1.0f);
        }
        public override void Update(GameTime gameTime)
        {
            if (this.hongerniveau > 0)
            {
                if (this.hongerniveau > 60 || this.target is Carnivoor)   //paren
                    checkPartner();

                 if (hongerniveau < 60 || this.target is Herbivoor)  // eten 
                    checkFood();

                timers(gameTime);
                move();
            }
            else // dood
            {
                this.color = DarkenColor(this.color);
                if (this.color.A == 0)
                {
                    this.Dead();
                }
            }
        }

        #region Methodes
        protected override void checkPartner() 
        {
            if (this.vruchtbaar <= 0)
            {
                  for (int i = 0; i < Carnivoor.Carnivoren.Count; i++)
                  {
                       checkPartner(Carnivoor.Carnivoren[i], VISION_DISTANCE);
                  }
            }
        }
        protected void checkFood()
        {
            float a, b, c;

            for (int i = 0; i < Herbivoor.Herbivoren.Count; i++)
            {
                a = (Herbivoor.Herbivoren[i].position.X + Herbivoor.Herbivoren[i].origin.X) - (this.position.X + this.origin.X);
                b = (Herbivoor.Herbivoren[i].position.Y + Herbivoor.Herbivoren[i].origin.Y) - (this.position.Y + this.origin.Y);
                c = RangeCheck(a, b);

                if (c < 10 && this.target == Herbivoor.Herbivoren[i]) // bespring Prooi
                {
                    this.position = Herbivoor.Herbivoren[i].position;
                    hongerniveau += ENERGY_GAIN;
                    HostileCollission(Herbivoor.Herbivoren[i]);
                }

                else if (this.target == Herbivoor.Herbivoren[i] && c < VISION_DISTANCE * grootte)  //jaag
                {
                    this.target = Herbivoor.Herbivoren[i];
                    this.moveTowards(Herbivoor.Herbivoren[i].position);
                    Herbivoor.Herbivoren[i].flee(this.versnelling, this); //laat prooi vluchten
                }

                else if(c < VISION_DISTANCE * grootte && this.target == null) // nieuwe prooi
                {
                    this.moveTowards(Herbivoor.Herbivoren[i].position);
                    this.target = Herbivoor.Herbivoren[i];
                    Herbivoor.Herbivoren[i].flee(this.versnelling, this);
                }
            }
        }

        private void calculateSoort()
        {
            bool calculated = false;

            if (AantalSoorten == 0)  // eerste soort aanmaken
            {
                AantalSoorten = 1;
                this.soort = AantalSoorten;
                this.color = RandomColor();
                calculated = true;
            }
            else
            {
                double DNAmin,DNAmax;
                for (int i = 1; i <= AantalSoorten; i++)
			    {
                    foreach (Carnivoor carnivoor in Carnivoren)
                    {
                        if (carnivoor.soort == i && carnivoor != this)
                        {
                            DNAmin = carnivoor.DNAscore - carnivoor.DNAscore * DNA_OFFSET;
                            DNAmax = carnivoor.DNAscore + carnivoor.DNAscore * DNA_OFFSET;

                            if (DNAmin < this.DNAscore && this.DNAscore < DNAmax)
                            {
                                this.soort = carnivoor.soort;
                                this.color = carnivoor.color;
                                i = AantalSoorten;
                                calculated = true;
                            }
                            break;
                        }
                    }
			    }
                if (!calculated)  //Maak nieuwe soort
                {
                    AantalSoorten++;
                    this.soort = AantalSoorten;
                    this.color = RandomColor();
                }
            }
        }
        private Color RandomColor()
        {
            int r = Game1.random.Next(0,255);
            int g = Game1.random.Next(0, 255);
            int b = Game1.random.Next(0, 255);

            return new Color(r,g,b,255);
        }

        public override void Dead()  //verwijder object van simulatie
        {
            Beesten.Remove(this);
            GameObjecten.Remove(this);
            Carnivoren.Remove(this);
        }
    }
        #endregion
}
