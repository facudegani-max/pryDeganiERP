using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Modificar : Form
    {
        public RecursosHumanos ownerRecursosHumanos;

        public Modificar()
        {
            InitializeComponent();

            this.Load += Modificar_Load;
            this.btnBuscar.Click += btnBuscar_Click;
            this.btnModificar.Click += btnModificar_Click;
            this.btnSalir.Click += btnSalir_Click;

            cmbProvincia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocalidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRedes.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public Modificar(RecursosHumanos owner) : this()
        {
            ownerRecursosHumanos = owner;
        }

        private void Modificar_Load(object sender, EventArgs e)
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

                string sql = @"SELECT U.Nombre, U.Apellido, U.Mail, U.Dni, U.Estado, D.Direccion, D.GPS, D.Provincia, D.Localidad, C.Telefono, C.Redes_Sociales
                               FROM ((Usuario U
                               LEFT JOIN Domicilio_Usuario D ON U.IdUsuario = D.Id_Usuario)
                               LEFT JOIN Contacto_Usuario C ON U.IdUsuario = C.Id_Usuario)
                               WHERE U.Dni = ?";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Dni", dni);

                OleDbDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtNombre.Text = dr["Nombre"].ToString();
                    txtApellido.Text = dr["Apellido"].ToString();
                    txtMail.Text = dr["Mail"].ToString();
                    txtGeografia.Text = dr["GPS"].ToString();
                    txtDireccion.Text = dr["Direccion"].ToString();
                    txtTelefono.Text = dr["Telefono"].ToString();

                    // Redes_Sociales stored as "NombreRed: valor" possibly
                    string redes = dr["Redes_Sociales"].ToString();
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

                    // Provincia y Localidad
                    string provincia = dr["Provincia"].ToString();
                    if (!string.IsNullOrEmpty(provincia))
                    {
                        cmbProvincia.SelectedItem = provincia;
                        // cargamos localidades para esa provincia y seleccionamos
                        CargarLocalidades(provincia);
                        cmbLocalidad.SelectedItem = dr["Localidad"].ToString();
                    }
                    else
                    {
                        cmbProvincia.SelectedIndex = -1;
                        cmbLocalidad.Items.Clear();
                    }

                    checkBoxActivo.Checked = Convert.ToInt32(dr["Estado"] ?? 0) == 1;
                }
                else
                {
                    MessageBox.Show("No se encontró un usuario con ese DNI.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dr.Close();
                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al buscar usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarLocalidades(string provincia)
        {
            cmbLocalidad.Items.Clear();

            if (string.IsNullOrEmpty(provincia))
                return;

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                string sql = @"SELECT L.nombre_localidad
                  FROM Localidad L
                  INNER JOIN Provincia P
                  ON L.id_provincia = P.Id_Provincia
                  WHERE P.Nombre_Provincia = ?
                  ORDER BY L.nombre_localidad";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Provincia", provincia);

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

        private void btnModificar_Click(object sender, EventArgs e)
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
                var con = bd.ObtenerConexion();

                // Update Usuario
                string sqlUser = "UPDATE Usuario SET Nombre = ?, Apellido = ?, Mail = ?, Estado = ? WHERE Dni = ?";
                OleDbCommand cmdUser = new OleDbCommand(sqlUser, con);
                cmdUser.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                cmdUser.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                cmdUser.Parameters.AddWithValue("@Mail", txtMail.Text.Trim());
                cmdUser.Parameters.AddWithValue("@Estado", checkBoxActivo.Checked ? 1 : 0);
                cmdUser.Parameters.AddWithValue("@Dni", dni);
                int rowsUser = cmdUser.ExecuteNonQuery();

                // Update Domicilio_Usuario (we need Id_Usuario). We'll try to update based on Usuario.IdUsuario via JOIN-like select
                string sqlId = "SELECT IdUsuario FROM Usuario WHERE Dni = ?";
                OleDbCommand cmdId = new OleDbCommand(sqlId, con);
                cmdId.Parameters.AddWithValue("@Dni", dni);
                object idObj = cmdId.ExecuteScalar();

                if (idObj != null)
                {
                    int idUsuario = Convert.ToInt32(idObj);

                    string sqlDomic = "UPDATE Domicilio_Usuario SET Direccion = ?, GPS = ?, Provincia = ?, Localidad = ? WHERE Id_Usuario = ?";
                    OleDbCommand cmdDomic = new OleDbCommand(sqlDomic, con);
                    cmdDomic.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                    cmdDomic.Parameters.AddWithValue("@GPS", txtGeografia.Text.Trim());
                    cmdDomic.Parameters.AddWithValue("@Provincia", cmbProvincia.SelectedIndex >= 0 ? cmbProvincia.Text : "");
                    cmdDomic.Parameters.AddWithValue("@Localidad", cmbLocalidad.SelectedIndex >= 0 ? cmbLocalidad.Text : "");
                    cmdDomic.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                    int rowsDomic = cmdDomic.ExecuteNonQuery();

                    string redesValor = cmbRedes.SelectedIndex >= 0 ? cmbRedes.Text + ": " + txtRedes.Text.Trim() : txtRedes.Text.Trim();

                    string sqlContacto = "UPDATE Contacto_Usuario SET Telefono = ?, Redes_Sociales = ? WHERE Id_Usuario = ?";
                    OleDbCommand cmdContacto = new OleDbCommand(sqlContacto, con);
                    cmdContacto.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                    cmdContacto.Parameters.AddWithValue("@Redes_Sociales", redesValor);
                    cmdContacto.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                    int rowsContacto = cmdContacto.ExecuteNonQuery();
                }

                bd.CerrarConexion();

                MessageBox.Show("Datos modificados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Volver a RecursosHumanos
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
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al modificar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
