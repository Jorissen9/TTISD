using System;
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

        private double width, height;

        private Point player1, player2;

        public Game()
        {
            InitializeComponent();
        }

        public void setState(GameState next)
        {
            this.state = next;
        }

        public void setPositions(Point p1, Point p2)
        {
            player1 = p1;
            player2 = p2;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void OpenGLControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;

            OpenGL gl = openGLControl.OpenGL;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0f,
                gl.RenderContextProvider.Width / gl.RenderContextProvider.Height,
                0.1f, 100.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //gl.SetDimensions((int)width, (int)height);
            //gl.Viewport(0, 0, (int)width, (int)height);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;

            //  Clear the color and depth buffers.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Reset the modelview matrix.
            //gl.LoadIdentity();

            switch (state)
            {
                case GameState.START:
                    gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
                    break;

                case GameState.IS_CALIBRATING_POINT_1:
                    gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

                    //  Reset the modelview matrix.
                    gl.LoadIdentity();

                    //  Move into a more central position.
                    //gl.Translate(-width/2.0, -height/2.0, -99.0f);
                    gl.Translate(0.0f, 0.0f, -99.0f);
                    
                    //  Provide the cube colors and geometry.
                    gl.Begin(OpenGL.GL_QUADS);

                    gl.Color(1.0f, 0.0f, 0.0f);
                    gl.Vertex(0.0f, 0.0f, 0.0f);
                    gl.Vertex(0.0f, height, 0.0f);
                    gl.Vertex(width, height, 0.0f);
                    gl.Vertex(width, 0.0f, 0.0f);
                    gl.End();

                    gl.Begin(OpenGL.GL_2D);
                    gl.Color(0.0f, 1.0f, 0.0f);
                    gl.Rect(10.0, 10.0, width/2.0, height/2.0);
                    gl.End();

                    //gl.Color(0.0f, 1.0f, 0.0f);
                    //gl.Vertex(1.0f, 1.0f, -1.0f);
                    //gl.Vertex(-1.0f, 1.0f, -1.0f);
                    //gl.Vertex(-1.0f, 1.0f, 1.0f);
                    //gl.Vertex(1.0f, 1.0f, 1.0f);

                    //gl.Color(1.0f, 0.5f, 0.0f);
                    //gl.Vertex(1.0f, -1.0f, 1.0f);
                    //gl.Vertex(-1.0f, -1.0f, 1.0f);
                    //gl.Vertex(-1.0f, -1.0f, -1.0f);
                    //gl.Vertex(1.0f, -1.0f, -1.0f);

                    //gl.Color(1.0f, 0.0f, 0.0f);
                    //gl.Vertex(1.0f, 1.0f, 1.0f);
                    //gl.Vertex(-1.0f, 1.0f, 1.0f);
                    //gl.Vertex(-1.0f, -1.0f, 1.0f);
                    //gl.Vertex(1.0f, -1.0f, 1.0f);

                    //gl.Color(1.0f, 1.0f, 0.0f);
                    //gl.Vertex(1.0f, -1.0f, -1.0f);
                    //gl.Vertex(-1.0f, -1.0f, -1.0f);
                    //gl.Vertex(-1.0f, 1.0f, -1.0f);
                    //gl.Vertex(1.0f, 1.0f, -1.0f);

                    //gl.Color(0.0f, 0.0f, 1.0f);
                    //gl.Vertex(-1.0f, 1.0f, 1.0f);
                    //gl.Vertex(-1.0f, 1.0f, -1.0f);
                    //gl.Vertex(-1.0f, -1.0f, -1.0f);
                    //gl.Vertex(-1.0f, -1.0f, 1.0f);

                    //gl.Color(1.0f, 0.0f, 1.0f);
                    //gl.Vertex(1.0f, 1.0f, -1.0f);
                    //gl.Vertex(1.0f, 1.0f, 1.0f);
                    //gl.Vertex(1.0f, -1.0f, 1.0f);
                    //gl.Vertex(1.0f, -1.0f, -1.0f);



                    break;

                case GameState.IS_CALIBRATING_POINT_2:
                    gl.ClearColor(0.75f, 0.0f, 0.0f, 1.0f);
                    break;

                case GameState.IS_CALIBRATING_POINT_3:
                    gl.ClearColor(0.50f, 0.0f, 0.0f, 1.0f);
                    break;

                case GameState.IS_CALIBRATING_POINT_4:
                    gl.ClearColor(0.25f, 0.0f, 0.0f, 1.0f);
                    break;

                case GameState.DONE_CALIBRATING:
                    gl.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);
                    break;

                case GameState.PLAYING:
                    gl.ClearColor(0.0f, 0.0f, 1.0f, 1.0f);
                    break;

                case GameState.PLAYER_1_WINS:
                    gl.ClearColor(1.0f, 1.0f, 0.0f, 1.0f);
                    break;

                case GameState.PLAYER_2_WINS:
                    gl.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
                    break;

                default:
                    break;
            }




            ////  Move the geometry into a fairly central position.
            //gl.Translate(-1.5f, 0.0f, -6.0f);

            ////  Draw a pyramid. First, rotate the modelview matrix.
            //gl.Rotate(rotatePyramid, 0.0f, 1.0f, 0.0f);

            ////  Start drawing triangles.
            //gl.Begin(OpenGL.GL_TRIANGLES);

            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f, 0.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(-1.0f, -1.0f, 1.0f);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f, -1.0f, 1.0f);

            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f, 0.0f);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f, -1.0f, 1.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(1.0f, -1.0f, -1.0f);

            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f, 0.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(1.0f, -1.0f, -1.0f);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f, -1.0f, -1.0f);

            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f, 0.0f);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f, -1.0f, -1.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(-1.0f, -1.0f, 1.0f);

            //gl.End();

            ////  Reset the modelview.
            //gl.LoadIdentity();

            ////  Move into a more central position.
            //gl.Translate(1.5f, 0.0f, -7.0f);

            ////  Rotate the cube.
            //gl.Rotate(rquad, 1.0f, 1.0f, 1.0f);

            ////  Provide the cube colors and geometry.
            //gl.Begin(OpenGL.GL_QUADS);

            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(1.0f, 1.0f, -1.0f);
            //gl.Vertex(-1.0f, 1.0f, -1.0f);
            //gl.Vertex(-1.0f, 1.0f, 1.0f);
            //gl.Vertex(1.0f, 1.0f, 1.0f);

            //gl.Color(1.0f, 0.5f, 0.0f);
            //gl.Vertex(1.0f, -1.0f, 1.0f);
            //gl.Vertex(-1.0f, -1.0f, 1.0f);
            //gl.Vertex(-1.0f, -1.0f, -1.0f);
            //gl.Vertex(1.0f, -1.0f, -1.0f);

            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(1.0f, 1.0f, 1.0f);
            //gl.Vertex(-1.0f, 1.0f, 1.0f);
            //gl.Vertex(-1.0f, -1.0f, 1.0f);
            //gl.Vertex(1.0f, -1.0f, 1.0f);

            //gl.Color(1.0f, 1.0f, 0.0f);
            //gl.Vertex(1.0f, -1.0f, -1.0f);
            //gl.Vertex(-1.0f, -1.0f, -1.0f);
            //gl.Vertex(-1.0f, 1.0f, -1.0f);
            //gl.Vertex(1.0f, 1.0f, -1.0f);

            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f, 1.0f, 1.0f);
            //gl.Vertex(-1.0f, 1.0f, -1.0f);
            //gl.Vertex(-1.0f, -1.0f, -1.0f);
            //gl.Vertex(-1.0f, -1.0f, 1.0f);

            //gl.Color(1.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f, 1.0f, -1.0f);
            //gl.Vertex(1.0f, 1.0f, 1.0f);
            //gl.Vertex(1.0f, -1.0f, 1.0f);
            //gl.Vertex(1.0f, -1.0f, -1.0f);

            //gl.End();


        
            //  Flush OpenGL.
            gl.Flush();
        }
    }
}
