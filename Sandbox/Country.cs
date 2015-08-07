using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Sandbox
{
    public class Country : INotifyPropertyChanged
    {
        private string _countryName;
        public string CountryName
        {
            get { return _countryName; }
            set { _countryName = value; OnPropertyChanged(); }
        }

        private string _continentName;
        public string ContinentName
        {
            get { return _continentName; }
            set { _continentName = value; OnPropertyChanged(); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private const string _asia = "Asia";
        private const string _europe = "Europe";
        private const string _northAmerica = "North America";
        private ICommand _selectCountriesCommand;
        private ICommand _deSelectCountriesCommand;

        public ViewModel()
        {
            this.Countries = new ObservableCollection<Country>();
            this.Countries.CollectionChanged += Countries_CollectionChanged;

            /* Asia */
            this.Countries.Add(new Country() { ContinentName = _asia, CountryName = "China" });
            this.Countries.Add(new Country() { ContinentName = _asia, CountryName = "India" });
            this.Countries.Add(new Country() { ContinentName = _asia, CountryName = "Japan" });
            this.Countries.Add(new Country() { ContinentName = _asia, CountryName = "Pakistan" });
            this.Countries.Add(new Country() { ContinentName = _asia, CountryName = "Thailand" });
            this.Countries.Add(new Country() { ContinentName = _asia, CountryName = "Vietnam" });

            /* Europe */
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "France" });
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "Germany" });
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "Italy" });
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "Russia" });
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "Spain" });
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "Sweden" });
            this.Countries.Add(new Country() { ContinentName = _europe, CountryName = "United Kingdom" });

            /* North America */
            this.Countries.Add(new Country() { ContinentName = _northAmerica, CountryName = "Canada" });
            this.Countries.Add(new Country() { ContinentName = _northAmerica, CountryName = "Mexico" });
            this.Countries.Add(new Country() { ContinentName = _northAmerica, CountryName = "USA" });

            _selectCountriesCommand = new DelegateCommand<string>((continentName) =>
            {
                SetIsSelectedProperty(continentName, true);
            });

            _deSelectCountriesCommand = new DelegateCommand<string>((continentName) =>
            {
                SetIsSelectedProperty(continentName, false);
            });
        }

          private void SetIsSelectedProperty(string continentName, bool isSelected)
    {
        IEnumerable<Country> countriesOnTheCurrentContinent =
                this.Countries.Where(c => c.ContinentName.Equals(continentName));
 
        foreach (Country country in countriesOnTheCurrentContinent)
        {
            INotifyPropertyChanged c = country as INotifyPropertyChanged;
            c.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            country.IsSelected = isSelected;
            c.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        }
    }
 
    public ICommand SelectCountriesCommand
    {
        get { return _selectCountriesCommand; }
    }
 
    public ICommand DeSelectCountriesCommand
    {
        get { return _deSelectCountriesCommand; }
    }

        private void Countries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object country in e.NewItems)
                {
                    (country as INotifyPropertyChanged).PropertyChanged
                        += new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }

            if (e.OldItems != null)
            {
                foreach (object country in e.OldItems)
                {
                    (country as INotifyPropertyChanged).PropertyChanged
                        -= new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Countries");
        }

        public ObservableCollection<Country> Countries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public class CountryCollectionToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Country> countries = values[0] as IEnumerable<Country>;
            string continentName = values[1] as string;
            if (countries != null && continentName != null)
            {
                IEnumerable<Country> countriesOnTheCurrentContinent
                    = countries.Where(c => c.ContinentName.Equals(continentName));

                int selectedCountries = countriesOnTheCurrentContinent
                    .Where(c => c.IsSelected)
                    .Count();

                if (selectedCountries.Equals(countriesOnTheCurrentContinent.Count()))
                    return true;

                if (selectedCountries > 0)
                    return null;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
