using Microsoft.AspNetCore.Blazor.Components;
using ReduxForDotNet;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ReduxForBlazor
{
    public class ReduxForBlazorComponent<TState>: BlazorComponent, IDisposable
    {
        protected Action<TState> updatePropsFromState;
        protected bool IsReadyToRender { get; set; } = true;

        protected static Action<TPropType> GetSetMethodDelegate<TTarget, TPropType>(TTarget target, PropertyInfo property)
        {
            return (Action<TPropType>)
               Delegate.CreateDelegate(typeof(Action<TPropType>), target,
                   property.GetSetMethod() ?? property.GetSetMethod(true));
        }

        protected Action<TState> Map<TTarget, TPropType>(TTarget target, Expression<Func<TTarget, TPropType>> expression, string selector)
        {
            return Map(target, expression, Store.GetSelector<TPropType>(selector));
        }

        protected Action<TState> Map<TTarget, TPropType>(TTarget target, Expression<Func<TTarget, TPropType>> expression, Func<TState, TPropType> selector)
        {
            try
            {
                var memberExpression = expression.Body as MemberExpression;
                var fieldInfo = memberExpression.Member as FieldInfo;
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    var propSetter = GetSetMethodDelegate<TTarget, TPropType>(target, propertyInfo);
                    return (state) => propSetter(selector(state));
                }
                if (fieldInfo != null)
                {
                    return (state) => (fieldInfo).SetValue(target, selector(state));
                }
            }
            catch(Exception ex)
            {
                Console.Out.WriteLine(ex);
                foreach(var line in ex.StackTrace.Split(new string[] {Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Console.WriteLine( line.Substring(0, line.Length < 75 ? line.Length: 75));
                }
            }
            return null;
        }

        private void Store_StateChanged(object sender, StateChangeEventArgs<TState> e)
        {
            Console.Out.WriteLine("Store State Changed");
            updatePropsFromState(e.NewState);
            StateHasChanged();
        }

        protected override void OnInit()
        {
            base.OnInit();
            Store.StateChanged += Store_StateChanged;
        }

        protected virtual void OnClosing()
        {
            Store.StateChanged -= Store_StateChanged;
        }

        public Func<TState,TReturn> GetSelector<TReturn>(string selectorKey)
        {
            return Store.GetSelector<TReturn>(selectorKey);
        }

        public void Dispatch<TAction>(TAction action)
        {
            Store.Dispatch(action);
        }

        protected void PauseRender()
        {
            IsReadyToRender = false;
        }

        protected void ResumeRender()
        {
            IsReadyToRender = true;
        }

        protected override bool ShouldRender()
        {
            return IsReadyToRender;
        }

        public void DispatchThunk<TResult>(Func<Task<TResult>> action, Action<Store<TState>, TResult> then, Action<Store<TState>, Exception> error)
        {
            Store.Dispatch<Action<Store<TState>>>(async (store) =>
            {
                try
                {
                    var result = await action();
                    then(store, result);
                }
                catch(Exception ex)
                {
                    error(store, ex);
                }
            });
        }

        public void DispatchThunk(Action<Store<TState>> action)
        {
            Store.Dispatch(action);
        }

        protected virtual void MapStateToProps(params Action<TState>[] actions)
        {
            foreach(var action in actions.Where(a => a != null))
            {
                updatePropsFromState += action;
            }
            updatePropsFromState(Store.State);
        }

        public void Dispose()
        {
            OnClosing();
        }

        [Inject] public Store<TState> Store { get; set; }

        
    }
}
