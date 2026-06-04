using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class Eliminar : Form
    {
        public RecursosHumanos ownerRecursosHumanos;

        public Eliminar()
        {
            InitializeComponent();

            // Wire buttons
            this.btnEliminar.Click += btnEliminar_Click;
            this.btnSalir.Click += btnSalir_Click;
        }

        // Constructor that receives the RecursosHumanos owner
        public Eliminar(RecursosHumanos owner) : this()
        {
            ownerRecursosHumanos = owner;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni))
            {
                MessageBox.Show("Ingrese un DNI.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            var confirm = MessageBox.Show($"¿Está seguro que desea desactivar al usuario con DNI {dni}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            Conexion bd = new Conexion();

            try
            {
                bd.AbrirConexion();

                string sql = "UPDATE Usuario SET Estado = ? WHERE Dni = ?";

                OleDbCommand cmd = new OleDbCommand(sql, bd.ObtenerConexion());

                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Dni", dni);

                int rows = cmd.ExecuteNonQuery();

                bd.CerrarConexion();

                if (rows > 0)
                {
                    MessageBox.Show("Usuario desactivado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Volver a RecursosHumanos si está disponible
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
                else
                {
                    MessageBox.Show("No se encontró un usuario con ese DNI.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                bd.CerrarConexion();
                MessageBox.Show("Error al desactivar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
