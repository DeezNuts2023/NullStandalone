using System;
using Prism.Commands;
using tfmClientHook;

namespace tfmStandalone
{
	public sealed class MapOverlayViewModel : FolderViewModel
	{
        private bool _isCollapsed;
        private FolderViewModel _selectedFolder;
        private bool _isSelectedFolderRoot;
        private bool _isSelectedFolderParentRoot;
        public static readonly string MapsFolder = AppDomain.CurrentDomain.BaseDirectory + "Maps";
		public DelegateCommand ToggleCollapsedCommand { get; }
		public DelegateCommand<FolderViewModel> SelectFolderCommand { get; }

        public bool IsCollapsed
        {
            get => this._isCollapsed;
            set => this.SetProperty(ref this._isCollapsed, value, "IsCollapsed");
        }

        public FolderViewModel SelectedFolder
        {
            get => this._selectedFolder;
            set
            {
                if (this.SetProperty(ref this._selectedFolder, value, "SelectedFolder"))
                {
                    this.IsSelectedFolderRoot = (value == this);
                    this.IsSelectedFolderParentRoot = value?.Parent == this;
                }
            }
        }

        public bool IsSelectedFolderRoot
        {
            get => this._isSelectedFolderRoot;
            set => this.SetProperty(ref this._isSelectedFolderRoot, value, "IsSelectedFolderRoot");
        }

        public bool IsSelectedFolderParentRoot
        {
            get => this._isSelectedFolderParentRoot;
            set => this.SetProperty(ref this._isSelectedFolderParentRoot, value, "IsSelectedFolderParentRoot");
        }

        public MapOverlayViewModel(ClientHook clientHook) : base(null, MapOverlayViewModel.MapsFolder, clientHook)
		{
			this.IsCollapsed = true;
			this.SelectedFolder = this;
			this.ToggleCollapsedCommand = new DelegateCommand(delegate()
			{
				this.IsCollapsed = !this.IsCollapsed;
			});
			this.SelectFolderCommand = new DelegateCommand<FolderViewModel>(delegate(FolderViewModel folder)
			{
				this.SelectedFolder = folder;
			});
		}
	}
}
