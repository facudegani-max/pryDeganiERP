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
            // Oculta la contraseña con asteriscos
            txtContraseña.PasswordChar = '*';
        }

        // Este evento se ejecuta SOLO al abrir el formulario
        private void Usuario_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Abrimos la conexión para traer los perfiles
                bd.AbrirConexion();

                // Consulta a tu tabla de Perfiles (columna Nombre)
                string consultaPerfiles = "SELECT Nombre FROM Perfil";

                OleDbCommand comando = new OleDbCommand(consultaPerfiles, bd.ObtenerConexion());
                OleDbDataReader lector = comando.ExecuteReader();

                // Limpiamos el ComboBox
                cmbPerfil.Items.Clear();

                // Recorremos la base de datos y agregamos los perfiles al combo
                while (lector.Read())
                {
                    cmbPerfil.Items.Add(lector["Nombre"].ToString());
                }

                // Deja seleccionado el primer perfil por defecto
                if (cmbPerfil.Items.Count > 0)
                {
                    cmbPerfil.SelectedIndex = 0;
                }

                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de perfiles: " + ex.Message);
                bd.CerrarConexion();
            }
        }

        // Este evento se ejecuta cuando el usuario presiona el botón "Ingresar"
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Verificamos que el usuario haya seleccionado un perfil de la lista
            if (cmbPerfil.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, seleccione su perfil antes de ingresar.");
                return;
            }

            // Guardamos lo que el usuario escribió y seleccionó en la pantalla
            string correo = txtUsuario.Text;
            string clave = txtContraseña.Text;
            string perfilSeleccionado = cmbPerfil.SelectedItem.ToString();

            try
            {
                bd.AbrirConexion();

                // CONSULTA SIMPLIFICADA: Solo busca en la tabla Usuario por Mail y Contraseña
                string consulta = "SELECT * FROM [Usuario] WHERE [Mail] = ? AND [Contraseña] = ?";

                OleDbCommand comando = new OleDbCommand(consulta, bd.ObtenerConexion());

                // Pasamos los parámetros en orden estricto
                comando.Parameters.AddWithValue("@Mail", correo);
                comando.Parameters.AddWithValue("@Contraseña", clave);

                OleDbDataReader lector = comando.ExecuteReader();

                // Si encuentra el usuario y la contraseña...
                if (lector.Read())
                {
                    string fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    Form ventanaAabrir = null;

                    // Evaluamos directamente lo que el usuario seleccionó en el ComboBox
                    switch (perfilSeleccionado)
                    {
                        case "Administrador":
                            Administrador ventanaAdmin = new Administrador();
                            ventanaAdmin.rolUsuario = perfilSeleccionado;
                            ventanaAdmin.fechaIngreso = fechaHora;
                            ventanaAabrir = ventanaAdmin;
                            break;

                        case "Recursos Humanos":
                            RecursosHumanos ventanaRRHH = new RecursosHumanos();
                            ventanaAabrir = ventanaRRHH;
                            break;

                        case "Contabilidad":
                            // Descomentar cuando crees el formulario Contabilidad
                            // Contabilidad ventanaConta = new Contabilidad();
                            // ventanaAabrir = ventanaConta;
                            break;

                        default:
                            MessageBox.Show("El perfil seleccionado no tiene un formulario asignado.");
                            break;
                    }

                    // Si el formulario asignado es válido, lo abrimos
                    if (ventanaAabrir != null)
                    {
                        ventanaAabrir.Show();
                        this.Hide();
                    }
                }
                else
                {
                    intentos--;
                    MessageBox.Show("Usuario o contraseña incorrectos. Intente nuevamente.");

                    if (intentos == 0)
                    {
                        MessageBox.Show("Se han agotado los 3 intentos permitidos.");
                        Application.Exit();
                    }
                }

                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el inicio de sesión: " + ex.Message);
                bd.CerrarConexion();
            }
        }
    }
}