using System.Collections.ObjectModel;
using PM2E17063.Models;
using PM2E17063.Controllers;

namespace PM2E17063.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageList : ContentPage
    {

        private Sitios sitio;

        public PageList()
        {
            InitializeComponent();
        }

        private void liestasistios_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            sitio = (Sitios)e.Item;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            listasitios.ItemsSource = await App.Instancia.ObtenerlistadoSitio();
        }

        private async void btnvermapa_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ver_mapa(sitio.latitud, sitio.longitud, sitio.descripcion));
            }
            catch
            {
                await DisplayAlert("Advertencia", "Favor seleccione el sitio donde desea ver en el mapa", "Ok");
            }
        }

        private async void btneliminarsitio_Clicked(object sender, EventArgs e)
        {
            try
            {

                bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmación", "¿Estás seguro de querer eliminar el sito seleccionado?", "Sí", "Cancelar");

                if (confirm)
                {
                    var eliminar = await App.Instancia.eliminarsitio(sitio);


                    if (eliminar != 0)
                    {
                        await DisplayAlert("Advertencia", "Sitio eliminado con exito", "Aceptar");
                        listasitios.ItemsSource = await App.Instancia.ObtenerlistadoSitio();

                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "Ha ocurrido un error", "Aceptar");
                    }
                }


                
            }
            catch
            {
                await DisplayAlert("Advertencia", "Favor seleccione que sitio desea eliminar", "Aceptar");
            }

        }
    }
}
