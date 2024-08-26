using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace tfmStandalone
{
	public partial class FlashPlayer : UserControl, IComponentConnector
	{
        public EventHandler<sbyte[]> EncryptionKeyReceived;
        public EventHandler<int[]> EncryptionVectorReceived;
        private FlashPlayer.AlignmentModeEnum _alignmentMode;
        private FlashPlayer.ZoomModeEnum _zoomMode;
        private FlashPlayer.QualityEnum _quality;
        private GameSettings _gameSettings;

        public FlashPlayer.AlignmentModeEnum AlignmentMode
		{
			get
			{
				return this._alignmentMode;
			}
			set
			{
				this._alignmentMode = value;
				if (this.WebBrowser.IsLoaded)
				{
					string text = string.Empty;
					switch (this._alignmentMode)
					{
					case FlashPlayer.AlignmentModeEnum.Center:
						text = "";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.CenterLeft:
						text = "L";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.CenterRight:
						text = "R";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.TopCenter:
						text = "T";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.TopLeft:
						text = "TL";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.TopRight:
						text = "TR";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.BottomLeft:
						text = "BL";
						goto IL_9E;
					case FlashPlayer.AlignmentModeEnum.BottomRight:
						text = "BR";
						goto IL_9E;
					}
					text = "B";
					IL_9E:
					this.WebBrowser.InvokeScript("SetAlignment", new object[]
					{
						text
					});
				}
				if (this.GameSettings != null)
				{
					this.GameSettings.AlignmentMode = this.AlignmentMode;
				}
			}
		}

		public FlashPlayer.ZoomModeEnum ZoomMode
		{
			get
			{
				return this._zoomMode;
			}
			set
			{
				this._zoomMode = value;
				if (this.WebBrowser.IsLoaded)
				{
					string text = string.Empty;
					FlashPlayer.ZoomModeEnum zoomMode = this._zoomMode;
					if (zoomMode != FlashPlayer.ZoomModeEnum.StretchToFit)
					{
						if (zoomMode == FlashPlayer.ZoomModeEnum.Stretch)
						{
							text = "exactFit";
						}
						else
						{
							text = "noScale";
						}
					}
					else
					{
						text = "showAll";
					}
					this.WebBrowser.InvokeScript("SetZoom", new object[]
					{
						text
					});
				}
				if (this.GameSettings != null)
				{
					this.GameSettings.ZoomMode = this.ZoomMode;
				}
			}
		}

		public FlashPlayer.QualityEnum Quality
		{
			get
			{
				return this._quality;
			}
			set
			{
				this._quality = value;
				if (this.WebBrowser.IsLoaded)
				{
					string text = string.Empty;
					FlashPlayer.QualityEnum quality = this._quality;
					if (quality != FlashPlayer.QualityEnum.Low)
					{
						if (quality != FlashPlayer.QualityEnum.Medium)
						{
							text = "high";
						}
						else
						{
							text = "medium";
						}
					}
					else
					{
						text = "low";
					}
					this.WebBrowser.InvokeScript("SetQuality", new object[]
					{
						text
					});
				}
				if (this.GameSettings != null)
				{
					this.GameSettings.Quality = this._quality;
				}
			}
		}

		public WindowService WindowService { get; set; }
        private bool Loading { get; set; }
        private string TempFile { get; set; }

        public GameSettings GameSettings
		{
			get
			{
				return this._gameSettings;
			}
			set
			{
				this._gameSettings = value;
				this.AlignmentMode = this.GameSettings.AlignmentMode;
				this.ZoomMode = this.GameSettings.ZoomMode;
				this.Quality = this.GameSettings.Quality;
			}
		}

		public FlashPlayer()
		{
			this.InitializeComponent();
			this.WebBrowser.ObjectForScripting = new ScriptingObject(this);
			this.WebBrowser.LoadCompleted += this.WebBrowserOnLoadCompleted;
		}

		private void FlashPlayerLoaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		private void WebBrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
		{
			if (this.GameSettings != null)
			{
				this.AlignmentMode = this.GameSettings.AlignmentMode;
				this.ZoomMode = this.GameSettings.ZoomMode;
				this.Quality = this.GameSettings.Quality;
			}
			if (File.Exists(this.TempFile))
			{
				File.Delete(this.TempFile);
			}
			this.Loading = false;
		}

		public void AnalyzeSWF()
		{
			this.WebBrowser.InvokeScript("AnalyzeSWF");
			this.ZoomMode = this.GameSettings.ZoomMode;
			this.Quality = this.GameSettings.Quality;
		}

		private void Load()
		{
			if (this.Loading)
			{
				return;
			}
			this.Loading = true;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			this.TempFile = Path.GetTempFileName();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("tfmStandalone.Resources.ChatIcon.ico"))
			{
				if (manifestResourceStream != null)
				{
					using (FileStream fileStream = File.Create(this.TempFile))
					{
						byte[] array = new byte[8192];
						int count;
						while ((count = manifestResourceStream.Read(array, 0, array.Length)) > 0)
						{
							fileStream.Write(array, 0, count);
						}
					}
				}
			}
			string text = string.Empty;
			using (Stream manifestResourceStream2 = executingAssembly.GetManifestResourceStream("tfmStandalone.Resources.MainIcon.ico"))
			{
				if (manifestResourceStream2 != null)
				{
					using (StreamReader streamReader = new StreamReader(manifestResourceStream2))
					{
						byte[] bytes = Convert.FromBase64String(streamReader.ReadToEnd());
						text = Encoding.UTF8.GetString(bytes).Replace("$chargeur$", string.Format("file:///{0}", this.TempFile));
					}
				}
			}
			this.WebBrowser.NavigateToString(text);
			this.WebBrowser.Focusable = false;
		}

		public void Reload()
		{
			this.Load();
		}

		public void EncryptionKey(string key)
		{
			sbyte[] e = key.Split(new char[]
			{
				','
			}).Select(new Func<string, sbyte>(sbyte.Parse)).ToArray<sbyte>();
			EventHandler<sbyte[]> encryptionKeyReceived = this.EncryptionKeyReceived;
			if (encryptionKeyReceived == null)
			{
				return;
			}
			encryptionKeyReceived(this, e);
		}

		public void EncryptionVector(string vector)
		{
			int[] e = vector.Split(new char[]
			{
				','
			}).Select(new Func<string, int>(int.Parse)).ToArray<int>();
			EventHandler<int[]> encryptionVectorReceived = this.EncryptionVectorReceived;
			if (encryptionVectorReceived == null)
			{
				return;
			}
			encryptionVectorReceived(this, e);
		}

		public enum AlignmentModeEnum
		{
			Center,
			CenterLeft,
			CenterRight,
			TopCenter = 4,
			TopLeft,
			TopRight,
			BottomCenter = 8,
			BottomLeft,
			BottomRight
		}

		public enum ZoomModeEnum
		{
			NoZoom = 3,
			Stretch = 2,
			StretchToFit = 0
		}

		public enum QualityEnum
		{
			Low,
			Medium,
			High
		}
	}
}
