using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class CommunityOverlayViewModel : BindableBase
	{
        private bool _isCollapsed;
		public ObservableCollection<CommunityViewModel> Whitelist { get; }
		public ObservableCollection<CommunityViewModel> Blacklist { get; }
		public DelegateCommand ToggleViewCommand { get; }
		public DelegateCommand<CommunityViewModel> WhitelistToggleCommand { get; }
		public DelegateCommand<CommunityViewModel> BlacklistToggleCommand { get; }
		private GameInfo GameInfo { get; }
		private ClientHook ClientHook { get; }

        public bool IsCollapsed
        {
            get => this._isCollapsed;
            set => this.SetProperty(ref this._isCollapsed, value, nameof(IsCollapsed));
        }

        public CommunityOverlayViewModel(GameInfo gameInfo, ClientHook clientHook)
		{
			this.GameInfo = gameInfo;
			this.ClientHook = clientHook;
			this.IsCollapsed = true;
			this.Whitelist = new ObservableCollection<CommunityViewModel>();
			this.Blacklist = new ObservableCollection<CommunityViewModel>();
			this.ToggleViewCommand = new DelegateCommand(delegate()
			{
				this.IsCollapsed = !this.IsCollapsed;
			});
			this.WhitelistToggleCommand = new DelegateCommand<CommunityViewModel>(delegate(CommunityViewModel c)
			{
				c.IsSelected = !c.IsSelected;
				if (c.IsSelected)
				{
					if (this.Blacklist.Any((CommunityViewModel bc) => bc.IsSelected))
					{
						EnumerableExtensions.ForEach<CommunityViewModel>(this.Blacklist, delegate(CommunityViewModel bc)
						{
							bc.IsSelected = false;
						});
						this.GameInfo.IgnoredCommunities.Clear();
					}
					this.GameInfo.AllowedCommunities.Add(c.Community.ToLowerInvariant());
					this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
					{
						Message = string.Format("[{0}] has been added to the whitelist.", c.Community)
					});
					return;
				}
				this.GameInfo.AllowedCommunities.Remove(c.Community.ToLowerInvariant());
				this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
				{
					Message = string.Format("[{0}] has been removed from the whitelist.", c.Community)
				});
			});
			this.BlacklistToggleCommand = new DelegateCommand<CommunityViewModel>(delegate(CommunityViewModel c)
			{
				c.IsSelected = !c.IsSelected;
				if (c.IsSelected)
				{
					if (this.Whitelist.Any((CommunityViewModel bc) => bc.IsSelected))
					{
						EnumerableExtensions.ForEach<CommunityViewModel>(this.Whitelist, delegate(CommunityViewModel bc)
						{
							bc.IsSelected = false;
						});
						this.GameInfo.AllowedCommunities.Clear();
					}
					this.GameInfo.IgnoredCommunities.Add(c.Community.ToLowerInvariant());
					this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
					{
						Message = string.Format("[{0}] ignored.", c.Community)
					});
					return;
				}
				this.GameInfo.IgnoredCommunities.Remove(c.Community.ToLowerInvariant());
				this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
				{
					Message = string.Format("[{0}] unignored.", c.Community)
				});
			});
		}

		public void Initialize()
		{
			List<CommunityViewModel> list = new List<CommunityViewModel>();
			List<CommunityViewModel> list2 = new List<CommunityViewModel>();
			foreach (KeyValuePair<int, string> keyValuePair in ServerInfo.TribulleCommunities)
			{
				list.Add(new CommunityViewModel
				{
					Community = keyValuePair.Value.ToUpperInvariant()
				});
				list2.Add(new CommunityViewModel
				{
					Community = keyValuePair.Value.ToUpperInvariant()
				});
			}
			EnumerableExtensions.ForEach<CommunityViewModel>(from c in list
			orderby c.Community
			select c, delegate(CommunityViewModel c)
			{
				this.Whitelist.Add(c);
			});
			EnumerableExtensions.ForEach<CommunityViewModel>(from c in list2
			orderby c.Community
			select c, delegate(CommunityViewModel c)
			{
				this.Blacklist.Add(c);
			});
		}
	}
}
