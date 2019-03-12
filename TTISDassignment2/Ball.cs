using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace TTISDassignment2
{
    class Ball : Mover
    {
        public static readonly Point3D InitSpeedToRight = new Point3D(0.4, 0.2, 0.0);
        public static readonly Point3D InitSpeedToLeft  = new Point3D(-0.4, -0.2, 0.0);

        private Texture texture = new Texture();
        private bool initialized = false;

        private Player lastPlayer = null;

        public Ball(double x, double y, double z, double w, double h, double b = 1)
            : base(x, y, z, w, h, b)
        {

        }

        public Ball(Point3D pos, Size size, Color color, double b = 1)
            : base(pos, size, color, b)
        {

        }

        public override void update(Point3D collide_rect)
        {
            base.update(collide_rect);

            if (this.wentOutOfLeftBorder)
            {
                this._pos.X = collide_rect.X / 4 * 1;
                this._pos.Y = collide_rect.Y / 2 + this.Size.Y / 2;
                this.Speed = Ball.InitSpeedToRight;
            }
            else if (this.wentOutOfRightBorder)
            {
                this._pos.X = collide_rect.X / 4 * 3;
                this._pos.Y = collide_rect.Y / 2 + this.Size.Y / 2;
                this.Speed = Ball.InitSpeedToLeft;
            }
        }

        public bool collidesWith(Brick b)
        {
            if (base.collidesWith(b))
            {
                if (lastPlayer != null)
                {
                    lastPlayer.HitBrick(b);
                }

                b.Hit();

                this._speed.X = Math.Sign(Speed.X) * Math.Abs(InitSpeedToRight.X);
                this._speed.Y = Math.Sign(Speed.Y) * Math.Abs(InitSpeedToRight.Y);

                return true;
            }

            return false;
        }

        public bool collidesWith(Player b)
        {
            if (base.collidesWith(b))
            {
                lastPlayer = b;

                //Vector3D rel = lastPlayer.RelativeSpeed;
                //this._speed.X *= rel.X;
                //this._speed.Y *= rel.Y;                

                this._speed.X *= 1.25;
                this._speed.Y *= 1.05;
                return true;
            }

            return false;
        }

        public void init(OpenGL gl)
        {
            var img = (BitmapImage)App.Current.Resources["texBall"];
            texture.Create(gl, Util.BitmapImage2Bitmap(img));
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.TextureName);

            initialized = true;
        }

        public override void drawFilled(OpenGL gl)
        {
            if (!initialized)
            {
                init(gl);
            }

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.TextureName);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(Pos.X, Pos.Y, Pos.Z);                    // Bottom Left Of The Texture and Quad
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(Pos.X + Size.X, Pos.Y, Pos.Z);           // Bottom Right Of The Texture and Quad
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(Pos.X + Size.X, Pos.Y + Size.Y, Pos.Z);  // Top Right Of The Texture and Quad
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(Pos.X, Pos.Y + Size.Y, Pos.Z);           // Top Left Of The Texture and Quad
            gl.End();

            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);


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
