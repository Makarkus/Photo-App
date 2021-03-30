// <copyright file="Form1.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SMPImage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    [DataContract]
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [DataMember]
        private List<Bitmap> bitmaps = new List<Bitmap>();
        private Random random = new Random();

        public Form1()
        {
            this.InitializeComponent();
        }

        public async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var sw = Stopwatch.StartNew();
                this.menuStrip1.Enabled = this.trackBar1.Enabled = false;
                this.pictureBox1.Image = null;
                this.bitmaps.Clear();
                var bitmap = new Bitmap(this.openFileDialog1.FileName);
                await Task.Run(() => { this.RunProccessing(bitmap); });
                this.menuStrip1.Enabled = this.trackBar1.Enabled = true;
                sw.Stop();
                this.Text = sw.Elapsed.ToString();
                Log.Info("Menu item was clicked");
            }
        }

        public void RunProccessing(Bitmap bitmap)
        {
            var pixels = this.GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;

            var currentPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);
            for (int i = 1; i < this.trackBar1.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = this.random.Next(pixels.Count);
                    currentPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }

                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                foreach (var pixel in currentPixelsSet)
                {
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                }

                this.bitmaps.Add(currentBitmap);
                this.Invoke(new Action(() =>
                {
                    this.Text = $"{i} %";
                }));
                Log.Info("Proccess is running");
            }

            this.bitmaps.Add(bitmap);
        }

        public List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point() { X = x, Y = y },
                    });
                }
            }

            Log.Info("Get pixels");
            return pixels;
        }

        public void TrackBar1_Scroll(object sender, EventArgs e)
        {
            if (this.bitmaps == null || this.bitmaps.Count == 0)
            {
                return;
            }

            this.pictureBox1.Image = this.bitmaps[this.trackBar1.Value - 1];
        }

        public void Serialize(object sender, EventArgs e)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Bitmap>));
            Stream stream = new FileStream("data1.json", FileMode.OpenOrCreate);
            jsonSerializer.WriteObject(stream, this.bitmaps);
            stream.Close();

            stream = new FileStream("data1.json", FileMode.Open);
            List<Bitmap> newJson = (List<Bitmap>)jsonSerializer.ReadObject(stream);
            stream.Close();
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }
    }
}
