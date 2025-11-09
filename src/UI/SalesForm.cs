using Minimarket.Data;
using Models;

namespace Minimarket.UI
{
    public class SalesForm : Form
    {
        private TextBox txtCodigo = new TextBox { PlaceholderText = "Código de producto" };
        private NumericUpDown nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 1 };
        private Button btnAgregar = new Button { Text = "Agregar" };
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Label lblTotal = new Label { Text = "Total: $0" };
        private ComboBox cboPago = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnConfirmar = new Button { Text = "Confirmar Venta" };

        private BindingSource bs = new BindingSource();
        private List<SaleItem> carrito = new();

        public SalesForm()
        {
            Text = "Ventas";
            Width = 900; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            cboPago.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "QR" });
            cboPago.SelectedIndex = 0;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] { txtCodigo, nudCantidad, btnAgregar, cboPago, lblTotal, btnConfirmar });

            grid.DataSource = bs;
            Controls.Add(grid);
            Controls.Add(top);

            btnAgregar.Click += (s, e) => AgregarProducto();
            btnConfirmar.Click += (s, e) => ConfirmarVenta();

            CargarGrid();
        }

        private void CargarGrid()
        {
            bs.DataSource = carrito.Select(x => new
            {
                x.Code,
                x.Descripcion,
                x.Qty,
                x.UnitPrice,
                Subtotal = x.Subtotal
            }).ToList();
            lblTotal.Text = $"Total: ${carrito.Sum(x => x.Subtotal):0.00}";
        }

        private void AgregarProducto()
        {
            var codigo = txtCodigo.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigo)) { MessageBox.Show("Ingrese un código"); return; }
            var p = ProductRepository.GetByCodigo(codigo);
            if (p == null) { MessageBox.Show("Producto no encontrado"); return; }
            int cant = (int)nudCantidad.Value;
            if (p.Stock < cant) { MessageBox.Show("Stock insuficiente"); return; }

            var existente = carrito.FirstOrDefault(i => i.ProductId == p.Id);
            if (existente == null)
            {
                carrito.Add(new SaleItem
                {
                    Descripcion = p.Name,
                    Code = p.Code,
                    ProductId = p.Id,
                    Qty = cant,
                    UnitPrice = p.Price
                });
            }
            else
            {
                existente.Qty += cant;
            }
            CargarGrid();
            txtCodigo.Clear();
            nudCantidad.Value = 1;
        }

        private void ConfirmarVenta()
        {
            if (carrito.Count == 0) { MessageBox.Show("No hay items en el carrito"); return; }
            var venta = new Sale
            {
                DateTime = DateTime.Now,
                PaymentMethod = cboPago.SelectedItem!.ToString()!,
                Total = carrito.Sum(x => x.Subtotal),
                SaleItems = carrito
            };
            try
            {
                int id = SaleRepository.CrearVenta(venta);
                MessageBox.Show($"Venta registrada. N° {id}");
                carrito = new();
                CargarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la venta: " + ex.Message);
            }
        }
    }
}
