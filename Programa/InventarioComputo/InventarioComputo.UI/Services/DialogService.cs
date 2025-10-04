using System;
using System.Collections.Generic;
using System.Windows;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace InventarioComputo.UI.Services
{
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : BaseViewModel
            where TView : Window
        {
            _viewModelToViewMap[typeof(TViewModel)] = typeof(TView);
        }

        public bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState) where TViewModel : BaseViewModel
        {
            try
            {
                var vmType = typeof(TViewModel);
                Type viewType;

                if (_viewModelToViewMap.TryGetValue(vmType, out var registeredViewType))
                {
                    viewType = registeredViewType;
                }
                else
                {
                    var viewTypeName = vmType.FullName!
                        .Replace("ViewModels", "Views")
                        .Replace("ViewModel", "View");

                    var resolved = Type.GetType(viewTypeName);
                    if (resolved == null)
                        throw new InvalidOperationException($"No se encontró la vista para {vmType.Name} ({viewTypeName}). Regístrela con Register<>().");

                    viewType = resolved;
                }

                var window = _serviceProvider.GetService(viewType) as Window
                             ?? ActivatorUtilities.CreateInstance(_serviceProvider, viewType) as Window
                             ?? throw new InvalidOperationException($"La vista {viewType.Name} no es una Window.");

                var vmObj = _serviceProvider.GetService(vmType)
                           ?? ActivatorUtilities.CreateInstance(_serviceProvider, vmType);

                if (vmObj is not TViewModel vm)
                    throw new InvalidOperationException($"No se pudo crear instancia de {vmType.Name}.");

                setViewModelState(vm);
                window.DataContext = vm;

                if (System.Windows.Application.Current?.MainWindow != null)
                {
                    window.Owner = System.Windows.Application.Current.MainWindow;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                // Respetar tamaños definidos en XAML de la ventana (no forzar SizeToContent aquí)
                var result = window.ShowDialog();

                if (vm is IEditorViewModel evm && result == null)
                    return evm.DialogResult;

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar diálogo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void ShowInfo(string message) => MessageBox.Show(message, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        public void ShowError(string message) => MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        public bool Confirm(string message, string title) => MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}