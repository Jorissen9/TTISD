using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SharpGL;

namespace TTISDassignment2
{
    class Brick : Block
    {
        private static readonly Color[] colors = {
            Colors.White,
            Color.FromRgb(0x8E, 0xFF, 0x00),
            Color.FromRgb(0xF3, 0xFF, 0x00),
            Color.FromRgb(0xFF, 0xBD, 0x00),
            Color.FromRgb(0xFF, 0x00, 0x00),
            Color.FromRgb(0xFF, 0x00, 0xD0),
        };

        public static int MaxHP { get => colors.Length - 1; }

        public Brick(Point3D pos, Size size, int hp = 1) : base(pos, size, Colors.White, 0.035)
        {
            this.hitpoints = hp;
            this.FillColor = colors[Util.Clamp(this.hitpoints, 0, MaxHP)];
            this.BorderColor = Colors.White;
        }

        public override void Hit()
        {
            base.Hit();
            this.FillColor = colors[Util.Clamp(this.hitpoints, 0, MaxHP)];
        }

        public override void drawBorder(OpenGL gl)
        {
            base.drawBorder(gl);
        }

        public override void drawFilled(OpenGL gl)
        {
            base.drawFilled(gl);
        }
    }
}
