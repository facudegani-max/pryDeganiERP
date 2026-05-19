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
    public partial class Main : Form
    {
        Conexion cn = new Conexion();
        public Main()
        {
            InitializeComponent();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            Conexion cn = new Conexion();

            try
            {
                cn.AbrirConexion();

                string sql = "SELECT * FROM Usuario";

                OleDbDataAdapter da = new OleDbDataAdapter(sql, cn.ObtenerConexion());

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataGridView1.DataSource = dt;

                cn.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    
}
