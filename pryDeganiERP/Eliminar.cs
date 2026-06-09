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

                    // Make fields read-only so user cannot edit
                    SetFieldsReadOnly(true);
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

                string sql = "UPDATE Usuario SET Estado = ? WHERE Dni = ?";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());

                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Dni", dni);

                int rows = cmd.ExecuteNonQuery();

                bd.CerrarConexion();

                if (rows > 0)
                {
                    MessageBox.Show("Usuario desactivado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Volver a RecursosHumanos si está disponible
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
                else
                {
                    MessageBox.Show("No se encontró un usuario con ese DNI.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al desactivar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
