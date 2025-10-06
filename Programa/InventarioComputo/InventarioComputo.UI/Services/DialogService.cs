using System;
using System.Collections.Concurrent;
using System.Windows;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace InventarioComputo.UI.Services
{
    public sealed class DialogService : IDialogService
    {
        private readonly IServiceProvider _rootProvider;
        private readonly ConcurrentDictionary<Type, Type> _registry = new();

        public DialogService(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : BaseViewModel
            where TView : Window
        {
            _registry[typeof(TViewModel)] = typeof(TView);
        }

        public bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState)
            where TViewModel : BaseViewModel
        {
            if (!_registry.TryGetValue(typeof(TViewModel), out var viewType))
                throw new InvalidOperationException($"No se registró una vista para {typeof(TViewModel).Name}");

            // Scope por diálogo: DbContext y dependencias aisladas
            using var scope = _rootProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var vm = sp.GetRequiredService<TViewModel>();
            setViewModelState?.Invoke(vm);

            var viewObj = sp.GetRequiredService(viewType);
            if (viewObj is not Window view)
                throw new InvalidOperationException($"El tipo registrado {viewType.Name} no es una Window.");

            if (view.DataContext == null)
                view.DataContext = vm;

            if (System.Windows.Application.Current?.MainWindow != view)
                view.Owner = System.Windows.Application.Current?.MainWindow;

            // Mostrar el diálogo; al cerrar, se libera el scope (y el DbContext)
            var result = view.ShowDialog();
            return result;
        }

        public void ShowInfo(string message) =>
            MessageBox.Show(message, "Información", MessageBoxButton.OK, MessageBoxImage.Information);

        public void ShowError(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        public bool Confirm(string message, string title) =>
            MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}