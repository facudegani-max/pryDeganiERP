using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Usuario : Form
    {
        int intentos = 3;

        Conexion bd = new Conexion();

        public Usuario()
        {
            InitializeComponent();

            txtContraseña.PasswordChar = '*';

            // Allow Enter key to trigger the Ingresar button
            this.AcceptButton = btnIngresar;

            // Wire salir button to exit application
            this.btnSalir.Click += btnSalir_Click;

            // Mostrar/ocultar contraseña
            this.checkBoxContraseña.CheckedChanged += CheckBoxContraseña_CheckedChanged;
        }

        private void CheckBoxContraseña_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxContraseña.Checked)
            {
                txtContraseña.PasswordChar = '\0';
            }
            else
            {
                txtContraseña.PasswordChar = '*';
            }
        }

        private void Usuario_Load(object sender, EventArgs e)
        {
            // Ya no hace falta cargar perfiles
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string correo = txtUsuario.Text.Trim();
            string clave = txtContraseña.Text.Trim();

            if (correo == "")
            {
                MessageBox.Show("Debe completar usuario");
                return;
            }
            else if (clave == "")
            {
                MessageBox.Show("Debe completar contraseña.");
                return;
            }

            try
            {
                bd.AbrirConexion();

                string consulta = @"
                SELECT Perfil.Nombre
                FROM (Perfil
                INNER JOIN Relacion_Usuario_Perfil
                ON Perfil.IdPerfil = Relacion_Usuario_Perfil.IdPerfil)
                INNER JOIN Usuario
                ON Relacion_Usuario_Perfil.IdUsuario = Usuario.IdUsuario
                WHERE Usuario.Mail = ?
                AND Usuario.Contraseña = ?
                AND Usuario.Estado = True";

                OleDbCommand comando = new OleDbCommand(consulta, bd.ObtenerConexion());

                comando.Parameters.AddWithValue("@Mail", correo);
                comando.Parameters.AddWithValue("@Contraseña", clave);

                OleDbDataReader lector = comando.ExecuteReader();

                if (lector.Read())
                {
                    string perfil = lector["Nombre"].ToString();


                    // 1. Agregamos corchetes a [Fecha_Hora] por seguridad, ya que es una palabra sensible en Access
                    string sqlAuditoria = @"INSERT INTO Auditoria_Usuario (Usuario, [Fecha_Hora], Estado_Login, Opcion_Sistema) 
                        VALUES (?, ?, ?, ?)";

                    OleDbCommand cmdAuditoria = new OleDbCommand(sqlAuditoria, bd.ObtenerConexion());

                    // 2. Definimos explícitamente los tipos de datos en el ORDEN EXACTO de los signos de pregunta (?)
                    cmdAuditoria.Parameters.Add("@Usuario", OleDbType.VarChar).Value = correo;

                    // ESTA LINEA EVITA EL ERROR: Le dice a Access que va un tipo Fecha nativo, no un String adaptado
                    cmdAuditoria.Parameters.Add("@Fecha_Hora", OleDbType.Date).Value = DateTime.Now;

                    cmdAuditoria.Parameters.Add("@Estado_Login", OleDbType.VarChar).Value = "Ingreso";
                    cmdAuditoria.Parameters.Add("@Opcion_Sistema", OleDbType.VarChar).Value = perfil;

                    // 3. Ejecutamos la inserción
                    cmdAuditoria.ExecuteNonQuery();


                    string fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    Form ventanaAabrir = null;

                    switch (perfil)
                    {
                        case "Administrador":

                            Administrador ventanaAdmin = new Administrador();

                            ventanaAdmin.rolUsuario = perfil;
                            ventanaAdmin.fechaIngreso = fechaHora;
                            ventanaAdmin.usuarioActual = correo; // <-- set current user

                            ventanaAabrir = ventanaAdmin;

                            break;

                        case "Recursos Humanos":

                            ventanaAabrir = new RecursosHumanos();

                            break;

                        case "Contabilidad":

                            // Cuando tengas el formulario creado
                            // ventanaAabrir = new Contabilidad();

                            MessageBox.Show("Formulario Contabilidad aún no implementado.");

                            break;

                        default:

                            MessageBox.Show("Perfil no reconocido.");

                            break;
                    }

                    if (ventanaAabrir != null)
                    {
                        this.Hide();
                        ventanaAabrir.Show();
                    }
                }
                else
                {
                    intentos--;

                    MessageBox.Show(
                        "Usuario o contraseña incorrectos.\n\n" +
                        "Intentos restantes: " + intentos);

                    if (intentos <= 0)
                    {
                        MessageBox.Show(
                            "Se agotaron los intentos permitidos.");

                        Application.Exit();
                    }
                }

                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al iniciar sesión:\n\n" + ex.Message);

                bd.CerrarConexion();
            }

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}