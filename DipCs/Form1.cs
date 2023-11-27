using ImageProcess2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;
using HNUDIP;

namespace DipCs
{
    public partial class Form1 : Form
    {
        Bitmap loadedImage, resultImage;
        Bitmap imageB, imageA, colorgreen, resultImage2;
        Device[] mgadevice;
        Bitmap processed;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loadedImage = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loadedImage;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog2.FileName);
            pictureBox1.Image = imageB;
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog3.FileName);
            pictureBox2.Image = imageA;
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            resultImage.Save(saveFileDialog1.FileName);
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load Image
            openFileDialog1.ShowDialog();
        }

        private void loadImageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void loadBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save Image
            saveFileDialog1.ShowDialog();
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Basic Copy
            resultImage = new Bitmap(loadedImage.Width, loadedImage.Height);
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    Color pixel = loadedImage.GetPixel(x, y);
                    resultImage.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = resultImage;
        }

        private void greyScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Grey Scale
            resultImage = new Bitmap(loadedImage.Width, loadedImage.Height);
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    Color pixel = loadedImage.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    resultImage.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }
            pictureBox2.Image = resultImage;
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Invert
            resultImage = new Bitmap(loadedImage.Width, loadedImage.Height);
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    Color pixel = loadedImage.GetPixel(x, y);
                    resultImage.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }
            pictureBox2.Image = resultImage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mgadevice = DeviceManager.GetAllDevices();
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mgadevice[0].ShowWindow(pictureBox1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IDataObject data;
            Image bmap;
            mgadevice[0].Sendmessage();
            data = Clipboard.GetDataObject();
            bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));
            Bitmap b = new Bitmap(bmap); // loaded
            //BitmapFilter.GrayScale(b);
            BitmapFilter.TimeWarp(b, 20, true);
            // Subtract(ref Bitmap a, ref Bitmap b,ref Bitmap result, int value)

            pictureBox2.Image = b;
            /*
            Color pixel;
            int greyvalue;
            processed = new Bitmap(b.Width, b.Height);
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    pixel = loadedImage.GetPixel(x, y);
                    greyvalue = (byte)((pixel.R + pixel.G + pixel.B) / 3);
                    processed.SetPixel(x, y, Color.FromArgb(greyvalue, greyvalue, greyvalue));
                }
            }
            pictureBox2.Image = processed;
            */
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mgadevice[0].Stop();
        }

        private void subtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageProcess.Subtract(ref imageB, ref imageA, ref resultImage2, 20);
            pictureBox3.Image = resultImage2;
        }

        private void timeWarpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Histogram
            resultImage = new Bitmap(loadedImage.Width, loadedImage.Height);
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    Color pixel = loadedImage.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    resultImage.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }
            Color sample;
            int[] histogramData = new int[256];
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    sample = resultImage.GetPixel(x, y); // 0-255 either R, G, or B
                    //histogramData[sample.R] = histogramData[sample.R] + 1;
                    //histogramData[sample.R] += 1;
                    histogramData[sample.R]++;
                }
            }
            Bitmap myData = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 800; y++)
                {
                    myData.SetPixel(x, y, Color.White);
                }
            }

            //plot histogramData
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histogramData[x] / 5, 800); y++)
                {
                    myData.SetPixel(x, 799 - y, Color.Black);
                }
            }
            pictureBox2.Image = myData;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Sepia
            resultImage = new Bitmap(loadedImage.Width, loadedImage.Height);
            for (int x = 0; x < loadedImage.Width; x++)
            {
                for (int y = 0; y < loadedImage.Height; y++)
                {
                    Color pixel = loadedImage.GetPixel(x, y);
                    int sepiaR = (int)Math.Min(255, (0.393 * pixel.R) + (0.769 * pixel.G) + (0.189 * pixel.B));
                    int sepiaG = (int)Math.Min(255, (0.349 * pixel.R) + (0.686 * pixel.G) + (0.168 * pixel.B));
                    int sepiaB = (int)Math.Min(255, (0.272 * pixel.R) + (0.534 * pixel.G) + (0.131 * pixel.B));

                    resultImage.SetPixel(x, y, Color.FromArgb(sepiaR, sepiaG, sepiaB));
                }
            }
            pictureBox2.Image = resultImage;
        }

        private void subtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultImage2 = new Bitmap(imageB.Width, imageB.Height);
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R +  mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            for(int x = 0; x < imageB.Width; x++)
            {
                for(int y = 0; y < imageB.Height; y++)
                {
                    Color pixel = imageB.GetPixel(x, y);
                    Color backpixel = imageA.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > threshold)
                        resultImage2.SetPixel(x, y, pixel);
                    else
                        resultImage2.SetPixel(x, y, backpixel);
                }
            }
            pictureBox3.Image = resultImage2;
        }

        public Form1()
        {
            InitializeComponent();
            this.Width = 1067;
            this.Height = 385;
        }
    }
}