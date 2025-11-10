using Minimarket.Data;
using Models;

namespace Minimarket.UI
{
    public class SalesForm : Form
    {
        private ComboBox cmbProducto = new ComboBox { Width = 300 };
        private NumericUpDown nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 1, Width = 80 };
        private Button btnAgregar = new Button { Text = "Agregar", AutoSize = true };
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Label lblTotal = new Label { Text = "Total: $0", Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = true };
        private Button btnConfirmar = new Button { Text = "Confirmar Venta", AutoSize = true };

        private List<SaleItem> carrito = new();

        public SalesForm()
        {
            Text = "Ventas";
            Width = 1000; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con controles alineados
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            topPanel.Controls.Add(new Label { Text = "Producto:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            topPanel.Controls.Add(cmbProducto);
            topPanel.Controls.Add(new Label { Text = "Cantidad:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(nudCantidad);
            btnAgregar.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnAgregar);

            // Espaciador para empujar botón a la derecha
            var spacer = new Panel { Width = 200, Height = 1 };
            topPanel.Controls.Add(spacer);

            btnConfirmar.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnConfirmar);

            // Panel para el total (abajo del topPanel)
            var totalPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight
            };
            totalPanel.Controls.Add(lblTotal);

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Code", HeaderText = "Código", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descripcion", HeaderText = "Descripción" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Qty", HeaderText = "Cantidad", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Precio Unit.", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", HeaderText = "Subtotal", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(totalPanel);
            Controls.Add(topPanel);

            Load += (s, e) => CargarProductos();
            btnAgregar.Click += (s, e) => AgregarProducto();
            btnConfirmar.Click += (s, e) => ConfirmarVenta();
            cmbProducto.TextChanged += FiltrarProductos;

            CargarGrid();
        }

        private void CargarProductos()
        {
            var productos = ProductRepository.GetAllForSale();
            cmbProducto.DataSource = productos;
            cmbProducto.DisplayMember = "Name";
            cmbProducto.ValueMember = "Id";
            cmbProducto.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbProducto.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbProducto.SelectedIndex = -1; // No seleccionar ninguno por defecto
            cmbProducto.Text = ""; // Limpiar el texto
        }

        private void FiltrarProductos(object sender, EventArgs e)
        {
            var texto = cmbProducto.Text.ToLower();
            
            var productos = ProductRepository.GetAllForSale();
            
            if (!string.IsNullOrWhiteSpace(texto))
            {
                productos = productos.Where(p => 
                    p.Name.ToLower().Contains(texto) || 
                    p.Code.ToLower().Contains(texto)
                ).ToList();
            }

            // Temporalmente desconectar el evento para evitar recursión
            cmbProducto.TextChanged -= FiltrarProductos;
            
            var textoActual = cmbProducto.Text;
            var posicionCursor = cmbProducto.SelectionStart;
            
            cmbProducto.DataSource = productos;
            cmbProducto.DisplayMember = "Name";
            cmbProducto.ValueMember = "Id";
            cmbProducto.SelectedIndex = -1;
            
            cmbProducto.Text = textoActual;
            cmbProducto.SelectionStart = posicionCursor;
            
            // Reconectar el evento
            cmbProducto.TextChanged += FiltrarProductos;
        }

        private void CargarGrid()
        {
            grid.DataSource = null;
            grid.DataSource = carrito.Select(x => new
            {
                x.Code,
                x.Descripcion,
                x.Qty,
                x.UnitPrice,
                x.Subtotal
            }).ToList();
            lblTotal.Text = $"Total: {carrito.Sum(x => x.Subtotal):C2}";
            lblTotal.ForeColor = carrito.Count > 0 ? Color.Green : Color.Black;
        }

        private void AgregarProducto()
        {
            if (cmbProducto.SelectedItem == null || cmbProducto.SelectedIndex == -1) 
            { 
                MessageBox.Show("Seleccione un producto de la lista"); 
                return; 
            }

            var p = (Product)cmbProducto.SelectedItem;
            int cant = (int)nudCantidad.Value;
            
            if (p.Stock < cant) 
            { 
                MessageBox.Show($"Stock insuficiente. Stock disponible: {p.Stock}"); 
                return; 
            }

            var existente = carrito.FirstOrDefault(i => i.ProductId == p.Id);
            if (existente == null)
            {
                carrito.Add(new SaleItem
                {
                    Descripcion = p.Name,
                    Code = p.Code,
                    ProductId = p.Id,
                    Qty = cant,
                    UnitPrice = p.Price,
                    Subtotal = cant * p.Price
                });
            }
            else
            {
                int nuevaCantidad = existente.Qty + cant;
                if (p.Stock < nuevaCantidad)
                {
                    MessageBox.Show($"Stock insuficiente. Stock disponible: {p.Stock}, ya tiene {existente.Qty} en el carrito");
                    return;
                }
                existente.Qty = nuevaCantidad;
                existente.Subtotal = nuevaCantidad * existente.UnitPrice;
            }
            CargarGrid();
            nudCantidad.Value = 1;
            cmbProducto.Text = "";
            cmbProducto.SelectedIndex = -1;
            cmbProducto.Focus();
        }

        private void ConfirmarVenta()
        {
            if (carrito.Count == 0) { MessageBox.Show("No hay items en el carrito"); return; }
            
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("No hay usuario logueado");
                return;
            }

            // Preguntar método de pago al confirmar
            using var formPago = new Form
            {
                Text = "Método de Pago",
                Width = 300,
                Height = 150,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblPregunta = new Label 
            { 
                Text = "Seleccione el método de pago:", 
                AutoSize = true, 
                Location = new Point(20, 20) 
            };
            
            var cmbMetodoPago = new ComboBox 
            { 
                DropDownStyle = ComboBoxStyle.DropDownList, 
                Width = 200, 
                Location = new Point(20, 50) 
            };
            cmbMetodoPago.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "QR" });
            cmbMetodoPago.SelectedIndex = 0;

            var btnOk = new Button 
            { 
                Text = "Aceptar", 
                DialogResult = DialogResult.OK, 
                Location = new Point(145, 85) 
            };
            
            var btnCancelar = new Button 
            { 
                Text = "Cancelar", 
                DialogResult = DialogResult.Cancel, 
                Location = new Point(60, 85) 
            };

            formPago.Controls.AddRange(new Control[] { lblPregunta, cmbMetodoPago, btnCancelar, btnOk });
            formPago.AcceptButton = btnOk;
            formPago.CancelButton = btnCancelar;

            if (formPago.ShowDialog() != DialogResult.OK)
                return;
            
            var venta = new Sale
            {
                DateTime = DateTime.Now,
                PaymentMethod = cmbMetodoPago.SelectedItem!.ToString()!,
                Total = carrito.Sum(x => x.Subtotal),
                UserId = Session.CurrentUser.Id,
                SaleItems = carrito
            };
            try
            {
                int id = SaleRepository.CrearVenta(venta);
                
                // Obtener la venta completa para generar ticket
                var ventaCompleta = SaleRepository.GetByIdWithItems(id);
                if (ventaCompleta != null)
                {
                    try
                    {
                        // Crear carpeta Tickets si no existe
                        var carpetaTickets = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tickets");
                        Directory.CreateDirectory(carpetaTickets);
                        
                        var rutaTicket = Path.Combine(carpetaTickets, $"Ticket_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                        Minimarket.Helpers.PdfHelper.GenerarTicketVenta(ventaCompleta, rutaTicket);
                        
                        var result = MessageBox.Show($"Venta registrada exitosamente.\nN° de operación: {id}\n\n¿Desea abrir el ticket PDF?", 
                            "Venta Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        
                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = rutaTicket,
                                UseShellExecute = true
                            });
                        }
                    }
                    catch (Exception exPdf)
                    {
                        // Mostrar error detallado con stack trace
                        var errorMsg = $"Venta registrada (N° {id}) pero no se pudo generar el ticket.\n\n" +
                                      $"Error: {exPdf.Message}\n\n" +
                                      $"Tipo: {exPdf.GetType().Name}\n\n";
                        
                        if (exPdf.InnerException != null)
                        {
                            errorMsg += $"Error interno: {exPdf.InnerException.Message}\n\n";
                        }
                        
                        errorMsg += $"StackTrace:\n{exPdf.StackTrace}";
                        
                        MessageBox.Show(errorMsg, "Error al generar PDF", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la venta: " + ex.Message);
            }
        }
    }
}
