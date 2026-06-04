using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Auditoria : Form
    {
        Conexion bd = new Conexion();

        public Auditoria()
        {
            InitializeComponent();
            this.Load += Auditoria_Load;
        }

        private void Auditoria_Load(object sender, EventArgs e)
        {
            CargarAuditoria();
        }

        private void CargarAuditoria()
        {
            try
            {
                bd.AbrirConexion();

                string consulta = "SELECT * FROM Auditoria";

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
                MessageBox.Show(
                    "Error al cargar auditoría: " +
                    ex.Message);

                bd.CerrarConexion();
            }
        }
    }
}
