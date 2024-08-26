using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace tfmStandalone
{
	public partial class ScreenShotOverlayWindow : Window, IComponentConnector
	{
        public EventHandler GifStartedRecording;
        public EventHandler GifFinishedRecording;
        public volatile bool IsRecording;
        public volatile bool SaveRecording;
        private double _x;
        private double _y;
        private bool _isMouseDown;
        private bool _isTakingGif;
        private GameSettings GameSettings { get; }
		public ScreenShotOverlayWindow(GameSettings gameSettings)
		{
			this.InitializeComponent();
			this.GameSettings = gameSettings;
		}

		public void Show(bool isTakingGif)
		{
			this._isTakingGif = isTakingGif;
			base.Show();
		}

		protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.CaptureCanvas.Children.Clear();
				base.Hide();
				this._x = (this._y = 0.0);
				this._isMouseDown = false;
			}
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this._isMouseDown = true;
			this._x = e.GetPosition(null).X;
			this._y = e.GetPosition(null).Y;
		}

		private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (!this._isMouseDown)
			{
				return;
			}
			double x = e.GetPosition(null).X;
			double y = e.GetPosition(null).Y;
			System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
			SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.White);
			rectangle.Stroke = solidColorBrush;
			rectangle.Fill = solidColorBrush;
			rectangle.StrokeThickness = 1.0;
			rectangle.Width = Math.Abs(x - this._x);
			rectangle.Height = Math.Abs(y - this._y);
			this.CaptureCanvas.Children.Clear();
			this.CaptureCanvas.Children.Add(rectangle);
			Canvas.SetLeft(rectangle, (x > this._x) ? this._x : x);
			Canvas.SetTop(rectangle, (y > this._y) ? this._y : y);
			if (e.LeftButton == MouseButtonState.Released)
			{
				double width = rectangle.Width;
				double height = rectangle.Height;
				double num = (x > this._x) ? this._x : x;
				double num2 = (y > this._y) ? this._y : y;
				System.Windows.Point point = base.PointToScreen(new System.Windows.Point((double)((int)num), (double)((int)num2)));
				this.CaptureCanvas.Children.Clear();
				base.Hide();
				if (width > 50.0 && height > 50.0)
				{
					this.CaptureScreen(point.X, point.Y, width, height);
				}
				this._x = (this._y = 0.0);
				this._isMouseDown = false;
			}
		}

		private void CaptureScreen(double x, double y, double width, double height)
		{
			int ix = Convert.ToInt32(x);
			int iy = Convert.ToInt32(y);
			int iw = Convert.ToInt32(width);
			int ih = Convert.ToInt32(height);
			if (this._isTakingGif)
			{
				string tempDir = System.IO.Path.GetTempPath() + "\\tfm_snapshot\\";
				if (Directory.Exists(tempDir))
				{
					Directory.Delete(tempDir, true);
				}
				Directory.CreateDirectory(tempDir);
				EventHandler gifStartedRecording = this.GifStartedRecording;
				if (gifStartedRecording != null)
				{
					gifStartedRecording(this, EventArgs.Empty);
				}
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs args)
				{
					int num = 0;
					while (DateTime.UtcNow.AddSeconds((double)this.GameSettings.GifLength) > DateTime.UtcNow && this.IsRecording)
					{
						Bitmap bitmap2 = new Bitmap(iw, ih, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
						using (Graphics graphics = Graphics.FromImage(bitmap2))
						{
							graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih), CopyPixelOperation.SourceCopy);
						}
						FileStream fileStream = new FileStream(tempDir + num.ToString("00000") + ".png", FileMode.OpenOrCreate);
						bitmap2.Save(fileStream, ImageFormat.Png);
						bitmap2.Dispose();
						fileStream.Close();
						num++;
						Thread.Sleep(50);
					}
					if (this.SaveRecording)
					{
						GifBitmapEncoder gifBitmapEncoder = new GifBitmapEncoder();
						List<FileStream> list = new List<FileStream>();
						string[] files = Directory.GetFiles(tempDir, "*.png", SearchOption.TopDirectoryOnly);
						for (int i = 0; i < files.Length; i++)
						{
							FileStream fileStream2 = new FileStream(files[i], FileMode.Open);
							BitmapFrame item = BitmapFrame.Create(fileStream2);
							list.Add(fileStream2);
							gifBitmapEncoder.Frames.Add(item);
						}
						using (FileStream fileStream3 = new FileStream(tempDir + "temp.gif", FileMode.OpenOrCreate))
						{
							gifBitmapEncoder.Save(fileStream3);
						}
						foreach (FileStream fileStream4 in list)
						{
							fileStream4.Close();
						}
					}
				};
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
				{
					TaskHelpers.UiInvoke(delegate
					{
						EventHandler gifFinishedRecording = this.GifFinishedRecording;
						if (gifFinishedRecording == null)
						{
							return;
						}
						gifFinishedRecording(this, EventArgs.Empty);
					});
					if (this.SaveRecording)
					{
						SaveFileDialog saveFileDialog2 = new SaveFileDialog
						{
							DefaultExt = "gif",
							Filter = "GIF Files|*.gif"
						};
						if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							File.Copy(tempDir + "temp.gif", saveFileDialog2.FileName, true);
						}
					}
					if (Directory.Exists(tempDir))
					{
						Directory.Delete(tempDir, true);
					}
				};
				this.IsRecording = true;
				this.SaveRecording = true;
				backgroundWorker.RunWorkerAsync();
				return;
			}
			Bitmap bitmap = new Bitmap(iw, ih, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Graphics.FromImage(bitmap).CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih), CopyPixelOperation.SourceCopy);
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = "png",
				Filter = "png Files|*.png"
			};
			if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
			}
		}
	}
}
