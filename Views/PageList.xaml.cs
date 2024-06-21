using System.Collections.ObjectModel;
using System.Windows.Input;
using PM2E17063.Models;
using PM2E17063.Controllers;

namespace PM2E17063.Views
{
    public partial class PageList : ContentPage
    {
        public ObservableCollection<Sitios> SitiosCollection { get; set; }
        private readonly DBSitios _dbSitios;

        public ICommand DeleteSelectedCommand { get; }
        public ICommand ShareImageSelectedCommand { get; }

        public PageList()
        {
            InitializeComponent();

            _dbSitios = App.Instancia;

            DeleteSelectedCommand = new Command(async () => await OnDeleteCommand(), CanExecuteDeleteOrShare);
            ShareImageSelectedCommand = new Command(async () => await OnShareCommandImage(), CanExecuteDeleteOrShare);

            LoadData();

            BindingContext = this;
        }

        private async void LoadData()
        {
            try
            {
                var sitiosList = await _dbSitios.ObtenerlistadoSitio();
                SitiosCollection = new ObservableCollection<Sitios>(sitiosList);

                // Monitorea cambios en la selección para habilitar/deshabilitar botones
                foreach (var sitio in SitiosCollection)
                {
                    sitio.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(Sitios.IsSelected))
                        {
                            (DeleteSelectedCommand as Command)?.ChangeCanExecute();
                            (ShareImageSelectedCommand as Command)?.ChangeCanExecute();
                        }
                    };
                }

                sitiosCollectionView.ItemsSource = SitiosCollection;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al cargar datos: " + ex.Message, "Aceptar");
            }
        }




        private bool CanExecuteDeleteOrShare()
        {
            return SitiosCollection != null && SitiosCollection.Any(s => s.IsSelected);
        }

        private async Task OnDeleteCommand()
        {
            var selectedItems = SitiosCollection.Where(s => s.IsSelected).ToList();

            if (selectedItems.Any())
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmación", "¿Estás seguro de querer eliminar los elementos seleccionados?", "Sí", "Cancelar");

                if (confirm)
                {
                    foreach (var sitio in selectedItems)
                    {
                        await _dbSitios.eliminarsitio(sitio);
                        SitiosCollection.Remove(sitio);
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "No hay elementos seleccionados para eliminar.", "Aceptar");
            }
        }

        private async Task OnShareCommandImage()
        {
            var selectedItems = SitiosCollection.Where(s => s.IsSelected).ToList();

            if (selectedItems.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "No hay elementos seleccionados para compartir.", "Aceptar");
                return;
            }

            List<ShareFile> shareFiles = new List<ShareFile>();

            // Recorrer los elementos seleccionados y guardar cada imagen en almacenamiento temporal
            foreach (var sitio in selectedItems)
            {
                var tempImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{sitio.id}_sharedimage.png");
                File.WriteAllBytes(tempImagePath, sitio.imagen);
                shareFiles.Add(new ShareFile(tempImagePath));
            }

            // Compartir las imágenes usando .NET MAUI
            await Share.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Compartir imágenes",
                Files = shareFiles
            });
        }

    }
}
