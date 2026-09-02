using System;
using System.Globalization;
using System.Windows.Forms;

namespace miPrimeaAplicacion
{
    public partial class Form1 : Form
    {
        // Factores de conversión a metros cuadrados
        // El índice debe coincidir con el SelectedIndex del ComboBox
        private readonly double[] valores =
        {
            0.092903,  // Pie cuadrado
            0.698896,  // Vara cuadrada
            0.836127,  // Yarda cuadrada
            1.0,       // Metro cuadrado
            437.5,     // Tarea
            6988.96,   // Manzana
            10000.0    // Hectárea
        };

        private readonly string[] unidades =
        {
            "Pie²",
            "Vara²",
            "Yarda²",
            "m²",
            "Tarea",
            "Manzana",
            "Hectárea"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cargar las unidades en los ComboBox
            cboDe.Items.Clear();
            cboA.Items.Clear();

            foreach (string unidad in unidades)
            {
                cboDe.Items.Add(unidad);
                cboA.Items.Add(unidad);
            }

            // Seleccionar valores iniciales
            cboDe.SelectedIndex = 0;
            cboA.SelectedIndex = 1;
        }

        private void btnCalcular_Click_1(object sender, EventArgs e)
        {
            int origen = cboDe.SelectedIndex;
            int destino = cboA.SelectedIndex;

            if (origen < 0 || destino < 0)
            {
                MessageBox.Show(
                    "Seleccione la unidad de origen y la unidad de destino.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // Permite escribir números usando punto o coma decimal
            string textoCantidad = txtCantidad.Text.Trim().Replace(',', '.');

            if (!double.TryParse(
                    textoCantidad,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double cantidad))
            {
                MessageBox.Show(
                    "Ingrese una cantidad válida.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            if (cantidad < 0)
            {
                MessageBox.Show(
                    "La cantidad no puede ser negativa.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            // Convertir primero a metros cuadrados
            double cantidadEnMetrosCuadrados =
                cantidad * valores[origen];

            // Convertir de metros cuadrados a la unidad destino
            double respuesta =
                cantidadEnMetrosCuadrados / valores[destino];

            lblRespuesta.Text =
                $"{cantidad:N4} {unidades[origen]} equivalen a " +
                $"{respuesta:N4} {unidades[destino]}";
        }
    }
}
