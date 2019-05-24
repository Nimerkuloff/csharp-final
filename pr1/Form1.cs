using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace lab10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string[] sites;
        Bitmap bitmap;
        int iter;
        OpenFileDialog dialog;

        private void Button1_Click(object sender, EventArgs e)
        {
            this.MaximumSize = new Size(Size.Width, Size.Height);
            this.MinimumSize = new Size(Size.Width, Size.Height);
            webBrowser1.ScriptErrorsSuppressed = true;

            GetSites();

            for (; ; )
            {
                foreach (string site in sites)
                {
                    
                    webBrowser1.Navigate(site);
                    string siteName = site.Replace("http://", "");
                    string path = Path.Combine(Application.StartupPath, siteName + iter + ".jpg");

                    while (webBrowser1.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }
                    bitmap.Save(path);


                    if (iter > 0)
                    {
                        Bitmap img1 = new Bitmap(siteName + iter + ".jpg");
                        Bitmap img2 = new Bitmap(siteName + (iter - 1) + ".jpg");

                        if (GetDiffInPercents(img1, img2) > 1)
                        {

                            File.AppendAllText(dialog.FileName, "\n" + DateTime.Now.ToString() + " " + siteName + " ACTIVE");
                        }
                        else
                        {
                            File.AppendAllText(dialog.FileName, "\n" + DateTime.Now.ToString() + " " + siteName + "NO RESPONSE");
                        }
                    }
                }

                iter++;
                // webBrowser1.Hide();
                Thread.Sleep(5000);
            }
        }
        private void GetSites()
        {
            dialog = new OpenFileDialog();
            dialog.Filter = "Txt with sites (*.TXT)| *.txt";
            if (dialog.ShowDialog() == DialogResult.OK)//вызываем диалоговое окно и проверяем выбран лифайл
            {
                sites = File.ReadLines(dialog.FileName).ToArray();
            }
        }


        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            bitmap = new Bitmap(webBrowser1.Document.Body.ScrollRectangle.Width, webBrowser1.Document.Body.ScrollRectangle.Height);
            webBrowser1.DrawToBitmap(bitmap, new Rectangle(0, 0, webBrowser1.Document.Body.ScrollRectangle.Width, webBrowser1.Document.Body.ScrollRectangle.Height));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            webBrowser1.Dispose();
        }


        private float GetDiffInPercents(Bitmap img1, Bitmap img2)
        {

            float diff = 0;

            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    Color pixel1 = img1.GetPixel(x, y);
                    Color pixel2 = img2.GetPixel(x, y);

                    diff += Math.Abs(pixel1.R - pixel2.R);
                    diff += Math.Abs(pixel1.G - pixel2.G);
                    diff += Math.Abs(pixel1.B - pixel2.B);
                }
            }
            return 100 * (diff / 255) / (img1.Width * img1.Height * 3);
        }
    }
}



