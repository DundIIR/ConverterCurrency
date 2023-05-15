using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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





public class MainViewModel : INotifyPropertyChanged
{
    public MainViewModel()
    {
        LoadValues();

    }

    private DateTime _selectionDate = DateTime.Today;
    public DateTime SelectionDate
    {
        get => _selectionDate;
        set
        {
            if (_selectionDate == value) return;
            _selectionDate = value;
            OnPropertyChanged(nameof(SelectionDate));
            LoadValues();
        }
    }


    private double? _valueFirstInputDouble;

    private string _valueFirstInput;
    public string ValueFirstInput
    {
        get => _valueFirstInput; 
        set
        {
            if(_valueFirstInput == value) return;
            _valueFirstInput = value;
            OnPropertyChanged(nameof(ValueFirstInput));

            if (double.TryParse(value, out double result))
            {
                _valueFirstInputDouble = result;
            }
            else
            {
                _valueFirstInputDouble = null;
            }
            OnPropertyChanged(nameof(Result));
        }
    }

    public string Result
    {
        get
        {
            if (_selectionFirstCurrency == null || _selectionSecondCurrency == null) return "";
            else
            {
                //return Math.Round(((_valueFirstInputDouble * SelectionFirstCurrency.Value / SelectionFirstCurrency.Nominal) /
                //                                     (SelectionSecondCurrency.Value / SelectionSecondCurrency.Nominal)) ?? 0, 6);
                return ((_valueFirstInputDouble * SelectionFirstCurrency.Value / SelectionFirstCurrency.Nominal) /
                        (SelectionSecondCurrency.Value / SelectionSecondCurrency.Nominal))?.ToString("F6");
            }
        }
    }




    private Currency _selectionFirstCurrency;
    public Currency SelectionFirstCurrency
    {
        get => _selectionFirstCurrency;
        set
        {
            if (_selectionFirstCurrency == value) return;
            _selectionFirstCurrency = value;
            OnPropertyChanged(nameof(SelectionFirstCurrency));
            OnPropertyChanged(nameof(Result));
        }
    }

    private Currency _selectionSecondCurrency;
    public Currency SelectionSecondCurrency
    {
        get => _selectionSecondCurrency;
        set
        {
            if (_selectionSecondCurrency == value) return;
            _selectionSecondCurrency = value;
            OnPropertyChanged(nameof(SelectionSecondCurrency));
            OnPropertyChanged(nameof(Result));
        }
    }

    public ObservableCollection<Currency> CollectionCurrency { get; } = new ObservableCollection<Currency> { };

    private async Task LoadValues()
    {
        string date = _selectionDate.ToString("yyyy/MM/dd").Replace(".", "/");
        var ReceivedData = await $"https://www.cbr-xml-daily.ru/archive/{date}/daily_json.js".GetJsonAsync<Root>();


        foreach (var property in typeof(Valute).GetProperties())
        {
            Currency currency = (Currency)property.GetValue(ReceivedData.Valute);
            CollectionCurrency.Add(currency);
        }

        CollectionCurrency.Add(new Currency{ CharCode = "RUB", Name = "Российский рубль", Value = 1, Nominal = 1} );

        SelectionFirstCurrency = CollectionCurrency.FirstOrDefault(c => c.CharCode == "RUB");
        SelectionSecondCurrency = CollectionCurrency.FirstOrDefault(c => c.CharCode == "USD");

    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

