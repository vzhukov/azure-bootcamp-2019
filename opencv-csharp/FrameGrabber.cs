using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Emgu.CV;
using CascadeClassifier = Emgu.CV.CascadeClassifier;

namespace OpenCVLiveVideoStreamAnalysis
{
    public class FrameGrabber
    {
        public FrameGrabber()
        {
            Detector = new CascadeClassifier("haarcascade_profileface.xml");
        }
        public Action<Mat> FrameProvidedAction { get; set; }
        public Action<Image> FrameProcessedAction { get; set; }
        private CascadeClassifier Detector { get; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public async Task StartAsync(int cameraIndex = 0)
        {
            if (Reader != null && Reader.CaptureSource == VideoCapture.CaptureModuleType.Camera)
            {
                return;
            }

            await StopAsync().ConfigureAwait(false);
            Reader = new VideoCapture(cameraIndex);

            Fps = 30;
            Width = Reader.Width;
            Height = Reader.Height;
            Start(TimeSpan.FromSeconds(1 / Fps), () => DateTime.Now);
        }

        protected void Start(TimeSpan frameGrabDelay, Func<DateTime> timestampFn)
        {
            ProducerTask = Task.Factory.StartNew(() =>
            {
                while (!Stopping)
                {
                    var image = Reader.QueryFrame();
                    FrameProvidedAction?.Invoke(image);
                    if (!Analyzing)
                    {
                        ProcessFrameAsync(image);
                    }
                }
                Reader.Dispose();
                Reader = null;

            }, TaskCreationOptions.LongRunning);
        }

        private async void ProcessFrameAsync(Mat image)
        {
            Analyzing = true;
            await Task.Run(() =>
            {

                var items = Detector.DetectMultiScale(image);
                Image img;
                using (var ms = new MemoryStream())
                {
                    image.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img = Image.FromStream(ms);
                    foreach (var x in items)
                    {
                        DrawRectangle(img, x);
                    }
                }
                FrameProcessedAction?.Invoke(img);
            });
            Analyzing = false;
        }

        public async Task StopAsync()
        {
            Stopping = true;
            if (ProducerTask != null)
            {
                await ProducerTask;
                ProducerTask = null;
            }
            Stopping = false;
        }

        /// <summary>
        /// Draw rectangle across an object
        /// </summary>
        /// <param name="image">Frame</param>
        /// <param name="rectangle">Rectangle</param>
        /// <param name="text">Caption</param>
        private static void DrawRectangle(Image image, Rectangle rectangle, string text = "Face")
        {
            var rect = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            var graphics = Graphics.FromImage(image);
            graphics.DrawRectangle(new Pen(Color.Blue, 3), rect);
            var font = new Font(FontFamily.GenericSansSerif, 12);
            var textSize = graphics.MeasureString(text, font);
            var height = Convert.ToInt32(textSize.Height * 1.1);
            var width = Convert.ToInt32(textSize.Width * 1.1);
            graphics.FillRectangle(Brushes.Blue, rectangle.X, rectangle.Y, width, height);
            graphics.DrawString(text, font, Brushes.Beige, rectangle.X, rectangle.Y);
        }


        protected VideoCapture Reader;
        protected bool Stopping;
        protected bool Analyzing;
        protected Task ProducerTask;
        protected double Fps;
    }
}