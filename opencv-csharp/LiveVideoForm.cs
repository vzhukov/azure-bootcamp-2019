using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OpenCVLiveVideoStreamAnalysis
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
                using (var ms = new MemoryStream())
                {
                    image.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    var img = Image.FromStream(ms);
                    UpdatePicture(originFrame, img);
                }
            };
            _grabber.FrameProcessedAction = image =>
            {
                UpdatePicture(processedFrame, image);
            };
            _grabber.StartAsync(1).Wait();
        }

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
        private readonly FrameGrabber _grabber = new FrameGrabber();
    }
}
