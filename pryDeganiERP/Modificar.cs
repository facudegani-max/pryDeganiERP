using System;
using System;
using System.Data.OleDb;
using System.Drawing;
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

            // Trigger search on Enter key in DNI field
            this.txtDni.KeyDown += TxtDni_KeyDown;

            // Input restrictions
            this.txtDni.KeyPress += TxtNumeric_KeyPress;
            this.textBox1.KeyPress += TxtNumeric_KeyPress;
            this.txtTelefono.KeyPress += TxtNumeric_KeyPress;

            this.txtNombre.KeyPress += TxtLettersOnly_KeyPress;
            this.txtApellido.KeyPress += TxtLettersOnly_KeyPress;



            // Disable editing until a user is found
            SetControlsEnabled(false);

            cmbProvincia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocalidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRedes.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void TxtDni_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscar.PerformClick();
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            // Personal
            textBox1.Enabled = enabled; // personal Dni editable after search
            txtNombre.Enabled = enabled;
            txtApellido.Enabled = enabled;

            // Contacto
            txtMail.Enabled = enabled;
            txtTelefono.Enabled = enabled;
            cmbRedes.Enabled = enabled;
            txtRedes.Enabled = enabled;

            // Domicilio
            txtDireccion.Enabled = enabled;
            txtGeografia.Enabled = enabled;
            cmbProvincia.Enabled = enabled;
            cmbLocalidad.Enabled = enabled;

            checkBoxActivo.Enabled = enabled;

            btnModificar.Enabled = enabled;
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
            ResetFieldColors();

            string dni = txtDni.Text.Trim();
            if (string.IsNullOrWhiteSpace(dni))
            {
                MarkInvalid(txtDni);
                MessageBox.Show("El campo DNI está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                string sql = @"SELECT U.IdUsuario, U.Nombre, U.Apellido, U.Mail, U.Dni, U.Estado, D.Direccion, D.GPS, D.Provincia, D.Localidad, C.Telefono, C.RedesSociales
                               FROM ((Usuario U
                               LEFT JOIN Domicilio_Usuario D ON U.IdUsuario = D.Id_Usuario)
                               LEFT JOIN Contacto_Usuario C ON U.IdUsuario = C.Id_Usuario)
                               WHERE U.Dni = ?";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Dni", dni);

                OleDbDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    // Show DNI in personal data so it can be edited if needed
                    textBox1.Text = dr["Dni"].ToString();

                    // Enable editing after a successful search
                    SetControlsEnabled(true);

                    txtNombre.Text = dr["Nombre"].ToString();
                    txtApellido.Text = dr["Apellido"].ToString();
                    txtMail.Text = dr["Mail"].ToString();
                    txtGeografia.Text = dr["GPS"].ToString();
                    txtDireccion.Text = dr["Direccion"].ToString();
                    txtTelefono.Text = dr["Telefono"].ToString();

                    // RedesSociales stored as "NombreRed: valor" possibly
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
            ResetFieldColors();

            string dni = txtDni.Text.Trim();
            if (string.IsNullOrWhiteSpace(dni))
            {
                MarkInvalid(txtDni);
                MessageBox.Show("El campo DNI está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Validate editable fields
            string newDni = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(newDni))
            {
                MarkInvalid(textBox1);
                MessageBox.Show("El campo DNI personal está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }
            if (!IsDigits(newDni))
            {
                MarkInvalid(textBox1);
                MessageBox.Show("El DNI personal solo debe contener números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (cmbLocalidad.SelectedIndex < 0)
            {
                MarkInvalid(cmbLocalidad);
                MessageBox.Show("Seleccioná una Localidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbLocalidad.Focus();
                return;
            }

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
                var con = bd.ObtenerConexion();

                // Update Usuario (allow changing the DNI: set new Dni and use original dni in WHERE)
                newDni = textBox1.Text.Trim();
                string sqlUser = "UPDATE Usuario SET Nombre = ?, Apellido = ?, Mail = ?, Estado = ?, Dni = ? WHERE Dni = ?";
                OleDbCommand cmdUser = new OleDbCommand(sqlUser, con);
                cmdUser.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                cmdUser.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                cmdUser.Parameters.AddWithValue("@Mail", txtMail.Text.Trim());
                cmdUser.Parameters.AddWithValue("@Estado", checkBoxActivo.Checked ? 1 : 0);
                cmdUser.Parameters.AddWithValue("@NewDni", newDni);
                cmdUser.Parameters.AddWithValue("@OriginalDni", dni);
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

                    // Update Contacto_Usuario: Telefono and RedesSociales stored as "Red: valor"
                    string redesValor = cmbRedes.SelectedIndex >= 0
                        ? cmbRedes.Text + ": " + txtRedes.Text.Trim()
                        : txtRedes.Text.Trim();

                    string sqlContacto = "UPDATE Contacto_Usuario SET Telefono = ?, RedesSociales = ? WHERE Id_Usuario = ?";
                    OleDbCommand cmdContacto = new OleDbCommand(sqlContacto, con);
                    cmdContacto.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                    cmdContacto.Parameters.AddWithValue("@RedesSociales", redesValor);
                    cmdContacto.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                    int rowsContacto = cmdContacto.ExecuteNonQuery();
                }

                bd.CerrarConexion();

                MessageBox.Show("Datos modificados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mantenerse en el formulario: deshabilitar edición hasta nueva búsqueda
                SetControlsEnabled(false);
                btnModificar.Enabled = false;
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al modificar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Contacto / Domicilio actions ---
        private int GetUsuarioIdByDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return -1;
            Conexion bd = new Conexion();
            try
            {
                bd.AbrirConexion();
                OleDbCommand cmd = new OleDbCommand("SELECT IdUsuario FROM Usuario WHERE Dni = ?", bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Dni", dni);
                object obj = cmd.ExecuteScalar();
                bd.CerrarConexion();
                if (obj != null) return Convert.ToInt32(obj);
            }
            catch { try { bd.CerrarConexion(); } catch { } }
            return -1;
        }

        private void BtnAgregarContacto_Click(object sender, EventArgs e)
        {
            // Validate
            ResetFieldColors();
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MarkInvalid(txtTelefono);
                MessageBox.Show("El campo Teléfono está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return;
            }
            if (!IsDigits(txtTelefono.Text.Trim()))
            {
                MarkInvalid(txtTelefono);
                MessageBox.Show("El Teléfono solo debe contener números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRedes.Text))
            {
                MarkInvalid(txtRedes);
                MessageBox.Show("El campo Redes está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRedes.Focus();
                return;
            }

            // Find user id
            string dni = textBox1.Text.Trim();
            int id = GetUsuarioIdByDni(dni);
            if (id < 0)
            {
                MessageBox.Show("No se pudo encontrar el usuario para agregar el contacto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Conexion bd = new Conexion();
            try
            {
                bd.AbrirConexion();
                string redesValor = cmbRedes.SelectedIndex >= 0
                    ? cmbRedes.Text + ": " + txtRedes.Text.Trim()
                    : txtRedes.Text.Trim();

                OleDbCommand cmd = new OleDbCommand("INSERT INTO Contacto_Usuario (Id_Usuario, Telefono, RedesSociales) VALUES (?, ?, ?)", bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Id_Usuario", id);
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                cmd.Parameters.AddWithValue("@RedesSociales", redesValor);
                cmd.ExecuteNonQuery();
                bd.CerrarConexion();
                MessageBox.Show("Contacto agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { bd.CerrarConexion(); } catch { }
                MessageBox.Show("Error al agregar contacto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarContacto_Click(object sender, EventArgs e)
        {
            string dni = textBox1.Text.Trim();
            int id = GetUsuarioIdByDni(dni);
            if (id < 0)
            {
                MessageBox.Show("No se pudo encontrar el usuario para eliminar el contacto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show("Se va a eliminar el último contacto guardado para este usuario. ¿Desea continuar?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            Conexion bd = new Conexion();
            try
            {
                bd.AbrirConexion();
                OleDbCommand cmdGet = new OleDbCommand("SELECT MAX(Id_Contacto) FROM Contacto_Usuario WHERE Id_Usuario = ?", bd.ObtenerConexion());
                cmdGet.Parameters.AddWithValue("@Id_Usuario", id);
                object obj = cmdGet.ExecuteScalar();
                if (obj == null || obj == DBNull.Value)
                {
                    bd.CerrarConexion();
                    MessageBox.Show("No se encontraron contactos para eliminar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int lastId = Convert.ToInt32(obj);
                OleDbCommand cmdDel = new OleDbCommand("DELETE FROM Contacto_Usuario WHERE Id_Contacto = ?", bd.ObtenerConexion());
                cmdDel.Parameters.AddWithValue("@Id_Contacto", lastId);
                int rows = cmdDel.ExecuteNonQuery();
                bd.CerrarConexion();
                if (rows > 0) MessageBox.Show("Último contacto eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("No se pudo eliminar el contacto.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { bd.CerrarConexion(); } catch { }
                MessageBox.Show("Error al eliminar contacto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarDomicilio_Click(object sender, EventArgs e)
        {
            ResetFieldColors();
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
            if (cmbProvincia.SelectedIndex < 0)
            {
                MarkInvalid(cmbProvincia);
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

            string dni = textBox1.Text.Trim();
            int id = GetUsuarioIdByDni(dni);
            if (id < 0)
            {
                MessageBox.Show("No se pudo encontrar el usuario para agregar domicilio.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Conexion bd = new Conexion();
            try
            {
                bd.AbrirConexion();
                OleDbCommand cmd = new OleDbCommand("INSERT INTO Domicilio_Usuario (Id_Usuario, GPS, Provincia, Localidad, Direccion) VALUES (?, ?, ?, ?, ?)", bd.ObtenerConexion());
                cmd.Parameters.AddWithValue("@Id_Usuario", id);
                cmd.Parameters.AddWithValue("@GPS", txtGeografia.Text.Trim());
                cmd.Parameters.AddWithValue("@Provincia", cmbProvincia.Text);
                cmd.Parameters.AddWithValue("@Localidad", cmbLocalidad.Text);
                cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                cmd.ExecuteNonQuery();
                bd.CerrarConexion();
                MessageBox.Show("Domicilio agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { bd.CerrarConexion(); } catch { }
                MessageBox.Show("Error al agregar domicilio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarDomicilio_Click(object sender, EventArgs e)
        {
            string dni = textBox1.Text.Trim();
            int id = GetUsuarioIdByDni(dni);
            if (id < 0)
            {
                MessageBox.Show("No se pudo encontrar el usuario para eliminar el domicilio.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show("Se va a eliminar el último domicilio guardado para este usuario. ¿Desea continuar?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            Conexion bd = new Conexion();
            try
            {
                bd.AbrirConexion();
                OleDbCommand cmdGet = new OleDbCommand("SELECT MAX(Id_Domicilio) FROM Domicilio_Usuario WHERE Id_Usuario = ?", bd.ObtenerConexion());
                cmdGet.Parameters.AddWithValue("@Id_Usuario", id);
                object obj = cmdGet.ExecuteScalar();
                if (obj == null || obj == DBNull.Value)
                {
                    bd.CerrarConexion();
                    MessageBox.Show("No se encontraron domicilios para eliminar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int lastId = Convert.ToInt32(obj);
                OleDbCommand cmdDel = new OleDbCommand("DELETE FROM Domicilio_Usuario WHERE Id_Domicilio = ?", bd.ObtenerConexion());
                cmdDel.Parameters.AddWithValue("@Id_Domicilio", lastId);
                int rows = cmdDel.ExecuteNonQuery();
                bd.CerrarConexion();
                if (rows > 0) MessageBox.Show("Último domicilio eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("No se pudo eliminar el domicilio.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { bd.CerrarConexion(); } catch { }
                MessageBox.Show("Error al eliminar domicilio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void Modificar_Load_1(object sender, EventArgs e)
        {

        }
    }
}
