﻿using SQLite;
using PM2E17063.Models;

namespace PM2E17063.Controllers
{
    public class DBSitios
    {
        readonly SQLiteAsyncConnection _connection;
        public DBSitios(string dbpath)
        {
            _connection = new SQLiteAsyncConnection(dbpath);


            _connection.CreateTableAsync<Sitios>();
        }

        public Task<int> sitioSave(Sitios sitios)
        {
            if (sitios.id != 0)
            {
                return _connection.UpdateAsync(sitios);
            }
            else
            {
                return _connection.InsertAsync(sitios);
            }
        }
        public Task<List<Sitios>> ObtenerlistadoSitio()

        {
            return _connection.Table<Sitios>().ToListAsync();
        }

        public Task<int> eliminarsitio(Sitios sitios)
        {
            return _connection.DeleteAsync(sitios);

        }
        public Task<Sitios> ObtenerLongitud(string uLongitud)
        {
            return _connection.Table<Sitios>().Where(i => i.longitud == uLongitud).FirstOrDefaultAsync();
        }

        public Task<Sitios> ObtenerLatitud(string uLatitud)
        {
            return _connection.Table<Sitios>().Where(i => i.latitud == uLatitud).FirstOrDefaultAsync();
        }

        public Task<Sitios> ObtenerDescripcion(String uDescripcion)
        {
            return _connection.Table<Sitios>().Where(i => i.descripcion == uDescripcion).FirstOrDefaultAsync();
        }


    }
}
