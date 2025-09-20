using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;

namespace InventarioComputo.UI.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        // Diccionario para mapear ViewModels a Vistas
        private readonly Dictionary<Type, Type> _mappings = new();

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Nuevo método para registrar las asociaciones
        public void Register<TViewModel, TView>() where TViewModel : BaseViewModel where TView : Window
        {
            _mappings[typeof(TViewModel)] = typeof(TView);
        }

        public bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState) where TViewModel : BaseViewModel
        {
            if (!_mappings.ContainsKey(typeof(TViewModel)))
            {
                throw new KeyNotFoundException($"No se encontró un mapeo para el ViewModel '{typeof(TViewModel).Name}'");
            }

            var viewType = _mappings[typeof(TViewModel)];
            var window = (Window)_serviceProvider.GetRequiredService(viewType);
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            setViewModelState(viewModel);
            window.DataContext = viewModel;

            return window.ShowDialog();
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool Confirm(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}