using BlazorSample.Shared;

namespace BlazorSample.Client
{
    public class SampleClientState
    {
        public int CurrentCount { get; set; }
        public WeatherForecast[] WeatherForecasts { get; set; } 
    }
}
