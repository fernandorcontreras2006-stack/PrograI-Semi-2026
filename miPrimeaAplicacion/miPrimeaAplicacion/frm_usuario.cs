using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace miPrimeaAplicacion
{
    public class frm_usuario : Form
    {
        private TextBox txtIdUsuario, txtUsuario, txtClave, txtConfirmarClave, txtNombre, txtDireccion, txtTelefono;
        private DataGridView dgvUsuarios;
        private Button btnAgregar, btnModificar, btnEliminar, btnLimpiar, btnPrimero, btnAnterior, btnSiguiente, btnUltimo;
        private DataTable tabla;
        private int indice = -1;

        public frm_usuario()
        {
            Text = "Mantenimiento de usuarios";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(850, 650);
            Size = new Size(980, 720);
            Font = new Font("Segoe UI", 9F);
            ConstruirInterfaz();
            CargarUsuarios();
        }

        private void ConstruirInterfaz()
        {
            Label titulo = new Label { Text = "MANTENIMIENTO DE USUARIOS", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.FromArgb(31, 78, 121), AutoSize = true, Location = new Point(25, 15) };
            Controls.Add(titulo);
            int labelX = 28, inputX = 170, y = 65, gap = 42;
            AgregarCampo("ID Usuario", "txtIdUsuario", labelX, inputX, y, out txtIdUsuario); txtIdUsuario.ReadOnly = true; y += gap;
            AgregarCampo("Usuario", "txtUsuario", labelX, inputX, y, out txtUsuario); txtUsuario.MaxLength = 35; y += gap;
            AgregarCampo("Clave", "txtClave", labelX, inputX, y, out txtClave); txtClave.UseSystemPasswordChar = true; txtClave.MaxLength = 35; y += gap;
            AgregarCampo("Confirmar clave", "txtConfirmarClave", labelX, inputX, y, out txtConfirmarClave); txtConfirmarClave.UseSystemPasswordChar = true; txtConfirmarClave.MaxLength = 35; y += gap;
            AgregarCampo("Nombre", "txtNombre", labelX, inputX, y, out txtNombre); txtNombre.MaxLength = 65; y += gap;
            AgregarCampo("Dirección", "txtDireccion", labelX, inputX, y, out txtDireccion); txtDireccion.MaxLength = 85; y += gap;
            AgregarCampo("Teléfono", "txtTelefono", labelX, inputX, y, out txtTelefono); txtTelefono.MaxLength = 9;

            btnAgregar = Boton("Agregar", 430, 72, Color.FromArgb(46, 125, 50)); btnAgregar.Click += (s, e) => Guardar("nuevo");
            btnModificar = Boton("Modificar", 550, 72, Color.FromArgb(31, 78, 121)); btnModificar.Click += (s, e) => Guardar("modificar");
            btnEliminar = Boton("Eliminar", 670, 72, Color.FromArgb(183, 28, 28)); btnEliminar.Click += btnEliminar_Click;
            btnLimpiar = Boton("Limpiar", 790, 72, Color.DimGray); btnLimpiar.Click += (s, e) => Limpiar();
            btnPrimero = Boton("|<", 430, 125, Color.Gray); btnPrimero.Width = 65; btnPrimero.Click += (s, e) => MostrarIndice(0);
            btnAnterior = Boton("<", 505, 125, Color.Gray); btnAnterior.Width = 65; btnAnterior.Click += (s, e) => MostrarIndice(indice - 1);
            btnSiguiente = Boton(">", 580, 125, Color.Gray); btnSiguiente.Width = 65; btnSiguiente.Click += (s, e) => MostrarIndice(indice + 1);
            btnUltimo = Boton(" >|", 655, 125, Color.Gray); btnUltimo.Width = 65; btnUltimo.Click += (s, e) => MostrarIndice(tabla == null ? -1 : tabla.Rows.Count - 1);

            dgvUsuarios = new DataGridView { Name = "dgvUsuarios", Location = new Point(25, 390), Size = new Size(910, 260), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White };
            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
            Label lblLista = new Label { Text = "Usuarios registrados", Font = new Font("Segoe UI", 11F, FontStyle.Bold), AutoSize = true, Location = new Point(25, 360) };
            Controls.AddRange(new Control[] { btnAgregar, btnModificar, btnEliminar, btnLimpiar, btnPrimero, btnAnterior, btnSiguiente, btnUltimo, lblLista, dgvUsuarios });
        }

        private void AgregarCampo(string etiqueta, string nombre, int lx, int tx, int y, out TextBox caja)
        {
            Controls.Add(new Label { Text = etiqueta, AutoSize = true, Location = new Point(lx, y + 5) });
            caja = new TextBox { Name = nombre, Location = new Point(tx, y), Width = 220 };
            Controls.Add(caja);
        }

        private Button Boton(string texto, int x, int y, Color color)
        {
            Button b = new Button { Text = texto, Location = new Point(x, y), Size = new Size(105, 36), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            return b;
        }

        private void CargarUsuarios(int idSeleccionar = -1)
        {
            try
            {
                Conexion cn = new Conexion();
                DataSet ds = cn.obtenerDatos();
                tabla = ds.Tables["usuarios_usss032426"];
                dgvUsuarios.DataSource = tabla;
                if (tabla != null && tabla.Rows.Count > 0)
                {
                    int objetivo = 0;
                    if (idSeleccionar > 0)
                    {
                        for (int i = 0; i < tabla.Rows.Count; i++)
                            if (Convert.ToInt32(tabla.Rows[i]["idUsuario"]) == idSeleccionar) { objetivo = i; break; }
                    }
                    MostrarIndice(objetivo);
                }
                else Limpiar();
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar usuarios: " + ex.Message); }
        }

        private bool Validar(out string clave)
        {
            clave = txtClave.Text;
            string usuario = txtUsuario.Text.Trim();
            if (usuario.Length < 6 || usuario.Length > 16)
            { MessageBox.Show("El usuario debe tener entre 6 y 16 caracteres."); txtUsuario.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDireccion.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            { MessageBox.Show("Complete todos los campos."); return false; }
            if (clave != txtConfirmarClave.Text)
            { MessageBox.Show("Las claves no coinciden."); txtConfirmarClave.Focus(); return false; }
            bool letra = false, numero = false;
            foreach (char c in clave) { if (char.IsLetter(c)) letra = true; if (char.IsDigit(c)) numero = true; }
            if (clave.Length == 0 || clave.Length > 35 || !letra || !numero)
            { MessageBox.Show("La clave debe tener entre 1 y 35 caracteres e incluir letras y números."); txtClave.Focus(); return false; }
            if (txtTelefono.Text.Trim().Length > 9)
            { MessageBox.Show("El teléfono admite máximo 9 caracteres."); txtTelefono.Focus(); return false; }
            return true;
        }

        private void Guardar(string accion)
        {
            string clave;
            if (!Validar(out clave)) return;
            if (accion == "modificar" && string.IsNullOrWhiteSpace(txtIdUsuario.Text))
            { MessageBox.Show("Seleccione un usuario para modificar."); return; }
            string[] datos = { txtIdUsuario.Text, txtUsuario.Text.Trim(), clave, txtNombre.Text.Trim(), txtDireccion.Text.Trim(), txtTelefono.Text.Trim() };
            try
            {
                Conexion cn = new Conexion();
                string resultado = cn.mantenimiento_usuarios_usss032426(datos, accion);
                if (resultado.StartsWith("ERROR:") || !int.TryParse(resultado, out int filas) || filas < 1)
                    MessageBox.Show("No se pudo guardar el usuario. " + resultado, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    MessageBox.Show(accion == "nuevo" ? "Usuario agregado correctamente." : "Usuario modificado correctamente.");
                    int id = 0; int.TryParse(txtIdUsuario.Text, out id);
                    CargarUsuarios(accion == "modificar" ? id : -1);
                    if (accion == "nuevo") Limpiar();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtIdUsuario.Text, out id)) { MessageBox.Show("Seleccione un usuario para eliminar."); return; }
            if (MessageBox.Show("¿Desea eliminar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string[] datos = { txtIdUsuario.Text, "", "", "", "", "" };
            Conexion cn = new Conexion();
            string resultado = cn.mantenimiento_usuarios_usss032426(datos, "eliminar");
            if (resultado.StartsWith("ERROR:") || !int.TryParse(resultado, out int filas) || filas < 1) MessageBox.Show("No se pudo eliminar. " + resultado);
            else { MessageBox.Show("Usuario eliminado."); CargarUsuarios(); }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.IsNewRow) return;
            DataGridViewRow r = dgvUsuarios.CurrentRow;
            if (r.Cells["idUsuario"].Value == null) return;
            txtIdUsuario.Text = Convert.ToString(r.Cells["idUsuario"].Value);
            txtUsuario.Text = Convert.ToString(r.Cells["usuario"].Value);
            txtClave.Text = Convert.ToString(r.Cells["clave"].Value);
            txtConfirmarClave.Text = txtClave.Text;
            txtNombre.Text = Convert.ToString(r.Cells["nombre"].Value);
            txtDireccion.Text = Convert.ToString(r.Cells["direccion"].Value);
            txtTelefono.Text = Convert.ToString(r.Cells["telefono"].Value);
            indice = r.Index;
        }

        private void MostrarIndice(int nuevoIndice)
        {
            if (tabla == null || tabla.Rows.Count == 0) { indice = -1; return; }
            if (nuevoIndice < 0) nuevoIndice = 0;
            if (nuevoIndice >= tabla.Rows.Count) nuevoIndice = tabla.Rows.Count - 1;
            indice = nuevoIndice;
            dgvUsuarios.ClearSelection();
            dgvUsuarios.Rows[indice].Selected = true;
            dgvUsuarios.CurrentCell = dgvUsuarios.Rows[indice].Cells[0];
            dgvUsuarios_SelectionChanged(this, EventArgs.Empty);
        }

        private void Limpiar()
        {
            txtIdUsuario.Clear(); txtUsuario.Clear(); txtClave.Clear(); txtConfirmarClave.Clear();
            txtNombre.Clear(); txtDireccion.Clear(); txtTelefono.Clear(); indice = -1;
            txtUsuario.Focus();
        }
    }
}
