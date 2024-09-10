using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymnasieArbete2025.Sprites
{
    interface IGameObject
    {
        bool IsDead { get; set; }
        Vector2 Position { get; set; }
        float Radius { get; set; }
        Vector2 Speed { get; set; }
        float Rotation { get; set; }
    }
}
