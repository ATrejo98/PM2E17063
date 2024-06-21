using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
namespace PM2E17063.Views;


public partial class ver_mapa : ContentPage
{
    String maplatitud, maplongitud, mapdescripcion;
    public ver_mapa(String latitud, String longitud, String descripcion)
    {
        InitializeComponent();
        maplatitud = latitud;
        maplongitud = longitud;
        mapdescripcion = descripcion;
    }

    private async void btnCompartir_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Share.RequestAsync(
               new ShareTextRequest
               {
                   Title = "Ubicacion",
                   Text = "Hola, te comparto la ubicación de "+ mapdescripcion,
                   Uri = "https://maps.google.com/?q=" + maplongitud + "," + maplatitud
               }
                );
        }
        catch
        {

        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var localizacion = Geolocation.GetLocationAsync();
        if (localizacion == null)
        {
            DisplayAlert("Advertencia", "Su GPS esta desactivado", "Ok");
        }

        Pin ubicacion = new Pin();
        ubicacion.Label = mapdescripcion.ToString();
        ubicacion.Type = PinType.Place;
        ubicacion.Location = new Location(Double.Parse(maplongitud), Double.Parse(maplatitud));
        mapa.Pins.Add(ubicacion);
        mapa.IsShowingUser = true;
        mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Double.Parse(maplongitud),
        Double.Parse(maplatitud)), Distance.FromMeters(500.0)));

    }
}


