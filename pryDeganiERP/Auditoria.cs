using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Auditoria : Form
    {
        Conexion bd = new Conexion();

        public Administrador ownerAdminRef;
        public string usuarioActual;
        public string rolUsuario;

        public Auditoria()
        {
            InitializeComponent();
            this.Load += Auditoria_Load;

            this.btnSalir.Click += btnSalir_Click;

            this.cmblistaAuditoria.SelectedIndexChanged += CmblistaAuditoria_SelectedIndexChanged;
            this.cmbDesde.SelectedIndexChanged += (s, e) => CargarAuditoria();
            this.cmbHasta.SelectedIndexChanged += (s, e) => CargarAuditoria();
            this.cmbEstado.SelectedIndexChanged += (s, e) => CargarAuditoria();
            this.chkbuttonAscendente.CheckedChanged += SortingOptionChanged;
            this.chkbuttonDescendente.CheckedChanged += SortingOptionChanged;

            // Make controls non-editable by user
            cmblistaAuditoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDesde.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHasta.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            dgvAuditoria.ReadOnly = true;
            dgvAuditoria.AllowUserToAddRows = false;
            dgvAuditoria.AllowUserToDeleteRows = false;
            dgvAuditoria.AllowUserToOrderColumns = false;
            dgvAuditoria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        // Constructor que recibe el Administrador que la abrió
        public Auditoria(Administrador admin) : this()
        {
            ownerAdminRef = admin;
            if (admin != null)
            {
                usuarioActual = admin.usuarioActual;
                rolUsuario = admin.rolUsuario;
            }
        }

        private void Auditoria_Load(object sender, EventArgs e)
        {
            CargarListaTablas();
            CargarFiltros();
            CargarAuditoria();
        }

        private void CargarFiltros()
        {
            try
            {
                bd.AbrirConexion();

                // Cargar fechas distintas (solo la parte fecha)
                OleDbCommand cmdFechas = new OleDbCommand(
                    "SELECT DISTINCT DateValue([Fecha_Hora]) AS Fecha FROM Auditoria_Usuario ORDER BY DateValue([Fecha_Hora])",
                    bd.ObtenerConexion());

                OleDbDataReader lector = cmdFechas.ExecuteReader();

                cmbDesde.Items.Clear();
                cmbHasta.Items.Clear();

                var listaFechas = new System.Collections.Generic.List<DateTime>();

                while (lector.Read())
                {
                    if (lector["Fecha"] != DBNull.Value)
                    {
                        DateTime d = Convert.ToDateTime(lector["Fecha"]);
                        listaFechas.Add(d);
                    }
                }

                foreach (var f in listaFechas)
                {
                    string texto = f.ToString("dd/MM/yyyy");
                    cmbDesde.Items.Add(texto);
                    cmbHasta.Items.Add(texto);
                }

                if (cmbDesde.Items.Count > 0)
                    cmbDesde.SelectedIndex = 0;
                if (cmbHasta.Items.Count > 0)
                    cmbHasta.SelectedIndex = cmbHasta.Items.Count - 1;

                lector.Close();

                // Cargar estados fijos (Ingreso / Salida)
                cmbEstado.Items.Clear();
                cmbEstado.Items.Add("Todos");
                cmbEstado.Items.Add("Ingreso");
                cmbEstado.Items.Add("Salida");
                cmbEstado.SelectedIndex = 0;

                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                try { bd.CerrarConexion(); } catch { }
                // No interrumpir flujo
            }
        }

        private void CargarListaTablas()
        {
            try
            {
                // Expose specific tables the user can choose to view in the auditoría
                cmblistaAuditoria.Items.Clear();
                cmblistaAuditoria.Items.Add("Auditoria_Usuario");
                cmblistaAuditoria.Items.Add("Usuario");
                cmblistaAuditoria.Items.Add("Domicilio_Usuario");
                cmblistaAuditoria.Items.Add("Contacto_Usuario");

                // Select the auditoría table by default
                if (cmblistaAuditoria.Items.Count > 0)
                    cmblistaAuditoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar lista de tablas: " + ex.Message);
            }
        }

        private void CmblistaAuditoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarAuditoria();
        }

        private void SortingOptionChanged(object sender, EventArgs e)
        {
            // Ensure only one radio is checked
            if (sender == chkbuttonAscendente && chkbuttonAscendente.Checked)
                chkbuttonDescendente.Checked = false;
            if (sender == chkbuttonDescendente && chkbuttonDescendente.Checked)
                chkbuttonAscendente.Checked = false;

            CargarAuditoria();
        }

        private void CargarAuditoria()
        {
            try
            {
                bd.AbrirConexion();

                string tabla = cmblistaAuditoria.SelectedIndex >= 0 ? cmblistaAuditoria.Text : "Auditoria_Usuario";

                string orden = "";

                if (chkbuttonAscendente.Checked)
                    orden = " ORDER BY 1 ASC";
                else if (chkbuttonDescendente.Checked)
                    orden = " ORDER BY 1 DESC";

                DataTable dt = new DataTable();

                // Si estamos viendo la tabla Auditoria_Usuario aplicamos filtros por fecha y estado
                if (tabla.Equals("Auditoria_Usuario", StringComparison.OrdinalIgnoreCase))
                {
                    string where = "";

                    var parametros = new System.Collections.Generic.List<OleDbParameter>();

                    if (cmbDesde.SelectedIndex >= 0 && cmbHasta.SelectedIndex >= 0)
                    {
                        // Parseamos las fechas en el formato dd/MM/yyyy que usamos en los combos
                        DateTime desde = DateTime.ParseExact(cmbDesde.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime hasta = DateTime.ParseExact(cmbHasta.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);

                        where = " WHERE [Fecha_Hora] BETWEEN ? AND ?";
                        parametros.Add(new OleDbParameter("@p1", OleDbType.Date) { Value = desde });
                        parametros.Add(new OleDbParameter("@p2", OleDbType.Date) { Value = hasta });
                    }

                    if (cmbEstado.SelectedIndex > 0 && !string.IsNullOrEmpty(cmbEstado.Text) && cmbEstado.Text != "Todos")
                    {
                        where += string.IsNullOrEmpty(where) ? " WHERE " : " AND ";
                        where += "[Estado_Login] = ?";
                        parametros.Add(new OleDbParameter("@p3", OleDbType.VarChar) { Value = cmbEstado.Text });
                    }

                    string consulta = $"SELECT * FROM [{tabla}]" + where + orden;

                    OleDbDataAdapter da = new OleDbDataAdapter(consulta, bd.ObtenerConexion());
                    if (parametros.Count > 0)
                        da.SelectCommand.Parameters.AddRange(parametros.ToArray());

                    da.Fill(dt);
                    dgvAuditoria.DataSource = dt;
                }
                else
                {
                    string consulta = $"SELECT * FROM [{tabla}]" + orden;

                    OleDbDataAdapter da = new OleDbDataAdapter(
                        consulta,
                        bd.ObtenerConexion());

                    da.Fill(dt);
                    dgvAuditoria.DataSource = dt;
                }

                bd.CerrarConexion();
            }
            catch (Exception ex)
            {
                // Suppress noisy messages when a table is not found (e.g. invalid selection)
                var lower = ex.Message?.ToLowerInvariant() ?? string.Empty;
                if (lower.Contains("no value given for one or more required parameters")
                    || lower.Contains("could not find")
                    || lower.Contains("not found")
                    || lower.Contains("no existe")
                    || lower.Contains("does not exist"))
                {
                    // quietly clear grid and return
                    try { dgvAuditoria.DataSource = null; } catch { }
                    bd.CerrarConexion();
                    return;
                }

                MessageBox.Show(
                    "Error al cargar auditoría: " +
                    ex.Message);

                bd.CerrarConexion();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (ownerAdminRef != null && ownerAdminRef.rolUsuario == "Administrador")
            {
                this.Hide();
                ownerAdminRef.Show();
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
    }
}
