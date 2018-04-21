using BlazorSample.Shared;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using ReduxForDotNet;
using System;

namespace BlazorSample.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
                services.AddSingleton(
                    StoreBuilder<SampleClientState>
                    .Create()
                    .WithInitialState(
                        new SampleClientState
                        {
                            CurrentCount = 0,
                            WeatherForecasts = null
                        }
                    )
                    .WithThunkMiddleWare()
                    .WithSelector("CURRENTCOUNT", (state) => state.CurrentCount)
                    .WithSelector("WEATHERFORECASTS", (state) => state.WeatherForecasts)
                    .WithReducers<Increment>((state, action) => new SampleClientState { CurrentCount = state.CurrentCount + 1, WeatherForecasts = state.WeatherForecasts })
                    .WithReducers<WeatherForecast[]>((state, action) => new SampleClientState { CurrentCount = state.CurrentCount, WeatherForecasts = action })
                    .Build()
                    )
            );

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
