using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymnasieArbete2025
{
    class ScreenInfo
    {
        public static int ScreenWidth = 1920;
        public static int ScreenHeight = 1080;

        public static Rectangle GameArea
        {
            get
            {
                return new Rectangle(-80, -80, ScreenWidth + 160, ScreenHeight + 160);
            }
        }
        public static Rectangle RespawnArea
        {
            get
            {
                return new Rectangle((int)CenterScreen.X - 300,
                    (int)CenterScreen.Y - 300, 600, 600);
            }
        }
        public static Vector2 CenterScreen
        {
            get
            {
                return new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            }
        }
    }
}

