using PM2E17063.Models;
using Microsoft.Maui.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace PM2E17063.Views
{
    public partial class PageHome : ContentPage
    {

        public PageHome()
        {
            InitializeComponent();       
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


        private async void OnSalirClicked(object sender, EventArgs e)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmación", "¿Desea cerrar la App?", "Sí", "Cancelar");

            if (confirm)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
