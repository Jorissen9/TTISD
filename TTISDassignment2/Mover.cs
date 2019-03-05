using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SharpGL;

namespace TTISDassignment2
{
    abstract class Mover
    {
        private Point3D _pos;
        public Point3D Pos { get => _pos; }

        public Point Size { get; set; }

        private Point3D _speed;
        public Point3D Speed { get => _speed; set => _speed = value; }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public double Border { get; set; }

        public Mover(double x, double y, double z, double w, double h, double b = 1)
        {
            this._pos = new Point3D(x, y, z);
            this.Size = new Point(w, h);
            this.FillColor = Colors.White;
            this.Border = b;
        }

        public Mover(Point3D pos, Size size, Color color, double b = 1)
        {
            this._pos = pos;
            this.Size = new Point(size.Width, size.Height);
            this.FillColor = color;
            this.Border = b;
        }

        public void set2DPosition(Point pos)
        {
            this._pos.X = pos.X;
            this._pos.Y = pos.Y;
        }

        public void move(Point3D speed)
        {
            this._pos.Offset(Speed.X, Speed.Y, Speed.Z);
        }

        public void update(Point3D collide_rect)
        {
            this.move(Speed);

            // Collide with game borders
            if (Pos.X <= 0) {
                _speed.X = -_speed.X;
                _pos.X = 0.0;
            } else if (Pos.X + Size.X >= collide_rect.X) {
                _speed.X = -_speed.X;
                _pos.X = collide_rect.X - Size.X;
            }

            if (Pos.Y <= 0) {
                _speed.Y = -_speed.Y;
                _pos.Y = 0.0;
            } else if (Pos.Y + Size.Y >= collide_rect.Y) {
                _speed.Y = -_speed.Y;
                _pos.Y = collide_rect.Y - Size.Y;
            }
        }

        public bool collidesWith(Block b)
        {


            return false;
        }

        public void grow(Point growrate)
        {
            this.Size.Offset(growrate.X, growrate.Y);
        }

        public void growFromMiddle(Point growrate)
        {
            grow(growrate);
            _pos.X -= growrate.X / 2.0;
            _pos.Y -= growrate.Y / 2.0;
        }

        public abstract void drawFilled(OpenGL gl);
        public abstract void drawBorder(OpenGL gl);
    }
}
