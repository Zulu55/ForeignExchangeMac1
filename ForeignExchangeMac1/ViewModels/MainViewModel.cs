namespace ForeignExchangeMac1.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;
    using Xamarin.Forms;

    public class MainViewModel : INotifyPropertyChanged
    {
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
		#endregion

		#region Attributes
        ObservableCollection<Rate> _rates;
        bool _isEnabled;
        bool _isRunning;
        Rate _sourceRate;
        Rate _targetRate;
		string _amount;
		string _result;
		#endregion

		#region Properties
		public ObservableCollection<Rate> Rates
        {
            get
            {
                return _rates;
            }
            set
            {
                if (_rates != value)
                {
                    _rates = value;
                    PropertyChanged?.Invoke(
                        this, 
                        new PropertyChangedEventArgs(nameof(Rates)));
                }
            }
        }

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(IsEnabled)));
				}
			}
		}

		public bool IsRunning
		{
			get
			{
				return _isRunning;
			}
			set
			{
				if (_isRunning != value)
				{
					_isRunning = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(IsRunning)));
				}
			}
		}

        public string Result
        {
			get
			{
				return _result;
			}
			set
			{
				if (_result != value)
				{
					_result = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Result)));
				}
			}        
        }

        public Rate SourceRate
		{
			get
			{
				return _sourceRate;
			}
			set
			{
				if (_sourceRate != value)
				{
					_sourceRate = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(SourceRate)));
				}
			}
		}

		public Rate TargetRate
		{
			get
			{
				return _targetRate;
			}
			set
			{
				if (_targetRate != value)
				{
					_targetRate = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(TargetRate)));
				}
			}
		}

		public string Amount
		{
			get
			{
				return _amount;
			}
			set
			{
				if (_amount != value)
				{
					_amount = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Amount)));
				}
			}
		}
        #endregion

        #region Constructors
        public MainViewModel()
        {
            apiService = new ApiService();

            Result = "Enter an amount, select source rate, select target " +
                "rate and press convert button";

            LoadRates();
        }
        #endregion

        #region Methods
        async void LoadRates()
        {
            IsRunning = true;
            IsEnabled = false;

            var response = await apiService.GetRates();

            if (!response.IsSuccess)
            {
                IsRunning = false;
				IsEnabled = false;
				Result = response.Message;
                return;
            }

            var rates = (List<Rate>)response.Result;
            Rates = new ObservableCollection<Rate>(rates);

            IsRunning = false;
			IsEnabled = true;
		}
		#endregion

		#region Commands
        public ICommand ChangeCommand
        {
			get { return new RelayCommand(Change); }
		}

        void Change()
        {
            var aux = SourceRate;
            SourceRate = TargetRate;
            TargetRate = aux;
            ConvertPlus();
        }

        public ICommand ConvertPlusCommand
        {
            get { return new RelayCommand(ConvertPlus);  }
        }

        async void ConvertPlus()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    "You must enter a value in amount.", 
                    "Accept");
                return;
            }

            decimal amount = 0;
            if (!decimal.TryParse(Amount, out amount))
            {
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"You must enter a numeric value in amount.",
					"Accept");
				return;
			}

			if (SourceRate == null)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"You must select a source rate.",
					"Accept");
				return;
			}

			if (TargetRate == null)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"You must select a target rate.",
					"Accept");
				return;
			}

            var amountConverted = amount / 
                                  (decimal)SourceRate.TaxRate * 
                                  (decimal)TargetRate.TaxRate;

            Result = string.Format(
                "{0:C2} {1} = {2:C2} {3}", 
                amount, 
                SourceRate.Code, 
                amountConverted, 
                TargetRate.Code);

		}
        #endregion
    }
}