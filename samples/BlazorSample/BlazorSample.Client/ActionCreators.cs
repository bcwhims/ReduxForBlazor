using ReduxForDotNet;
using ReduxForBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using System.Net.Http;
using BlazorSample.Shared;

namespace BlazorSample.Client
{
    public class Increment { }
    public class ActionCreators
    {
        public static Increment GetIncrementAction()
        {
            return null;
        }

        public static Action<Store<SampleClientState>> GetWeatherForecastsAction(HttpClient httpClient)
        {
            return async (store) =>
            {
                try
                {
                    var result = await httpClient.GetJsonAsync<WeatherForecast[]>("/api/SampleData/WeatherForecasts"); ;
                    store.Dispatch(result);
                }
                catch (Exception)
                {
                }
            };
        }
    }
}
