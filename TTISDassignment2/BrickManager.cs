using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace TTISDassignment2
{
    class BrickManager
    {
        private int brickAmount;
        public Brick[] bricks;

        public BrickManager(double br_width, double br_height, Point3D gameSize)
        {
            brickAmount = 50;
            Size br_size = new Size(br_width, br_height);

            bricks = new Brick[50];
            for (int i = 0; i < 10; i++)
            {
                bricks[i] = new Brick(new Point3D((gameSize.X / 2) - (br_width / 2) - (br_width * 2), 1 + (br_height * i), -11), br_size, 1);
            }
            for (int i = 0; i < 10; i++)
            {
                bricks[i + 10] = new Brick(new Point3D((gameSize.X / 2) - (br_width / 2) - (br_width), 1 + (br_height * i), -11), br_size, 3);
            }
            for (int i = 0; i < 10; i++)
            {
                bricks[i + 20] = new Brick(new Point3D((gameSize.X / 2) - (br_width / 2), 1 + (br_height * i), -11), br_size, Brick.MaxHP);
            }
            for (int i = 0; i < 10; i++)
            {
                bricks[i + 30] = new Brick(new Point3D((gameSize.X / 2) - (br_width / 2) + (br_width), 1 + (br_height * i), -11), br_size, 3);
            }
            for (int i = 0; i < 10; i++)
            {
                bricks[i + 40] = new Brick(new Point3D((gameSize.X / 2) - (br_width / 2) + (br_width * 2), 1 + (br_height * i), -11), br_size, 1);
            }
        }

        public void brickDestroyed()
        {
            --brickAmount;
        }

        public bool allBricksDestoyed()
        {
            return brickAmount <= 0;
        }
    }
}
