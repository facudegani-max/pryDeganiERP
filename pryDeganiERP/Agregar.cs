using System;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Agregar : Form
    {
        public RecursosHumanos ownerRecursos;

        public Agregar()
        {
            InitializeComponent();

            this.Load += Agregar_Load;

            this.btnCargar.Click += btnCargar_Click;
            this.btnSalir.Click += btnSalir_Click;
            // Input restrictions
            this.textBox1.KeyPress += TxtNumeric_KeyPress; // DNI
            this.txtTelefono.KeyPress += TxtNumeric_KeyPress;
            this.txtNombre.KeyPress += TxtLettersOnly_KeyPress;
            this.txtApellido.KeyPress += TxtLettersOnly_KeyPress;
        }

        public Agregar(RecursosHumanos owner) : this()
        {
            ownerRecursos = owner;
        }

        private void Agregar_Load(object sender, EventArgs e)
        {
            CargarProvincias();
            CargarRedes();

          
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

        private void cmbProvincia_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            CargarLocalidades();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            // Basic validation
            ResetFieldColors();

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MarkInvalid(textBox1);
                MessageBox.Show("El campo DNI está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (!IsDigits(textBox1.Text.Trim()))
            {
                MarkInvalid(textBox1);
                MessageBox.Show("El DNI solo debe contener números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MarkInvalid(txtNombre);
                MessageBox.Show("El campo Nombre está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (!IsLetters(txtNombre.Text.Trim()))
            {
                MarkInvalid(txtNombre);
                MessageBox.Show("El Nombre sólo puede contener letras.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MarkInvalid(txtApellido);
                MessageBox.Show("El campo Apellido está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return;
            }

            if (!IsLetters(txtApellido.Text.Trim()))
            {
                MarkInvalid(txtApellido);
                MessageBox.Show("El Apellido sólo puede contener letras.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                MarkInvalid(txtMail);
                MessageBox.Show("El campo Mail está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MarkInvalid(cmbLocalidad);
                MessageBox.Show("Seleccioná una Localidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbLocalidad.Focus();
                return;
            }

            // Only allow adding users if their Estado is Activado
            if (!checkBoxActivo.Checked)
            {
                MessageBox.Show("Solo se pueden agregar usuarios que estén Activados.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If telefono provided, ensure numeric
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text) && !IsDigits(txtTelefono.Text.Trim()))
            {
                MarkInvalid(txtTelefono);
                MessageBox.Show("El Teléfono solo debe contener números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return;
            }

            // Validate Redes, Direccion, Geografia (required)
            if (string.IsNullOrWhiteSpace(txtRedes.Text))
            {
                MarkInvalid(txtRedes);
                MessageBox.Show("El campo Redes está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRedes.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MarkInvalid(txtDireccion);
                MessageBox.Show("El campo Dirección está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDireccion.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGeografia.Text))
            {
                MarkInvalid(txtGeografia);
                MessageBox.Show("El campo Geografía está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGeografia.Focus();
                return;
            }

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();
                OleDbConnection con = bd.ObtenerConexion();

                string sqlUsuario =
                    @"INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, Dni, Estado)
              VALUES (?, ?, ?, ?, ?, ?)";

                OleDbCommand cmdUsuario = new OleDbCommand(sqlUsuario, con);
                cmdUsuario.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Mail", txtMail.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Contrasena", "");
                cmdUsuario.Parameters.AddWithValue("@Dni", textBox1.Text.Trim());
                cmdUsuario.Parameters.AddWithValue("@Estado", checkBoxActivo.Checked ? 1 : 0);
                cmdUsuario.ExecuteNonQuery();
                

                OleDbCommand cmdId = new OleDbCommand("SELECT @@IDENTITY", con);
                int nuevoId = Convert.ToInt32(cmdId.ExecuteScalar());

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

                string sqlContacto =
                    @"INSERT INTO Contacto_Usuario (Id_Usuario, Telefono, RedesSociales)
              VALUES (?, ?, ?)";

                string redesValor = cmbRedes.SelectedIndex >= 0
                    ? cmbRedes.Text + ": " + txtRedes.Text.Trim()
                    : txtRedes.Text.Trim();

                OleDbCommand cmdContacto = new OleDbCommand(sqlContacto, con);
                cmdContacto.Parameters.AddWithValue("@Id_Usuario", nuevoId);
                cmdContacto.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                cmdContacto.Parameters.AddWithValue("@RedesSociales", redesValor);
                cmdContacto.ExecuteNonQuery();

                bd.CerrarConexion();

                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Volver a RecursosHumanos (usa comprobación IsDisposed)
                VolverAOwnerRecursos();
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al guardar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            VolverAOwnerRecursos();
        }

        // Comprueba si la referencia ownerRecursos sigue viva antes de mostrarla.
        private void VolverAOwnerRecursos()
        {
            if (ownerRecursos != null && !ownerRecursos.IsDisposed)
            {
                this.Hide();
                ownerRecursos.Show();
                this.Close();
                return;
            }

            // Si ownerRecursos fue dispuesto o es null, abrir una nueva instancia
            RecursosHumanos nueva = new RecursosHumanos();
            this.Hide();
            nueva.Show();
            this.Close();
        }

        private void MarkInvalid(Control c)
        {
            try { c.BackColor = Color.LightPink; } catch { }
        }

        private void ResetFieldColors()
        {
            try { textBox1.BackColor = SystemColors.Window; } catch { }
            try { txtNombre.BackColor = SystemColors.Window; } catch { }
            try { txtApellido.BackColor = SystemColors.Window; } catch { }
            try { txtMail.BackColor = SystemColors.Window; } catch { }
            try { txtTelefono.BackColor = SystemColors.Window; } catch { }
            try { cmbLocalidad.BackColor = SystemColors.Window; } catch { }
        }

        private bool IsDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char ch in s)
            {
                if (!char.IsDigit(ch)) return false;
            }
            return true;
        }

        private bool IsLetters(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char ch in s)
            {
                if (!char.IsLetter(ch) && !char.IsWhiteSpace(ch)) return false;
            }
            return true;
        }

        private void TxtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtLettersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
       
    }
}
