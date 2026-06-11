using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Administrador : Form
    {
        Conexion cn = new Conexion();

        public string usuarioActual;
        public string rolUsuario;
        public string fechaIngreso;
        public Administrador()
        {
            InitializeComponent();
            // Wire button clicks to open other forms / return to login
            this.btnAuditoria.Click += btnAuditoria_Click;
            this.btnRecursosHumanos.Click += btnRecursosHumanos_Click;
            this.btnSalir.Click += btnSalir_Click;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
            lblRol.Text = "Rol: " + rolUsuario;

            lblFecha.Text = "Ingreso: " + fechaIngreso;

            try
            {
                cn.AbrirConexion();

                toolStripStatusLabel1.Text = "Conectado a la base de datos";

                toolStripStatusLabel1.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Error de conexión";

                toolStripStatusLabel1.ForeColor = Color.Red;

                MessageBox.Show(ex.Message);
            }
        }

        private void lblRol_Click(object sender, EventArgs e)
        {

        }

        private void lblFecha_Click(object sender, EventArgs e)
        {

        }

        private void btnAuditoria_Click(object sender, EventArgs e)
        {
            Auditoria ventana = new Auditoria(this);
            this.Hide();
            ventana.Show();
        }

        private void btnRecursosHumanos_Click(object sender, EventArgs e)
        {
            RecursosHumanos ventana = new RecursosHumanos(this);
            this.Hide();
            ventana.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Usuario login = new Usuario();
            this.Hide();
            login.Show();

            this.Close();
            
        }

        private void btnSalir_Click_1(object sender, EventArgs e)
        {

        }
    }

}
