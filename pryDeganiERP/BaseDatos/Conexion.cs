using System;
using System.Data;
using System.Data.OleDb;

namespace pryDeganiERP
{
    internal class Conexion
    {
        // Ruta de la base de datos
        private string cadenaConexion =@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\BaseDatos\Degani.accdb;";
        private OleDbConnection conexion;


        // Constructor
        public Conexion()
        {
            conexion = new OleDbConnection(cadenaConexion);
        }

        // Abrir conexión
        public void AbrirConexion()
        {
            if (conexion.State == ConnectionState.Closed)
            {
                conexion.Open();
            }
        }

        // Cerrar conexión
        public void CerrarConexion()
        {
            if (conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }

        // Obtener conexión
        public OleDbConnection ObtenerConexion()
        {
            return conexion;
        }
    }
}