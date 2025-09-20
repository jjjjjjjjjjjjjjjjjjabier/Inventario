using InventarioComputo.UI.ViewModels;
using InventarioComputo.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;

namespace InventarioComputo.UI.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _mappings = new();

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mappings.Add(typeof(EstadoEditorViewModel), typeof(EstadoEditorView));
            _mappings.Add(typeof(UnidadEditorViewModel), typeof(UnidadEditorView));
            _mappings.Add(typeof(SedeEditorViewModel), typeof(SedeEditorView));
            _mappings.Add(typeof(AreaEditorViewModel), typeof(AreaEditorView));
            _mappings.Add(typeof(ZonaEditorViewModel), typeof(ZonaEditorView));
            _mappings.Add(typeof(TipoEquipoEditorViewModel), typeof(TipoEquipoEditorView)); 
            _mappings.Add(typeof(EquipoComputoEditorViewModel), typeof(EquipoComputoEditorView));
        }

        public bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState) where TViewModel : class, IEditorViewModel
        {
            var viewType = _mappings[typeof(TViewModel)];
            var view = (Window)_serviceProvider.GetRequiredService(viewType);
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            setViewModelState(viewModel);
            view.DataContext = viewModel;
            return view.ShowDialog();
        }
    }
}