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
    class Player : Block
    {
        private const uint SAMPLE_MAX = 50;
        private uint sample = 0;
        private Vector3D prevPos;
        private double SPEED_MOD = 0.5;

        private int id = 0;

        public override bool Alive { get => Score > 0; }

        public Vector3D RelativeSpeed
        {
            get => (prevPos - ((Vector3D)this.Pos)) * SPEED_MOD;
        }

        private int _score = 1;
        public int Score { get => Math.Max(_score, 0); }

        public Player(Point3D pos, Size size, Color color, double b = 1, int id = 0)
            : base(pos, size, color, b)
        {
            this.id = id;
        }

        public void MissedBall()
        {
            _score -= 5;
        }

        public void HitBrick(Brick b)
        {
            if (Alive)
            {
                _score += b.CurrentHP;
            }
        }

        public override void move(Point3D speed)
        {
            if (this.sample++ > SAMPLE_MAX)
            {
                prevPos = (Vector3D)this.Pos;
                this.sample = 0;
            }
            base.move(speed);
        }

        public override void drawFilled(OpenGL gl)
        {
            base.drawFilled(gl);
            gl.DrawText(gl.RenderContextProvider.Width / 4 * (id == 0 ? 1 : 3),
                        10,
                        FillColor.R, FillColor.G, FillColor.B,
                        "Arial", 64,
                        Score.ToString());
        }
    }
}
