using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class SanctionViewModel : BindableBase
	{
        private string _description;
        private SanctionOccurenceViewModel _firstOccurence;
        private SanctionOccurenceViewModel _secondOccurence;
        private SanctionOccurenceViewModel _thirdOccurence;
        private SanctionOccurenceViewModel _fourthOccurence;
        private string _additionalInformation;
        private bool _importantAdditionalInfo;

        public string Description
        {
            get => this._description;
            set => this.SetProperty(ref this._description, value, nameof(Description));
        }

        public SanctionOccurenceViewModel FirstOccurence
        {
            get => this._firstOccurence;
            set => this.SetProperty(ref this._firstOccurence, value, nameof(FirstOccurence));
        }

        public SanctionOccurenceViewModel SecondOccurence
        {
            get => this._secondOccurence;
            set => this.SetProperty(ref this._secondOccurence, value, nameof(SecondOccurence));
        }

        public SanctionOccurenceViewModel ThirdOccurence
        {
            get => this._thirdOccurence;
            set => this.SetProperty(ref this._thirdOccurence, value, nameof(ThirdOccurence));
        }

        public SanctionOccurenceViewModel FourthOccurence
        {
            get => this._fourthOccurence;
            set => this.SetProperty(ref this._fourthOccurence, value, nameof(FourthOccurence));
        }

        public string AdditionalInformation
        {
            get => this._additionalInformation;
            set => this.SetProperty(ref this._additionalInformation, value, nameof(AdditionalInformation));
        }

        public bool ImportantAdditionalInfo
        {
            get => this._importantAdditionalInfo;
            set => this.SetProperty(ref this._importantAdditionalInfo, value, nameof(ImportantAdditionalInfo));
        }
    }
}
