using System;
using System.Collections.ObjectModel;
using PM2E17063.Models;
using PM2E17063.Controllers;
using System.Threading.Tasks;

namespace PM2E17063.Views
{
    public partial class PageList : ContentPage
    {
        public ObservableCollection<Sitios> SitiosCollection { get; set; }
        private readonly DBSitios _dbSitios;

        public PageList()
        {
            InitializeComponent();

            _dbSitios = App.Instancia; 

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var sitiosList = await _dbSitios.ObtenerlistadoSitio();

                SitiosCollection = new ObservableCollection<Sitios>(sitiosList);

                sitiosCollectionView.ItemsSource = SitiosCollection;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al cargar datos: " + ex.Message, "Aceptar");
            }
        }
    }
}
