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
        private int hp;
        public bool Alive { get => hp > 0; }

        public Brick(Point3D pos, Size size, Color color, double b = 0.05, int hp = 1) : base(pos, size, color, b)
        {
            this.hp = hp;
        }

        public void hit()
        {
            --this.hp;
        }

        public override bool collidesWith(Mover b)
        {
            if (base.collidesWith(b))
            {
                --this.hp;

                return true;
            }
            return false;
        }

        public override void drawBorder(OpenGL gl)
        {
            base.drawBorder(gl);
        }

        public override void drawFilled(OpenGL gl)
        {
            base.drawFilled(gl);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void move(Point3D speed)
        {
            base.move(speed);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
