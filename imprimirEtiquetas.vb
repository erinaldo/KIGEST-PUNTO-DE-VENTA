﻿
Imports System.Drawing.Printing

Public Class imprimirEtiquetas
    Dim ProPref As String
    Dim ProId As String
    Dim ProEtiquetaCod As String
    Dim ProNomb As String
    Dim ProCant As String
    Dim ProPrecio As String


    Private Sub cargarCategoriasProd()
        Try
            Reconectar()
            conexionPrinc.ChangeDatabase(database)

            'cargamos categorias
            Dim tablacatprod As New MySql.Data.MySqlClient.MySqlDataAdapter("select * from fact_categoria_insum order by nombre asc", conexionPrinc)
            Dim readcat As New DataSet
            Dim readcat2 As New DataSet
            tablacatprod.Fill(readcat)
            tablacatprod.Fill(readcat2)
            cmbCategoria.DataSource = readcat.Tables(0)
            cmbCategoria.DisplayMember = readcat.Tables(0).Columns(1).Caption.ToString.ToUpper
            cmbCategoria.ValueMember = readcat.Tables(0).Columns(0).Caption.ToString
            cmbCategoria.SelectedValue = My.Settings.idCatDef
            cargarProductos()
        Catch ex As Exception

        End Try
    End Sub
    'cargamos listas
    Private Sub CargarListas()
        Dim tablalistas As New MySql.Data.MySqlClient.MySqlDataAdapter("select id, nombre from fact_listas_precio", conexionPrinc)
        Dim readlis As New DataSet
        tablalistas.Fill(readlis)
        cmblistas.DataSource = readlis.Tables(0)
        cmblistas.DisplayMember = readlis.Tables(0).Columns(1).Caption.ToString
        cmblistas.ValueMember = readlis.Tables(0).Columns(0).Caption.ToString
        cmblistas.SelectedValue = My.Settings.idListaDef
    End Sub
    Private Sub cargarProductos()
        Dim EnProgreso As New Form
        EnProgreso.ControlBox = False
        EnProgreso.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D
        EnProgreso.Size = New Point(430, 30)
        EnProgreso.StartPosition = FormStartPosition.CenterScreen
        EnProgreso.TopMost = True
        Dim Etiqueta As New Label
        Etiqueta.AutoSize = True
        Etiqueta.Text = "La consulta esta en progreso, esto puede tardar unos momentos, por favor espere ..."
        Etiqueta.Location = New Point(5, 5)
        EnProgreso.Controls.Add(Etiqueta)
        'Dim Barra As New ProgressBar
        'Barra.Style = ProgressBarStyle.Marquee
        'Barra.Size = New Point(270, 40)
        'Barra.Location = New Point(10, 30)
        'Barra.Value = 100
        'EnProgreso.Controls.Add(Barra)
        EnProgreso.Show()
        Application.DoEvents()
        Dim categoria As Integer = cmbCategoria.SelectedValue
        Try
            Reconectar()
            conexionPrinc.ChangeDatabase(database)


            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT pro.id as CodInterno, pro.descripcion as Descripcion, pro.codigo as PLU                
            from fact_insumos as pro, fact_categoria_insum as cat where pro.categoria=cat.id and pro.categoria in (" & cmbCategoria.SelectedValue & ")", conexionPrinc)
            Dim tablaprod As New DataTable
            Dim filasProd() As DataRow
            consulta.Fill(tablaprod)
            filasProd = tablaprod.Select("")
            Dim Botonprod(tablaprod.Rows.Count - 1) As Button
            pnProductos.Controls.Clear()
            For i = 0 To tablaprod.Rows.Count - 1
                Botonprod(i) = New Button
                Botonprod(i).Text = filasProd(i)(1)
                Botonprod(i).Height = 110
                Botonprod(i).Dock = DockStyle.Top
                Botonprod(i).Tag = filasProd(i)(2)
                Botonprod(i).FlatStyle = FlatStyle.Flat
                Botonprod(i).Font = New Font(Botonprod(i).Font.Name, 15)
                Botonprod(i).ForeColor = Color.White
                Botonprod(i).BackColor = Color.FromArgb(64, 64, 64)
                Botonprod(i).Name = filasProd(i)(1)
                pnProductos.Controls.Add(Botonprod(i))
                AddHandler Botonprod(i).Click, AddressOf ControlProductos
            Next

            EnProgreso.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            EnProgreso.Close()
        End Try
    End Sub


    Private Sub CargarControlCantidades()
        Dim unid As String = My.Settings.UnidDef

        pnCantidades.Controls.Clear()
        Dim botonCant(8) As Button
        botonCant(0) = New Button
        botonCant(0).Text = "0,25 " & unid
        botonCant(0).Width = 110
        botonCant(0).Dock = DockStyle.Left
        botonCant(0).Tag = "0,25"
        botonCant(0).FlatStyle = FlatStyle.Flat
        botonCant(0).Font = New Font(botonCant(0).Font.Name, 12)
        botonCant(0).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(0))

        botonCant(1) = New Button
        botonCant(1).Text = "0,5 " & unid
        botonCant(1).Width = 110
        botonCant(1).Dock = DockStyle.Left
        botonCant(1).Tag = "0,5"
        botonCant(1).FlatStyle = FlatStyle.Flat
        botonCant(1).Font = New Font(botonCant(1).Font.Name, 12)
        botonCant(1).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(1))

        botonCant(2) = New Button
        botonCant(2).Text = "1 " & unid
        botonCant(2).Width = 110
        botonCant(2).Dock = DockStyle.Left
        botonCant(2).Tag = "1"
        botonCant(2).FlatStyle = FlatStyle.Flat
        botonCant(2).Font = New Font(botonCant(2).Font.Name, 12)
        botonCant(2).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(2))

        botonCant(3) = New Button
        botonCant(3).Text = "1,5 " & unid
        botonCant(3).Width = 110
        botonCant(3).Dock = DockStyle.Left
        botonCant(3).Tag = "1,5"
        botonCant(3).FlatStyle = FlatStyle.Flat
        botonCant(3).Font = New Font(botonCant(3).Font.Name, 12)
        botonCant(3).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(3))



        botonCant(4) = New Button
        botonCant(4).Text = "2,25 " & unid
        botonCant(4).Width = 110
        botonCant(4).Dock = DockStyle.Left
        botonCant(4).Tag = "2,25"
        botonCant(4).FlatStyle = FlatStyle.Flat
        botonCant(4).Font = New Font(botonCant(4).Font.Name, 12)
        botonCant(4).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(4))

        botonCant(5) = New Button
        botonCant(5).Text = "3 " & unid
        botonCant(5).Width = 110
        botonCant(5).Dock = DockStyle.Left
        botonCant(5).Tag = "3"
        botonCant(5).FlatStyle = FlatStyle.Flat
        botonCant(5).Font = New Font(botonCant(5).Font.Name, 12)
        botonCant(5).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(5))

        botonCant(6) = New Button
        botonCant(6).Text = "5 " & unid
        botonCant(6).Width = 110
        botonCant(6).Dock = DockStyle.Left
        botonCant(6).Tag = "5"
        botonCant(6).FlatStyle = FlatStyle.Flat
        botonCant(6).Font = New Font(botonCant(6).Font.Name, 12)
        botonCant(6).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(6))

        botonCant(7) = New Button
        botonCant(7).Text = "10 " & unid
        botonCant(7).Width = 110
        botonCant(7).Dock = DockStyle.Left
        botonCant(7).Tag = "10"
        botonCant(7).FlatStyle = FlatStyle.Flat
        botonCant(7).Font = New Font(botonCant(7).Font.Name, 12)
        botonCant(7).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(7))

        botonCant(8) = New Button
        botonCant(8).Text = "20 " & unid
        botonCant(8).Width = 110
        botonCant(8).Dock = DockStyle.Left
        botonCant(8).Tag = "20"
        botonCant(8).FlatStyle = FlatStyle.Flat
        botonCant(8).Font = New Font(botonCant(8).Font.Name, 12)
        botonCant(8).ForeColor = Color.White
        pnCantidades.Controls.Add(botonCant(8))

        For i = 0 To 8
            AddHandler botonCant(i).Click, AddressOf ControlCantidades
        Next

    End Sub
    Private Sub cmbCategoria_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cmbCategoria.SelectionChangeCommitted
        Try
            My.Settings.idCatDef = cmbCategoria.SelectedValue
            My.Settings.Save()
            cargarProductos()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub imprimirEtiquetas_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CargarControlCantidades()
        cargarCategoriasProd()
        CargarListas()
    End Sub
    Private Sub ControlCantidades(ByVal sender As System.Object, ByVal e As System.EventArgs)

        For Each boton As Control In pnCantidades.Controls
            If TypeOf (boton) Is Button Then
                boton.BackColor = Color.FromArgb(64, 64, 64)
            End If
        Next
        pnCantidadPerson.BackColor = Color.FromArgb(64, 64, 64)

        If DirectCast(sender, Button).BackColor = Color.FromArgb(64, 64, 64) Then
            DirectCast(sender, Button).BackColor = Color.FromArgb(255, 128, 0)
            ProCant = FormatNumber(DirectCast(sender, Button).Tag, 2,)
        End If

        For Each Botonprod As Control In pnProductos.Controls
            If TypeOf (Botonprod) Is Button Then
                If Botonprod.BackColor = Color.FromArgb(255, 128, 0) Then
                    ProId = Botonprod.Tag
                    ProNomb = Botonprod.Text
                End If
            End If
        Next
        LlenarEtiqueta()
    End Sub
    Private Sub ControlProductos(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'MsgBox(DirectCast(sender, Button).Tag)
        For Each boton As Control In pnProductos.Controls
            If TypeOf (boton) Is Button Then
                boton.BackColor = Color.FromArgb(64, 64, 64)
            End If
        Next
        'pnCantidadPersonal.BackColor = Color.FromArgb(64, 64, 64)
        If DirectCast(sender, Button).BackColor = Color.FromArgb(64, 64, 64) Then
            DirectCast(sender, Button).BackColor = Color.FromArgb(255, 128, 0)
            ProId = DirectCast(sender, Button).Tag
            ProNomb = DirectCast(sender, Button).Text
        End If

        For Each BotonCant As Control In pnCantidades.Controls
            If TypeOf (BotonCant) Is Button Then
                If BotonCant.BackColor = Color.FromArgb(255, 128, 0) Then
                    ProCant = FormatNumber(BotonCant.Tag, 2)
                End If
            End If
        Next
        LlenarEtiqueta()
    End Sub

    Private Sub txtcant_GotFocus(sender As Object, e As EventArgs)
        For Each boton As Control In pnProductos.Controls
            If TypeOf (boton) Is Button Then
                boton.BackColor = Color.FromArgb(64, 64, 64)
            End If
        Next
        'pnCantidadPersonal.BackColor = Color.FromArgb(255, 128, 0)
    End Sub

    Private Function calcularPromociones(ByRef codprod As String, ByRef cant As Double) As Double
        Try
            Dim montomin As Double
            Dim porcdesc As Double = 1

            Reconectar()
            Dim consultaDescProd As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT prom.id,concat('Descuento producto ' , ins.descripcion,' ', prom.descuento_porc ,'%'),prom.compra_min,prom.descuento_porc " &
            "From fact_promociones as prom, fact_insumos as ins where ins.id=prom.idproducto and ins.codigo=" & codprod & " and prom.compra_min<= " & cant, conexionPrinc)
            Dim tablaDescProd As New DataTable
            Dim filasDescProd() As DataRow
            consultaDescProd.Fill(tablaDescProd)
            filasDescProd = tablaDescProd.Select("")

            Dim actualVal As Integer, maxVal As Integer, idPromMax As Integer
            maxVal = 0
            'MsgBox(tablaDescProd.Rows.Count)
            For i As Integer = 0 To tablaDescProd.Rows.Count - 1
                actualVal = tablaDescProd.Rows(i).Item(2)
                If actualVal > maxVal Then
                    maxVal = actualVal
                    'montomin = tablaDescProd.Rows(i).Item(2)
                    porcdesc = (CType(tablaDescProd.Rows(i).Item(3), Double) + 100) / 100
                End If
            Next

            Return porcdesc

        Catch ex As Exception
            Return 1
        End Try
    End Function

    Private Function calcularPrecio(ByRef codProd As String) As Double
        Try
            Dim ganancia As Double
            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT precio, ganancia, iva, moneda,utilidad1,utilidad2 FROM fact_insumos where codigo=" & ProId, conexionPrinc)
            Dim tablaprod As New DataTable
            Dim filasProd() As DataRow
            consulta.Fill(tablaprod)
            filasProd = tablaprod.Select("")

            'cargamos listas de precios
            Dim consultalis As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT utilidad, auxcol FROM fact_listas_precio where id=" & cmblistas.SelectedValue, conexionPrinc)
            Dim tablalistas As New DataTable
            Dim filaslistas() As DataRow
            consultalis.Fill(tablalistas)
            filaslistas = tablalistas.Select("")

            'cargamos la moneda perteneciente a este producto
            Reconectar()
            Dim lector As System.Data.IDataReader
            Dim sql As New MySql.Data.MySqlClient.MySqlCommand
            sql.Connection = conexionPrinc
            sql.CommandText = "Select (Select cotizacion from fact_moneda  where  id =" & filasProd(0)(3) & ") As cotiza, (Select valor from fact_configuraciones where  id =1) As lista"
            sql.CommandType = CommandType.Text
            lector = sql.ExecuteReader
            lector.Read()

            Dim cotizacion As Double = FormatNumber(lector("cotiza").ToString)
            Dim precioCosto As Double = FormatNumber(filasProd(0)(0))
            Dim iva As Double = (FormatNumber(filasProd(0)(2)) + 100) / 100
            Dim listaTXT As String = filaslistas(0)(0)
            Dim lista As Double
            Dim codaux As Integer = filaslistas(0)(1)
            Dim utilidad As Double
            Select Case codaux
                Case 0
                    utilidad = (FormatNumber(filasProd(0)(1)) + 100) / 100
                Case 1
                    utilidad = (FormatNumber(filasProd(0)(4)) + 100) / 100
                Case 2
                    utilidad = (FormatNumber(filasProd(0)(5)) + 100) / 100
            End Select

            Dim PrecioSinIva As Double
            Dim PrecioVenta As Double

            If InStr(listaTXT, "%") <> 0 Then
                lista = (FormatNumber(listaTXT.Replace("%", "") + 100) / 100)
                PrecioSinIva = precioCosto * cotizacion * lista
                PrecioVenta = PrecioSinIva * iva
            Else
                lista = (FormatNumber(filaslistas(0)(0) + 100) / 100)
                PrecioSinIva = precioCosto * cotizacion * utilidad * lista
                PrecioVenta = PrecioSinIva * iva
            End If
            Return Math.Round(PrecioVenta, 2)

        Catch ex As Exception
            Return 0
        End Try
    End Function

    Private Sub LlenarEtiqueta()
        For Each boton As Control In pnProductos.Controls
            If TypeOf (boton) Is Button Then
                If boton.BackColor = Color.FromArgb(255, 128, 0) Then
                    ProId = boton.Tag
                    ProNomb = boton.Text
                End If
            End If
        Next

        For Each boton As Control In pnCantidades.Controls
            If TypeOf (boton) Is Button Then
                If boton.BackColor = Color.FromArgb(255, 128, 0) Then
                    ProCant = boton.Tag
                End If
            End If
        Next


        Dim promocion As Double = calcularPromociones(ProId, ProCant)

        Dim precioUnit As Double = calcularPrecio(ProId) / promocion
        lblprecios.Text = "$" & Math.Round(ProCant * precioUnit, 2)
        ProPrecio = Math.Round(ProCant * precioUnit, 2)
        ProEtiquetaCod = "00" & ProId & ObtenerSiguienteCodigo()

    End Sub
    Private Sub ImprimirBoleta(ByVal sender As System.Object, ByVal e As PrintPageEventArgs)

        If My.Settings.TipoEtiqueta = 0 Then
            ' letra
            'Dim font1 As New Font("EAN-13", 40)
            Dim font2 As New Font("Arial", 8)
            Dim font3 As New Font("Arial", 8)
            Dim font4 As New Font("Arial", 18)
            Dim font5 As New Font("Arial", 6)

            Dim alto As Single = 0
            'If TextBox1.Text <> "" Then
            '    alto = Convert.ToSingle(TextBox1.Text)
            'End If
            Dim bm As Bitmap = Nothing
            bm = Codigos.codigo128("A" & ProEtiquetaCod & "B", True, 40)
            If Not IsNothing(bm) Then
                PictureBox1.Image = bm
            End If

            If GuardarProducto() = True Then
                ' impresion
                e.Graphics.DrawImage(PictureBox1.Image, 0, 0)
                'e.Graphics.DrawString(ProEtiquetaCod, font1, Brushes.Black, 0, 0) 'CODIGO DE BARRAS
                'e.Graphics.DrawString("*" & Me.ProId.Trim & "*", font2, Brushes.Black, 50, 47) ' CODIGO NUMERICO
                e.Graphics.DrawString(ProNomb, font2, Brushes.Black, 0, 60) 'PRODUCTO
                e.Graphics.DrawString("x " & ProCant & My.Settings.UnidDef, font3, Brushes.Black, 0, 70) 'CANTIDAD
                e.Graphics.DrawString(ProPrecio, font4, Brushes.Black, 60, 70) 'PRECIO
                e.Graphics.DrawImage(Image.FromFile(Application.StartupPath & "\logo2.jpg"), 25, 100)
                e.Graphics.DrawString("Fecha: " & Format(Now, "dd-MM-yyy HH:mm:ss"), font5, Brushes.Black, 0, 140)
                LlenarEtiqueta()
            Else
                MsgBox("No se pudo guardar el producto en la base de datos, la etiqueta no se puede imprimir")
            End If

        ElseIf My.Settings.TipoEtiqueta = 1 Then

            ' letra
            'Dim font1 As New Font("EAN-13", 40)
            Dim font2 As New Font("Arial", 7)
            Dim font3 As New Font("Arial", 7)
            Dim font4 As New Font("Arial", 14)
            Dim font5 As New Font("Arial", 10)

            Dim alto As Single = 0
            'If TextBox1.Text <> "" Then
            '    alto = Convert.ToSingle(TextBox1.Text)
            'End If
            Dim bm As Bitmap = Nothing
            bm = Codigos.codigo128("A" & ProEtiquetaCod & "B", True, 25)
            If Not IsNothing(bm) Then
                PictureBox1.Image = bm
            End If

            If GuardarProducto() = True Then
                ' impresion
                'e.Graphics.DrawImage(PictureBox1.Image, 10, 0)

                e.Graphics.DrawString("VITA SANA", font5, Brushes.Black, 50, 0) 'titulo
                e.Graphics.DrawString(ProNomb, font2, Brushes.Black, 15, 18) 'PRODUCTO

                e.Graphics.DrawImage(PictureBox1.Image, 20, 30)

                e.Graphics.DrawString("x " & ProCant & My.Settings.UnidDef, font3, Brushes.Black, 5, 75) 'CANTIDAD
                e.Graphics.DrawString(ProPrecio, font4, Brushes.Black, 90, 75) 'PRECIO
                'e.Graphics.DrawImage(Image.FromFile(Application.StartupPath & "\logo2.jpg"), 25, 100)
                'e.Graphics.DrawString("Fecha: " & Format(Now, "dd-MM-yyy HH:mm:ss"), font5, Brushes.Black, 0, 67)
                LlenarEtiqueta()
            Else
                MsgBox("No se pudo guardar el producto en la base de datos, la etiqueta no se puede imprimir")
            End If

        End If
    End Sub


    Private Function ObtenerSiguienteCodigo() As String
        Try
            Reconectar()
            Dim lector As System.Data.IDataReader
            Dim sql As New MySql.Data.MySqlClient.MySqlCommand
            Dim ultimo As String
            sql.Connection = conexionPrinc
            sql.CommandText = "select max(id) as ultimo from fact_insumos_produccion"
            sql.CommandType = CommandType.Text
            lector = sql.ExecuteReader
            lector.Read()
            ultimo = lector("ultimo").ToString

            Dim siguiente As Integer
            If ultimo = "" Then
                siguiente = 1
            Else
                siguiente = CType(ultimo, Integer) + 1
            End If
            Return String.Format("{0:00000}", siguiente)
            'MsgBox(siguiente)
        Catch ex As Exception
            MsgBox(ex.Message)
            'Return 0
        End Try
    End Function

    Private Function GuardarProducto() As Boolean
        Try
            Reconectar()
            Dim fecha As String = Format(CDate(Now), "yyyy-MM-dd")
            Dim sqlQuery As String = "INSERT INTO fact_insumos_produccion(codigobarras,codigo_producto,producto,cantidad,precio,fecha_alta) values(
            ?codbarr,?codprod,?prod,?cant,?precio,?fecha)"
            Dim comandoadd As New MySql.Data.MySqlClient.MySqlCommand(sqlQuery, conexionPrinc)
            With comandoadd.Parameters
                .AddWithValue("?codbarr", ProEtiquetaCod)
                .AddWithValue("?codprod", ProId)
                .AddWithValue("?prod", ProNomb)
                .AddWithValue("?cant", FormatNumber(ProCant))
                .AddWithValue("?precio", FormatNumber(ProPrecio))
                .AddWithValue("?fecha", fecha)
            End With
            comandoadd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ProId <> "" And ProCant <> "" Then
            ProPrecio = lblprecios.Text
            ' documento
            Dim PrintTxt As New PrintDocument
            Dim pgSize As New PaperSize

            pgSize.RawKind = Printing.PaperKind.Custom
            If My.Settings.TipoEtiqueta = 0 Then
                pgSize.Width = 180 '196.8 '
                pgSize.Height = 173.23 '100
            ElseIf My.Settings.TipoEtiqueta = 1 Then
                pgSize.Width = 196.8 '  135 '180 '196.8 '
                pgSize.Height = 118  '78 '173.23 '100

            End If

            PrintTxt.DefaultPageSettings.PaperSize = pgSize
            ' evento print
            AddHandler PrintTxt.PrintPage, AddressOf ImprimirBoleta
            PrintTxt.PrinterSettings.PrinterName = My.Settings.EtiquetadoraNmb
            PrintTxt.Print()
        End If

    End Sub

    Private Sub txtbuscarprod_TextChanged(sender As Object, e As EventArgs) Handles txtbuscarprod.TextChanged
        Dim busq As String = DirectCast(sender, TextBox).Text.ToUpper
        Dim prod As String
        For Each producto As Control In pnProductos.Controls
            prod = producto.Text.ToUpper
            'MsgBox(busq & " - " & prod & "=" & InStr(prod, busq))
            If TypeOf (producto) Is Button And InStr(prod, busq) = 0 Then
                producto.Visible = False
            Else
                producto.Visible = True
            End If
        Next
    End Sub

    Private Sub txtbuscarprod_GotFocus(sender As Object, e As EventArgs) Handles txtbuscarprod.GotFocus
        DirectCast(sender, TextBox).Text = ""
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            pnProductos.VerticalScroll.Value -= 100
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            pnProductos.VerticalScroll.Value += 100
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            If Button2.Focused Then
                pnProductos.VerticalScroll.Value -= 100
            ElseIf Button3.Focused Then
                pnProductos.VerticalScroll.Value += 100
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button2_MouseUp(sender As Object, e As MouseEventArgs) Handles Button2.MouseUp
        Timer1.Enabled = False
    End Sub

    Private Sub Button2_MouseDown(sender As Object, e As MouseEventArgs) Handles Button2.MouseDown
        Timer1.Enabled = True
    End Sub

    Private Sub Button3_MouseDown(sender As Object, e As MouseEventArgs) Handles Button3.MouseDown
        Timer1.Enabled = True
    End Sub

    Private Sub Button3_MouseUp(sender As Object, e As MouseEventArgs) Handles Button3.MouseUp
        Timer1.Enabled = False
    End Sub
    Private Sub imprimirEtiquetas_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = "."c Then
            e.Handled = True
            SendKeys.Send(",")
        End If
    End Sub

    Private Sub txtcantperson_TextChanged(sender As Object, e As EventArgs) Handles txtcantperson.TextChanged

        If IsNumeric(DirectCast(sender, TextBox).Text) Then
            ProCant = FormatNumber(DirectCast(sender, TextBox).Text, 2)
            LlenarEtiqueta()
        End If
    End Sub

    Private Sub txtcantperson_GotFocus(sender As Object, e As EventArgs) Handles txtcantperson.GotFocus
        For Each boton As Control In pnCantidades.Controls
            If TypeOf (boton) Is Button Then
                boton.BackColor = Color.FromArgb(64, 64, 64)
            End If
        Next
        pnCantidadPerson.BackColor = Color.FromArgb(255, 128, 0)

        If IsNumeric(DirectCast(sender, TextBox).Text) Then
            ProCant = FormatNumber(DirectCast(sender, TextBox).Text, 2)
            LlenarEtiqueta()
        End If
    End Sub

    Private Sub cmbCategoria_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCategoria.SelectedIndexChanged

    End Sub

    Private Sub cmblistas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmblistas.SelectedIndexChanged

    End Sub

    Private Sub cmblistas_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cmblistas.SelectionChangeCommitted
        My.Settings.idListaDef = cmblistas.SelectedValue
        My.Settings.Save()
    End Sub
End Class