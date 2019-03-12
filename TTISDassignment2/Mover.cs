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

        public bool wentOutOfLeftBorder  = false;
        public bool wentOutOfRightBorder = false;
        public bool hadCollision         = false;

        public Mover(double x, double y, double z, double w, double h, double b = 1)
        {
            this._pos      = new Point3D(x, y, z);
            this.Size      = new Point(w, h);
            this.Speed     = new Point3D(0, 0, 0);
            this.FillColor = Colors.White;
            this.Border    = b;
        }

        public Mover(Point3D pos, Size size, Color color, double b = 1)
        {
            this._pos      = pos;
            this.Size      = new Point(size.Width, size.Height);
            this.Speed     = new Point3D(0, 0, 0);
            this.FillColor = color;
            this.Border    = b;
        }

        public void set2DPosition(Point pos)
        {
            this._pos.X = pos.X;
            this._pos.Y = pos.Y;
        }

        public virtual void move(Point3D speed)
        {
            this._pos.Offset(Speed.X, Speed.Y, Speed.Z);
        }

        public void update(Point3D collide_rect)
        {
            this.move(Speed);
            this.hadCollision = false;
            this.wentOutOfLeftBorder = false;
            this.wentOutOfRightBorder = false;

            // Collide with game borders
            if (Pos.X <= 0) {
                _speed.X = -_speed.X;
                _pos.X = 0.0;
                this.wentOutOfLeftBorder = true;
                this.hadCollision = true;
            } else if (Pos.X + Size.X >= collide_rect.X) {
                _speed.X = -_speed.X;
                _pos.X = collide_rect.X - Size.X;
                this.wentOutOfRightBorder = true;
                this.hadCollision = true;
            }

            if (Pos.Y <= 0) {
                _speed.Y = -_speed.Y;
                _pos.Y = 0.0;
                this.hadCollision = true;
            } else if (Pos.Y + Size.Y >= collide_rect.Y) {
                _speed.Y = -_speed.Y;
                _pos.Y = collide_rect.Y - Size.Y;
                this.hadCollision = true;
            }
        }

        public virtual bool collidesWith(Mover b)
        {
            bool left  = this.Pos.X < b.Pos.X + b.Size.X;
            bool right = this.Pos.X + this.Size.X > b.Pos.X;
            bool bot   = this.Pos.Y < b.Pos.Y + b.Size.Y;
            bool top   = this.Pos.Y + this.Size.Y > b.Pos.Y;

            if (left && right && bot && top)
            {
                this.hadCollision = true;
                return true;
            }

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
