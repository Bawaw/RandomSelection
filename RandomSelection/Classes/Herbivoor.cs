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
    class Herbivoor : Creature
    {
        //instellingen
        private const int VISION_DISTANCE = 150; //afstand van waar een Herbivoor andere objecten kan vinden
        private const float DNA_OFFSET = 0.10f;  //Percent verschil voor DNA voor nieuwe soort
        private const float ENERGY_GAIN = 20f;  //energie percent bij gekregen per eet beurt

        #region declareren/initialiseren props,fields,list
        static public List<Herbivoor> Herbivoren = new List<Herbivoor>();
        Color color;

        public static int AantalSoorten = 0;
        public int soort { get; private set; }
        #endregion

        public Herbivoor(float snelheid, int sex, float grootte, float nakomelingen, Vector2 position, Texture2D texture)
            : base(snelheid, sex, grootte, nakomelingen,position, texture)
        {
            DNAscore = this.snelheid * 0.35 + this.grootte * 0.55 + this.nakomelingen * 0.2;
            Herbivoren.Add(this);
            calculateSoort();
        }

        public override void Draw(SpriteBatch spritebatch) //tekenen van herbivoren
        {
            float angleInRadians = (float) Math.Atan2(versnelling.X, versnelling.Y);
            spritebatch.Draw(this.Texture, position, null, this.color, angleInRadians * -1, origin, this.grootte, SpriteEffects.None, 0.9f);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.hongerniveau > 0)
            {
                if ((this.hongerniveau > 70 || this.target is Herbivoor) && !(this.target is Carnivoor))  // paren
                    checkPartner();

                else if ((hongerniveau < 70 || this.target is Vegitatie) && !(this.target is Carnivoor)) // eten
                    checkFood();

                timers(gameTime);
                move();
            }
            else //dood
            {
                this.color = DarkenColor(this.color);
                if (this.color.A == 0)
                    this.Dead();
            }
        }
        #region Methodes
        protected override void checkPartner()
        {
            if (this.vruchtbaar <= 0)
            {
                for (int i = 0; i < Herbivoor.Herbivoren.Count; i++)
                {
                    checkPartner(Herbivoor.Herbivoren[i], VISION_DISTANCE);
                }
            }
        }

        protected void checkFood()
        {
            float a, b, c;
           
            for (int i = 0; i < Vegitatie.planten.Count; i++)
            {
                a = (Vegitatie.planten[i].position.X + Vegitatie.planten[i].origin.X) - (this.position.X + this.origin.X);
                b = (Vegitatie.planten[i].position.Y + Vegitatie.planten[i].origin.Y)- (this.position.Y + this.origin.Y);
                c = RangeCheck(a, b);

                if (c < VISION_DISTANCE * grootte && this.target == null && Vegitatie.planten[i].target == null)
                {
                    this.moveTowards(Vegitatie.planten[i].position + Vegitatie.planten[i].origin);
                    this.target = Vegitatie.planten[i];
                    Vegitatie.planten[i].target = this;
                }
                else if (c < 25 && this.target == Vegitatie.planten[i]) // Spring op prooi
                {
                    this.position = Vegitatie.planten[i].position + Vegitatie.planten[i].origin;
                    hongerniveau += ENERGY_GAIN;
                    HostileCollission(Vegitatie.planten[i]);
                }
            }
        }

        public void flee(Vector2 incomingVelocity, GameObject target) //vluchten
        {
            this.target = target;
            this.versnelling = incomingVelocity * 1.2f; //sprint
        }

        private void calculateSoort()
        {
            bool calculated = false;

            if (AantalSoorten == 0) //eerse soort aanmaken
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
                    foreach (Herbivoor Herbivoor in Herbivoren)
                    {
                        if (Herbivoor.soort == i && Herbivoor != this)
                        {
                            DNAmin = Herbivoor.DNAscore - Herbivoor.DNAscore * DNA_OFFSET;
                            DNAmax = Herbivoor.DNAscore + Herbivoor.DNAscore * DNA_OFFSET;

                            if (DNAmin < this.DNAscore && this.DNAscore < DNAmax)
                            {
                                this.soort = Herbivoor.soort;
                                this.color = Herbivoor.color;
                                i = AantalSoorten;
                                calculated = true;
                            }
                            break;
                        }
                    }
			    }

                if (!calculated) //Maak nieuwe soort
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
            int g = Game1.random.Next(0,255);
            int b = Game1.random.Next(0,255);

            return new Color(r,g,b);
        }
        public override void Dead() //verwijder object van simulatie
        {
            Beesten.Remove(this);
            GameObjecten.Remove(this);
            Herbivoren.Remove(this);
        }
    }
        #endregion
}