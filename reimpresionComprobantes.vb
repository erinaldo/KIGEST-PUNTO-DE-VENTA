﻿Imports System.Drawing.Printing
Imports System.Security
Public Class reimpresionComprobantes
    Dim IdFactura As Integer
    Private Sub cmdbuscar_Click_1(sender As Object, e As EventArgs) Handles cmdbuscar.Click
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
        Dim columna As Integer
        Dim limite As Integer = Val(TextBox1.Text)
        Try

            Reconectar()
            Dim tablaprod As New DataTable
            'Dim filasProd() As DataRow

            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT 
            fact.id, concat(fis.abrev,' ',lpad(fact.ptovta,4,'0'),'-',lpad(fact.num_fact,8,'0')) as factnum ,fact.fecha,fact.razon,fact.direccion, 
            fact.localidad, con.condicion, 
            case when fis.debcred='C' then 
            concat('-',FORMAT(fact.total,2,'es_AR')) 
            else FORMAT(fact.total,2,'es_AR') end as total, fact.observaciones2, fact.tipofact,fact.ptovta 
            from fact_conffiscal as fis, fact_facturas as fact, fact_condventas as con 
            where fis.donfdesc=fact.tipofact and con.id=fact.condvta and fis.ptovta=fact.ptovta
            order by fact.id desc limit " & limite, conexionPrinc)
            columna = 7
            consulta.Fill(tablaprod)
            Dim i As Integer


            dtfacturas.DataSource = tablaprod
            dtfacturas.Columns(0).Visible = False
            dtfacturas.Columns(9).Visible = False



            EnProgreso.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            EnProgreso.Close()
        End Try
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            'buscamos el punto de venta el que pertenece el comprobante
            Dim ptovta As Integer = dtfacturas.CurrentRow.Cells(10).Value
            IdFactura = dtfacturas.CurrentRow.Cells(0).Value
            'Dim tabIVComp As New MySql.Data.MySqlClient.MySqlDataAdapter

            If My.Settings.ImprTikets = 1 And chkImprimirA4.CheckState = CheckState.Unchecked Then
                Dim PrintTxt As New PrintDocument
                Dim pgSize As New PaperSize
                pgSize.RawKind = Printing.PaperKind.Custom
                pgSize.Width = 147 '196.8 '
                'pgSize.Height = 173.23 '100
                PrintTxt.DefaultPageSettings.PaperSize = pgSize
                ' evento print

                If ptovta <> FacturaElectro.puntovtaelect Then
                    AddHandler PrintTxt.PrintPage, AddressOf ImprimirTiketVenta
                    PrintTxt.PrinterSettings.PrinterName = My.Settings.ImprTiketsNombre
                    PrintTxt.Print()
                Else
                    AddHandler PrintTxt.PrintPage, AddressOf ImprimirTiketFiscal
                    PrintTxt.PrinterSettings.PrinterName = My.Settings.ImprTiketsNombre
                    PrintTxt.Print()
                End If

            Else
                Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
                Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
                Dim fac As New datosfacturas

                Reconectar()

                tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  
            emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, 
            emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, 
            concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, 
            concat(fac.id_cliente,'-',fac.razon,' - tel: ',cl.telefono) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, 
            concat(vend.apellido,', ',vend.nombre) as facvend, condvent.condicion as faccondvta, fac.observaciones2 as facobserva,format(fac.iva105,2,'es_AR') as iva105, format(fac.iva21,2,'es_AR') as iva21,
            '','',fis.donfdesc, fac.cae, fis.letra as facletra, fis.codfiscal as faccodigo, fac.vtocae, fac.codbarra,format(fac.total,2,'es_AR')  
            FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac,fact_condventas as condvent  
            where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and fis.ptovta=fac.ptovta and condvent.id=fac.condvta and fac.id=" & IdFactura, conexionPrinc)

                tabEmp.Fill(fac.Tables("factura_enca"))
                Reconectar()

                tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select 
            plu,
            format(replace(cantidad,',','.'),2,'es_AR') as cant, descripcion, 
            format(replace(iva,',','.'),2,'es_AR') as iva ,
            format(replace(punit,',','.'),2,'es_AR') as punit ,
            format(replace(ptotal,',','.'),2,'es_AR') as ptotal 
            from fact_items where id_fact=" & IdFactura, conexionPrinc)

                tabFac.Fill(fac.Tables("facturax"))

                Dim imprimirx As New imprimirFX
                With imprimirx
                    .MdiParent = Me.MdiParent
                    .rptfx.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
                    'If ptovta <> FacturaElectro.puntovtaelect Then

                    Select Case dtfacturas.CurrentRow.Cells(9).Value
                        Case 1 To 3, 6 To 8, 11 To 13
                            .rptfx.LocalReport.ReportPath = System.Environment.CurrentDirectory & "\reportes\facturaelectro.rdlc"
                        Case Else
                            .rptfx.LocalReport.ReportPath = System.Environment.CurrentDirectory & "\reportes\facturax.rdlc"

                    End Select

                    'End If

                    .rptfx.LocalReport.DataSources.Clear()
                    .rptfx.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("encabezado", fac.Tables("factura_enca")))
                    .rptfx.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("items", fac.Tables("facturax")))
                    .rptfx.DocumentMapCollapsed = True
                    .rptfx.RefreshReport()
                    .Show()
                End With
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ImprimirTiketFiscal(ByVal sender As System.Object, ByVal e As PrintPageEventArgs)
        ' letra
        'Dim font1 As New Font("EAN-13", 40)
        Dim printfont As New Font("Courier New", 6)
        Dim font3 As New Font("Courier New", 8)
        Dim font4 As New Font("Courier New", 18)
        Dim font5 As New Font("Courier New", 6)
        Dim fontCAE As New Font("Courier New", 5.3, FontStyle.Italic)

        Dim alto As Single = 0
        Dim topMargin As Double '= e.MarginBounds.Top
        Dim yPos As Double = 0
        Dim count As Integer = 0
        Dim texto As String = ""

        Dim codigo As String = ""
        Dim unidad As String = ""
        Dim detalle As String = ""
        Dim valoruni As String = ""
        Dim valortot As String = ""
        Dim tabulacion As String = ""
        Dim compensador As Integer = 0
        Dim total As String = ""
        Dim lvalor As String
        Dim lineatotal As String
        Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim ivaProd As String = ""
        Dim fac As New datosfacturas

        Dim facTotal As String = ""
        Dim facSubtotal As String = ""
        Dim FacIva21 As String = ""
        Dim FacIva105 As String = ""

        Dim facCAE As String = ""
        Dim facVtoCAE As String = ""
        Dim facCodBARRA As String = ""



        Reconectar()

        tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  
        emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, 
        emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, 
        concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, 
        concat(fac.id_cliente,'-',fac.razon) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, 
        concat(vend.apellido,', ',vend.nombre) as facvend, condvent.condicion as faccondvta, fac.observaciones2 as facobserva,format(fac.iva105,2,'es_AR') as iva105, format(fac.iva21,2,'es_AR') as iva21,
        '','',fis.donfdesc, fac.cae, fis.letra as facletra, fis.codfiscal as faccodigo, fac.vtocae, fac.codbarra, format(fac.total,2,'es_AR'),format(fac.subtotal,2,'es_AR')   
        FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac,fact_condventas as condvent  
        where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and condvent.id=fac.condvta and fac.ptovta=fis.ptovta and fac.id=" & IdFactura, conexionPrinc)

        Dim tablaEmpresa As New DataTable
        tabEmp.Fill(tablaEmpresa)

        Reconectar()

        tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select 
            plu,
            format(replace(cantidad,',','.'),2,'es_AR') as cant, descripcion, 
            format(replace(iva,',','.'),2,'es_AR') as iva ,
            format(replace(punit,',','.'),2,'es_AR') as punit ,
            format(replace(ptotal,',','.'),2,'es_AR') as ptotal 
            from fact_items where id_fact=" & IdFactura, conexionPrinc)
        Dim tablaProd As New DataTable
        tabFac.Fill(tablaProd)

        facTotal = tablaEmpresa.Rows(0).Item(30)
        facSubtotal = tablaEmpresa.Rows(0).Item(31)
        FacIva21 = tablaEmpresa.Rows(0).Item(21)
        FacIva105 = ""

        facCAE = tablaEmpresa.Rows(0).Item(25)
        facCodBARRA = tablaEmpresa.Rows(0).Item(29)
        facVtoCAE = tablaEmpresa.Rows(0).Item(28)
        Dim TipoFact As Integer = tablaEmpresa.Rows(0).Item(24)

        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(1), font5, Brushes.Black, 0, 100) 'RAZON SOCIAL
        e.Graphics.DrawString("CUIT Nro: " & tablaEmpresa.Rows(0).Item(4), font5, Brushes.Black, 0, 110) '
        e.Graphics.DrawString("Ing. Brutos: " & tablaEmpresa.Rows(0).Item(5).ToString, font5, Brushes.Black, 0, 120) '
        e.Graphics.DrawString("Domicilio: " & tablaEmpresa.Rows(0).Item(2), font5, Brushes.Black, 0, 130)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(3), font5, Brushes.Black, 0, 140) '
        e.Graphics.DrawString("Inicio de actividades: " & tablaEmpresa.Rows(0).Item(7), font5, Brushes.Black, 0, 150)
        e.Graphics.DrawString("IVA " & tablaEmpresa.Rows(0).Item(6), font5, Brushes.Black, 0, 160)

        e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 170)
        e.Graphics.DrawString("FACTURA '" & tablaEmpresa.Rows(0).Item(26) & "' (" & tablaEmpresa.Rows(0).Item(27) & ")", font5, Brushes.Black, 0, 180)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(10).ToString, font5, Brushes.Black, 0, 190)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(11).ToString, font5, Brushes.Black, 0, 200)
        e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 210)

        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(12), font5, Brushes.Black, 0, 220)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(13), font5, Brushes.Black, 0, 230)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(14), font5, Brushes.Black, 0, 240)
        e.Graphics.DrawString("CUIT Nro: " & tablaEmpresa.Rows(0).Item(16), font5, Brushes.Black, 0, 250)
        e.Graphics.DrawString("IVA " & tablaEmpresa.Rows(0).Item(15), font5, Brushes.Black, 0, 260)
        e.Graphics.DrawString("CONDICION DE VENTA " & tablaEmpresa.Rows(0).Item(18), font5, Brushes.Black, 0, 270)
        e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 280)

        Dim i As Integer
        Dim j As Integer
        Dim car As Integer

        For i = 0 To tablaProd.Rows.Count - 1
            codigo = tablaProd.Rows(i).Item(0)
            unidad = tablaProd(i).Item(1)
            detalle = tablaProd(i).Item(2)
            valoruni = tablaProd(i).Item(4)
            valortot = FormatNumber(tablaProd(i).Item(5), 2)
            ivaProd = tablaProd(i).Item(3)
            texto = unidad & " x " & valoruni & Chr(9) & "  (" & ivaProd & ")"
            yPos = 290 + topMargin + (count * printfont.GetHeight(e.Graphics)) ' Calcula la posición en la que se escribe la línea            


            If detalle.Length <= 25 Then
                car = 25 - detalle.Length
                For j = 0 To car
                    detalle &= " "
                Next
            Else
                car = detalle.Length - 25
                detalle = detalle.Remove(26, car - 1)
            End If

            If valortot.Length <= 7 Then
                car = 7 - valortot.Length
                For j = 0 To car
                    valortot = " " & valortot
                Next

            End If


            'If Not row.IsNewRow Then
            e.Graphics.DrawString(texto, printfont, System.Drawing.Brushes.Black, 0, yPos)
            count += 1
            yPos = yPos + 10
            e.Graphics.DrawString(detalle & "  " & valortot, printfont, System.Drawing.Brushes.Black, 0, yPos)
            'total += valor
            'End If
            count += 1

        Next
        If FacIva21.Length <= 7 Then
            car = 7 - FacIva21.Length
            For j = 0 To car
                FacIva21 = " " & FacIva21
            Next

        End If


        If facSubtotal.Length <= 7 Then
            car = 7 - facSubtotal.Length
            For j = 0 To car
                facSubtotal = " " & facSubtotal
            Next
        End If

        If facTotal.Length <= 7 Then
            car = 7 - facTotal.Length
            For j = 0 To car
                facTotal = " " & facTotal
            Next
        End If




        yPos += 20
        Dim textosub As String = "Subtotal"
        Dim textoIva21 As String = "Alicuota 21%"
        Dim textoTotal As String = "Total"



        Dim lineaSep = StrDup(27, " ")
        e.Graphics.DrawString(lineaSep & "__________", printfont, System.Drawing.Brushes.Black, 0, yPos)
        Dim XXX As Integer = 0

        If TipoFact < 3 Then

            XXX = 27 - (textosub.Length + facSubtotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textosub & lineatotal & facSubtotal, font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoIva21.Length + FacIva21.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textoIva21 & lineatotal & FacIva21, font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoTotal.Length + facTotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10

        Else
            XXX = 27 - (textosub.Length + facTotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textosub & lineatotal & facTotal, font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoIva21.Length + 1)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textoIva21 & lineatotal & "0", font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoTotal.Length + facTotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textoTotal & lineatotal & facTotal, font3, System.Drawing.Brushes.Black, 0, yPos)
            yPos += 30
        End If

        e.Graphics.DrawString("COMPROBANTE AUTORIZADO POR WEB SERVICE", fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10

        e.Graphics.DrawString("CAE: " & facCAE, fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10

        e.Graphics.DrawString("F. Vto CAE: " & facVtoCAE, fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10
        e.Graphics.DrawString(facCodBARRA, fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10

        e.Graphics.DrawString(My.Settings.TextoPieTiket, font3, System.Drawing.Brushes.Black, 15, yPos)
        yPos += 10
        e.Graphics.DrawString("Gracias por tu compra!!!", font3, System.Drawing.Brushes.Black, 15, yPos)



    End Sub

    Private Sub ImprimirTiketVenta(ByVal sender As System.Object, ByVal e As PrintPageEventArgs)
        ' letra
        'Dim font1 As New Font("EAN-13", 40)
        Dim printfont As New Font("Courier New", 6)
        Dim font3 As New Font("Courier New", 8)
        Dim font4 As New Font("Courier New", 18)
        Dim font5 As New Font("Courier New", 6)
        Dim fontCAE As New Font("Courier New", 5.3, FontStyle.Italic)

        Dim alto As Single = 0
        Dim topMargin As Double '= e.MarginBounds.Top
        Dim yPos As Double = 0
        Dim count As Integer = 0
        Dim texto As String = ""

        Dim codigo As String = ""
        Dim unidad As String = ""
        Dim detalle As String = ""
        Dim valoruni As String = ""
        Dim valortot As String = ""
        Dim tabulacion As String = ""
        Dim compensador As Integer = 0
        Dim total As String = ""
        Dim lvalor As String
        Dim lineatotal As String
        Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim ivaProd As String = ""
        Dim fac As New datosfacturas

        Dim facTotal As String = ""
        Dim facSubtotal As String = ""
        Dim FacIva21 As String = ""
        Dim FacIva105 As String = ""

        Dim facCAE As String = ""
        Dim facVtoCAE As String = ""
        Dim facCodBARRA As String = ""



        Reconectar()

        tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  
        emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, 
        emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, 
        concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, 
        concat(fac.id_cliente,'-',fac.razon) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, 
        concat(vend.apellido,', ',vend.nombre) as facvend, condvent.condicion as faccondvta, fac.observaciones2 as facobserva,format(fac.iva105,2,'es_AR') as iva105, format(fac.iva21,2,'es_AR') as iva21,
        '','',fis.donfdesc, fac.cae, fis.letra as facletra, fis.codfiscal as faccodigo, fac.vtocae, fac.codbarra, format(fac.total,2,'es_AR'),format(fac.subtotal,2,'es_AR')   
        FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac,fact_condventas as condvent  
        where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and condvent.id=fac.condvta and fac.ptovta=fis.ptovta and fac.id=" & IdFactura, conexionPrinc)

        Dim tablaEmpresa As New DataTable
        tabEmp.Fill(tablaEmpresa)

        Reconectar()

        tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select 
            plu,
            format(replace(cantidad,',','.'),2,'es_AR') as cant, descripcion, 
            format(replace(iva,',','.'),2,'es_AR') as iva ,
            format(replace(punit,',','.'),2,'es_AR') as punit ,
            format(replace(ptotal,',','.'),2,'es_AR') as ptotal 
            from fact_items where id_fact=" & IdFactura, conexionPrinc)
        Dim tablaProd As New DataTable
        tabFac.Fill(tablaProd)

        facTotal = tablaEmpresa.Rows(0).Item(30)
        facSubtotal = tablaEmpresa.Rows(0).Item(31)
        FacIva21 = tablaEmpresa.Rows(0).Item(21)
        FacIva105 = ""

        facCAE = tablaEmpresa.Rows(0).Item(25)
        facCodBARRA = tablaEmpresa.Rows(0).Item(29)
        facVtoCAE = tablaEmpresa.Rows(0).Item(28)


        e.Graphics.DrawImage(Image.FromFile(Application.StartupPath & "\logo2.jpg"), 5, 15)
        'fac.Tables("factura_enca")["empnombre"].tostring()

        e.Graphics.DrawString("Razón social: " & tablaEmpresa.Rows(0).Item(0), font5, Brushes.Black, 0, 100) 'RAZON SOCIAL
        e.Graphics.DrawString("Tiket N°: " & tablaEmpresa.Rows(0).Item(10), font5, Brushes.Black, 0, 110) '
        e.Graphics.DrawString("Fecha: " & tablaEmpresa.Rows(0).Item(11).ToString, font5, Brushes.Black, 0, 120) '
        e.Graphics.DrawString("Ciente: " & tablaEmpresa.Rows(0).Item(12), font5, Brushes.Black, 0, 130) '
        e.Graphics.DrawString("#Articulos:" & tablaProd.Rows.Count, font5, Brushes.Black, 0, 140) '
        e.Graphics.DrawString(StrDup(65, "#"), font5, Brushes.Black, 0, 150)
        e.Graphics.DrawString("### TIKET NO VALIDO COMO FACTURA ###", font5, Brushes.Black, 0, 160)
        e.Graphics.DrawString(StrDup(65, "#"), font5, Brushes.Black, 0, 170)
        'e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 180)
        'e.Graphics.DrawString("CODIGO      |    CANT    | PRE. UNI | PRE. TOTAL ", font5, Brushes.Black, 0, 190)
        'e.Graphics.DrawString("DESCRIPCION", font5, Brushes.Black, 0, 200)
        'e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 210)

        Dim i As Integer
        Dim j As Integer
        Dim car As Integer

        For i = 0 To tablaProd.Rows.Count - 1
            codigo = tablaProd.Rows(i).Item(0)
            unidad = tablaProd(i).Item(1)
            detalle = tablaProd(i).Item(2)
            valoruni = tablaProd(i).Item(4)
            valortot = FormatNumber(tablaProd(i).Item(5), 2)
            ivaProd = tablaProd(i).Item(3)
            texto = unidad & " x " & valoruni & Chr(9) & "  (" & ivaProd & ")"
            yPos = 190 + topMargin + (count * printfont.GetHeight(e.Graphics)) ' Calcula la posición en la que se escribe la línea            


            If detalle.Length <= 25 Then
                car = 25 - detalle.Length
                For j = 0 To car
                    detalle &= " "
                Next
            Else
                car = detalle.Length - 25
                detalle = detalle.Remove(26, car - 1)
            End If

            If valortot.Length <= 7 Then
                car = 7 - valortot.Length
                For j = 0 To car
                    valortot = " " & valortot
                Next

            End If


            'If Not row.IsNewRow Then
            e.Graphics.DrawString(texto, printfont, System.Drawing.Brushes.Black, 0, yPos)
            count += 1
            yPos = yPos + 10
            e.Graphics.DrawString(detalle & "  " & valortot, printfont, System.Drawing.Brushes.Black, 0, yPos)
            'total += valor
            'End If
            count += 1

        Next
        If FacIva21.Length <= 7 Then
            car = 7 - FacIva21.Length
            For j = 0 To car
                FacIva21 = " " & FacIva21
            Next

        End If


        If facSubtotal.Length <= 7 Then
            car = 7 - facSubtotal.Length
            For j = 0 To car
                facSubtotal = " " & facSubtotal
            Next
        End If

        If facTotal.Length <= 7 Then
            car = 7 - facTotal.Length
            For j = 0 To car
                facTotal = " " & facTotal
            Next
        End If




        yPos += 20
        Dim textosub As String = "Subtotal"
        Dim textoIva21 As String = "Alicuota 21%"
        Dim textoTotal As String = "Total"



        Dim lineaSep = StrDup(27, " ")
        e.Graphics.DrawString(lineaSep & "__________", printfont, System.Drawing.Brushes.Black, 0, yPos)
        Dim XXX As Integer = 0

        XXX = 27 - (textosub.Length + facSubtotal.Length)
        lineatotal = StrDup(XXX, ".")
        yPos += 10
        e.Graphics.DrawString(textosub & lineatotal & facSubtotal, font3, System.Drawing.Brushes.Black, 0, yPos)

        XXX = 27 - (textoIva21.Length + FacIva21.Length)
        lineatotal = StrDup(XXX, ".")
        yPos += 10
        e.Graphics.DrawString(textoIva21 & lineatotal & FacIva21, font3, System.Drawing.Brushes.Black, 0, yPos)

        XXX = 27 - (textoTotal.Length + facTotal.Length)
        lineatotal = StrDup(XXX, ".")
        yPos += 10
        e.Graphics.DrawString(textoTotal & lineatotal & facTotal, font3, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 30

        e.Graphics.DrawString("Gracias por tu compra!!!", font3, System.Drawing.Brushes.Black, 15, yPos)



    End Sub

End Class