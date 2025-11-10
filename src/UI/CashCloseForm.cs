using Models;
using Minimarket.Data;

namespace Minimarket.UI
{
    public class CashCloseForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private DateTimePicker dtFecha = new DateTimePicker { Value = DateTime.Today, Width = 120 };
        private NumericUpDown nudEfectivo = new NumericUpDown { DecimalPlaces = 2, Minimum = -1000000, Maximum = 1000000, Width = 120 };
        private NumericUpDown nudPOS = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Width = 120 };
        private NumericUpDown nudQR = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Width = 120 };
        private Label lblSistema = new Label { AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };
        private Label lblDiferencia = new Label { AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };
        private Label lblDesglose = new Label { AutoSize = true, Font = new Font("Arial", 9, FontStyle.Regular), ForeColor = System.Drawing.Color.DarkBlue };
        private TextBox txtNotas = new TextBox { PlaceholderText = "Notas (opcional)", Width = 300 };
        private Button btnCalcular = new Button { Text = "Calcular Sistema", AutoSize = true };
        private Button btnCalculoAutomatico = new Button { Text = "⚡ Cálculo Automático", AutoSize = true, BackColor = System.Drawing.Color.LightGreen, Font = new Font("Arial", 9, FontStyle.Bold) };
        private Button btnCerrar = new Button { Text = "Registrar Cierre", AutoSize = true };
        private Button btnVerDetalle = new Button { Text = "Ver Detalle del Día", AutoSize = true };
        private ToolTip tooltip = new ToolTip();

        public CashCloseForm()
        {
            Text = "Cierre / Arqueo de Caja";
            Width = 1100; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con controles alineados
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            topPanel.Controls.Add(new Label { Text = "Fecha:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            topPanel.Controls.Add(dtFecha);
            
            btnCalcular.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnCalcular);

            btnVerDetalle.Margin = new Padding(5, 3, 5, 3);
            topPanel.Controls.Add(btnVerDetalle);

            btnCalculoAutomatico.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnCalculoAutomatico);

            // Nueva línea
            topPanel.Controls.Add(new Panel { Width = 1100, Height = 1 });

            topPanel.Controls.Add(new Label { Text = "Efectivo contado:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            topPanel.Controls.Add(nudEfectivo);

            topPanel.Controls.Add(new Label { Text = "Total Tarjeta:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(nudPOS);

            topPanel.Controls.Add(new Label { Text = "Total QR:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(nudQR);

            topPanel.Controls.Add(new Label { Text = "Notas:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(txtNotas);

            btnCerrar.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnCerrar);

            // Configurar tooltips
            tooltip.SetToolTip(dtFecha, "Seleccione la fecha del cierre de caja");
            tooltip.SetToolTip(btnCalcular, "Calcula el total que debería haber según ventas y movimientos registrados en el sistema");
            tooltip.SetToolTip(btnCalculoAutomatico, "Calcula automáticamente cuánto debería haber en cada método de pago, considerando ventas Y egresos. ¡Recomendado!");
            tooltip.SetToolTip(nudEfectivo, "Dinero en efectivo físico (billetes y monedas) que contó en la caja. Puede ajustar el valor cargado automáticamente");
            tooltip.SetToolTip(nudPOS, "Total de ventas con tarjeta (débito/crédito) del día. Puede ajustar el valor cargado automáticamente");
            tooltip.SetToolTip(nudQR, "Total de ventas con QR (MercadoPago, etc.) del día. Puede ajustar el valor cargado automáticamente");
            tooltip.SetToolTip(txtNotas, "Agregue observaciones opcionales sobre el cierre (ej: faltante por error en cambio)");
            tooltip.SetToolTip(lblDiferencia, "Diferencia = (Efectivo + Tarjeta + QR) - Total Sistema\nVerde: Cuadra | Azul: Sobrante | Rojo: Faltante");
            tooltip.SetToolTip(btnVerDetalle, "Ver todos los movimientos de caja del día seleccionado para entender el cálculo");

            // Panel para información del sistema
            var infoPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = System.Drawing.Color.LightYellow
            };

            infoPanel.Controls.Add(lblSistema);
            infoPanel.Controls.Add(new Label { Text = "  |  ", AutoSize = true, Margin = new Padding(10, 0, 10, 0) });
            infoPanel.Controls.Add(lblDiferencia);
            
            // Nueva línea para desglose
            infoPanel.Controls.Add(new Panel { Width = 1100, Height = 1 });
            infoPanel.Controls.Add(lblDesglose);

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", HeaderText = "Fecha", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "User", HeaderText = "Usuario", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CashInHand", HeaderText = "Efectivo", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PosTotal", HeaderText = "POS", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SystemTotal", HeaderText = "Sistema", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Difference", HeaderText = "Diferencia", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Notes", HeaderText = "Notas" });

            // Ordenar por fecha descendente
            grid.DataBindingComplete += (s, e) =>
            {
                if (grid.Columns["Date"] != null)
                {
                    grid.Sort(grid.Columns["Date"], System.ComponentModel.ListSortDirection.Descending);
                }
            };

            Controls.Add(grid);
            Controls.Add(infoPanel);
            Controls.Add(topPanel);

            Load += (s, e) => { Cargar(); ActualizarLabels(); };
            btnCalcular.Click += (s, e) => CalcularSistema();
            btnCalculoAutomatico.Click += (s, e) => CalculoAutomatico();
            btnCerrar.Click += (s, e) => Registrar();
            btnVerDetalle.Click += (s, e) => VerDetalleDelDia();
            nudEfectivo.ValueChanged += (s, e) => ActualizarLabels();
            nudPOS.ValueChanged += (s, e) => ActualizarLabels();
            nudQR.ValueChanged += (s, e) => ActualizarLabels();
            dtFecha.ValueChanged += (s, e) => { ActualizarLabels(); CalcularSistema(); lblDesglose.Text = ""; };
        }

        private void Cargar()
        {
            grid.DataSource = CashCloseRepository.GetAllDto();
        }

        private void CalcularSistema()
        {
            var saldoSistema = CashCloseRepository.GetSaldoSistemaDia(dtFecha.Value.Date);
            lblSistema.Text = $"Total Sistema: {saldoSistema:C2}";
            ActualizarLabels();
        }

        private void CalculoAutomatico()
        {
            var fecha = dtFecha.Value.Date;
            var (efectivoReal, tarjetaReal, qrReal, detalle) = CashCloseRepository.CalcularCierreAutomatico(fecha);
            
            // Primero calculamos el sistema para tener el total esperado
            CalcularSistema();
            
            // Cargamos los valores calculados
            nudEfectivo.Value = efectivoReal;
            nudPOS.Value = tarjetaReal;
            nudQR.Value = qrReal;
            
            // Mostramos el detalle
            lblDesglose.Text = detalle.Replace("\n", " ");
            
            // Mostramos mensaje informativo
            var totalCalculado = efectivoReal + tarjetaReal + qrReal;
            if (totalCalculado == 0)
            {
                MessageBox.Show($"No hay movimientos registrados para el {fecha:dd/MM/yyyy}", 
                    "Sin movimientos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"✅ Cálculo automático completado.\n\n" +
                              $"Los montos han sido calculados considerando:\n" +
                              $"• Ventas del día por método de pago\n" +
                              $"• Egresos de efectivo (compras, gastos)\n" +
                              $"• Otros ingresos registrados\n\n" +
                              $"Puede ajustar los valores manualmente si es necesario.\n\n" +
                              $"Ver el panel amarillo para más detalles.",
                              "Cálculo Automático", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            ActualizarLabels();
        }

        private void ActualizarLabels()
        {
            // Extraer valor del label sistema
            decimal saldoSistema = 0;
            if (!string.IsNullOrEmpty(lblSistema.Text) && lblSistema.Text.Contains("Total Sistema:"))
            {
                var texto = lblSistema.Text.Replace("Total Sistema:", "").Replace("$", "").Replace(".", "").Replace(",", ".").Trim();
                decimal.TryParse(texto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out saldoSistema);
            }

            decimal efectivo = nudEfectivo.Value;
            decimal pos = nudPOS.Value;
            decimal qr = nudQR.Value;
            decimal totalContado = efectivo + pos + qr;
            decimal diferencia = totalContado - saldoSistema;

            lblDiferencia.Text = $"Diferencia: {diferencia:C2} (Contado: {totalContado:C2} - Sistema: {saldoSistema:C2})";
            lblDiferencia.ForeColor = diferencia == 0 ? System.Drawing.Color.Green : 
                                      diferencia > 0 ? System.Drawing.Color.Blue : System.Drawing.Color.Red;
        }

        private void VerDetalleDelDia()
        {
            var fecha = dtFecha.Value.Date;
            var movimientos = CashCloseRepository.GetMovimientosDelDia(fecha);

            if (movimientos.Count == 0)
            {
                MessageBox.Show($"No hay movimientos de caja registrados para el {fecha:dd/MM/yyyy}");
                return;
            }

            var form = new Form
            {
                Text = $"Movimientos del {fecha:dd/MM/yyyy}",
                Width = 900,
                Height = 500,
                StartPosition = FormStartPosition.CenterParent
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Fecha/Hora", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "Tipo", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Amount", HeaderText = "Monto", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Concept", HeaderText = "Concepto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Usuario", HeaderText = "Usuario", Width = 120 });

            grid.DataSource = movimientos;

            var ingresos = movimientos.Where(m => m.Type == Constants.CashMovementTypeIn).Sum(m => m.Amount);
            var egresos = movimientos.Where(m => m.Type == Constants.CashMovementTypeOut).Sum(m => m.Amount);
            var saldo = ingresos - egresos;

            var panelResumen = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.LightYellow
            };

            var lblResumen = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Text = $"Ingresos: {ingresos:C2}  |  Egresos: {egresos:C2}  |  Saldo: {saldo:C2}"
            };

            panelResumen.Controls.Add(lblResumen);

            form.Controls.Add(grid);
            form.Controls.Add(panelResumen);
            form.ShowDialog();
        }

        private void Registrar()
        {
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("No hay usuario logueado");
                return;
            }

            // Extraer valor del label sistema
            decimal saldoSistema = 0;
            if (string.IsNullOrEmpty(lblSistema.Text) || !lblSistema.Text.Contains("Total Sistema:"))
            {
                MessageBox.Show("Debe calcular el total del sistema primero (botón 'Calcular')");
                return;
            }
            
            var texto = lblSistema.Text.Replace("Total Sistema:", "").Replace("$", "").Replace(".", "").Replace(",", ".").Trim();
            if (!decimal.TryParse(texto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out saldoSistema))
            {
                MessageBox.Show("Error al obtener el total del sistema");
                return;
            }

            decimal efectivo = nudEfectivo.Value;
            decimal pos = nudPOS.Value;
            decimal qr = nudQR.Value;
            
            if (efectivo == 0 && pos == 0 && qr == 0)
            {
                var result = MessageBox.Show(
                    "No ha ingresado montos.\n\n" +
                    "💡 TIP: Use el botón '⚡ Cálculo Automático' para calcular automáticamente los montos reales considerando ventas Y egresos.\n\n" +
                    "• Efectivo contado: Dinero físico en la caja\n" +
                    "• Total Tarjeta: Ventas con tarjeta del día\n" +
                    "• Total QR: Ventas con QR del día\n\n" +
                    "¿Desea continuar sin ingresar montos?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result != DialogResult.Yes)
                    return;
            }

            decimal diferencia = (efectivo + pos + qr) - saldoSistema;
            var notas = txtNotas.Text.Trim();

            var cierre = new CashClose
            {
                Date = dtFecha.Value.Date,
                UserId = Session.CurrentUser.Id,
                CashInHand = efectivo,
                PosTotal = pos + qr, // Sumamos tarjeta y QR en POS
                SystemTotal = saldoSistema,
                Difference = diferencia,
                Notes = notas
            };

            CashCloseRepository.Create(cierre);
            
            // Generar PDF del acta de cierre
            try
            {
                // Obtener el cierre completo con datos del usuario
                var cierreCompleto = CashCloseRepository.GetById(cierre.Id);
                if (cierreCompleto != null)
                {
                    var carpetaCierres = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cierres");
                    Directory.CreateDirectory(carpetaCierres);
                    
                    var rutaActa = Path.Combine(carpetaCierres, $"Cierre_{cierre.Date:yyyyMMdd}_{DateTime.Now:HHmmss}.pdf");
                    Minimarket.Helpers.PdfHelper.GenerarActaCierre(cierreCompleto, rutaActa);
                    
                    var result = MessageBox.Show($"Cierre registrado correctamente.\n\nResumen:\n" +
                          $"• Total Sistema: {saldoSistema:C2}\n" +
                          $"• Efectivo contado: {efectivo:C2}\n" +
                          $"• Tarjeta: {pos:C2}\n" +
                          $"• QR: {qr:C2}\n" +
                          $"• Total contado: {(efectivo + pos + qr):C2}\n" +
                          $"• Diferencia: {diferencia:C2}\n\n" +
                          $"¿Desea abrir el acta PDF?",
                          "Cierre Exitoso", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = rutaActa,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception exPdf)
            {
                MessageBox.Show($"Cierre registrado pero no se pudo generar el PDF: {exPdf.Message}", 
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            Cargar();
            nudEfectivo.Value = 0;
            nudPOS.Value = 0;
            nudQR.Value = 0;
            txtNotas.Clear();
            lblSistema.Text = "";
            lblDiferencia.Text = "";
            lblDesglose.Text = "";
        }
    }
}
