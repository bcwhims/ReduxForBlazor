using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;

namespace ReduxForBlazor
{
    public static class ExtensionMethods
    {
        public static async Task<TAction> GetJsonAsync<TJson, TAction>(this HttpClient httpClient, string requestUri, Func<TJson, TAction> actionCreator)
        {
            var response = await httpClient.GetJsonAsync<TJson>(requestUri);
            return actionCreator(response);
        }
        public static async Task<TAction> PostJsonAsync<TJson, TAction>(this HttpClient httpClient, string requestUri, object content, Func<TJson, TAction> actionCreator)
        {
            var response = await httpClient.PostJsonAsync<TJson>(requestUri, content);
            return actionCreator(response);
        }
        public static async Task<TAction> PutJsonAsync<TJson, TAction>(this HttpClient httpClient, string requestUri, object content, Func<TJson, TAction> actionCreator)
        {
            var response = await httpClient.PutJsonAsync<TJson>(requestUri, content);
            return actionCreator(response);
        }
        public static async Task<TAction> SendJsonAsync<TJson, TAction>(this HttpClient httpClient, HttpMethod method, string requestUri, object content, Func<TJson, TAction> actionCreator)
        {
            var response = await httpClient.SendJsonAsync<TJson>(method, requestUri, content);
            return actionCreator(response);
        }
    }
}
