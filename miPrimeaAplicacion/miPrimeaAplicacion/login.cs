
using System;
using System.Drawing;
using System.Windows.Forms;

namespace miPrimeaAplicacion
{
    public class login : Form
    {
        private TextBox txtUsuario;
        private TextBox txtClave;
        private Button btnLogin;
        private Conexion conexion;

        public login()
        {
            Text = "Login - Sistema Académico";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(390, 250);
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.WhiteSmoke;

            Label titulo = new Label
            {
                Text = "INICIO DE SESIÓN",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(82, 22)
            };

            Label lblUsuario = new Label
            {
                Text = "Usuario",
                AutoSize = true,
                Location = new Point(38, 83)
            };

            txtUsuario = new TextBox
            {
                Name = "txtUsuario",
                Location = new Point(145, 78),
                Width = 205,
                MaxLength = 35
            };

            Label lblClave = new Label
            {
                Text = "Contraseña",
                AutoSize = true,
                Location = new Point(38, 123)
            };

            txtClave = new TextBox
            {
                Name = "txtClave",
                Location = new Point(145, 118),
                Width = 205,
                UseSystemPasswordChar = true,
                MaxLength = 35
            };

            btnLogin = new Button
            {
                Text = "Iniciar sesión",
                Location = new Point(145, 165),
                Width = 205,
                Height = 36,
                BackColor = Color.FromArgb(31, 78, 121),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLogin.Click += btnLogin_Click;

            txtClave.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnLogin.PerformClick();
                    e.SuppressKeyPress = true;
                }
            };

            // Agregar los controles al formulario
            Controls.AddRange(new Control[]
            {
                titulo,
                lblUsuario,
                txtUsuario,
                lblClave,
                txtClave,
                btnLogin
            });

            // Permitir iniciar sesión al presionar Enter
            AcceptButton = btnLogin;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrEmpty(txtClave.Text))
            {
                MessageBox.Show(
                    "Ingrese usuario y contraseña.",
                    "Datos requeridos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                conexion = new Conexion();

                if (conexion.validarUsuario(
                    txtUsuario.Text.Trim(),
                    txtClave.Text))
                {
                    Hide();

                    using (frmPrincipal principal = new frmPrincipal())
                    {
                        principal.ShowDialog();
                    }

                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "Usuario o contraseña incorrectos.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    txtClave.Clear();
                    txtClave.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar con la base de datos.\n" + ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
