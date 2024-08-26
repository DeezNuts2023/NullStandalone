using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class MapFileViewModel : BindableBase
	{
        private string _map;
        private string _maskMap;
        private string _filePath;
		public DelegateCommand NppMapCommand { get; }
		public DelegateCommand NpMapCommand { get; }

        public string Map
        {
            get => this._map;
            set => this.SetProperty(ref this._map, value, "Map");
        }

        public string MaskMap
        {
            get => this._maskMap;
            set => this.SetProperty(ref this._maskMap, value, "MaskMap");
        }

        public string FilePath
        {
            get => this._filePath;
            set => this.SetProperty(ref this._filePath, value, "FilePath");
        }

        public MapFileViewModel(string filePath, ClientHook clientHook)
		{
			MapFileViewModel current = this;
			this.FilePath = filePath;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			if (fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("_"))
			{
				string[] array = fileNameWithoutExtension.Split(new char[]
				{
					'_'
				});
				if (array.Length == 2)
				{
					this._map = array[0];
					this._maskMap = array[1];
				}
			}
			else
			{
				this._map = fileNameWithoutExtension;
			}
			this.NppMapCommand = new DelegateCommand(delegate()
			{
				string command = string.IsNullOrWhiteSpace(current.MaskMap) ? string.Format("npp {0}", current.Map) : string.Format("npp {0} {1}", current.Map, current.MaskMap);
				clientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = command
				});
			});
			this.NpMapCommand = new DelegateCommand(delegate()
			{
				string command = string.IsNullOrWhiteSpace(current.MaskMap) ? string.Format("np {0}", current.Map) : string.Format("np {0} {1}", current.Map, current.MaskMap);
				clientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = command
				});
			});
		}
	}
}
