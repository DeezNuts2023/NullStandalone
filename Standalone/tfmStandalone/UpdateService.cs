using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace tfmStandalone
{
	public sealed class UpdateService
	{
        private const string AUTH = "cnp3UlVLcTMzODQxQ0E0TUo6cE5Jd0xIM3hjZldFNnh1SkM=";
        public EventHandler<double> LatestVersionReceived;

        public void Initialize()
		{
			string tempDir = Path.GetTempPath() + "tfm_update";
			TaskHelpers.Delay(delegate
			{
				try
				{
					if (Directory.Exists(tempDir))
					{
						Directory.Delete(tempDir, true);
					}
				}
				catch (Exception)
				{
				}
			}, 1000);
			try
			{
				using (WebClient webClient = new WebClient())
				{
                    webClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", AUTH);
                    webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args)
					{
						if (args.Error != null)
						{
							return;
						}
						double version;
						if (double.TryParse(args.Result, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out version))
						{
							TaskHelpers.UiInvoke(delegate
							{
								EventHandler<double> latestVersionReceived = this.LatestVersionReceived;
								if (latestVersionReceived == null)
								{
									return;
								}
								latestVersionReceived(this, version);
							});
						}
					};
					webClient.DownloadStringAsync(new Uri("http://mahcheese.com/version/version.php"));
				}
			}
			catch (Exception)
			{
			}
		}

		public void Update()
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string text = Path.Combine(new string[]
			{
				Path.GetTempPath() + "tfm_update"
			});
			string text2 = Path.Combine(text, "Update.exe");
			Directory.CreateDirectory(text);
			bool flag = false;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("tfmStandalone.Resources.Update.exe"))
			{
				if (manifestResourceStream != null)
				{
					using (FileStream fileStream = File.Create(text2))
					{
						byte[] array = new byte[8192];
						int count;
						while ((count = manifestResourceStream.Read(array, 0, array.Length)) > 0)
						{
							fileStream.Write(array, 0, count);
						}
					}
					flag = true;
				}
			}
			if (flag)
			{
				new Process
				{
					StartInfo = 
					{
						FileName = text2,
						Arguments = string.Format("\"{0}\"", directoryName),
						UseShellExecute = true
					}
				}.Start();
				Application.Current.Shutdown();
			}
		}
	}
}
