using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace VisualEffects
{
    public class ParticleDrawer : DrawableGameComponent
    {
        public ParticleDrawer() : base(VisualEffect.Game) { }
        public override void Draw(GameTime gameTime)
        {
            Particle.DrawAll(gameTime, VisualEffect.Game.GraphicsDevice);
            base.Draw(gameTime);
        }
        public override void Update(GameTime gameTime)
        {
            this.DrawOrder = Particle.WholeDrawOrder;
            base.Update(gameTime);
        }
    }
}
