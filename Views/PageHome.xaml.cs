
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PM2E17063.Views;
using PM2E17063.Models;
using PM2E17063.Controllers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;

namespace PM2E17063.Views
{
    public partial class PageHome : ContentPage
    {

        FileResult photo;
        public PageHome()
        {
            InitializeComponent();
            obtenerLatitudLongitud();
        }

        //Obtener GPS
        public async void obtenerLatitudLongitud()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

                var tokendecancelacion = new System.Threading.CancellationTokenSource();

                var localizacion = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);


                if (localizacion != null)
                {
                    txtlatitud.Text = localizacion.Latitude.ToString();
                    txtlongitud.Text = localizacion.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "Ok");
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Advertencia", "Su GPS se encuentra desactivado, favor volver a ingresar con el GPS activado", "Ok");
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Geolocalizacion" + pEx, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "Ok");
            }
        }


        byte[] imageToSave;
        //void para tomar la foto
        private async void btnfoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    imageToSave = null;

                    string localizacion = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using (Stream sourceStream = await photo.OpenReadAsync())
                    {
                        using (FileStream imagenLocal = File.OpenWrite(localizacion))
                        {
                            await sourceStream.CopyToAsync(imagenLocal);
                        }

                        sourceStream.Position = 0;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await sourceStream.CopyToAsync(memoryStream);
                            imageToSave = memoryStream.ToArray();
                        }

                        img.Source = ImageSource.FromStream(() => new MemoryStream(imageToSave));
                    }

                    obtenerLatitudLongitud();
                    txtdescripcion.Focus();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Se ha generado el siguiente error al agregar la imagen: " + ex.Message, "Aceptar");
            }
        }


        private async void OnAgregarClicked(object sender, EventArgs e)
        {
            if (imageToSave == null)
            {
                await DisplayAlert("Aviso!", "Ingrese una imagen del sitio", "Aceptar");
            }
            else if (txtdescripcion.Text == null)
            {
                await DisplayAlert("Aviso!", "Ingrese una descripcion del sitio", "Aceptar");
            }
            else
            {
                var sitio = new Sitios { imagen = imageToSave, longitud = txtlatitud.Text, latitud = txtlongitud.Text, descripcion = txtdescripcion.Text };
                var resultado = await App.Instancia.sitioSave(sitio);

                if (resultado != 0)
                {
                    await DisplayAlert("Aviso", "Sitio registrado con exito!", "Aceptar");
                    txtdescripcion.Text = "";
                    img.Source = "anadir.png";
                    imageToSave = null;

                }
                else
                {
                    await DisplayAlert("Aviso", "Ha Ocurrido un Error", "Aceptar");
                }


                await Navigation.PopAsync();
            }

        }

        private async void OnListaClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new PageList());

        }

        private void OnSalirClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();

        }
    }
}
