using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SharpGL;

namespace TTISDassignment2
{
    class Ball : Mover
    {
        public Ball(double x, double y, double z, double w, double h, double b = 1)
            : base(x, y, z, w, h, b)
        {

        }

        public Ball(Point3D pos, Size size, Color color, double b = 1)
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

            //gl.Begin(OpenGL.GL_LINE_LOOP);
            //gl.Color(FillColor.R, FillColor.G, FillColor.B, FillColor.A);

            //for (int i = 0; i <= 10; i++)
            //{
            //    double angle = 2 * Math.PI * i / 10;
            //    double x = Math.Cos(angle) - Pos.X;
            //    double y = Math.Sin(angle) - Pos.Y;

            //    gl.Vertex(x, y, Pos.Z);
            //}

            //gl.End();
        }

        public override void drawBorder(OpenGL gl)
        {

        }
    }
}
