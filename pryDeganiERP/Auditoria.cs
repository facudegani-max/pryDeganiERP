using System;
using System.Data;
using System.Data.OleDb;
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
            this.chkbuttonAscendente.CheckedChanged += SortingOptionChanged;
            this.chkbuttonDescendente.CheckedChanged += SortingOptionChanged;

            // Make controls non-editable by user
            cmblistaAuditoria.DropDownStyle = ComboBoxStyle.DropDownList;
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
            CargarAuditoria();
        }

        private void CargarListaTablas()
        {
            try
            {
                bd.AbrirConexion();

                // Get table names from schema
                DataTable schema = bd.ObtenerConexion().GetSchema("Tables");

                cmblistaAuditoria.Items.Clear();

                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row[2].ToString();
                    // Filter system tables if needed
                    if (!tableName.StartsWith("MSys"))
                        cmblistaAuditoria.Items.Add(tableName);
                }

                bd.CerrarConexion();
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

                string consulta = $"SELECT * FROM [{tabla}]" + orden;

                OleDbDataAdapter da = new OleDbDataAdapter(
                    consulta,
                    bd.ObtenerConexion());

                DataTable dt = new DataTable();

                da.Fill(dt);

                dgvAuditoria.DataSource = dt;

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
