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
            this.components = new System.ComponentModel.Container();
            this.Registro_botton = new System.Windows.Forms.Button();
            this.Usuario = new System.Windows.Forms.TextBox();
            this.Contraseña = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Desconectar = new System.Windows.Forms.Button();
            this.Login = new System.Windows.Forms.Button();
            this.Rapido = new System.Windows.Forms.RadioButton();
            this.Ganadores = new System.Windows.Forms.RadioButton();
            this.Viciado = new System.Windows.Forms.RadioButton();
            this.Consultar = new System.Windows.Forms.Button();
            this.ConectadosGrid = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // Registro_botton
            // 
            this.Registro_botton.Location = new System.Drawing.Point(79, 96);
            this.Registro_botton.Name = "Registro_botton";
            this.Registro_botton.Size = new System.Drawing.Size(90, 23);
            this.Registro_botton.TabIndex = 0;
            this.Registro_botton.Text = "Registro";
            this.Registro_botton.UseVisualStyleBackColor = true;
            this.Registro_botton.Click += new System.EventHandler(this.Registro_botton_Click_1);
            // 
            // Usuario
            // 
            this.Usuario.Location = new System.Drawing.Point(79, 22);
            this.Usuario.Name = "Usuario";
            this.Usuario.Size = new System.Drawing.Size(90, 20);
            this.Usuario.TabIndex = 1;
            // 
            // Contraseña
            // 
            this.Contraseña.Location = new System.Drawing.Point(79, 58);
            this.Contraseña.Name = "Contraseña";
            this.Contraseña.Size = new System.Drawing.Size(90, 20);
            this.Contraseña.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Usuario:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Contraseña:";
            // 
            // Desconectar
            // 
            this.Desconectar.Location = new System.Drawing.Point(585, 12);
            this.Desconectar.Name = "Desconectar";
            this.Desconectar.Size = new System.Drawing.Size(90, 23);
            this.Desconectar.TabIndex = 6;
            this.Desconectar.Text = "Desconectar";
            this.Desconectar.UseVisualStyleBackColor = true;
            this.Desconectar.Click += new System.EventHandler(this.Desconectar_Click);
            // 
            // Login
            // 
            this.Login.Location = new System.Drawing.Point(79, 125);
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
            this.Rapido.Size = new System.Drawing.Size(190, 17);
            this.Rapido.TabIndex = 9;
            this.Rapido.TabStop = true;
            this.Rapido.Text = "Quien ha ganado en menos tiempo";
            this.Rapido.UseVisualStyleBackColor = true;
            // 
            // Ganadores
            // 
            this.Ganadores.AutoSize = true;
            this.Ganadores.Location = new System.Drawing.Point(485, 81);
            this.Ganadores.Name = "Ganadores";
            this.Ganadores.Size = new System.Drawing.Size(202, 17);
            this.Ganadores.TabIndex = 10;
            this.Ganadores.TabStop = true;
            this.Ganadores.Text = "Quien ha ganado partidas contra Joel";
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
            // ConectadosGrid
            // 
            this.ConectadosGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConectadosGrid.Location = new System.Drawing.Point(220, 59);
            this.ConectadosGrid.Name = "ConectadosGrid";
            this.ConectadosGrid.Size = new System.Drawing.Size(240, 150);
            this.ConectadosGrid.TabIndex = 13;
           
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 320);
            this.Controls.Add(this.ConectadosGrid);
            this.Controls.Add(this.Consultar);
            this.Controls.Add(this.Viciado);
            this.Controls.Add(this.Ganadores);
            this.Controls.Add(this.Rapido);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.Desconectar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Contraseña);
            this.Controls.Add(this.Usuario);
            this.Controls.Add(this.Registro_botton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Registro_botton;
        private System.Windows.Forms.TextBox Usuario;
        private System.Windows.Forms.TextBox Contraseña;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Desconectar;
        private System.Windows.Forms.Button Login;
        private System.Windows.Forms.RadioButton Rapido;
        private System.Windows.Forms.RadioButton Ganadores;
        private System.Windows.Forms.RadioButton Viciado;
        private System.Windows.Forms.Button Consultar;
        private System.Windows.Forms.DataGridView ConectadosGrid;
        private System.Windows.Forms.Timer timer1;
    }
}

