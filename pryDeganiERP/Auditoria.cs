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
