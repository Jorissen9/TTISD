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
        public Brick(Point3D pos, Size size, Color color, double b = 0.05, int hp = 1) : base(pos, size, color, b)
        {
            this.hitpoints = hp;
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
