using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class SanctionGridViewModel : BindableBase
	{
        private bool _isCollapsed;
        public ObservableCollection<SanctionCategoryViewModel> SanctionCategories { get; }
        public DelegateCommand ToggleViewCommand { get; }

        public bool IsCollapsed
        {
            get => this._isCollapsed;
            set => this.SetProperty(ref this._isCollapsed, value, "isCollapsed");
        }

        public SanctionGridViewModel()
		{
			this.SanctionCategories = new ObservableCollection<SanctionCategoryViewModel>();
			this.ToggleViewCommand = new DelegateCommand(delegate()
			{
				this.IsCollapsed = !this.IsCollapsed;
			});
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic YldtWEp4YzROUDBJMFE6WTluc2x2UiQ1TEc5YlJS";
				webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args)
				{
					if (args.Error == null)
					{
						string result = args.Result;
						List<SanctionCategory> data = JsonConvert.DeserializeObject<List<SanctionCategory>>(result);
						TaskHelpers.UiInvoke(delegate
						{
							foreach (SanctionCategory sanctionCategory in data)
							{
								SanctionCategoryViewModel sanctionCategoryViewModel = new SanctionCategoryViewModel
								{
									Description = sanctionCategory.Description
								};
								foreach (Sanction sanction in sanctionCategory.Sanctions)
								{
									SanctionViewModel item = new SanctionViewModel
									{
										Description = sanction.Description,
										FirstOccurence = new SanctionOccurenceViewModel
										{
											Mute = sanction.FirstOccurence.Mute,
											Ban = sanction.FirstOccurence.Ban,
											Other = sanction.FirstOccurence.Other
										},
										SecondOccurence = new SanctionOccurenceViewModel
										{
											Mute = sanction.SecondOccurence.Mute,
											Ban = sanction.SecondOccurence.Ban,
											Other = sanction.SecondOccurence.Other
										},
										ThirdOccurence = new SanctionOccurenceViewModel
										{
											Mute = sanction.ThirdOccurence.Mute,
											Ban = sanction.ThirdOccurence.Ban,
											Other = sanction.ThirdOccurence.Other
										},
										FourthOccurence = new SanctionOccurenceViewModel
										{
											Mute = sanction.FourthOccurence.Mute,
											Ban = sanction.FourthOccurence.Ban,
											Other = sanction.FourthOccurence.Other
										},
										AdditionalInformation = sanction.Information,
										ImportantAdditionalInfo = sanction.IsInfoImportant
									};
									sanctionCategoryViewModel.Sanctions.Add(item);
								}
								this.SanctionCategories.Add(sanctionCategoryViewModel);
							}
						});
					}
				};
				webClient.DownloadStringAsync(new Uri("http://mahcheese.com/data/sanctions.json"));
			}
		}
	}
}
