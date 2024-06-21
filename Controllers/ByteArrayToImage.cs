namespace PM2E17063.Converters
{
    public class ByteArrayToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[] byteArray)
            {
                try
                {
                    return ImageSource.FromStream(() => new MemoryStream(byteArray));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al convertir byte[] a ImageSource: " + ex.Message);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
