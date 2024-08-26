using System;
using System.Collections.ObjectModel;
using System.IO;
using Prism.Mvvm;
using tfmClientHook;

namespace tfmStandalone
{
	public class FolderViewModel : BindableBase
	{
        private FolderViewModel _parent;
        private string _name;
        public ObservableCollection<FolderViewModel> ChildFolders { get; }
        public ObservableCollection<MapFileViewModel> Files { get; }

        public FolderViewModel Parent
        {
            get => this._parent;
            set => this.SetProperty(ref this._parent, value, "Parent");
        }

        public string Name
        {
            get => this._name;
            set => this.SetProperty(ref this._name, value, "Name");
        }

        public FolderViewModel(FolderViewModel parent, string directory, ClientHook clientHook)
		{
			this.Parent = parent;
			this.Name = Path.GetFileName(directory);
			this.ChildFolders = new ObservableCollection<FolderViewModel>();
			this.Files = new ObservableCollection<MapFileViewModel>();
			if (Directory.Exists(directory))
			{
				foreach (string directory2 in Directory.GetDirectories(directory))
				{
					this.ChildFolders.Add(new FolderViewModel(this, directory2, clientHook));
				}
				foreach (string filePath in Directory.GetFiles(directory, "*.png"))
				{
					this.Files.Add(new MapFileViewModel(filePath, clientHook));
				}
			}
		}
	}
}
