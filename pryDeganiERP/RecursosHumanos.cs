using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class RecursosHumanos : Form
    {
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
        }

        private void RecursosHumanos_Load(object sender, EventArgs e)
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
    }
}