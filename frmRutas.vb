Private clsDatos As New ClsDatos()

Private Sub frmRutas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    CargarCombos()
End Sub

Private Sub CargarCombos()
    ' RUTAS ACTIVAS
    cmbRuta.DataSource = clsDatos.ObtenerDatos("SELECT id_ruta, nombre_ruta FROM RutaDistribucion WHERE estado_ruta = 'Activa'")
    cmbRuta.DisplayMember = "nombre_ruta"
    cmbRuta.ValueMember = "id_ruta"
    
    ' CONDUCTORES (Empleados con licencia)
    cmbConductor.DataSource = clsDatos.ObtenerDatos("SELECT id_empleado, nombre_empleado FROM Empleado WHERE licencia_conducir IS NOT NULL AND estado_empleado = 'Activo'")
    cmbConductor.DisplayMember = "nombre_empleado"
    cmbConductor.ValueMember = "id_empleado"
    
    ' VEHÍCULOS (Transporte activo)
    cmbTransporte.DataSource = clsDatos.ObtenerDatos("SELECT id_transporte, placa_transporte FROM Transporte WHERE estado_transporte = 'Activo'")
    cmbTransporte.DisplayMember = "placa_transporte"
    cmbTransporte.ValueMember = "id_transporte"
    
    ' Limpiar selección inicial
    cmbRuta.SelectedIndex = -1
    cmbConductor.SelectedIndex = -1
    cmbTransporte.SelectedIndex = -1
End Sub

Private Sub btnAsignar_Click(sender As Object, e As EventArgs) Handles btnAsignar.Click
    If cmbRuta.SelectedValue Is Nothing OrElse cmbConductor.SelectedValue Is Nothing OrElse cmbTransporte.SelectedValue Is Nothing Then
        MessageBox.Show("Debe seleccionar la Ruta, el Conductor y el Vehículo para asignar.", "Error")
        Return
    End If

    Dim idRuta As Integer = CInt(cmbRuta.SelectedValue)
    Dim idConductor As Integer = CInt(cmbConductor.SelectedValue)
    Dim idTransporte As Integer = CInt(cmbTransporte.SelectedValue)

    If MessageBox.Show($"¿Confirmar la asignación para la ruta {cmbRuta.Text}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
        If clsDatos.AsignarRuta(idRuta, idConductor, idTransporte) Then
            MessageBox.Show("Asignación de ruta y vehículo actualizada correctamente.", "Éxito")
            CargarCombos() ' Opcional: recargar para reflejar cambios
        Else
            ' El error ya fue manejado en ClsDatos
        End If
    End If
End Sub
