using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Usuario : Form
    {
        // Cantidad máxima de intentos
        int intentos = 3;

        // Objeto de conexión
        Conexion bd = new Conexion();

        public Usuario()
        {
            InitializeComponent();

            // Oculta la contraseña
            txtContraseña.PasswordChar = '*';
        }

        private void Usuario_Load(object sender, EventArgs e)
        {

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Guarda lo que escribe el usuario
            string correo = txtUsuario.Text;
            string clave = txtContraseña.Text;

            try
            {
                // Abre conexión
                bd.AbrirConexion();

                // Consulta SQL
                string consulta = "SELECT * FROM Usuarios " +
                                  "WHERE Correo = ? AND Clave = ?";

                // Comando SQL
                OleDbCommand comando = new OleDbCommand(
                    consulta,
                    bd.ObtenerConexion());

                // Parámetros
                comando.Parameters.AddWithValue("@Correo", correo);
                comando.Parameters.AddWithValue("@Clave", clave);

                // Ejecuta lectura
                OleDbDataReader lector = comando.ExecuteReader();

                // Si encuentra usuario
                if (lector.Read())
                {
                    // Obtiene el rol desde la BD
                    string rol = lector["Rol"].ToString();

                    // Fecha y hora actual
                    string fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Abre formulario principal
                    Main ventana = new Main();

                    // Envía datos al otro formulario
                    ventana.rolUsuario = rol;
                    ventana.fechaIngreso = fechaHora;

                    // Muestra ventana principal
                    ventana.Show();

                    // Oculta login
                    this.Hide();
                }
                else
                {
                    // Resta intentos
                    intentos--;

                    // Muestra intentos restantes
                    lblIntentos.Text = "Intentos restantes: " + intentos;

                    MessageBox.Show("Correo o contraseña incorrectos");

                    // Si llega a 0
                    if (intentos == 0)
                    {
                        MessageBox.Show("Se agotaron los intentos");

                        Application.Exit();
                    }
                }

                // Cierra conexión
                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                bd.CerrarConexion();
            }
        }
    }
}