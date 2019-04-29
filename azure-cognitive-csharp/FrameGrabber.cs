using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using OpenCvSharp;

namespace LiveVideoStreamAnalysis
{
    public class FrameGrabber
    {
        /// <summary>
        /// Action to invoke once new frame is captured
        /// </summary>
        public Action<Mat> FrameProvidedAction { get; set; }

        /// <summary>
        /// Action to invoke once frame is processed
        /// </summary>
        public Action<Image> FrameProcessedAction { get; set; }
        public double FpsOrigin { get; set; }
        public double FpsProcessing { get; set; }

        /// <summary>
        /// Start analysis
        /// </summary>
        /// <param name="cameraIndex"></param>
        /// <returns></returns>
        public async Task StartAsync(int cameraIndex = 0)
        {
            if (Reader != null && Reader.CaptureType == CaptureType.Camera)
            {
                return;
            }

            await StopAsync().ConfigureAwait(false);
            Reader = new VideoCapture(cameraIndex);
            Start();
        }

        /// <summary>
        /// Stop analysis
        /// </summary>
        /// <returns></returns>
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

        private void Start()
        {
            ProducerTask = Task.Factory.StartNew(() =>
            {
                var sw = new Stopwatch();
                while (!Stopping)
                {
                    sw.Reset();
                    sw.Start();
                    var image = new Mat();
                    var success = Reader.Read(image);
                    if (!success)
                    {
                        if (Reader.CaptureType == CaptureType.File)
                        {
                            StopAsync().Wait();
                            break;
                        }
                        continue;
                    }
                    FrameProvidedAction?.Invoke(image);
                    sw.Stop();
                    FpsOrigin = 1000.0 / sw.ElapsedMilliseconds;

                    if (!Analyzing)
                    {
                        ProcessFrameAsync(image.ToBytes(".jpg"));
                    }
                }
                Reader.Dispose();
                Reader = null;

            }, TaskCreationOptions.LongRunning);
        }

        private async void ProcessFrameAsync(byte[] bytes)
        {
            Analyzing = true;
            var sw = new Stopwatch();
            using (var ms = new MemoryStream(bytes))
            {
                sw.Reset();
                sw.Start();

                // Send image to Azure Cognitive service to detect objects
                var task = await _visionClient.DetectObjectsInStreamAsync(ms);

                sw.Stop();

                // Draw rectangle around all detected objects
                var items = task?.Objects;
                if (items != null)
                {
                    using (var imgms = new MemoryStream(bytes))
                    {
                        var image = Image.FromStream(imgms);

                        foreach (var x in items)
                        {
                            if (x.Rectangle == null)
                            {
                                continue;
                            }
                            var text = x.ObjectProperty;
                            DrawRectangle(image, x.Rectangle, text);
                        }
                        FpsProcessing = 1000.0 / sw.ElapsedMilliseconds;
                        FrameProcessedAction?.Invoke(image);
                    }
                }
            }
            Analyzing = false;
        }

        /// <summary>
        /// Draw rectangle across an object
        /// </summary>
        /// <param name="image">Frame</param>
        /// <param name="rectangle">Rectangle</param>
        /// <param name="text">Caption</param>
        private static void DrawRectangle(Image image, BoundingRect rectangle, string text)
        {
            var rect = new Rectangle(rectangle.X, rectangle.Y, rectangle.W, rectangle.H);
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

        // Azure Cognitive Service client
        readonly ComputerVisionClient _visionClient = new ComputerVisionClient(
            new ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["AzureCognitiveKey"]))
        {
            Endpoint = ConfigurationManager.AppSettings["AzureCognitiveEndpoint"]
        };

    }
}