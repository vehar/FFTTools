﻿using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Emgu.CV;
using Emgu.CV.Structure;
using FFTTools;

namespace fftblinder
{
    /// <summary>
    ///     Main application window
    /// </summary>
    public partial class MainForm : RibbonForm
    {
        public MainForm()
        {
            InitializeComponent();
            InitSkinGallery();
            openFileDialog1.Filter =
                saveFileDialog1.Filter =
                    @"Bitmap Images (*.bmp)|*.bmp|All Files (*.*)|*.*";
        }

        private void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

        private void openFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                var bitmap = new Bitmap(openFileDialog1.FileName);
                pictureEdit1.Image = bitmap;
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void saveAsFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) return;
                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                bitmap.Save(saveFileDialog1.FileName);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void blur_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) return;
                Size size = bitmap.Size;
                using (var blinderDialog = new BlinderDialog(new Size(size.Width*3/4, size.Height*3/4)))
                {
                    if (blinderDialog.ShowDialog() != DialogResult.OK) return;

                    Size blinderSize = blinderDialog.BlinderSize;

                    using (var builder = new BlurBuilder(blinderSize))
                        pictureEdit1.Image = builder.Blur(bitmap);
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void iAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var aboutBox = new AboutBox())
                aboutBox.ShowDialog();
        }

        private void sharp_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) return;
                Size size = bitmap.Size;
                using (var blinderDialog = new BlinderDialog(new Size(size.Width*3/4, size.Height*3/4)))
                {
                    if (blinderDialog.ShowDialog() != DialogResult.OK) return;

                    Size blinderSize = blinderDialog.BlinderSize;

                    using (var builder = new SharpBuilder(blinderSize))
                        pictureEdit1.Image = builder.Sharp(bitmap);
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) return;
                using (var image = new Image<Bgr, double>(bitmap))
                {
                    double[,,] data = image.Data;
                    int length = data.Length;
                    var bytes = new byte[length];
                    Buffer.BlockCopy(data, 0, bytes, 0, length);
                    double average = bytes.Average(x => (double) x);
                    double delta = Math.Sqrt(bytes.Average(x => (double) x*x) - average*average);
                    double minValue = bytes.Min(x => (double)x);
                    double maxValue = bytes.Max(x => (double)x);
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Format("Length {0}", length));
                    sb.AppendLine(string.Format("Average {0}", average));
                    sb.AppendLine(string.Format("Delta {0}", delta));
                    sb.AppendLine(string.Format("minValue {0}", minValue));
                    sb.AppendLine(string.Format("maxValue {0}", maxValue));
                    siInfo.Caption = sb.ToString();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}