namespace PM2E17063
{
    public partial class App : Application
    {

        static Controllers.DBSitios dBSitios;
        public static Controllers.DBSitios Instancia
        {
            get
            {
                if (dBSitios == null)
                {
                    String dbname = "SitiosDB.db3";
                    String dbpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    String dbfulp = Path.Combine(dbpath, dbname);
                    dBSitios = new Controllers.DBSitios(dbfulp);

                }
                return dBSitios;
            }
        }


        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.PageHome());
        }

    }
}
