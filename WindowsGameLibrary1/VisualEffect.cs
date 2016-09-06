using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VisualEffects
{
    public class VisualEffect
    {
        static ParticleDrawer pd;
        public static Game Game { get; private set; }
        public static void Init(Game game)
        {
            Game = game;
            pd = new ParticleDrawer();
            EnableParticle();
            game.Components.ComponentAdded += (s,arg) =>
            {
                if(!game.Components.Contains(pd))
                {
                    game.Components.Add(pd);
                }
            };
        }
        public static bool ParticleEnabled
        {
            get { return Game.Components.Contains(pd); }
        }
        public static void EnableParticle()
        {
            Game.Components.Add(pd);
        }
    }
}
