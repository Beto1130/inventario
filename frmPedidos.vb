Private clsDatos As New ClsDatos()
Private dtDetallesVenta As New DataTable()

Private Sub frmPedidos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    InicializarGridDetalles()
    CargarCombos()
End Sub

Private Sub InicializarGridDetalles()
    dtDetallesVenta.Columns.Add("id_producto", GetType(Integer))
    dtDetallesVenta.Columns.Add("Producto", GetType(String))
    dtDetallesVenta.Columns.Add("cantidad", GetType(Integer))
    dtDetallesVenta.Columns.Add("precio_unitario", GetType(Decimal))
    
    ' Columna calculada
    Dim subtotalCol As New DataColumn("Subtotal", GetType(Decimal))
    subtotalCol.Expression = "cantidad * precio_unitario"
    dtDetallesVenta.Columns.Add(subtotalCol)

    dgvDetalles.DataSource = dtDetallesVenta
    If dgvDetalles.Columns.Contains("id_producto") Then
        dgvDetalles.Columns("id_producto").Visible = False
    End If
End Sub

Private Sub CargarCombos()
    ' CLIENTES
    cmbCliente.DataSource = clsDatos.ObtenerDatos("SELECT id_cliente, nombre_cliente FROM Cliente WHERE estado_cliente = 'Activo'")
    cmbCliente.DisplayMember = "nombre_cliente"
    cmbCliente.ValueMember = "id_cliente"
    
    ' RUTAS
    cmbRuta.DataSource = clsDatos.ObtenerDatos("SELECT id_ruta, nombre_ruta FROM RutaDistribucion WHERE estado_ruta = 'Activa'")
    cmbRuta.DisplayMember = "nombre_ruta"
    cmbRuta.ValueMember = "id_ruta"
    
    ' PRODUCTOS
    cmbProducto.DataSource = clsDatos.ObtenerDatos("SELECT id_producto, nombre_producto, precio_unitario FROM Producto WHERE estado_producto = 'Activo'")
    cmbProducto.DisplayMember = "nombre_producto"
    cmbProducto.ValueMember = "id_producto"
    
    ' Limpiar selección inicial
    cmbCliente.SelectedIndex = -1
    cmbRuta.SelectedIndex = -1
    cmbProducto.SelectedIndex = -1
End Sub

Private Sub cmbProducto_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbProducto.SelectedIndexChanged
    If cmbProducto.SelectedValue IsNot Nothing Then
        ' Obtener la fila del producto seleccionado
        Dim dtProductos = CType(cmbProducto.DataSource, DataTable)
        Dim idProducto As Integer = CInt(cmbProducto.SelectedValue)
        
        Dim selectedRow = dtProductos.AsEnumerable().FirstOrDefault(Function(row) row.Field(Of Integer)("id_producto") = idProducto)
        
        If selectedRow IsNot Nothing Then
            txtPrecio.Text = selectedRow.Field(Of Decimal)("precio_unitario").ToString("N2")
        End If
    End If
End Sub

Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
    ' Validaciones
    If cmbProducto.SelectedValue Is Nothing OrElse Not Integer.TryParse(txtCantidad.Text, Nothing) OrElse Not Decimal.TryParse(txtPrecio.Text, Nothing) Then
        MessageBox.Show("Seleccione producto e ingrese cantidad/precio válidos.", "Error")
        Return
    End If

    Dim idProducto = CInt(cmbProducto.SelectedValue)
    Dim cantidad = CInt(txtCantidad.Text)
    Dim precio = CDec(txtPrecio.Text)
    
    If cantidad <= 0 Then
        MessageBox.Show("La cantidad debe ser mayor a cero.", "Error")
        Return
    End If

    ' Crear la fila de detalle
    Dim newRow = dtDetallesVenta.NewRow()
    newRow("id_producto") = idProducto
    newRow("Producto") = cmbProducto.Text
    newRow("cantidad") = cantidad
    newRow("precio_unitario") = precio
    
    dtDetallesVenta.Rows.Add(newRow)
    ActualizarTotalPedido()
    
    ' Limpiar campos de detalle para el siguiente producto
    txtCantidad.Clear()
    txtPrecio.Clear()
    cmbProducto.SelectedIndex = -1
End Sub

Private Sub ActualizarTotalPedido()
    If dtDetallesVenta.Rows.Count > 0 Then
        ' Usa Linq para sumar la columna "Subtotal"
        Dim total As Decimal = dtDetallesVenta.AsEnumerable().Sum(Function(row) row.Field(Of Decimal)("Subtotal"))
        lblTotalPedido.Text = total.ToString("C2")
    Else
        lblTotalPedido.Text = "S/ 0.00"
    End If
End Sub

Private Sub btnConfirmarVenta_Click(sender As Object, e As EventArgs) Handles btnConfirmarVenta.Click
    If cmbCliente.SelectedValue Is Nothing OrElse cmbRuta.SelectedValue Is Nothing OrElse dtDetallesVenta.Rows.Count = 0 Then
        MessageBox.Show("Complete el encabezado del pedido y agregue productos.", "Error")
        Return
    End If

    Dim idCliente As Integer = CInt(cmbCliente.SelectedValue)
    Dim idRuta As Integer = CInt(cmbRuta.SelectedValue)
    Dim totalPedido As Decimal = dtDetallesVenta.AsEnumerable().Sum(Function(row) row.Field(Of Decimal)("Subtotal"))
    
    ' Clonar y preparar la tabla para el Stored Procedure
    Dim detallesClonados As DataTable = dtDetallesVenta.Copy()
    ' Eliminar las columnas que NO existen en DetallePedidoType
    detallesClonados.Columns.Remove("Producto")
    detallesClonados.Columns.Remove("Subtotal")

    Dim nuevoID = clsDatos.CrearPedido(idCliente, idRuta, totalPedido, detallesClonados)

    If nuevoID > 0 Then
        MessageBox.Show($"¡Venta exitosa! Pedido ID: {nuevoID}", "Éxito")
        ' Limpiar
        dtDetallesVenta.Clear()
        ActualizarTotalPedido()
        CargarCombos()
    Else
        ' El error ya fue manejado en ClsDatos
        ' Si devuelve -1, algo falló.
    End If
End Sub
