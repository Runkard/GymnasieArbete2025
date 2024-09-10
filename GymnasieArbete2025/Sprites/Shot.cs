using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymnasieArbete2025.Sprites
{
    class Shot : GameObject
    {
        public Shot()
        {
            Radius = 16;
        }

        public void Update(GameTime gameTime)
        {
            Position += Speed;
            Rotation += 0.08f;

            if (Rotation > MathHelper.TwoPi)
                Rotation = 0;
        }
    }
}
