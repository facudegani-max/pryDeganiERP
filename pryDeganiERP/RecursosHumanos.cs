using System;
using System.Windows.Forms;

namespace pryDeganiERP
{
    public partial class RecursosHumanos : Form
    {
        public Administrador ownerAdminRef; // reference to Administrador if opened from there
        public string usuarioActual;
        public string rolUsuario;

        public RecursosHumanos()
        {
            InitializeComponent();

            // Wire button clicks
            this.btnSalir.Click += btnSalir_Click;
            this.btnEliminar.Click += btnEliminar_Click; // open Eliminar form
            this.btnModificar.Click += btnModificar_Click; // open Modificar form
            this.btnGuardar.Click += btnGuardar_Click; // open Agregar form
        }

        // Constructor que recibe referencia al Administrador
        public RecursosHumanos(Administrador admin) : this()
        {
            ownerAdminRef = admin;
            if (admin != null)
            {
                usuarioActual = admin.usuarioActual;
                rolUsuario = admin.rolUsuario;
            }
        }

        private void RecursosHumanos_Load_1(object sender, EventArgs e)
        {
            // Designer wiring method - nothing needed here because this form only shows buttons
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Eliminar ventana = new Eliminar(this);
            // Marcar el formulario creado como propiedad de este formulario
            ventana.Owner = this;
            this.Hide();
            ventana.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Modificar ventana = new Modificar(this);
            // Marcar el formulario creado como propiedad de este formulario
            ventana.Owner = this;
            this.Hide();
            ventana.Show();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Agregar ventana = new Agregar(this);
            // Marcar el formulario creado como propiedad de este formulario
            ventana.Owner = this;
            this.Hide();
            ventana.Show();
        }

        // Botón salir: vuelve al Administrador si se abrió desde él y el rol es Administrador,
        // sino vuelve a la pantalla de Usuario (login).
        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (ownerAdminRef != null && ownerAdminRef.rolUsuario == "Administrador")
            {
                // Mostrar el administrador y ocultar este formulario.
                // No cerrar el administrador (ownerAdminRef) aquí.
                ownerAdminRef.Show();
                this.Hide();
                // Cerrar este formulario después de ocultarlo para liberar recursos
                this.Close();
            }
            else
            {
                Usuario login = new Usuario();
                login.Show();
                this.Hide();
                this.Close();
            }
        }
    }
}