using System;
using System.Timers;
using System.Windows;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using InputSimulatorEx;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Windows.Forms;

namespace AutoAcceptLoL
{
    public partial class MainWindow : Window
    {
        private readonly System.Timers.Timer _checkTimer;

        public MainWindow()
        {
            InitializeComponent();
            _checkTimer = new System.Timers.Timer(1000); // 1 seconde
            _checkTimer.Elapsed += CheckTimer_Elapsed;
        }

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private void AutoAcceptCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Statut : Actif";
            _checkTimer.Start();
        }

        private void AutoAcceptCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Statut : Inactif";
            _checkTimer.Stop();
        }

        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = "Statut : Vérification...";
            });

            if (IsMatchFound(out System.Drawing.Point match))
            {
                Dispatcher.Invoke(() =>
                {
                    using var template = new Image<Bgr, byte>("Assets/accept_button.png");
                    int offsetX = template.Width / 2;
                    int offsetY = template.Height / 2;

                    StatusText.Text = $"Bouton détecté à {match.X}, {match.Y} — clic";
                    SetCursorPos(match.X + offsetX, match.Y + offsetY);
                    Thread.Sleep(100);
                    var sim = new InputSimulator();
                    sim.Mouse.LeftButtonClick();
                });

            }
        }

        private bool IsMatchFound(out System.Drawing.Point matchLocation)
        {
            matchLocation = new System.Drawing.Point();

            try
            {
                // Utilisation de WPF pur pour la résolution d'écran
                int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
                int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
                var bounds = new System.Drawing.Rectangle(0, 0, screenWidth, screenHeight);

                using var screenBmp = new Bitmap(bounds.Width, bounds.Height);
                using (Graphics g = Graphics.FromImage(screenBmp))
                {
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                }

                using var template = new Image<Bgr, byte>("Assets/accept_button.png");
                using var sourceMat = screenBmp.ToMat(); // nécessite Emgu.CV.Bitmap
                using var source = sourceMat.ToImage<Bgr, byte>();

                using var result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed);
                result.MinMax(out double[] minValues, out double[] maxValues, out System.Drawing.Point[] minLoc, out System.Drawing.Point[] maxLoc);

                if (maxValues[0] > 0.85)
                {
                    matchLocation = maxLoc[0];
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la détection : {ex.Message}");
            }

            return false;
        }

    }
}
