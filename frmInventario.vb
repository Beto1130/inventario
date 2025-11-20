Private clsDatos As New ClsDatos()

Private Sub frmInventario_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    CargarInventario()
End Sub

Private Sub CargarInventario()
    Dim query As String = "SELECT 
                           i.id_inventario, 
                           p.nombre_producto AS Producto, 
                           a.nombre_almacen AS Almacen, 
                           i.cantidad_stock AS Stock, 
                           i.stock_minimo AS Minimo, 
                           i.fecha_actualizacion AS Ultima_Actualizacion
                           FROM Inventario i
                           INNER JOIN Producto p ON i.id_producto = p.id_producto
                           INNER JOIN Almacen a ON i.id_almacen = a.id_almacen
                           ORDER BY p.nombre_producto, a.nombre_almacen"
    dgvInventario.DataSource = clsDatos.ObtenerDatos(query)
    
    ' Ocultar el ID que se usa internamente
    If dgvInventario.Columns.Contains("id_inventario") Then
        dgvInventario.Columns("id_inventario").Visible = False
    End If
End Sub

Private Sub dgvInventario_SelectionChanged(sender As Object, e As EventArgs) Handles dgvInventario.SelectionChanged
    ' Limpiar la caja de texto al seleccionar una nueva fila
    txtNuevaCantidad.Clear()
End Sub

Private Sub btnActualizarStock_Click(sender As Object, e As EventArgs) Handles btnActualizarStock.Click
    If dgvInventario.SelectedRows.Count = 0 Then
        MessageBox.Show("Seleccione la fila de un producto para actualizar.", "Error")
        Return
    End If
    
    If Not Integer.TryParse(txtNuevaCantidad.Text, Nothing) OrElse CInt(txtNuevaCantidad.Text) < 0 Then
        MessageBox.Show("Ingrese una cantidad de stock válida (entero positivo).", "Error")
        Return
    End If

    Dim idInventario As Integer = CInt(dgvInventario.SelectedRows(0).Cells("id_inventario").Value)
    Dim nombreProducto As String = CStr(dgvInventario.SelectedRows(0).Cells("Producto").Value)
    Dim nuevaCantidad As Integer = CInt(txtNuevaCantidad.Text)

    If MessageBox.Show($"¿Desea actualizar el stock de {nombreProducto} a {nuevaCantidad}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
        If clsDatos.ActualizarStock(idInventario, nuevaCantidad) Then
            MessageBox.Show("Stock actualizado correctamente.", "Éxito")
            CargarInventario() 
            txtNuevaCantidad.Clear()
        End If
    End If
End Sub
