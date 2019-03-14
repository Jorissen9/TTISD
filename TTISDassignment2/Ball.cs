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
        private static readonly double shrinkRatio = 0.1;

        private static Random rand = new Random();

        Point shrinkedSize;
        Point normalSize;
        Point shrinkedOffsetRight;
        
        private Texture texture = new Texture();
        private bool initialized = false;

        private Player lastPlayer = null;

    
        public Ball(Point3D pos, Size size, Color color, double b = 0.05)
            : base(pos, size, color, b)
        {
            shrinkedSize = new Point(this.Size.X * shrinkRatio,
                                     this.Size.Y * shrinkRatio);
            normalSize = this.Size;
            shrinkedOffsetRight = new Point(normalSize.X - shrinkedSize.X,
                                           (normalSize.Y - shrinkedSize.Y) / 2.0);
        }

        private double randomYSpeed()
        {
            return rand.NextDouble() * InitSpeedToRight.Y * 2.0 - InitSpeedToRight.Y;
        }

        public override void update(Point3D collide_rect)
        {
            base.update(collide_rect);

            if (this.wentOutOfLeftBorder)
            {
                this._pos.X = collide_rect.X / 4 * 1;
                this._pos.Y = collide_rect.Y / 2 + this.Size.Y / 2;
                this._speed.X = Ball.InitSpeedToRight.X;
                this._speed.Y = randomYSpeed();
            }
            else if (this.wentOutOfRightBorder)
            {
                this._pos.X = collide_rect.X / 4 * 3;
                this._pos.Y = collide_rect.Y / 2 + this.Size.Y / 2;
                this._speed.X = Ball.InitSpeedToLeft.X;
                this._speed.Y = randomYSpeed();
            }
        }

        private bool collideShrinked(Mover b)
        {
            // Collide with point ball, but this will fail for up/down collisions...
            //bool goesLeft = Speed.X < 0;

            //this.grow(goesLeft, true);
            //bool collide = base.collidesWith(b);
            //this.grow(goesLeft, false);

            //return collide;

            return base.collidesWith(b); 
        }

        public bool collidesWith(Brick b)
        {
            if (this.collideShrinked(b))
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
            // Shift ball size smaller

            if (this.collideShrinked(b))
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

            //this.drawBorder(gl);
        }

        public override void drawBorder(OpenGL gl)
        {
            bool goesLeft = Speed.X < 0;

            this.grow(goesLeft, true);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(BorderColor.R, BorderColor.G, BorderColor.B, BorderColor.A);
            gl.Vertex(Pos.X, Pos.Y + Size.Y, Pos.Z);
            gl.Vertex(Pos.X, Pos.Y, Pos.Z);
            gl.Vertex(Pos.X + Size.X, Pos.Y, Pos.Z);
            gl.Vertex(Pos.X + Size.X, Pos.Y + Size.Y, Pos.Z);
            gl.End();

            this.grow(goesLeft, false);
        }

        public void grow(bool left, bool shrink)
        {
            if (left)
            {   // Grow/shrink to left side
                if (shrink)
                {
                    this.Size = shrinkedSize;
                    _pos.Offset(0.0, shrinkedOffsetRight.Y, 0.0);
                } else
                {
                    this.Size = normalSize;
                    _pos.Offset(0.0, -shrinkedOffsetRight.Y, 0.0);
                }
            } else
            {   // Grow/shrink to right side
                if (shrink)
                {
                    this.Size = shrinkedSize;
                    _pos.Offset(shrinkedOffsetRight.X, shrinkedOffsetRight.Y, 0.0);
                } else
                {
                    this.Size = normalSize;
                    _pos.Offset(-shrinkedOffsetRight.X, -shrinkedOffsetRight.Y, 0.0);
                }
            }
        }
    }
}
