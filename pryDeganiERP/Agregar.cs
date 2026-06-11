using System;
using System.Data.OleDb;
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
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("DNI obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Nombre obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Apellido obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                MessageBox.Show("Mail obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    @"INSERT INTO Contacto_Usuario (Id_Usuario, Telefono, Redes_Sociales)
              VALUES (?, ?, ?)";

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

                // Volver a RecursosHumanos
                if (ownerRecursos != null)
                {
                    this.Hide();
                    ownerRecursos.Show();
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al guardar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (ownerRecursos != null)
            {
                this.Hide();
                ownerRecursos.Show();
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
    }
}
