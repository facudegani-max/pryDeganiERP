namespace pryDeganiERP
{
    partial class Auditoria
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Auditoria));
            this.dgvAuditoria = new System.Windows.Forms.DataGridView();
            this.btnSalir = new System.Windows.Forms.Button();
            this.chkbuttonAscendente = new System.Windows.Forms.RadioButton();
            this.chkbuttonDescendente = new System.Windows.Forms.RadioButton();
            this.cmblistaAuditoria = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAuditoria
            // 
            this.dgvAuditoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuditoria.Location = new System.Drawing.Point(13, 62);
            this.dgvAuditoria.Margin = new System.Windows.Forms.Padding(4);
            this.dgvAuditoria.Name = "dgvAuditoria";
            this.dgvAuditoria.Size = new System.Drawing.Size(951, 462);
            this.dgvAuditoria.TabIndex = 11;
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.Tomato;
            this.btnSalir.Location = new System.Drawing.Point(521, 532);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(443, 73);
            this.btnSalir.TabIndex = 12;
            this.btnSalir.Text = "Salir\r\n";
            this.btnSalir.UseVisualStyleBackColor = false;
            // 
            // chkbuttonAscendente
            // 
            this.chkbuttonAscendente.AutoSize = true;
            this.chkbuttonAscendente.Location = new System.Drawing.Point(189, 21);
            this.chkbuttonAscendente.Name = "chkbuttonAscendente";
            this.chkbuttonAscendente.Size = new System.Drawing.Size(107, 20);
            this.chkbuttonAscendente.TabIndex = 13;
            this.chkbuttonAscendente.TabStop = true;
            this.chkbuttonAscendente.Text = "Ascendente";
            this.chkbuttonAscendente.UseVisualStyleBackColor = true;
            // 
            // chkbuttonDescendente
            // 
            this.chkbuttonDescendente.AutoSize = true;
            this.chkbuttonDescendente.Location = new System.Drawing.Point(307, 21);
            this.chkbuttonDescendente.Name = "chkbuttonDescendente";
            this.chkbuttonDescendente.Size = new System.Drawing.Size(117, 20);
            this.chkbuttonDescendente.TabIndex = 14;
            this.chkbuttonDescendente.TabStop = true;
            this.chkbuttonDescendente.Text = "Descendente";
            this.chkbuttonDescendente.UseVisualStyleBackColor = true;
            // 
            // cmblistaAuditoria
            // 
            this.cmblistaAuditoria.FormattingEnabled = true;
            this.cmblistaAuditoria.Location = new System.Drawing.Point(18, 20);
            this.cmblistaAuditoria.Name = "cmblistaAuditoria";
            this.cmblistaAuditoria.Size = new System.Drawing.Size(140, 24);
            this.cmblistaAuditoria.TabIndex = 15;
            // 
            // Auditoria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 618);
            this.Controls.Add(this.cmblistaAuditoria);
            this.Controls.Add(this.chkbuttonDescendente);
            this.Controls.Add(this.chkbuttonAscendente);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.dgvAuditoria);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Auditoria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consultar Base Datos";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAuditoria;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.RadioButton chkbuttonAscendente;
        private System.Windows.Forms.RadioButton chkbuttonDescendente;
        private System.Windows.Forms.ComboBox cmblistaAuditoria;
    }
}