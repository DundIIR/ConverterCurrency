using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flurl.Http;



namespace ConverterCurrency;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
    }

}

public class Currency
{
    public string ID { get; set; }
    public string NumCode { get; set; }
    public string CharCode { get; set; }
    public int Nominal { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public double Previous { get; set; }

    public override string ToString()
    {
        return $"{CharCode}: {Name}";
    }
}

public class Valute
{
    public Currency AUD { get; set; }
    public Currency AZN { get; set; }
    public Currency GBP { get; set; }
    public Currency AMD { get; set; }
    public Currency BYN { get; set; }
    public Currency BGN { get; set; }
    public Currency BRL { get; set; }
    public Currency HUF { get; set; }
    public Currency VND { get; set; }
    public Currency HKD { get; set; }
    public Currency GEL { get; set; }
    public Currency DKK { get; set; }
    public Currency AED { get; set; }
    public Currency USD { get; set; }
    public Currency EUR { get; set; }
    public Currency EGP { get; set; }
    public Currency INR { get; set; }
    public Currency IDR { get; set; }
    public Currency KZT { get; set; }
    public Currency CAD { get; set; }
    public Currency QAR { get; set; }
    public Currency KGS { get; set; }
    public Currency CNY { get; set; }
    public Currency MDL { get; set; }
    public Currency NZD { get; set; }
    public Currency NOK { get; set; }
    public Currency PLN { get; set; }
    public Currency RON { get; set; }
    public Currency XDR { get; set; }
    public Currency SGD { get; set; }
    public Currency TJS { get; set; }
    public Currency THB { get; set; }
    public Currency TRY { get; set; }
    public Currency TMT { get; set; }
    public Currency UZS { get; set; }
    public Currency UAH { get; set; }
    public Currency CZK { get; set; }
    public Currency SEK { get; set; }
    public Currency CHF { get; set; }
    public Currency RSD { get; set; }
    public Currency ZAR { get; set; }
    public Currency KRW { get; set; }
    public Currency JPY { get; set; }
}

public class Root
{
    public DateTime Date { get; set; }
    public DateTime PreviousDate { get; set; }
    public string PreviousURL { get; set; }
    public DateTime Timestamp { get; set; }
    public Valute Valute { get; set; }
}



public sealed class MainViewModel : INotifyPropertyChanged
{

    private string _firstCurrency = "USD", _secondCurrency = "RUB";
    

    public ICommand ReverseCommand { get; }

    private DateTime _selectionDate = DateTime.Today;
    public DateTime SelectionDate
    {
        get => _selectionDate;
        set
        {
            if (_selectionDate == value) return;
            _selectionDate = value;
            OnPropertyChanged(nameof(SelectionDate));
            if(SelectionFirstCurrency != null)
                _firstCurrency = SelectionFirstCurrency.CharCode;
            if (SelectionSecondCurrency != null)
                _secondCurrency = SelectionSecondCurrency.CharCode;
            LoadValues();
        }
    }


    

    private string _valueFirstInput;
    public string ValueFirstInput
    {
        get => _valueFirstInput;
        set
        {
            if(_valueFirstInput == value) return;
            _valueFirstInput = value;

            OnPropertyChanged(nameof(ValueFirstInput));
            UpdateValueSecondInput();

        }
    }
    
    private string _valueSecondInput;
    public string ValueSecondInput
    {
        get => _valueSecondInput;
        set
        {
            if (_valueSecondInput == value) return;
            _valueSecondInput = value;

            OnPropertyChanged(nameof(ValueSecondInput));
            UpdateValueFirstInput();
            
        }
    }

    private void UpdateValueFirstInput()
    {
        if (double.TryParse(ValueSecondInput, out double result))
        {
            if (SelectionFirstCurrency != null && SelectionSecondCurrency != null)
                ValueFirstInput = ((result * SelectionSecondCurrency.Value / SelectionSecondCurrency.Nominal) /
                                   (SelectionFirstCurrency.Value / SelectionFirstCurrency.Nominal)).ToString();
            else
                ValueFirstInput = null;
        }
        else
        {
            ValueFirstInput = null;
        }
    }

    private void UpdateValueSecondInput()
    {
        if (double.TryParse(ValueFirstInput, out double result))
        {
            if (SelectionFirstCurrency != null && SelectionSecondCurrency != null)
                ValueSecondInput = ((result * SelectionFirstCurrency.Value / SelectionFirstCurrency.Nominal) /
                                    (SelectionSecondCurrency.Value / SelectionSecondCurrency.Nominal)).ToString();
            else
                ValueSecondInput = null;
        }
        else
        {
            ValueSecondInput = null;
        }
    }






    private Currency _selectionFirstCurrency;
    public Currency SelectionFirstCurrency
    {
        get => _selectionFirstCurrency;
        set
        {
            if (_selectionFirstCurrency == value || value == null) return;
            _selectionFirstCurrency = value;
            OnPropertyChanged(nameof(SelectionFirstCurrency));
            UpdateValueSecondInput();
        }
    }

    private Currency _selectionSecondCurrency;
    public Currency SelectionSecondCurrency
    {
        get => _selectionSecondCurrency;
        set
        {
            if (_selectionSecondCurrency == value || value == null) return;
            _selectionSecondCurrency = value;
            OnPropertyChanged(nameof(SelectionSecondCurrency));
            UpdateValueSecondInput();
        }
    }

    public ObservableCollection<Currency> CollectionCurrency { get; } = new ObservableCollection<Currency> { };

    private async Task LoadValues()
    {
        try
        {
            https://www.cbr-xml-daily.ru/archive/2023/05/13/daily_json.js
            var ReceivedData = await $"https://www.cbr-xml-daily.ru/archive/{SelectionDate:yyyy'/'MM'/'dd}/daily_json.js".GetJsonAsync<Root>();

            CollectionCurrency.Clear();

            foreach (var property in typeof(Valute).GetProperties())
            {
                Currency currency = (Currency)property.GetValue(ReceivedData.Valute);
                CollectionCurrency.Add(currency);
            }

            CollectionCurrency.Add(new Currency{ CharCode = "RUB", Name = "Российский рубль", Value = 1, Nominal = 1} );

            SelectionFirstCurrency = CollectionCurrency.FirstOrDefault(c => c.CharCode == _firstCurrency);
            SelectionSecondCurrency = CollectionCurrency.FirstOrDefault(c => c.CharCode == _secondCurrency);
            
        }
        catch (FlurlHttpException ex) when (ex.StatusCode == 404)
        {
            //if((DateTime.Today - SelectionDate).TotalDays >= 25 )
            //    SelectionDate = SelectionDate.AddDays(+1);
            //else
            SelectionDate = SelectionDate.AddDays(-1);
        }
        
    }

    public MainViewModel()
    {
#pragma warning disable CS4014
        LoadValues();
#pragma warning restore CS4014
        ValueFirstInput = "1";

        ReverseCommand = new Command(s =>
        {
            (SelectionFirstCurrency, SelectionSecondCurrency) = (SelectionSecondCurrency, SelectionFirstCurrency);
            
        });
    }


    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


