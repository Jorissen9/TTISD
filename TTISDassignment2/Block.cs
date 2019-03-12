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
    class Block : Mover
    {
        protected int hitpoints = 1;

        public int CurrentHP { get => Math.Max(hitpoints, 0); }
        public virtual bool Alive { get => CurrentHP > 0; }

        public virtual void Hit()
        {
            if (this.Alive)
            {
                this.hitpoints--;
            }
        }

        public Block(double x, double y, double z, double w, double h, double b = 1)
            : base(x, y, z, w, h, b)
        {

        }

        public Block(Point3D pos, Size size, Color color, double b = 1)
            : base(pos, size, color, b)
        {

        }

        public override void drawFilled(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(FillColor.R, FillColor.G, FillColor.B, FillColor.A);

            gl.Vertex(Pos.X, Pos.Y + Size.Y, Pos.Z);
            gl.Vertex(Pos.X, Pos.Y, Pos.Z);
            gl.Vertex(Pos.X + Size.X, Pos.Y, Pos.Z);
            gl.Vertex(Pos.X + Size.X, Pos.Y + Size.Y, Pos.Z);

            gl.End();
        }

        public override void drawBorder(OpenGL gl)
        {
            if (this.Border > 0)
            {
                gl.Begin(OpenGL.GL_QUADS);

                gl.Color(BorderColor.R, BorderColor.G, BorderColor.B, BorderColor.A);
                gl.Vertex(Pos.X, Pos.Y + Size.Y, Pos.Z);
                gl.Vertex(Pos.X, Pos.Y, Pos.Z);
                gl.Vertex(Pos.X + Size.X, Pos.Y, Pos.Z);
                gl.Vertex(Pos.X + Size.X, Pos.Y + Size.Y, Pos.Z);

                gl.Color(FillColor.R, FillColor.G, FillColor.B, FillColor.A);
                gl.Vertex(Pos.X + Border, Pos.Y + Size.Y - Border, Pos.Z);
                gl.Vertex(Pos.X + Border, Pos.Y + Border, Pos.Z);
                gl.Vertex(Pos.X + Size.X - Border, Pos.Y + Border, Pos.Z);
                gl.Vertex(Pos.X + Size.X - Border, Pos.Y + Size.Y - Border, Pos.Z);

                gl.End();

            } else {
                drawFilled(gl);
            }
        }
    }
}
