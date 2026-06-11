using System;
using System.ComponentModel;
using System.Data.OleDb;
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
                MessageBox.Show("Ingrese un DNI.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                string sql = @"SELECT U.Nombre, U.Apellido, U.Mail, U.Estado, C.Telefono, C.Redes_Sociales, D.Direccion, D.GPS, D.Provincia, D.Localidad
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
                    txtRedes.Text = dr["Redes_Sociales"] != DBNull.Value ? dr["Redes_Sociales"].ToString() : "";

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

                    // Delete contact
                    string sqlDelContacto = "DELETE FROM Contacto_Usuario WHERE Id_Usuario = ?";
                    OleDbCommand cmdDelContacto = new OleDbCommand(sqlDelContacto, bd.ObtenerConexion());
                    cmdDelContacto.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                    cmdDelContacto.ExecuteNonQuery();

                    // Delete domicilio
                    string sqlDelDomic = "DELETE FROM Domicilio_Usuario WHERE Id_Usuario = ?";
                    OleDbCommand cmdDelDomic = new OleDbCommand(sqlDelDomic, bd.ObtenerConexion());
                    cmdDelDomic.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                    cmdDelDomic.ExecuteNonQuery();

                    // Delete usuario
                    string sqlDelUser = "DELETE FROM Usuario WHERE IdUsuario = ?";
                    OleDbCommand cmdDelUser = new OleDbCommand(sqlDelUser, bd.ObtenerConexion());
                    cmdDelUser.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    int rows = cmdDelUser.ExecuteNonQuery();

                    bd.CerrarConexion();

                    if (rows > 0)
                    {
                        MessageBox.Show("Usuario y datos asociados eliminados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                        MessageBox.Show("No se pudo eliminar el usuario.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
