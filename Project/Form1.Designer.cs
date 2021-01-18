namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.Registro = new System.Windows.Forms.Button();
            this.Usuario = new System.Windows.Forms.TextBox();
            this.Contraseña = new System.Windows.Forms.TextBox();
            this.UsuarioLbl = new System.Windows.Forms.Label();
            this.PswLbl = new System.Windows.Forms.Label();
            this.Desconectar = new System.Windows.Forms.Button();
            this.Login = new System.Windows.Forms.Button();
            this.Rapido = new System.Windows.Forms.RadioButton();
            this.Ganadores = new System.Windows.Forms.RadioButton();
            this.Viciado = new System.Windows.Forms.RadioButton();
            this.Consultar = new System.Windows.Forms.Button();
            this.Invitar = new System.Windows.Forms.Button();
            this.ConectadosGrid = new System.Windows.Forms.DataGridView();
            this.InvitarLbl = new System.Windows.Forms.Label();
            this.Eliminar = new System.Windows.Forms.Button();
            this.MaxLbl = new System.Windows.Forms.Label();
            this.NombreLbl = new System.Windows.Forms.Label();
            this.InfoLbl = new System.Windows.Forms.Label();
            this.PswVisible = new System.Windows.Forms.CheckBox();
            this.RespuestasBox = new System.Windows.Forms.TextBox();
            this.NombreConsulta = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // Registro
            // 
            this.Registro.Location = new System.Drawing.Point(87, 154);
            this.Registro.Name = "Registro";
            this.Registro.Size = new System.Drawing.Size(90, 23);
            this.Registro.TabIndex = 0;
            this.Registro.Text = "Registro";
            this.Registro.UseVisualStyleBackColor = true;
            this.Registro.Click += new System.EventHandler(this.Registro_Click);
            // 
            // Usuario
            // 
            this.Usuario.Location = new System.Drawing.Point(87, 80);
            this.Usuario.Name = "Usuario";
            this.Usuario.Size = new System.Drawing.Size(90, 20);
            this.Usuario.TabIndex = 1;
            // 
            // Contraseña
            // 
            this.Contraseña.Location = new System.Drawing.Point(87, 116);
            this.Contraseña.Name = "Contraseña";
            this.Contraseña.PasswordChar = '✿';
            this.Contraseña.Size = new System.Drawing.Size(90, 20);
            this.Contraseña.TabIndex = 2;
            // 
            // UsuarioLbl
            // 
            this.UsuarioLbl.AutoSize = true;
            this.UsuarioLbl.Location = new System.Drawing.Point(20, 83);
            this.UsuarioLbl.Name = "UsuarioLbl";
            this.UsuarioLbl.Size = new System.Drawing.Size(46, 13);
            this.UsuarioLbl.TabIndex = 3;
            this.UsuarioLbl.Text = "Usuario:";
            // 
            // PswLbl
            // 
            this.PswLbl.AutoSize = true;
            this.PswLbl.Location = new System.Drawing.Point(20, 119);
            this.PswLbl.Name = "PswLbl";
            this.PswLbl.Size = new System.Drawing.Size(64, 13);
            this.PswLbl.TabIndex = 4;
            this.PswLbl.Text = "Contraseña:";
            // 
            // Desconectar
            // 
            this.Desconectar.Location = new System.Drawing.Point(612, 11);
            this.Desconectar.Name = "Desconectar";
            this.Desconectar.Size = new System.Drawing.Size(90, 23);
            this.Desconectar.TabIndex = 6;
            this.Desconectar.Text = "Desconectar";
            this.Desconectar.UseVisualStyleBackColor = true;
            this.Desconectar.Click += new System.EventHandler(this.Desconectar_Click);
            // 
            // Login
            // 
            this.Login.Location = new System.Drawing.Point(87, 183);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(90, 23);
            this.Login.TabIndex = 7;
            this.Login.Text = "Login";
            this.Login.UseVisualStyleBackColor = true;
            this.Login.Click += new System.EventHandler(this.Login_Click);
            // 
            // Rapido
            // 
            this.Rapido.AutoSize = true;
            this.Rapido.Location = new System.Drawing.Point(485, 59);
            this.Rapido.Name = "Rapido";
            this.Rapido.Size = new System.Drawing.Size(230, 17);
            this.Rapido.TabIndex = 9;
            this.Rapido.TabStop = true;
            this.Rapido.Text = "Quienes fueron los que ganaron mas rapido";
            this.Rapido.UseVisualStyleBackColor = true;
            // 
            // Ganadores
            // 
            this.Ganadores.AutoSize = true;
            this.Ganadores.Location = new System.Drawing.Point(485, 81);
            this.Ganadores.Name = "Ganadores";
            this.Ganadores.Size = new System.Drawing.Size(171, 17);
            this.Ganadores.TabIndex = 10;
            this.Ganadores.TabStop = true;
            this.Ganadores.Text = "Quien ha ganado partidas con:";
            this.Ganadores.UseVisualStyleBackColor = true;
            // 
            // Viciado
            // 
            this.Viciado.AutoSize = true;
            this.Viciado.Location = new System.Drawing.Point(485, 102);
            this.Viciado.Name = "Viciado";
            this.Viciado.Size = new System.Drawing.Size(165, 17);
            this.Viciado.TabIndex = 11;
            this.Viciado.TabStop = true;
            this.Viciado.Text = "Quien ha jugado mas partidas";
            this.Viciado.UseVisualStyleBackColor = true;
            // 
            // Consultar
            // 
            this.Consultar.Location = new System.Drawing.Point(543, 125);
            this.Consultar.Name = "Consultar";
            this.Consultar.Size = new System.Drawing.Size(90, 23);
            this.Consultar.TabIndex = 12;
            this.Consultar.Text = "Consultar";
            this.Consultar.UseVisualStyleBackColor = true;
            this.Consultar.Click += new System.EventHandler(this.Consultar_Click);
            // 
            // Invitar
            // 
            this.Invitar.Location = new System.Drawing.Point(310, 244);
            this.Invitar.Name = "Invitar";
            this.Invitar.Size = new System.Drawing.Size(90, 23);
            this.Invitar.TabIndex = 14;
            this.Invitar.Text = "Invitar";
            this.Invitar.UseVisualStyleBackColor = true;
            this.Invitar.Click += new System.EventHandler(this.Invitar_Click);
            // 
            // ConectadosGrid
            // 
            this.ConectadosGrid.BackgroundColor = System.Drawing.Color.PaleGreen;
            this.ConectadosGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConectadosGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ConectadosGrid.Location = new System.Drawing.Point(271, 83);
            this.ConectadosGrid.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ConectadosGrid.Name = "ConectadosGrid";
            this.ConectadosGrid.RowTemplate.Height = 28;
            this.ConectadosGrid.Size = new System.Drawing.Size(156, 129);
            this.ConectadosGrid.TabIndex = 15;
            this.ConectadosGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ConectadosGrid_CellClick);
            // 
            // InvitarLbl
            // 
            this.InvitarLbl.AutoSize = true;
            this.InvitarLbl.Location = new System.Drawing.Point(258, 226);
            this.InvitarLbl.Name = "InvitarLbl";
            this.InvitarLbl.Size = new System.Drawing.Size(0, 13);
            this.InvitarLbl.TabIndex = 16;
            // 
            // Eliminar
            // 
            this.Eliminar.Location = new System.Drawing.Point(512, 11);
            this.Eliminar.Name = "Eliminar";
            this.Eliminar.Size = new System.Drawing.Size(90, 23);
            this.Eliminar.TabIndex = 17;
            this.Eliminar.Text = "Eliminar usuario";
            this.Eliminar.UseVisualStyleBackColor = true;
            this.Eliminar.Click += new System.EventHandler(this.Eliminar_Click);
            // 
            // MaxLbl
            // 
            this.MaxLbl.AutoSize = true;
            this.MaxLbl.ForeColor = System.Drawing.Color.DarkRed;
            this.MaxLbl.Location = new System.Drawing.Point(232, 272);
            this.MaxLbl.Name = "MaxLbl";
            this.MaxLbl.Size = new System.Drawing.Size(0, 13);
            this.MaxLbl.TabIndex = 18;
            // 
            // NombreLbl
            // 
            this.NombreLbl.AutoSize = true;
            this.NombreLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NombreLbl.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.NombreLbl.Location = new System.Drawing.Point(19, 9);
            this.NombreLbl.Name = "NombreLbl";
            this.NombreLbl.Size = new System.Drawing.Size(0, 25);
            this.NombreLbl.TabIndex = 19;
            // 
            // InfoLbl
            // 
            this.InfoLbl.AutoSize = true;
            this.InfoLbl.Location = new System.Drawing.Point(186, 83);
            this.InfoLbl.Name = "InfoLbl";
            this.InfoLbl.Size = new System.Drawing.Size(0, 13);
            this.InfoLbl.TabIndex = 20;
            // 
            // PswVisible
            // 
            this.PswVisible.AutoSize = true;
            this.PswVisible.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PswVisible.Location = new System.Drawing.Point(182, 110);
            this.PswVisible.Name = "PswVisible";
            this.PswVisible.Size = new System.Drawing.Size(53, 37);
            this.PswVisible.TabIndex = 21;
            this.PswVisible.Text = "⚇";
            this.PswVisible.UseVisualStyleBackColor = true;
            this.PswVisible.CheckedChanged += new System.EventHandler(this.PswVisible_CheckedChanged);
            // 
            // RespuestasBox
            // 
            this.RespuestasBox.Location = new System.Drawing.Point(485, 160);
            this.RespuestasBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RespuestasBox.Multiline = true;
            this.RespuestasBox.Name = "RespuestasBox";
            this.RespuestasBox.Size = new System.Drawing.Size(334, 107);
            this.RespuestasBox.TabIndex = 22;
            // 
            // NombreConsulta
            // 
            this.NombreConsulta.Location = new System.Drawing.Point(657, 80);
            this.NombreConsulta.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.NombreConsulta.Name = "NombreConsulta";
            this.NombreConsulta.Size = new System.Drawing.Size(81, 20);
            this.NombreConsulta.TabIndex = 23;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 320);
            this.Controls.Add(this.NombreConsulta);
            this.Controls.Add(this.RespuestasBox);
            this.Controls.Add(this.PswVisible);
            this.Controls.Add(this.InfoLbl);
            this.Controls.Add(this.NombreLbl);
            this.Controls.Add(this.MaxLbl);
            this.Controls.Add(this.Eliminar);
            this.Controls.Add(this.InvitarLbl);
            this.Controls.Add(this.ConectadosGrid);
            this.Controls.Add(this.Invitar);
            this.Controls.Add(this.Consultar);
            this.Controls.Add(this.Viciado);
            this.Controls.Add(this.Ganadores);
            this.Controls.Add(this.Rapido);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.Desconectar);
            this.Controls.Add(this.PswLbl);
            this.Controls.Add(this.UsuarioLbl);
            this.Controls.Add(this.Contraseña);
            this.Controls.Add(this.Usuario);
            this.Controls.Add(this.Registro);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Registro;
        private System.Windows.Forms.TextBox Usuario;
        private System.Windows.Forms.TextBox Contraseña;
        private System.Windows.Forms.Label UsuarioLbl;
        private System.Windows.Forms.Label PswLbl;
        private System.Windows.Forms.Button Desconectar;
        private System.Windows.Forms.Button Login;
        private System.Windows.Forms.RadioButton Rapido;
        private System.Windows.Forms.RadioButton Ganadores;
        private System.Windows.Forms.RadioButton Viciado;
        private System.Windows.Forms.Button Consultar;
        private System.Windows.Forms.Button Invitar;
        private System.Windows.Forms.DataGridView ConectadosGrid;
        private System.Windows.Forms.Label InvitarLbl;
        private System.Windows.Forms.Button Eliminar;
        private System.Windows.Forms.Label MaxLbl;
        private System.Windows.Forms.Label NombreLbl;
        private System.Windows.Forms.Label InfoLbl;
        private System.Windows.Forms.CheckBox PswVisible;
        private System.Windows.Forms.TextBox RespuestasBox;
        private System.Windows.Forms.TextBox NombreConsulta;
    }
}

