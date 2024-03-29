﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SharpGL;

namespace TTISDassignment2
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        public GameState state = GameState.START;

        private Point3D gameOffset = new Point3D(-16.7, -8.7, -10);
        private Point3D gameSize   = new Point3D(1, 1, -10);
        private double aspect_ratio;
        private Size windowSize;

        private Point3D[] corners;

        private Player player1, player2;
        private Ball p1ball, p2ball;
        private BrickManager brickManager;

        private Color gamePlayer1Color = Color.FromArgb(255, 255, 0, 0);
        private Color gamePlayer2Color = Color.FromArgb(255, 0, 255, 0);

        private Color gameBGColor        = Color.FromArgb(255, 255, 255, 255);
        private Color gameCalibRectColor = Color.FromArgb(255, 255, 255, 0);
        private Color gameRectColor      = Color.FromArgb(255, 0, 0, 0);

        private Block gameRectangle;
        private Block gameCalibRectangle;

        private Block[] gameCalibBlocks;

        private static readonly int fontSize = 64;

        public Game()
        {
            InitializeComponent();
            Closed += Exit;

            windowSize = new Size(Math.Abs(gameOffset.X) * 2.0, Math.Abs(gameOffset.Y) * 2.0);
            gameSize = new Point3D(windowSize.Width, windowSize.Height, -11);
            aspect_ratio = windowSize.Width / windowSize.Height;

            corners = new Point3D[4];
            corners[0] = new Point3D(0, gameSize.Y, gameSize.Z);
            corners[1] = new Point3D(gameSize.X, gameSize.Y, gameSize.Z);
            corners[2] = new Point3D(gameSize.X, 0, gameSize.Z);
            corners[3] = new Point3D(0, 0, gameSize.Z);

            Size calib_size = new Size(0.05 * windowSize.Width, 0.05 * windowSize.Height * aspect_ratio);

            Point3D[] calib_pos = corners;
            calib_pos[0].Offset(0.0, -calib_size.Height, 0.0);
            calib_pos[1].Offset(-calib_size.Width, -calib_size.Height, 0.0);
            calib_pos[2].Offset(-calib_size.Width, 0.0, 0.0);

            gameCalibBlocks = new Block[4];
            gameCalibBlocks[0] = new Block(calib_pos[0], calib_size, Colors.Cyan);
            gameCalibBlocks[1] = new Block(calib_pos[1], calib_size, Colors.Cyan);
            gameCalibBlocks[2] = new Block(calib_pos[2], calib_size, Colors.Cyan);
            gameCalibBlocks[3] = new Block(calib_pos[3], calib_size, Colors.Cyan);

            gameRectangle      = new Block(new Point3D(0, 0, -11), windowSize, gameBGColor, 0.15);
            gameCalibRectangle = new Block(new Point3D(0, 0, -11), windowSize, gameBGColor, 0.15);

            gameRectangle.BorderColor      = gameRectColor;
            gameCalibRectangle.BorderColor = gameCalibRectColor;

            //this.Reset();
        }

        public void Exit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Reset()
        {
            Size p_size = new Size(0.025 * windowSize.Width, 0.1 * windowSize.Height * aspect_ratio);
            double screen_mid = gameSize.Y / 2 - p_size.Height / 2;
            player1 = new Player(new Point3D(0, screen_mid, -11), p_size, gamePlayer1Color, 1, 0);
            player2 = new Player(new Point3D(gameSize.X - p_size.Width, screen_mid, -11), p_size, gamePlayer2Color, 1, 1);

            double ball_size = 0.03 * windowSize.Width; //0.015 * windowSize.Width;
            Size b_size = new Size(ball_size, ball_size);
            screen_mid = gameSize.Y / 2 - b_size.Height / 2;

            p1ball = new Ball(new Point3D(ball_size * 3.0, screen_mid, -11), b_size, gamePlayer1Color);
            p2ball = new Ball(new Point3D(gameSize.X - ball_size * 3.0, screen_mid, -11), b_size, gamePlayer2Color);

            p1ball.Speed = Ball.InitSpeedToRight;
            p2ball.Speed = Ball.InitSpeedToLeft;

            // Generate bricks
            double br_width = 0.03 * windowSize.Width;
            double br_height = 0.09 * windowSize.Height;
            brickManager = new BrickManager(br_width, br_height, gameSize);
        }

        public List<Point> getCorners()
        {
            List<Point> points = new List<Point>(corners.Length);

            points.Add(new Point(corners[0].X, corners[0].Y));
            points.Add(new Point(corners[1].X, corners[1].Y));
            points.Add(new Point(corners[2].X, corners[2].Y));
            points.Add(new Point(corners[3].X, corners[3].Y));

            return points;
        }

        public void setState(GameState next)
        {
            this.state = next;
        }

        public void SetPositions(Point p1, Point p2)
        {
            player1.set2DPosition(p1);
            player2.set2DPosition(p2);
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void OpenGLControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //OpenGL gl = openGLControl.OpenGL;
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;

            //  Clear the color and depth buffers.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Reset the modelview matrix.
            gl.LoadIdentity();

            //  Move into a more central position.
            gl.Translate(gameOffset.X, gameOffset.Y, gameOffset.Z);

            int RenderContextProviderWidth = gl.RenderContextProvider.Width;
            int RenderContextProviderHeight = gl.RenderContextProvider.Height;

            switch (state)
            {
                case GameState.START:
                    gameCalibRectangle.drawBorder(gl);
                    gameCalibBlocks[0].drawFilled(gl);
                    gameCalibBlocks[1].drawFilled(gl);
                    gameCalibBlocks[2].drawFilled(gl);
                    gameCalibBlocks[3].drawFilled(gl);

                    break;

                case GameState.IS_CALIBRATING_POINT_1:
                    gl.ClearColor(gameBGColor.R, gameBGColor.G, gameBGColor.B, gameBGColor.A);

                    gameCalibRectangle.drawBorder(gl);
                    gameCalibBlocks[(int)state - 1].drawFilled(gl);

                    break;

                case GameState.IS_CALIBRATING_POINT_2:
                    gl.ClearColor(gameBGColor.R, gameBGColor.G, gameBGColor.B, gameBGColor.A);

                    gameCalibRectangle.drawBorder(gl);
                    gameCalibBlocks[(int)state - 1].drawFilled(gl);

                    break;

                case GameState.IS_CALIBRATING_POINT_3:
                    gl.ClearColor(gameBGColor.R, gameBGColor.G, gameBGColor.B, gameBGColor.A);

                    gameCalibRectangle.drawBorder(gl);
                    gameCalibBlocks[(int)state - 1].drawFilled(gl);

                    break;

                case GameState.IS_CALIBRATING_POINT_4:
                    gl.ClearColor(gameBGColor.R, gameBGColor.G, gameBGColor.B, gameBGColor.A);

                    gameCalibRectangle.drawBorder(gl);
                    gameCalibBlocks[(int)state - 1].drawFilled(gl);

                    break;

                case GameState.DONE_CALIBRATING:
                    gl.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);
                    break;

                case GameState.PLAYING:
                    gameRectangle.drawBorder(gl);

                    // Update Position and Speed
                    p1ball.update(gameSize);
                    p2ball.update(gameSize);

                    if(player1.Alive)
                    {
                        player1.update(gameSize);
                    }
                    if (player2.Alive)
                    {
                        player2.update(gameSize);
                    }

                    if (p1ball.wentOutOfLeftBorder || p2ball.wentOutOfLeftBorder)
                    {
                        if (player1.IsOnLeftSide)
                        {
                            player1.MissedBall();
                        }
                        if (player2.IsOnLeftSide)
                        {
                            player2.MissedBall();
                        }
                    } else if (p1ball.wentOutOfRightBorder || p2ball.wentOutOfRightBorder)
                    {
                        if (player1.IsOnRightSide)
                        {
                            player1.MissedBall();
                        }
                        if (player2.IsOnRightSide)
                        {
                            player2.MissedBall();
                        }
                    }

                    brickManager.update(p1ball, p2ball);

                    // Collide with ball
                    p1ball.collidesWith(player1);
                    p1ball.collidesWith(player2);

                    p2ball.collidesWith(player1);
                    p2ball.collidesWith(player2);

                    // Draw
                    p1ball.drawFilled(gl);
                    p2ball.drawFilled(gl);

                    brickManager.draw(gl);

                    if(player1.Alive)
                    {
                        player1.drawFilled(gl);
                    }
                    if (player2.Alive)
                    {
                        player2.drawFilled(gl);
                    }

                    // check aliveness and change states accordingly
                    if (!player1.Alive && !player2.Alive)
                    {
                        state = GameState.GAME_OVER;
                    }
                    else if (!player1.Alive && brickManager.allBricksDestoyed())
                    {
                        state = GameState.PLAYER_2_WINS;
                    }
                    else if (!player2.Alive && brickManager.allBricksDestoyed())
                    {
                        state = GameState.PLAYER_1_WINS;
                    }

                    break;

                case GameState.PLAYER_1_WINS:
                    gameRectangle.drawBorder(gl);

                    // Draw
                    p1ball.drawFilled(gl);
                    p2ball.drawFilled(gl);

                    brickManager.draw(gl);

                    player1.drawFilled(gl);
                    player2.drawFilled(gl);

                    // Draw text
                    gl.DrawText(RenderContextProviderWidth / 4 + 75,
                        RenderContextProviderHeight - 100,
                        gamePlayer1Color.R, gamePlayer1Color.G, gamePlayer1Color.B,
                        "Arial", fontSize, "Player 1 Wins!!!");

                    break;

                case GameState.PLAYER_2_WINS:
                    gameRectangle.drawBorder(gl);

                    // Draw
                    p1ball.drawFilled(gl);
                    p2ball.drawFilled(gl);

                    brickManager.draw(gl);

                    player1.drawFilled(gl);
                    player2.drawFilled(gl);

                    // Draw text
                    gl.DrawText(RenderContextProviderWidth / 4 + 75,
                        RenderContextProviderHeight - 100,
                        gamePlayer2Color.R, gamePlayer2Color.G, gamePlayer2Color.B,
                        "Arial", fontSize, "Player 2 Wins!!!");

                    break;

                case GameState.GAME_OVER:
                    gameRectangle.drawBorder(gl);

                    // Draw
                    p1ball.drawFilled(gl);
                    p2ball.drawFilled(gl);

                    brickManager.draw(gl);

                    player1.drawFilled(gl);
                    player2.drawFilled(gl);

                    // Draw text
                    gl.DrawText(RenderContextProviderWidth / 4 + 100,
                        RenderContextProviderHeight - 100,
                        0, 0, 1,
                        "Arial", fontSize, "GAME OVER...");

                    break;

                default:
                    break;
            }

            //  Flush OpenGL.
            gl.Flush();
        }
    }
}
