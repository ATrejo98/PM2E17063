using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Coordenadas centrales de San Pedro Sula, Honduras
        var sanPedroSulaCenter = new Location(15.5022, -88.0336);
        var initialRadius = Distance.FromMiles(10);

        // Inicializar el mapa centrado en SPS
        mapa.MoveToRegion(MapSpan.FromCenterAndRadius(sanPedroSulaCenter, initialRadius));

        var localizacion = await Geolocation.GetLocationAsync(new GeolocationRequest
        {
            DesiredAccuracy = GeolocationAccuracy.Best,
            Timeout = TimeSpan.FromSeconds(10)
        });

        if (localizacion == null)
        {
            await DisplayAlert("Advertencia", "Su GPS está desactivado o no se puede obtener la ubicación", "Ok");
            return;
        }


        // Ubicación actual
        var ubicacionActual = new Location(localizacion.Latitude, localizacion.Longitude);
        var pinActual = new Pin
        {
            Label = "Ubicación Actual",
            Type = PinType.Place,
            Location = ubicacionActual
        };
        mapa.Pins.Add(pinActual);

        // Ubicación de destino
        Pin ubicacionDestino = new Pin();
        ubicacionDestino.Label = mapdescripcion.ToString();
        ubicacionDestino.Type = PinType.Place;
        ubicacionDestino.Location = new Location(Double.Parse(maplongitud), Double.Parse(maplatitud));
        mapa.Pins.Add(ubicacionDestino);
        mapa.IsShowingUser = true;
        mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Double.Parse(maplongitud),
        Double.Parse(maplatitud)), Distance.FromMeters(10)));

        var ubicacionDestino2 = new Location(Double.Parse(maplongitud), Double.Parse(maplatitud));

        var polyline = new Polyline
        {
            StrokeColor = Microsoft.Maui.Graphics.Colors.Red,
            StrokeWidth = 5
        };
        polyline.Geopath.Add(ubicacionActual);
        polyline.Geopath.Add(ubicacionDestino2);

        mapa.MapElements.Add(polyline);

        var centralPosition = new Location(
            (ubicacionActual.Latitude + Double.Parse(maplongitud)) / 2,
            (ubicacionActual.Longitude + Double.Parse(maplatitud)) / 2
        );
        mapa.MoveToRegion(MapSpan.FromCenterAndRadius(centralPosition, Distance.FromMiles(10)));

        var distancia = Location.CalculateDistance(ubicacionActual, ubicacionDestino2, DistanceUnits.Kilometers);

        await DisplayAlert("Distancia", $"La distancia al destino es: {distancia:F2} km", "Ok");
    }
}


