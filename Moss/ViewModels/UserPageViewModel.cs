using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace Moss.ViewModels;
// TODO меню операції сортування, фільтрації, пошуку за декількома значеннями об’єктів
public class UserPageViewModel : FirstStepPageViewModelBase
{
    #region Variables

    private string? _city;
    private string? _pm2dot5;
    private string? _pm10;
    private string? _temp;
    private string? _humidity;
    private string? _apidata;
    private string? _aqi;
    private string? _searchCity;
    private string? _aqiBlockColor;
    private string? _chartRowColor;
    private string? _recievingDate;
    private string? _notificationText;
    private bool _notificationMessageIsVisible;
    private bool _tokenExists;
    public ObservableCollection<ObservableValue> PM10ChartValue = new ObservableCollection<ObservableValue>();
    public ObservableCollection<ObservableValue> PM2dot5ChartValue = new ObservableCollection<ObservableValue>();

    #endregion
    public UserPageViewModel()
    {
        //d448c4bac6e20b474fb870c912276d4263bc2664
        Initialize();
        chartRowColor = "#D9D9D9";
        aqiBlockColor = "#D9D9D9";
        NotificationIsVisible = false;
        LocalTransfer.CallUnderList = true;
        var canNavSettings = this.WhenAnyValue(x => x.CanNavigateSettings);
        
        LocalTransfer.PageFromListChangedEvent += CheckTokenExists;
        
        var canParseApiData = this.WhenAnyValue(x => x.TokenExists);
        LogOutCommand = ReactiveCommand.Create(LogOut);
        NavigateSettingsCommand = ReactiveCommand.Create(NavigateSettings, canNavSettings);
        ParseApiDataCommand = ReactiveCommand.Create(ParseCityDataAsync, canParseApiData);
        SaveUserDataCommand = ReactiveCommand.Create(SetDefaultCity, canParseApiData);

        Cityes = new AvaloniaList<string>();

        this.WhenAnyValue(x => x.aqi).Subscribe(_ => UpdateAQIBlockColor());
        this.WhenAnyValue(x => x.SearchCity).Subscribe(_ => UpdateSearchCity());
    }
    //d448c4bac6e20b474fb870c912276d4263bc2664

    #region Properties
    public ISeries[] Series { get; set; }
    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }
    public AvaloniaList<string> Cityes { get; set; }
    private AvaloniaList<string> stationNames { get; set; }
    public string? NotificationText
    {
        get => _notificationText;
        set => this.RaiseAndSetIfChanged(ref _notificationText, value);
    }
    public string? SearchCity
    {
        get => _searchCity;
        set => this.RaiseAndSetIfChanged(ref _searchCity, value);
    }
    public string? aqiBlockColor
    {
        get => _aqiBlockColor;
        set => this.RaiseAndSetIfChanged(ref _aqiBlockColor, value);
    }
    public string? chartRowColor
    {
        get => _chartRowColor;
        set => this.RaiseAndSetIfChanged(ref _chartRowColor, value);
    }
    public string? pm2dot5
    {
        get => $"PM2.5: {_pm2dot5} mcg/m\u00b3";
        set => this.RaiseAndSetIfChanged(ref _pm2dot5, value);
    }
    public string? pm10
    {
        get => $"PM10: {_pm10} mcg/m\u00b3";
        set => this.RaiseAndSetIfChanged(ref _pm10, value);
    }
    public string? temp
    {
        get => $"Temperature: {_temp} \u00b0C";
        set => this.RaiseAndSetIfChanged(ref _temp, value);
    }
    public string? humidity
    {
        get => $"Humidity: {_humidity} %";
        set => this.RaiseAndSetIfChanged(ref _humidity, value);
    }
    public string? recievingDate
    {
        get => $"Data recieving time: {_recievingDate}";
        set => this.RaiseAndSetIfChanged(ref _recievingDate, !string.IsNullOrEmpty(value) ? value.Substring(0,10) : value);
    }
    public string? city
    {
        get => _city;
        set => this.RaiseAndSetIfChanged(ref _city, value);
    }
    public string? apidata
    {
        get => _apidata;
        set => this.RaiseAndSetIfChanged(ref _apidata, value);
    }
    public string? aqi
    {
        get => _aqi;
        set => this.RaiseAndSetIfChanged(ref _aqi, value);
    }
    public bool TokenExists
    {
        get => _tokenExists;
        set => this.RaiseAndSetIfChanged(ref _tokenExists, value);
    }
    public bool NotificationIsVisible
    {
        get => _notificationMessageIsVisible;
        set => this.RaiseAndSetIfChanged(ref _notificationMessageIsVisible, value);
    }
    public override bool CanNavigateSettings
    {
        get => true;
        set => CanNavigateSettings = true;
    }
    public override bool CanNavigateAuthPage
    {
        get => true;
        set => CanNavigateAuthPage = true;
    }
    public override bool CanNavigateUserPage
    {
        get => false;
        set => CanNavigateSettings = false;
    }
    #endregion

    #region Interfaces
    public ICommand NavigateSettingsCommand { get; }
    public ICommand ParseApiDataCommand { get; }
    public ICommand LogOutCommand { get; }
    public ICommand SaveUserDataCommand { get; }
    #endregion

    #region Methods
    #region async
    private async void Notification(int ms_time, string text)
    {
        NotificationText = text;
        NotificationIsVisible = true;
        await Task.Delay(ms_time);
        NotificationIsVisible = false;
    }
    private async void ParseCityNamesAsync()
    {
        try
        {
            string str = null;
            using (WebClient client = new WebClient())
            {
                string addres = $"https://api.waqi.info/search/?token={LocalTransfer.UserToken}&keyword={SearchCity}";
                str = await client.DownloadStringTaskAsync(addres);
            }
            
            if (!str.Equals("{\"status\":\"error\",\"data\":\"Invalid key\"}"))
            {
                JsonNode obj = JsonObject.Parse(str);
                JsonArray dataArray = (JsonArray)obj["data"];
                stationNames = new AvaloniaList<string>();
                foreach (var dataItem in dataArray)
                {
                    string name = dataItem["station"]["name"].ToString();
                    stationNames.Add(ProccesCityName(name));
                }
                Cityes.Clear();
                Cityes.AddRange(stationNames);
            }
        }
        catch (InvalidCastException e)
        {
            Debug.WriteLine("Parse city names error");
            throw;
        }
        
    }
    private async void ParseCityDataAsync()
    {
        try
        {
            NotificationText = "searching";
            NotificationIsVisible = true;
            string str = null;
            using (WebClient client = new WebClient())
            {
                //string addres = $"https://api.waqi.info/feed/{SearchCity}/?token=d448c4bac6e20b474fb870c912276d4263bc2664";
                string addres = $"https://api.waqi.info/feed/{SearchCity}/?token={LocalTransfer.UserToken}";
                str = await client.DownloadStringTaskAsync(addres);
            }
            apidata = str;
            JsonNode obj = JsonObject.Parse(str);
            JsonObject jObj = (JsonObject)obj;
            PM10ChartValue.Clear();
            PM2dot5ChartValue.Clear();
            if (obj["status"].ToString().Equals("ok"))
            {
                if (jObj.ContainsKey("data"))
                {
                    JsonObject dataOBJ = (JsonObject)obj["data"];
                    
                    city = dataOBJ.ContainsKey("city")
                        ?ProccesCityName(obj["data"]["city"]["name"].ToString())
                        : "-";
                    aqi = dataOBJ.ContainsKey("aqi")
                        ?obj["data"]["aqi"].ToString()
                        : "-";
                    if (dataOBJ.ContainsKey("iaqi"))
                    {
                        JsonObject iaqiOBJ = (JsonObject)obj["data"]["iaqi"];
                        pm2dot5 = iaqiOBJ.ContainsKey("pm25")
                            ?obj["data"]["iaqi"]["pm25"]["v"].ToString()
                            :"-";
                        pm10 = iaqiOBJ.ContainsKey("pm10")
                            ?obj["data"]["iaqi"]["pm10"]["v"].ToString()
                            :"-";
                        temp = iaqiOBJ.ContainsKey("t")
                            ?obj["data"]["iaqi"]["t"]["v"].ToString()
                            :"-";
                        humidity = iaqiOBJ.ContainsKey("h")
                            ?obj["data"]["iaqi"]["h"]["v"].ToString()
                            :"-";
                    }
                    if (dataOBJ.ContainsKey("time"))
                    {
                        recievingDate = obj["data"]["time"]["s"].ToString();
                    }
                    if (dataOBJ.ContainsKey("forecast"))
                    {
                        JsonObject forecastDailyOBJ = (JsonObject)obj["data"]["forecast"]["daily"];
                        dynamic jsonObJ = Newtonsoft.Json.JsonConvert.DeserializeObject(str);
                        if (forecastDailyOBJ.ContainsKey("pm10"))
                        {
                            var pm10Array = jsonObJ.data.forecast.daily.pm10;
                            PM10ChartValue.Clear();
                            foreach (var item in pm10Array)
                            {
                                if (item.ContainsKey("avg"))
                                {
                                    PM10ChartValue.Add(new ObservableValue(Convert.ToDouble(item.avg)));
                                }
                            }
                        }

                        if (forecastDailyOBJ.ContainsKey("pm25"))
                        {
                            var pm25Array = jsonObJ.data.forecast.daily.pm25;
                            PM2dot5ChartValue.Clear();
                            foreach (var item in pm25Array)
                            {
                                
                                if (item.ContainsKey("avg"))
                                {
                                    
                                    //PM2dot5ChartValue.RemoveAt(0);
                                    PM2dot5ChartValue.Add(new ObservableValue(Convert.ToDouble(item.avg)));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                pm2dot5 = "-";
                (pm2dot5, pm10, aqi, humidity, temp) = ("-", "-", "-", "-","-");
                city = "error";
            }
            Debug.WriteLine(str);
        }
        catch (NullReferenceException e)
        {
            Debug.WriteLine("Error while parsing city data");
            throw;
        }
        finally
        {
            NotificationIsVisible = false;
        }
    }
    #endregion
    private void Initialize()
    {
        Series = new ISeries[]
        {
            new LineSeries<ObservableValue>()
            {
                Values = PM10ChartValue,
                Fill = new SolidColorPaint(new SKColor(215, 192, 69, 90)),
                Stroke = new SolidColorPaint(new SKColor(215, 192, 69)) {StrokeThickness = 3},
                GeometryStroke = new SolidColorPaint(new SKColor(215, 192, 69)) {StrokeThickness = 3}
            },
            new LineSeries<ObservableValue>()
            {
                Values = PM2dot5ChartValue,
                Fill = new SolidColorPaint(new SKColor(116, 215, 69, 90)),
                Stroke = new SolidColorPaint(new SKColor(116, 215, 69)) {StrokeThickness = 3},
                GeometryStroke = new SolidColorPaint(new SKColor(116, 215, 69)) {StrokeThickness = 3}
            }
        };
        XAxes = new Axis[]
        {
            new Axis
            {
                Name = "Days",
                NameTextSize = 10,
                NamePaint = new SolidColorPaint(SKColors.White),

                LabelsPaint = new SolidColorPaint(SKColors.White),
                TextSize = 10,

                SeparatorsPaint = null
            }
        };
        YAxes = new Axis[]
        {
            new Axis
            {
                Name = "Values",
                NameTextSize = 10,
                NamePaint = new SolidColorPaint(SKColors.White),
                TextSize = 10,

                LabelsPaint = new SolidColorPaint(SKColors.White),

                SeparatorsPaint = null
            }
        };

    }
    private void ClearPageData()
    {
        city = null;
        pm2dot5 = null;
        pm10 = null;
        temp = null;
        humidity = null;
        aqi = null;
        recievingDate = null;
        SearchCity = null;
        PM10ChartValue.Clear();
        PM2dot5ChartValue.Clear();
    }
    private void UpdateSearchCity()
    {
        ParseCityNamesAsync();
    }
    public void SetDefaultCity()
    {
        DataManager.InsertUserData(LocalTransfer.UserID, city);
        Notification(3000, "Complete!");
    }
    private void NavigateSettings()
    {
        LocalTransfer.PageFromList = 2;
        LocalTransfer.RaiseStaticEvent();
    }
    private void LogOut()
    {
        LocalTransfer.UserToken = null;
        ClearPageData();
        LocalTransfer.PageFromList = 0;
        LocalTransfer.RaiseStaticEvent();
    }
    private void CheckTokenExists()
    {
        if (!string.IsNullOrEmpty(DataManager.DisplayUsers()))
        {
            TokenExists = DataManager.CheckUserToken(LocalTransfer.UserID);
            if (!TokenExists)
            {
                Notification(10000,"Set Token!");
            }
            LocalTransfer.UserToken = DataManager.GetUserToken(LocalTransfer.UserID);
            SearchCity = DataManager.GetUserData(LocalTransfer.UserID);
        }
    }
    private void UpdateAQIBlockColor()
    {
        if (int.TryParse(aqi, out int aqiInt))
        {
            if (aqiInt <= 50)
            {
                aqiBlockColor = "#74D745";
            }
            else if (aqiInt <= 100)
            {
                aqiBlockColor = "#D7C045";
            }
            else if (aqiInt<= 150)
            {
                aqiBlockColor = "#D78B45";
            }
            else if (aqiInt<= 200)
            {
                aqiBlockColor = "#D74545";
            }
            else if (aqiInt<= 300)
            {
                aqiBlockColor = "#D74545";
            }
            else
            {
                aqiBlockColor = "#7445D7";
            }
        }
        else
        {
            aqiBlockColor = "#DDDDDD";
        }
    }

    private string ProccesCityName(string input)
    {
        if (!(input.IndexOf("(") == -1))
        {
            return input.Substring(0, input.IndexOf("("));
        }
        return input;
    }
    #endregion
}