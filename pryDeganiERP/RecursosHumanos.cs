using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class RecursosHumanos : Form
    {
        public Administrador ownerAdminRef; // reference to Administrador if opened from there
        public string usuarioActual;
        public string rolUsuario;

        public RecursosHumanos()
        {
            InitializeComponent();

            this.Load += RecursosHumanos_Load;

            cmbProvincia.SelectedIndexChanged += cmbProvincia_SelectedIndexChanged;

            txtDni.KeyPress += txtDni_KeyPress;
            txtTelefono.KeyPress += txtTelefono_KeyPress;

            txtNombre.KeyPress += SoloLetras;
            txtApellido.KeyPress += SoloLetras;

            txtDireccion.KeyPress += txtDireccion_KeyPress;
            txtGeografia.KeyPress += txtGeografia_KeyPress;
            txtRedes.KeyPress += txtRedes_KeyPress;

            cmbProvincia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocalidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRedes.DropDownStyle = ComboBoxStyle.DropDownList;

            // Manejar boton salir
            this.btnSalir.Click += btnSalir_Click;
            this.btnEliminar.Click += btnEliminar_Click; // open Eliminar form
            this.btnModificar.Click += btnModificar_Click; // open Modificar form
        }

        // Constructor que recibe referencia al Administrador
        public RecursosHumanos(Administrador admin) : this()
        {
            ownerAdminRef = admin;
            if (admin != null)
            {
                usuarioActual = admin.usuarioActual;
                rolUsuario = admin.rolUsuario;
            }
        }

        private void RecursosHumanos_Load(object sender, EventArgs e)
        {
            CargarProvincias();
            CargarRedes();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Eliminar ventana = new Eliminar(this);
            this.Hide();
            ventana.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Modificar ventana = new Modificar(this);
            this.Hide();
            ventana.Show();
        }

        private void CargarProvincias()
        {
            cmbProvincia.Items.Clear();

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                string sql = "SELECT Nombre_Provincia FROM Provincia ORDER BY Nombre_Provincia";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());

                OleDbDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    cmbProvincia.Items.Add(dr["Nombre_Provincia"].ToString());
                }

                dr.Close();
                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar provincias: " + ex.Message);
            }
        }

        private void CargarLocalidades()
        {
            cmbLocalidad.Items.Clear();

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                string sql =
                @"SELECT L.nombre_localidad
                  FROM Localidad L
                  INNER JOIN Provincia P
                  ON L.id_provincia = P.Id_Provincia
                  WHERE P.Nombre_Provincia = ?
                  ORDER BY L.nombre_localidad";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());

                cmd.Parameters.AddWithValue("@Provincia", cmbProvincia.Text);

                OleDbDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    cmbLocalidad.Items.Add(dr["nombre_localidad"].ToString());
                }

                dr.Close();
                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar localidades: " + ex.Message);
            }
        }

        private void CargarRedes()
        {
            cmbRedes.Items.Clear();

            cmbRedes.Items.Add("Instagram");
            cmbRedes.Items.Add("Facebook");
            cmbRedes.Items.Add("TikTok");
            cmbRedes.Items.Add("X");
            cmbRedes.Items.Add("LinkedIn");
            cmbRedes.Items.Add("YouTube");
        }

        private void cmbProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLocalidades();
        }

        // ==========================
        // VALIDACIONES
        // ==========================

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void SoloLetras(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar) &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtDireccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar) &&
                e.KeyChar != '.' &&
                e.KeyChar != ',' &&
                e.KeyChar != '-' &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtGeografia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) &&
                e.KeyChar != '.' &&
                e.KeyChar != ',' &&
                e.KeyChar != '-' &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtRedes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) &&
                e.KeyChar != '_' &&
                e.KeyChar != '.' &&
                e.KeyChar != '-' &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        // ==========================
        // EVENTOS GENERADOS POR VS
        // ==========================

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtDni_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDireccion_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtGeografia_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbLocalidad_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtMail_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRedes_TextChanged(object sender, EventArgs e)
        {

        }

        // Botón salir: vuelve al Administrador si se abrió desde él y el rol es Administrador,
        // sino vuelve a la pantalla de Usuario (login).
        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (ownerAdminRef != null && ownerAdminRef.rolUsuario == "Administrador")
            {
                this.Hide();
                ownerAdminRef.Show();
                // Do not close ownerAdminRef
                this.Close();
            }
            else
            {
                Usuario login = new Usuario();
                this.Hide();
                login.Show();
                this.Close();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // ==========================
            // VALIDACIONES
            // ==========================
            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("El DNI es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El Apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                MessageBox.Show("El Mail es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMail.Focus();
                return;
            }
            if (cmbProvincia.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccioná una Provincia.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProvincia.Focus();
                return;
            }
            if (cmbLocalidad.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccioná una Localidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbLocalidad.Focus();
                return;
            }

            // ==========================
            // INSERCIÓN EN BASE DE DATOS
            // ==========================
            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();
                OleDbConnection con = bd.ObtenerConexion();

                // ── 1. Insertar en tabla Usuario ──────────────────────────────
                string sqlUsuario =
                    @"INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, Dni, Estado)
              VALUES (?, ?, ?, ?, ?, ?)";

                OleDbCommand cmdUsuario = new OleDbCommand(sqlUsuario, con);
                cmdUsuario.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Mail", txtMail.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Contrasena", ""); // Sin campo en el form, se deja vacío o asignás uno por defecto
                cmdUsuario.Parameters.AddWithValue("@Dni", txtDni.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Estado", checkBoxActivo.Checked ? 1 : 0);
                cmdUsuario.ExecuteNonQuery();

                // ── Obtener el ID recién insertado ────────────────────────────
                OleDbCommand cmdId = new OleDbCommand("SELECT @@IDENTITY", con);
                int nuevoId = Convert.ToInt32(cmdId.ExecuteScalar());

                // ── 2. Insertar en tabla Domicilio_Usuario ────────────────────
                string sqlDomicilio =
                    @"INSERT INTO Domicilio_Usuario (Id_Usuario, GPS, Provincia, Localidad, Direccion)
              VALUES (?, ?, ?, ?, ?)";

                OleDbCommand cmdDomicilio = new OleDbCommand(sqlDomicilio, con);
                cmdDomicilio.Parameters.AddWithValue("@Id_Usuario", nuevoId);
                cmdDomicilio.Parameters.AddWithValue("@GPS", txtGeografia.Text.Trim());
                cmdDomicilio.Parameters.AddWithValue("@Provincia", cmbProvincia.Text);
                cmdDomicilio.Parameters.AddWithValue("@Localidad", cmbLocalidad.Text);
                cmdDomicilio.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                cmdDomicilio.ExecuteNonQuery();

                // ── 3. Insertar en tabla Contacto_Usuario ─────────────────────
                string sqlContacto =
                    @"INSERT INTO Contacto_Usuario (Id_Usuario, Telefono, Redes_Sociales)
              VALUES (?, ?, ?)";

                // Armamos el valor de redes: "NombreRed: @usuario"  (ej: "Instagram: @juan")
                string redesValor = cmbRedes.SelectedIndex >= 0
                    ? cmbRedes.Text + ": " + txtRedes.Text.Trim()
                    : txtRedes.Text.Trim();

                OleDbCommand cmdContacto = new OleDbCommand(sqlContacto, con);
                cmdContacto.Parameters.AddWithValue("@Id_Usuario", nuevoId);
                cmdContacto.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                cmdContacto.Parameters.AddWithValue("@Redes_Sociales", redesValor);
                cmdContacto.ExecuteNonQuery();

                bd.CerrarConexion();

                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al guardar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // LIMPIAR FORMULARIO
        // ==========================
        private void LimpiarFormulario()
        {
            txtDni.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtMail.Text = "";
            txtDireccion.Text = "";
            txtGeografia.Text = "";
            txtTelefono.Text = "";
            txtRedes.Text = "";
            cmbProvincia.SelectedIndex = -1;
            cmbLocalidad.SelectedIndex = -1;
            cmbRedes.SelectedIndex = -1;
            checkBoxActivo.Checked = false;
        }

        private void RecursosHumanos_Load_1(object sender, EventArgs e)
        {

        }
    }
}