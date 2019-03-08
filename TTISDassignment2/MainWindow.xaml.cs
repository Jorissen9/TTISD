using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Media.Media3D;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace TTISDassignment2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private Game game;

        private List<Point> m_calibPoints = new List<Point>(); //2d calibration points
        private List<SkeletonPoint> m_skeletonCalibPoints = new List<SkeletonPoint>(); //3d skeleton points

        private SkeletonPoint posPlayer1, posPlayer2;

        private Matrix3D m_groundPlaneTransform; //step 2 transform
        private Emgu.CV.Matrix<double> m_transform; //step 3 transform

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
            Closed += EndGame;

            this.game = new Game();
            this.game.Show();
        }

        private void EndGame(object sender, EventArgs e)
        {
            game.Close();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var sensorStatus = new KinectSensorChooser();

            sensorStatus.KinectChanged += KinectSensorChooserKinectChanged;

            kinectChooser.KinectSensorChooser = sensorStatus;
            sensorStatus.Start();

            btnPlaying.IsEnabled = false;
            btnCalibrateNext.IsEnabled = false;

            m_calibPoints = game.getCorners();
        }

        private void KinectSensorChooserKinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (sensor != null)
                sensor.SkeletonFrameReady -= KinectSkeletonFrameReady;

            sensor = e.NewSensor;

            if (sensor == null)
                return;

            switch (Convert.ToString(e.NewSensor.Status))
            {
                case "Connected":
                    txtStatus.Content = "Connected";
                    break;
                case "Disconnected":
                    txtStatus.Content = "Disconnected";
                    break;
                case "Error":
                    txtStatus.Content = "Error";
                    break;
                case "NotReady":
                    txtStatus.Content = "Not Ready";
                    break;
                case "NotPowered":
                    txtStatus.Content = "Not Powered";
                    break;
                case "Initializing":
                    txtStatus.Content = "Initialising";
                    break;
                default:
                    txtStatus.Content = "Undefined";
                    break;
            }

            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += KinectSkeletonFrameReady;
        }


        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var skeletons = new Skeleton[0];

            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length == 0)
            {
                return;
            }

            var player1 = skeletons.FirstOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);
            var player2 = skeletons.LastOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);

            if (player1 != null)
            {
                posPlayer1 = player1.Position;
                txtP1SkelPosX.Text = posPlayer1.X.ToString(CultureInfo.InvariantCulture);
                txtP1SkelPosY.Text = posPlayer1.Y.ToString(CultureInfo.InvariantCulture);
                txtP1SkelPosZ.Text = posPlayer1.Z.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                player1 = null;
                txtP1SkelPosX.Text = "?";
                txtP1SkelPosY.Text = "?";
                txtP1SkelPosZ.Text = "?";
            }

            if (player2 != null && player1.TrackingId != player2.TrackingId)
            {
                posPlayer2 = player2.Position;
                txtP2SkelPosX.Text = posPlayer2.X.ToString(CultureInfo.InvariantCulture);
                txtP2SkelPosY.Text = posPlayer2.Y.ToString(CultureInfo.InvariantCulture);
                txtP2SkelPosZ.Text = posPlayer2.Z.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                player2 = null;
                txtP2SkelPosX.Text = "?";
                txtP2SkelPosY.Text = "?";
                txtP2SkelPosZ.Text = "?";
            }


            //for (var i = 0; i < 4; i++)
            //{
            //    // Move to next...
            //    // Wait on confirm or time

            //    // Add current position
            //    m_skeletonCalibPoints.Add(skel.Position);
            //}

            //calibrate();

            switch (this.game.state)
            {
                case GameState.START:
                    break;

                case GameState.IS_CALIBRATING_POINT_1:
                    break;

                case GameState.IS_CALIBRATING_POINT_2:
                    break;

                case GameState.IS_CALIBRATING_POINT_3:

                    break;

                case GameState.IS_CALIBRATING_POINT_4:

                    break;

                case GameState.DONE_CALIBRATING:
                    break;

                case GameState.PLAYING:
                    game.SetPositions(kinectToProjectionPoint(posPlayer1), kinectToProjectionPoint(posPlayer2));

                    break;

                case GameState.PLAYER_1_WINS:
                    break;

                case GameState.PLAYER_2_WINS:
                    break;

                default:
                    break;
            }

        }

        private void Calibrate()
        {
            if (m_skeletonCalibPoints.Count == m_calibPoints.Count)
            {
                //seketon 3D positions --> 3d positions in depth camera
                Point3D p0 = convertSkeletonPointToDepthPoint(m_skeletonCalibPoints[0]);
                Point3D p1 = convertSkeletonPointToDepthPoint(m_skeletonCalibPoints[1]);
                Point3D p2 = convertSkeletonPointToDepthPoint(m_skeletonCalibPoints[2]);
                Point3D p3 = convertSkeletonPointToDepthPoint(m_skeletonCalibPoints[3]);

                //3d positions depth camera --> positions on a 2D plane
                Vector3D v1 = p1 - p0;
                v1.Normalize();

                Vector3D v2 = p2 - p0;
                v2.Normalize();

                Vector3D planeNormalVec = Vector3D.CrossProduct(v1, v2);
                planeNormalVec.Normalize();

                Vector3D resultingPlaneNormal = new Vector3D(0, 0, 1);
                m_groundPlaneTransform = Util.make_align_axis_matrix(resultingPlaneNormal, planeNormalVec);

                Point3D p0OnPlane = m_groundPlaneTransform.Transform(p0);
                Point3D p1OnPlane = m_groundPlaneTransform.Transform(p1);
                Point3D p2OnPlane = m_groundPlaneTransform.Transform(p2);
                Point3D p3OnPlane = m_groundPlaneTransform.Transform(p3);

                //2d plane positions --> exact 2d square on screen (using perspective transform)
                System.Drawing.PointF[] src = new System.Drawing.PointF[4];
                src[0] = new System.Drawing.PointF((float)p0OnPlane.X, (float)p0OnPlane.Y);
                src[1] = new System.Drawing.PointF((float)p1OnPlane.X, (float)p1OnPlane.Y);
                src[2] = new System.Drawing.PointF((float)p2OnPlane.X, (float)p2OnPlane.Y);
                src[3] = new System.Drawing.PointF((float)p3OnPlane.X, (float)p3OnPlane.Y);

                System.Drawing.PointF[] dest = new System.Drawing.PointF[4];
                dest[0] = new System.Drawing.PointF((float)m_calibPoints[0].X, (float)m_calibPoints[0].Y);
                dest[1] = new System.Drawing.PointF((float)m_calibPoints[1].X, (float)m_calibPoints[1].Y);
                dest[2] = new System.Drawing.PointF((float)m_calibPoints[2].X, (float)m_calibPoints[2].Y);
                dest[3] = new System.Drawing.PointF((float)m_calibPoints[3].X, (float)m_calibPoints[3].Y);

                Emgu.CV.Mat transform = Emgu.CV.CvInvoke.GetPerspectiveTransform(src, dest);

                m_transform = new Emgu.CV.Matrix<double>(transform.Rows, transform.Cols, transform.NumberOfChannels);
                transform.CopyTo(m_transform);

                //test to see if resulting perspective transform is correct
                //tResultx should be same as points in m_calibPoints
                Point tResult0 = kinectToProjectionPoint(m_skeletonCalibPoints[0]);
                Point tResult1 = kinectToProjectionPoint(m_skeletonCalibPoints[1]);
                Point tResult2 = kinectToProjectionPoint(m_skeletonCalibPoints[2]);
                Point tResult3 = kinectToProjectionPoint(m_skeletonCalibPoints[3]);

                txtCalib.Text = tResult0.ToString() + "; " +
                                tResult1.ToString() + "; " +
                                tResult2.ToString() + "; " +
                                tResult3.ToString();
            }
        }

        private Point3D convertSkeletonPointToDepthPoint(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint imgPt = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);

            return new Point3D(imgPt.X, imgPt.Y, imgPt.Depth);
        }

        private Point kinectToProjectionPoint(SkeletonPoint point)
        {
            DepthImagePoint depthP = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(point, DepthImageFormat.Resolution640x480Fps30);
            Point3D p = new Point3D(depthP.X, depthP.Y, depthP.Depth);

            Point3D pOnGroundPlane = m_groundPlaneTransform.Transform(p);

            System.Drawing.PointF[] testPoint = new System.Drawing.PointF[1];
            testPoint[0] = new System.Drawing.PointF((float)pOnGroundPlane.X, (float)pOnGroundPlane.Y);

            System.Drawing.PointF[] resultPoint = Emgu.CV.CvInvoke.PerspectiveTransform(testPoint, m_transform);

            return new Point(resultPoint[0].X, resultPoint[0].Y);
        }

        private void BtnCalibrate_Click(object sender, RoutedEventArgs e)
        {
            this.game.state = GameState.IS_CALIBRATING_POINT_1;

            btnCalibrateNext.IsEnabled = true;
            btnPlaying.IsEnabled = false;
        }

        private void BtnCalibrateNext_Click(object sender, RoutedEventArgs e)
        {
            if (game.state >= GameState.IS_CALIBRATING_POINT_1
                && game.state <= GameState.IS_CALIBRATING_POINT_4)
            {
                m_skeletonCalibPoints.Add(posPlayer1);

                this.game.state = (GameState)((int)game.state + 1);

                if (game.state > GameState.IS_CALIBRATING_POINT_4)
                {
                    btnCalibrateNext.IsEnabled = false;
                    btnPlaying.IsEnabled = true;

                    if(sensor != null)
                    {
                        this.Calibrate();
                    }
                }
            }
            else
            {
                btnCalibrateNext.IsEnabled = false;
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            btnPlaying.IsEnabled = false;
            this.game.state = GameState.PLAYING;
        }
    }
}
