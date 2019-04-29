using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveVideoStreamAnalysis
{
    public partial class LiveVideoForm : Form
    {
        public LiveVideoForm()
        {
            InitializeComponent();
        }

        private void LiveVideoForm_Load(object sender, EventArgs e)
        {
            _grabber.FrameProvidedAction = image =>
            {
                var imgStream = image.ToMemoryStream(".jpg");
                var img = Image.FromStream(imgStream);
                UpdatePicture(frameOriginal, img);
            };
            _grabber.FrameProcessedAction = image =>
            {
                UpdateLabel(label1, _grabber.FpsProcessing.ToString("0.00"));
                UpdateLabel(label2, _grabber.FpsOrigin.ToString("0.00"));
                UpdatePicture(processedImage, image);
            };
            // Camera index
            _grabber.StartAsync(0).Wait();
        }

        /// <summary>
        /// Update text of the label on the form
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="text">Text</param>
        private void UpdateLabel(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                var d = new UpdateLabelCallback(UpdateLabel);
                Invoke(d, label, text);
            }
            else
            {
                label.Text = text;
            }
        }

        /// <summary>
        /// Update image on the form
        /// </summary>
        /// <param name="box">Picture box</param>
        /// <param name="img">Image</param>
        private void UpdatePicture(PictureBox box, Image img)
        {
            if (box.InvokeRequired)
            {
                var d = new UpdatePictureCallback(UpdatePicture);
                Invoke(d, box, img);
            }
            else
            {
                box.Image = img;
            }
        }

        private delegate void UpdatePictureCallback(PictureBox box, Image img);
        private delegate void UpdateLabelCallback(Label label, string text);
        private readonly FrameGrabber _grabber = new FrameGrabber();
    }
}
