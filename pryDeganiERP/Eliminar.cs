using System;
using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Eliminar : Form
    {
        public RecursosHumanos ownerRecursosHumanos;

        public Eliminar()
        {
            InitializeComponent();

            // Wire buttons
            this.btnEliminar.Click += btnEliminar_Click;
            this.btnSalir.Click += btnSalir_Click;
            this.btnBuscar.Click += btnBuscar_Click;

            // Make fields read-only by default
            SetFieldsReadOnly(true);

            // Disable eliminar until a user is found
            btnEliminar.Enabled = false;

            // Trigger search on Enter
            this.txtDni.KeyDown += TxtDni_KeyDown;

            // Input restrictions
            this.txtDni.KeyPress += TxtNumeric_KeyPress;
            this.textBox1.KeyPress += TxtNumeric_KeyPress;
            this.txtTelefono.KeyPress += TxtNumeric_KeyPress;

            this.txtNombre.KeyPress += TxtLettersOnly_KeyPress;
            this.txtApellido.KeyPress += TxtLettersOnly_KeyPress;

            // Ensure personal DNI field is read-only initially
            textBox1.ReadOnly = true;
        }

        // Constructor that receives the RecursosHumanos owner
        public Eliminar(RecursosHumanos owner) : this()
        {
            ownerRecursosHumanos = owner;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni))
            {
                MarkInvalid(txtDni);
                MessageBox.Show("Ingrese un DNI.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            if (!IsDigits(dni))
            {
                MarkInvalid(txtDni);
                MessageBox.Show("El DNI solo debe contener números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                string sql = @"SELECT U.IdUsuario, U.Nombre, U.Apellido, U.Mail, U.Estado, C.Telefono, C.RedesSociales, D.Direccion, D.GPS, D.Provincia, D.Localidad
                               FROM ((Usuario U
                               LEFT JOIN Contacto_Usuario C ON U.IdUsuario = C.Id_Usuario)
                               LEFT JOIN Domicilio_Usuario D ON U.IdUsuario = D.Id_Usuario)
                               WHERE U.Dni = ?";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Dni", dni);

                OleDbDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtNombre.Text = dr["Nombre"].ToString();
                    txtApellido.Text = dr["Apellido"].ToString();
                    txtMail.Text = dr["Mail"].ToString();

                    txtTelefono.Text = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : "";
                    // Read RedesSociales stored in Contacto_Usuario as "Red: valor"
                    string redes = dr["RedesSociales"] != DBNull.Value ? dr["RedesSociales"].ToString() : "";
                    if (!string.IsNullOrEmpty(redes) && redes.Contains(":"))
                    {
                        var parts = redes.Split(new char[] { ':' }, 2);
                        string redNombre = parts[0].Trim();
                        string redValor = parts.Length > 1 ? parts[1].Trim() : "";
                        cmbRedes.SelectedItem = redNombre;
                        txtRedes.Text = redValor;
                    }
                    else
                    {
                        txtRedes.Text = redes;
                        cmbRedes.SelectedIndex = -1;
                    }

                    txtDireccion.Text = dr["Direccion"] != DBNull.Value ? dr["Direccion"].ToString() : "";
                    txtGeografia.Text = dr["GPS"] != DBNull.Value ? dr["GPS"].ToString() : "";

                    cmbProvincia.Text = dr["Provincia"] != DBNull.Value ? dr["Provincia"].ToString() : "";
                    cmbLocalidad.Text = dr["Localidad"] != DBNull.Value ? dr["Localidad"].ToString() : "";

                    checkBoxActivo.Checked = dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"]);

                    // Show the DNI used in the personal data field so it can be edited if required
                    textBox1.Text = dni;

                    // Keep most fields read-only and show the personal DNI (non-editable)
                    SetFieldsReadOnly(true);
                    textBox1.ReadOnly = true;

                    // Enable eliminar now that a user was found
                    btnEliminar.Enabled = true;

                    // Validate displayed contact/address fields (informational)
                    ValidateDisplayFields();
                }
                else
                {
                    MessageBox.Show("No se encontró un usuario con ese DNI.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                }

                dr.Close();
                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al buscar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetFieldsReadOnly(bool readOnly)
        {
            txtNombre.ReadOnly = readOnly;
            txtApellido.ReadOnly = readOnly;
            txtMail.ReadOnly = readOnly;
            txtTelefono.ReadOnly = readOnly;
            txtRedes.ReadOnly = readOnly;
            txtDireccion.ReadOnly = readOnly;
            txtGeografia.ReadOnly = readOnly;
            cmbProvincia.Enabled = !readOnly;
            cmbLocalidad.Enabled = !readOnly;
            cmbRedes.Enabled = !readOnly;
            checkBoxActivo.Enabled = !readOnly;
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtMail.Text = "";
            txtTelefono.Text = "";
            txtRedes.Text = "";
            txtDireccion.Text = "";
            txtGeografia.Text = "";
            cmbProvincia.Text = "";
            cmbLocalidad.Text = "";
            cmbRedes.Text = "";
            checkBoxActivo.Checked = false;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni))
            {
                MessageBox.Show("Ingrese un DNI.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            var confirm = MessageBox.Show($"¿Está seguro que desea desactivar al usuario con DNI {dni}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            // If user is already deactivated, show message and do nothing
            if (!checkBoxActivo.Checked)
            {
                MessageBox.Show("El usuario ya se encuentra desactivado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                // Find IdUsuario
                string sqlId = "SELECT IdUsuario FROM Usuario WHERE Dni = ?";
                OleDbCommand cmdId = new OleDbCommand(sqlId, bd.ObtenerConexion());
                cmdId.Parameters.AddWithValue("@Dni", dni);
                object idObj = cmdId.ExecuteScalar();

                if (idObj != null)
                {
                    int idUsuario = Convert.ToInt32(idObj);

                    // Instead of deleting the user and related data, mark the user as deactivated
                    string sqlUpdUser = "UPDATE Usuario SET Estado = 0 WHERE IdUsuario = ?";
                    OleDbCommand cmdUpdUser = new OleDbCommand(sqlUpdUser, bd.ObtenerConexion());
                    cmdUpdUser.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    int rows = cmdUpdUser.ExecuteNonQuery();

                    bd.CerrarConexion();

                    if (rows > 0)
                    {
                        MessageBox.Show("Usuario desactivado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear displayed fields and keep the form open
                        LimpiarCampos();
                        textBox1.Text = "";
                        txtDni.Text = "";
                        SetFieldsReadOnly(true);
                        textBox1.ReadOnly = true;
                        btnEliminar.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("No se pudo desactivar el usuario.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    bd.CerrarConexion();
                    MessageBox.Show("No se encontró un usuario con ese DNI.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al desactivar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtDni_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscar.PerformClick();
            }
        }

        // Helper: mark control invalid
        private void MarkInvalid(Control c)
        {
            try { c.BackColor = Color.LightPink; } catch { }
        }

        private void ResetFieldColors()
        {
            try { txtDni.BackColor = SystemColors.Window; } catch { }
            try { textBox1.BackColor = SystemColors.Window; } catch { }
            try { txtNombre.BackColor = SystemColors.Window; } catch { }
            try { txtApellido.BackColor = SystemColors.Window; } catch { }
            try { txtTelefono.BackColor = SystemColors.Window; } catch { }
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

        private void TxtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (backspace), digits only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtLettersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow letters, control and whitespace
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ValidateDisplayFields()
        {
            // Informational validation: mark and notify if contact/address fields are empty or invalid
            // Telefono: should be digits
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text) && !IsDigits(txtTelefono.Text.Trim()))
            {
                MarkInvalid(txtTelefono);
                MessageBox.Show("El Teléfono mostrado contiene caracteres inválidos (solo números).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (string.IsNullOrWhiteSpace(txtRedes.Text))
            {
                MarkInvalid(txtRedes);
                MessageBox.Show("El campo Redes está vacío.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MarkInvalid(txtDireccion);
                MessageBox.Show("El campo Dirección está vacío.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (string.IsNullOrWhiteSpace(txtGeografia.Text))
            {
                MarkInvalid(txtGeografia);
                MessageBox.Show("El campo Geografía está vacío.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (ownerRecursosHumanos != null)
            {
                this.Hide();
                ownerRecursosHumanos.Show();
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
    }
}
