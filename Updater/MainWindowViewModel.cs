using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows;

namespace Updater
{
	public sealed class MainWindowViewModel : BindableBase
	{
        private const string Auth = "cnp3UlVLcTMzODQxQ0E0TUo6cE5Jd0xIM3hjZldFNnh1SkM=";
        private string _exeLocation;
        private string UpdateFolder { get; }
        private string UpdateZip { get; }
        private bool _canNext;
        private bool _canBack;
        private bool _canCancel;
        private bool _downloading;
        private int _progress;
        private bool _downloadSuccessful;
        private bool _md5HashCheckPerformed;
        private bool _md5HashCheckSuccessful;
        private bool _backupFiles;
        private UpdateState _updateState;
        private Dictionary<string, string> ReplacedFilesDictionary { get; set; }
        private Dictionary<string, string> AddedFilesDictionary { get; set; }
        public string ClientFolder { get; set; }
        public EventHandler<bool> Processing;
        public DelegateCommand BackCommand { get; }
        public DelegateCommand NextCommand { get; }
        public ObservableCollection<string> AddedFiles { get; set; }
        public ObservableCollection<string> ReplacedFiles { get; set; }
        public DelegateCommand RedownloadCommand { get; set; }

        public UpdateState UpdateState { get => this._updateState; set => base.SetProperty(ref this._updateState, value, "UpdateState"); }
        public bool CanNext { get => this._canNext; set => base.SetProperty(ref this._canNext, value, "CanNext"); }
        public bool CanBack { get => this._canBack; set => base.SetProperty(ref this._canBack, value, "CanBack"); }
        public bool CanCancel { get => this._canCancel; set => base.SetProperty(ref this._canCancel, value, "CanCancel"); }
        public string ExeLocation { get => this._exeLocation; set { if (base.SetProperty(ref this._exeLocation, value, "ExeLocation")) { this.ClientFolder = Path.GetDirectoryName(value); if (this.UpdateState == UpdateState.CheckingForTransformiceExe) { this.CanNext = File.Exists(value); } } } }
        public bool Downloading { get => this._downloading; set => base.SetProperty(ref this._downloading, value, "Downloading"); }
        public int Progress { get => this._progress; set => base.SetProperty(ref this._progress, value, "Progress"); }
        public bool DownloadSuccessful { get => this._downloadSuccessful; set => base.SetProperty(ref this._downloadSuccessful, value, "DownloadSuccessful"); }
        public bool MD5HashCheckPerformed { get => this._md5HashCheckPerformed; set => base.SetProperty(ref this._md5HashCheckPerformed, value, "MD5HashCheckPerformed"); }
        public bool MD5HashCheckSuccessful { get => this._md5HashCheckSuccessful; set => base.SetProperty(ref this._md5HashCheckSuccessful, value, "MD5HashCheckSuccessful"); }
        public bool BackupFiles { get => this._backupFiles; set => base.SetProperty(ref this._backupFiles, value, "BackupFiles"); }

        public MainWindowViewModel(string exeFolder)
		{
			this.BackCommand = new DelegateCommand(delegate(object o)
			{
				this.PreviousState();
			});
			this.NextCommand = new DelegateCommand(delegate(object o)
			{
				this.NextState();
			});
			this.ClientFolder = exeFolder;
			if (!Directory.Exists(this.ClientFolder))
			{
				this.ClientFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			}
			this.ExeLocation = Path.Combine(this.ClientFolder, "Transformice.exe");
			this.UpdateFolder = Path.Combine(Path.GetTempPath(), "tfm_update");
			this.UpdateZip = Path.Combine(this.UpdateFolder, "update.zip");
			try
			{
				if (Directory.Exists(this.UpdateFolder))
				{
					Directory.Delete(this.UpdateFolder, true);
				}
			}
			catch (Exception)
			{
			}
			Directory.CreateDirectory(this.UpdateFolder);
			this.RedownloadCommand = new DelegateCommand(delegate(object o)
			{
				this.Download();
			});
			this.ReplacedFiles = new ObservableCollection<string>();
			this.AddedFiles = new ObservableCollection<string>();
		}

		private void PreviousState()
		{
			UpdateState updateState = this.UpdateState;
			if (updateState == UpdateState.Downloading)
			{
				this.UpdateState = UpdateState.CheckingForTransformiceExe;
				this.CanBack = false;
				return;
			}
			if (updateState != UpdateState.Updating)
			{
				return;
			}
			this.CanNext = false;
			this.UpdateState = UpdateState.Downloading;
			this.Download();
		}

		private void NextState()
		{
			switch (this.UpdateState)
			{
				case UpdateState.CheckingForTransformiceExe:
					if (!File.Exists(this.ExeLocation))
					{
						this.CanNext = false;
						break;
					}
					this.CanBack = true;
					this.CanNext = false;
					this.UpdateState = UpdateState.Downloading;
					this.Download();
                    break;
                case UpdateState.Downloading:
					this.UpdateState = UpdateState.Updating;
					this.SetupUpdate();
                    break;
                case UpdateState.Updating:
					this.Update();
                    break;
                default:
                    break;
            }
		}

		private void Download()
		{
			EventHandler<bool> processing = this.Processing;
			if (processing != null)
			{
				processing(this, true);
			}
			this.CanBack = false;
			this.Downloading = true;
			this.Progress = 0;
			this.DownloadSuccessful = false;
			this.MD5HashCheckPerformed = false;
			this.MD5HashCheckSuccessful = false;
			if (!Directory.Exists(this.UpdateFolder))
			{
				Directory.CreateDirectory(this.UpdateFolder);
			}
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", Auth);
					webClient.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs args)
					{
						Application.Current.Dispatcher.Invoke(delegate()
						{
							this.Progress = args.ProgressPercentage;
						});
					};
					webClient.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs args)
					{
						Application.Current.Dispatcher.Invoke(delegate()
						{
							this.Downloading = false;
							this.DownloadSuccessful = (args.Error == null);
							if (this.DownloadSuccessful)
							{
								this.Progress = 100;
								this.CheckMD5();
							}
							this.CanBack = true;
							EventHandler<bool> processing3 = this.Processing;
							if (processing3 == null)
							{
								return;
							}
							processing3(this, false);
						});
					};
					webClient.DownloadFileAsync(new Uri("http://www.mahcheese.com/update/update.php"), this.UpdateZip);
				}
			}
			catch (Exception)
			{
				this.CanBack = true;
				this.Downloading = false;
				this.DownloadSuccessful = false;
				EventHandler<bool> processing2 = this.Processing;
				if (processing2 != null)
				{
					processing2(this, false);
				}
			}
		}

		private void CheckMD5()
		{
			EventHandler<bool> processing = this.Processing;
			if (processing != null)
			{
				processing(this, true);
			}
			if (File.Exists(this.UpdateZip))
			{
				try
				{
					using (WebClient webClient = new WebClient())
					{
						webClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", Auth);
						webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args)
						{
							if (args.Error == null)
							{
								string md = this.GetMD5(this.UpdateZip);
								this.MD5HashCheckSuccessful = (md == args.Result);
								this.CanNext = this.MD5HashCheckSuccessful;
							}
							else
							{
								this.MD5HashCheckSuccessful = false;
							}
							this.MD5HashCheckPerformed = true;
							EventHandler<bool> processing4 = this.Processing;
							if (processing4 == null)
							{
								return;
							}
							processing4(this, false);
						};
						webClient.DownloadStringAsync(new Uri("http://mahcheese.com/update/md5.php"));
					}
				}
				catch (Exception)
				{
					this.MD5HashCheckSuccessful = false;
					this.MD5HashCheckPerformed = true;
					EventHandler<bool> processing2 = this.Processing;
					if (processing2 != null)
					{
						processing2(this, false);
					}
				}
				return;
			}
			this.MD5HashCheckSuccessful = false;
			EventHandler<bool> processing3 = this.Processing;
			if (processing3 == null)
			{
				return;
			}
			processing3(this, false);
		}

		private string GetMD5(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
			string result;
			using (MD5 md = MD5.Create())
			{
				using (FileStream fileStream = File.OpenRead(filePath))
				{
					result = Convert.ToBase64String(md.ComputeHash(fileStream));
				}
			}
			return result;
		}

		private void SetupUpdate()
		{
			this.CanNext = false;
			EventHandler<bool> processing = this.Processing;
			if (processing != null)
			{
				processing(this, true);
			}
			this.ReplacedFiles.Clear();
			this.ReplacedFilesDictionary = new Dictionary<string, string>();
			this.AddedFiles.Clear();
			this.AddedFilesDictionary = new Dictionary<string, string>();
			string text = Path.Combine(this.UpdateFolder, "files");
			if (Directory.Exists(text))
			{
				Directory.Delete(text, true);
			}
			ZipFile.ExtractToDirectory(this.UpdateZip, text);
			File.Delete(this.UpdateZip);
			if (!string.IsNullOrWhiteSpace(this.ExeLocation) && File.Exists(Path.Combine(text, "Transformice.exe")))
			{
				File.Move(Path.Combine(text, "Transformice.exe"), Path.Combine(text, Path.GetFileName(this.ExeLocation)));
			}
			this.FindFiles(text, this.ClientFolder);
			EventHandler<bool> processing2 = this.Processing;
			if (processing2 != null)
			{
				processing2(this, false);
			}
			this.CanNext = true;
		}

		private void FindFiles(string sourceDirectory, string destinationDirectory)
		{
			foreach (string path in Directory.GetFiles(sourceDirectory).Select(new Func<string, string>(Path.GetFileName)).ToList<string>())
			{
				string key = Path.Combine(sourceDirectory, path);
				string text = Path.Combine(destinationDirectory, path);
				if (File.Exists(text))
				{
					this.ReplacedFiles.Add(text);
					this.ReplacedFilesDictionary.Add(key, text);
				}
				else
				{
					this.AddedFiles.Add(text);
					this.AddedFilesDictionary.Add(key, text);
				}
			}
			Directory.GetDirectories(sourceDirectory).ToList<string>().ForEach(delegate(string d)
			{
				this.FindFiles(d, Path.Combine(destinationDirectory, Path.GetFileName(d)));
			});
		}

		private void Update()
		{
			List<string> list = new List<string>();
			foreach (string text in this.ReplacedFiles)
			{
				FileInfo fileInfo = new FileInfo(text);
				FileStream fileStream = null;
				try
				{
					fileStream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
				}
				catch (IOException)
				{
					list.Add(text);
				}
				finally
				{
					if (fileStream != null)
					{
						fileStream.Close();
					}
				}
			}
			if (list.Count != 0)
			{
				MessageBox.Show(string.Format("The following files are open or being used by another program: \r\n{0}", string.Join("\r\n", list)));
				return;
			}
			string path = "Update Backup";
			int num = 2;
			while (Directory.Exists(Path.Combine(this.ClientFolder, path)))
			{
				path = Path.Combine(this.ClientFolder, string.Format("Update Backup ({0})", num));
				num++;
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.ReplacedFilesDictionary)
			{
				if (this.BackupFiles)
				{
					string text2 = Path.Combine(this.ClientFolder, path) + Path.GetDirectoryName(keyValuePair.Value).Replace(this.ClientFolder, string.Empty);
					string destFileName = Path.Combine(text2, Path.GetFileName(keyValuePair.Key));
					Directory.CreateDirectory(text2);
					File.Copy(keyValuePair.Value, destFileName);
				}
				File.Delete(keyValuePair.Value);
				File.Copy(keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in this.AddedFilesDictionary)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(keyValuePair2.Value));
				File.Copy(keyValuePair2.Key, keyValuePair2.Value);
			}
			try
			{
				Directory.Delete(Path.Combine(this.UpdateFolder, "files"), true);
			}
			catch
			{
			}
			if (this.BackupFiles)
			{
				MessageBox.Show(string.Format("Files backed up to:\r\n{0}", Path.Combine(this.ClientFolder, path)));
			}
			new Process
			{
				StartInfo = 
				{
					FileName = this.ExeLocation,
					UseShellExecute = true
				}
			}.Start();
			Application.Current.Shutdown();
		}
	}
}
