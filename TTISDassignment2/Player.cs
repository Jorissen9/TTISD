using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TTISDassignment2
{
    class Player : Block
    {
        private const uint SAMPLE_MAX = 100;
        private uint sample = 0;
        private Vector3D prevPos;
        private double SPEED_MOD = 0.05;

        public override bool Alive { get => Score > 0; }
        public int Score { get; private set; } = 1;

        public Player(Point3D pos, Size size, Color color, double b = 1)
            : base(pos, size, color, b)
        {

        }

        public void MissedBall()
        {
            this.Score -= 5;
        }

        public void HitBrick(Brick b)
        {
            this.Score += b.CurrentHP;
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
    }
}
