Imports System.Data.SqlClient
Public Class F_ChiTietHoaDon
    Dim Ket_Noi As SqlConnection
    Dim viTriHienTai As Integer = 0 ' Khai báo biến toàn cục

    Private Sub F_ChiTietHoaDon_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Ket_Noi = New SqlConnection("Data Source=DESKTOP-V0AMEPA;Initial Catalog=QL_CUAHANGMAYTINH;Integrated Security=True;TrustServerCertificate=True")
            Ket_Noi.Open()

            ' Hiển thị dữ liệu từ bảng CHITIET_HOADON lên DataGridView:
            Load_dulieu()

            viTriHienTai = 0
            CapNhatDataGridView()

        Catch ex As Exception
            MsgBox("Lỗi kết nối: " & ex.Message, vbCritical, "Lỗi")
        End Try
    End Sub

    'Tạo 1 Thủ tục Load lại dữ liệu khi thêm mới, xoá, sửa:
    Private Sub Load_dulieu()
        Try
            Dim dt As New DataTable()
            Dim da As New SqlDataAdapter("SELECT MaChiTiet, MaHoaDon, MaSanPham, LoaiSanPham, SoLuong, GiaBan, ThanhTien FROM CHI_TIET_HOA_DON ORDER BY MaChiTiet", Ket_Noi)
            da.Fill(dt)

            ' Tạo cột STT
            Dim sttColumn As New DataColumn("STT", GetType(Integer))
            dt.Columns.Add(sttColumn)

            ' Thêm STT cho từng dòng
            For i As Integer = 0 To dt.Rows.Count - 1
                dt.Rows(i)("STT") = i + 1
            Next

            Data_DS.DataSource = dt

            ' Đưa cột STT lên đầu
            Data_DS.Columns("STT").DisplayIndex = 0

            ' Thiết lập tiêu đề cột cho DataGridView
            If Data_DS.Columns.Count > 0 Then
                Data_DS.Columns("STT").HeaderText = "STT"
                Data_DS.Columns("STT").Width = 50
                Data_DS.Columns("STT").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                Data_DS.Columns("MaChiTiet").HeaderText = "Mã Chi Tiết"
                Data_DS.Columns("MaHoaDon").HeaderText = "Mã Hóa Đơn"
                Data_DS.Columns("MaSanPham").HeaderText = "Mã Sản Phẩm"
                Data_DS.Columns("LoaiSanPham").HeaderText = "Loại Sản Phẩm"
                Data_DS.Columns("SoLuong").HeaderText = "Số Lượng"
                Data_DS.Columns("GiaBan").HeaderText = "Giá Bán"
                Data_DS.Columns("ThanhTien").HeaderText = "Thành Tiền"

                ' Định dạng cột tiền
                Data_DS.Columns("GiaBan").DefaultCellStyle.Format = "N0"
                Data_DS.Columns("GiaBan").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Data_DS.Columns("ThanhTien").DefaultCellStyle.Format = "N0"
                Data_DS.Columns("ThanhTien").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                ' Định dạng cột số lượng
                Data_DS.Columns("SoLuong").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                ' Không cho phép chỉnh sửa cột STT và Thành Tiền (tự tính)
                Data_DS.Columns("STT").ReadOnly = True
                Data_DS.Columns("ThanhTien").ReadOnly = True
            End If

            ' Hiển thị tổng số chi tiết hóa đơn (giả sử bạn có label tên lblTongSo)
            Try
                If Me.Controls.ContainsKey("lblTongSo") Then
                    Dim lblTongSo As Label = DirectCast(Me.Controls("lblTongSo"), Label)
                    lblTongSo.Text = "Tổng số chi tiết: " & dt.Rows.Count.ToString()
                End If
            Catch
                ' Nếu không có label thì bỏ qua
            End Try

            ' Cập nhật vị trí hiện tại
            If dt.Rows.Count > 0 Then
                If viTriHienTai >= dt.Rows.Count Then
                    viTriHienTai = dt.Rows.Count - 1
                End If
                CapNhatDataGridView()
            End If
        Catch ex As Exception
            MsgBox("Lỗi tải dữ liệu: " & ex.Message, vbCritical, "Lỗi")
        End Try
    End Sub

    ' Tạo thủ tục này để di chuyển giữa các bản ghi
    Private Sub CapNhatDataGridView()
        Try
            If Data_DS.Rows.Count > 0 AndAlso viTriHienTai >= 0 AndAlso viTriHienTai < Data_DS.Rows.Count Then
                Data_DS.ClearSelection()
                Data_DS.Rows(viTriHienTai).Selected = True
                Data_DS.CurrentCell = Data_DS.Rows(viTriHienTai).Cells(0)

                ' Hiển thị thông tin lên TextBox
                HienThiThongTin(viTriHienTai)
            End If
        Catch ex As Exception
            MsgBox("Lỗi cập nhật DataGridView: " & ex.Message, vbCritical, "Lỗi")
        End Try
    End Sub

    ' Hiển thị thông tin lên các TextBox
    Private Sub HienThiThongTin(viTri As Integer)
        Try
            If Data_DS.Rows.Count > 0 AndAlso viTri >= 0 AndAlso viTri < Data_DS.Rows.Count Then
                Dim row As DataGridViewRow = Data_DS.Rows(viTri)
                TxtMaChiTiet.Text = If(row.Cells("MaChiTiet").Value IsNot Nothing, row.Cells("MaChiTiet").Value.ToString(), "")
                TxtMaHoaDon.Text = If(row.Cells("MaHoaDon").Value IsNot Nothing, row.Cells("MaHoaDon").Value.ToString(), "")
                TxtMaSanPham.Text = If(row.Cells("MaSanPham").Value IsNot Nothing, row.Cells("MaSanPham").Value.ToString(), "")

                TxtSoLuong.Text = If(row.Cells("SoLuong").Value IsNot Nothing, row.Cells("SoLuong").Value.ToString(), "0")
                TxtGiaBan.Text = If(row.Cells("GiaBan").Value IsNot Nothing, row.Cells("GiaBan").Value.ToString(), "0")
                TxtThanhTien.Text = If(row.Cells("ThanhTien").Value IsNot Nothing, row.Cells("ThanhTien").Value.ToString(), "0")

                ' Hiển thị vị trí hiện tại (giả sử bạn có label tên lblViTri)
                Try
                    If Me.Controls.ContainsKey("lblViTri") Then
                        Dim lblViTri As Label = DirectCast(Me.Controls("lblViTri"), Label)
                        lblViTri.Text = "Chi tiết: " & (viTri + 1).ToString() & "/" & Data_DS.Rows.Count.ToString()
                    End If
                Catch
                    ' Nếu không có label thì bỏ qua
                End Try
            End If
        Catch ex As Exception
            MsgBox("Lỗi hiển thị thông tin: " & ex.Message, vbCritical, "Lỗi")
        End Try
    End Sub

    ' Xóa dữ liệu trên form
    Private Sub XoaDuLieuForm()
        TxtMaChiTiet.Clear()
        TxtMaHoaDon.Clear()
        TxtMaSanPham.Clear()

        TxtSoLuong.Clear()
        TxtGiaBan.Clear()
        TxtThanhTien.Clear()
        TxtMaChiTiet.Focus()
    End Sub

    ' Tính thành tiền
    Private Function TinhThanhTien(soLuong As Integer, giaBan As Decimal) As Decimal
        Return soLuong * giaBan
    End Function

    ' Kiểm tra mã hóa đơn có tồn tại không
    Private Function KiemTraMaHoaDon(maHD As String) As Boolean
        Try
            If Ket_Noi.State = ConnectionState.Closed Then
                Ket_Noi.Open()
            End If

            Dim query As String = "SELECT COUNT(*) FROM HOA_DON WHERE MaHoaDon = @MaHD"
            Dim cmd As New SqlCommand(query, Ket_Noi)
            cmd.Parameters.AddWithValue("@MaHD", maHD)

            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        Catch ex As Exception
            MsgBox("Lỗi kiểm tra mã hóa đơn: " & ex.Message, vbCritical, "Lỗi")
            Return False
        End Try
    End Function

    ' Cập nhật số lượng và giá bán khi change
    Private Sub TxtSoLuong_TextChanged(sender As Object, e As EventArgs) Handles TxtSoLuong.TextChanged
        TinhVaHienThiThanhTien()
    End Sub

    Private Sub TxtGiaBan_TextChanged(sender As Object, e As EventArgs)
        TinhVaHienThiThanhTien()
    End Sub

    Private Sub TinhVaHienThiThanhTien()
        Try
            Dim soLuong As Integer = 0
            Dim giaBan As Decimal = 0

            Integer.TryParse(TxtSoLuong.Text.Trim(), soLuong)
            Decimal.TryParse(TxtGiaBan.Text.Trim(), giaBan)

            Dim thanhTien As Decimal = TinhThanhTien(soLuong, giaBan)
            TxtThanhTien.Text = thanhTien.ToString("N0")
        Catch ex As Exception
            TxtThanhTien.Text = "0"
        End Try
    End Sub

    Private Sub THEM_Click(sender As Object, e As EventArgs) Handles THEM.Click
        ' Kiểm tra dữ liệu trước khi thêm
        If TxtMaChiTiet.Text.Trim() = "" Then
            MsgBox("Mã chi tiết không được để trống", vbExclamation, "Chú ý")
            TxtMaChiTiet.Focus()
            Exit Sub
        End If

        If TxtMaHoaDon.Text.Trim() = "" Then
            MsgBox("Mã hóa đơn không được để trống", vbExclamation, "Chú ý")
            TxtMaHoaDon.Focus()
            Exit Sub
        End If

        If TxtMaSanPham.Text.Trim() = "" Then
            MsgBox("Mã sản phẩm không được để trống", vbExclamation, "Chú ý")
            TxtMaSanPham.Focus()
            Exit Sub
        End If

        ' Kiểm tra mã hóa đơn có tồn tại không
        If Not KiemTraMaHoaDon(TxtMaHoaDon.Text.Trim()) Then
            MsgBox("Mã hóa đơn không tồn tại trong hệ thống", vbExclamation, "Chú ý")
            TxtMaHoaDon.Focus()
            Exit Sub
        End If

        ' Kiểm tra số lượng
        Dim soLuong As Integer = 0
        If Not Integer.TryParse(TxtSoLuong.Text.Trim(), soLuong) OrElse soLuong <= 0 Then
            MsgBox("Số lượng phải là số nguyên dương", vbExclamation, "Chú ý")
            TxtSoLuong.Focus()
            Exit Sub
        End If

        ' Kiểm tra giá bán
        Dim giaBan As Decimal = 0
        If Not Decimal.TryParse(TxtGiaBan.Text.Trim(), giaBan) OrElse giaBan <= 0 Then
            MsgBox("Giá bán phải là số dương", vbExclamation, "Chú ý")
            TxtGiaBan.Focus()
            Exit Sub
        End If

        Try
            ' Kiểm tra trùng mã chi tiết
            Dim SQL_KiemTra As New SqlDataAdapter("SELECT * FROM CHI_TIET_HOA_DON WHERE MaChiTiet=@MaChiTiet", Ket_Noi)
            SQL_KiemTra.SelectCommand.Parameters.AddWithValue("@MaChiTiet", TxtMaChiTiet.Text.Trim())
            Dim Db_Check As New DataTable
            SQL_KiemTra.Fill(Db_Check)

            If Db_Check.Rows.Count > 0 Then
                MsgBox("Mã chi tiết này đã tồn tại!", vbExclamation, "Thông báo")
                TxtMaChiTiet.Focus()
                Exit Sub
            End If

            ' Kiểm tra và mở kết nối nếu chưa mở
            If Ket_Noi.State = ConnectionState.Closed Then
                Ket_Noi.Open()
            End If
            Dim thanhTien As Decimal = TinhThanhTien(soLuong, giaBan)

            ' Lệnh INSERT:
            Dim SQL_DangKy As New SqlCommand("INSERT INTO CHI_TIET_HOA_DON (MaChiTiet, MaHoaDon, MaSanPham, LoaiSanPham, SoLuong, GiaBan, ThanhTien) VALUES (@MaChiTiet, @MaHoaDon, @MaSanPham, @LoaiSanPham, @SoLuong, @GiaBan, @ThanhTien)", Ket_Noi)
            SQL_DangKy.Parameters.AddWithValue("@MaChiTiet", TxtMaChiTiet.Text.Trim())
            SQL_DangKy.Parameters.AddWithValue("@MaHoaDon", TxtMaHoaDon.Text.Trim())
            SQL_DangKy.Parameters.AddWithValue("@MaSanPham", TxtMaSanPham.Text.Trim())

            SQL_DangKy.Parameters.AddWithValue("@SoLuong", soLuong)
            SQL_DangKy.Parameters.AddWithValue("@GiaBan", giaBan)
            SQL_DangKy.Parameters.AddWithValue("@ThanhTien", thanhTien)

            SQL_DangKy.ExecuteNonQuery()
            MsgBox("Thêm chi tiết hóa đơn thành công!", vbInformation, "Thông báo")

            Load_dulieu()
            XoaDuLieuForm()

        Catch ex As SqlException
            MsgBox("Lỗi SQL: " & ex.Message, vbCritical, "Lỗi")
        Catch ex As Exception
            MsgBox("Lỗi: " & ex.Message, vbCritical, "Lỗi")
        Finally
            If Ket_Noi.State = ConnectionState.Open Then
                Ket_Noi.Close()
            End If
        End Try
    End Sub

    Private Sub SUA_Click(sender As Object, e As EventArgs) Handles SUA.Click
        If TxtMaChiTiet.Text.Trim() = "" Then
            MsgBox("Vui lòng chọn chi tiết hóa đơn cần sửa từ danh sách", vbExclamation, "Thông báo")
            Exit Sub
        End If

        If TxtMaHoaDon.Text.Trim() = "" Then
            MsgBox("Mã hóa đơn không được để trống", vbExclamation, "Chú ý")
            TxtMaHoaDon.Focus()
            Exit Sub
        End If

        If TxtMaSanPham.Text.Trim() = "" Then
            MsgBox("Mã sản phẩm không được để trống", vbExclamation, "Chú ý")
            TxtMaSanPham.Focus()
            Exit Sub
        End If

        ' Kiểm tra mã hóa đơn có tồn tại không
        If Not KiemTraMaHoaDon(TxtMaHoaDon.Text.Trim()) Then
            MsgBox("Mã hóa đơn không tồn tại trong hệ thống", vbExclamation, "Chú ý")
            TxtMaHoaDon.Focus()
            Exit Sub
        End If

        ' Kiểm tra số lượng
        Dim soLuong As Integer = 0
        If Not Integer.TryParse(TxtSoLuong.Text.Trim(), soLuong) OrElse soLuong <= 0 Then
            MsgBox("Số lượng phải là số nguyên dương", vbExclamation, "Chú ý")
            TxtSoLuong.Focus()
            Exit Sub
        End If

        ' Kiểm tra giá bán
        Dim giaBan As Decimal = 0
        If Not Decimal.TryParse(TxtGiaBan.Text.Trim(), giaBan) OrElse giaBan <= 0 Then
            MsgBox("Giá bán phải là số dương", vbExclamation, "Chú ý")

            Exit Sub
        End If

        Try
            ' Kiểm tra và mở kết nối nếu chưa mở
            If Ket_Noi.State = ConnectionState.Closed Then
                Ket_Noi.Open()
            End If

            ' Tính thành tiền
            Dim thanhTien As Decimal = TinhThanhTien(soLuong, giaBan)

            Dim Sql As String = "UPDATE CHI_TIET_HOA_DON SET MaHoaDon=@MaHoaDon, MaSanPham=@MaSanPham, LoaiSanPham=@LoaiSanPham, SoLuong=@SoLuong, GiaBan=@GiaBan, ThanhTien=@ThanhTien WHERE MaChiTiet=@MaChiTiet"
            Using Cmd As New SqlCommand(Sql, Ket_Noi)
                Cmd.Parameters.AddWithValue("@MaChiTiet", TxtMaChiTiet.Text.Trim())
                Cmd.Parameters.AddWithValue("@MaHoaDon", TxtMaHoaDon.Text.Trim())
                Cmd.Parameters.AddWithValue("@MaSanPham", TxtMaSanPham.Text.Trim())

                Cmd.Parameters.AddWithValue("@SoLuong", soLuong)
                Cmd.Parameters.AddWithValue("@GiaBan", giaBan)
                Cmd.Parameters.AddWithValue("@ThanhTien", thanhTien)

                Dim result As Integer = Cmd.ExecuteNonQuery()

                If result > 0 Then
                    MsgBox("Cập nhật thành công!", vbInformation, "Thông báo")
                    Load_dulieu() ' Làm mới DataGridView
                    XoaDuLieuForm()
                Else
                    MsgBox("Không thể cập nhật, mã chi tiết không tồn tại!", vbExclamation, "Lỗi")
                End If
            End Using

        Catch ex As SqlException
            MsgBox("Lỗi SQL: " & ex.Message, vbCritical, "Lỗi")
        Catch ex As Exception
            MsgBox("Lỗi: " & ex.Message, vbCritical, "Lỗi")
        Finally
            If Ket_Noi.State = ConnectionState.Open Then
                Ket_Noi.Close()
            End If
        End Try
    End Sub

    Private Sub Data_DS_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Data_DS.CellClick
        If e.RowIndex >= 0 Then
            viTriHienTai = e.RowIndex
            HienThiThongTin(viTriHienTai)
        End If
    End Sub

    Private Sub TKiem_Click(sender As Object, e As EventArgs) Handles TKiem.Click
        Dim maChiTiet As String = InputBox("Nhập mã chi tiết cần tìm:", "Tìm Kiếm Chi Tiết Hóa Đơn")

        If maChiTiet.Trim() = "" Then
            MessageBox.Show("Bạn chưa nhập mã chi tiết!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            ' Tìm chi tiết hóa đơn trong database
            If Ket_Noi.State = ConnectionState.Closed Then
                Ket_Noi.Open()
            End If

            Dim query As String = "SELECT * FROM CHI_TIET_HOA_DON WHERE MaChiTiet = @MaChiTiet"
            Dim command As New SqlCommand(query, Ket_Noi)
            command.Parameters.AddWithValue("@MaChiTiet", maChiTiet.Trim())

            Dim adapter As New SqlDataAdapter(command)
            Dim table As New DataTable()
            adapter.Fill(table)

            If table.Rows.Count > 0 Then
                ' Hiển thị lên TextBox
                TxtMaChiTiet.Text = table.Rows(0)("MaChiTiet").ToString()
                TxtMaHoaDon.Text = If(table.Rows(0)("MaHoaDon") IsNot DBNull.Value, table.Rows(0)("MaHoaDon").ToString(), "")
                TxtMaSanPham.Text = If(table.Rows(0)("MaSanPham") IsNot DBNull.Value, table.Rows(0)("MaSanPham").ToString(), "")

                TxtSoLuong.Text = If(table.Rows(0)("SoLuong") IsNot DBNull.Value, table.Rows(0)("SoLuong").ToString(), "0")
                TxtGiaBan.Text = If(table.Rows(0)("GiaBan") IsNot DBNull.Value, table.Rows(0)("GiaBan").ToString(), "0")
                TxtThanhTien.Text = If(table.Rows(0)("ThanhTien") IsNot DBNull.Value, table.Rows(0)("ThanhTien").ToString(), "0")

                ' Đánh dấu dòng trong DataGridView
                For i As Integer = 0 To Data_DS.Rows.Count - 1
                    If Data_DS.Rows(i).Cells("MaChiTiet").Value.ToString() = maChiTiet.Trim() Then
                        viTriHienTai = i
                        CapNhatDataGridView()
                        Exit For
                    End If
                Next
            Else
                MessageBox.Show("Không tìm thấy chi tiết hóa đơn có mã này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Lỗi tìm kiếm: " & ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Ket_Noi.State = ConnectionState.Open Then
                Ket_Noi.Close()
            End If
        End Try
    End Sub

    Private Sub XOA_Click(sender As Object, e As EventArgs) Handles XOA.Click
        If TxtMaChiTiet.Text.Trim() = "" Then
            MessageBox.Show("Vui lòng chọn chi tiết hóa đơn cần xóa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim maChiTiet As String = TxtMaChiTiet.Text.Trim()

        ' Xác nhận xóa
        If MessageBox.Show("Bạn có chắc muốn xóa chi tiết hóa đơn '" & maChiTiet & "'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                If Ket_Noi.State = ConnectionState.Closed Then
                    Ket_Noi.Open()
                End If

                Dim query As String = "DELETE FROM CHI_TIET_HOA_DON WHERE MaChiTiet = @MaChiTiet"
                Dim command As New SqlCommand(query, Ket_Noi)
                command.Parameters.AddWithValue("@MaChiTiet", maChiTiet)

                Dim rowsAffected As Integer = command.ExecuteNonQuery()

                If rowsAffected > 0 Then
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Load_dulieu() ' Cập nhật lại danh sách
                    XoaDuLieuForm()
                Else
                    MessageBox.Show("Không tìm thấy chi tiết hóa đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            Catch ex As Exception
                MessageBox.Show("Lỗi khi xóa: " & ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If Ket_Noi.State = ConnectionState.Open Then
                    Ket_Noi.Close()
                End If
            End Try
        End If
    End Sub

    Private Sub THOAT_Click(sender As Object, e As EventArgs) Handles THOAT.Click
        ' Hỏi xác nhận trước khi thoát
        If MessageBox.Show("Bạn có chắc muốn thoát khỏi form Quản lý Chi tiết hóa đơn?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            If Ket_Noi IsNot Nothing AndAlso Ket_Noi.State = ConnectionState.Open Then
                Ket_Noi.Close()
            End If
            Close()
        End If
    End Sub

    Private Sub DAU_Click(sender As Object, e As EventArgs) Handles DAU.Click
        If Data_DS.Rows.Count > 0 Then
            viTriHienTai = 0
            CapNhatDataGridView()
        End If
    End Sub

    Private Sub TRUOC_Click(sender As Object, e As EventArgs) Handles TRUOC.Click
        If Data_DS.Rows.Count > 0 Then
            If viTriHienTai > 0 Then
                viTriHienTai -= 1
                CapNhatDataGridView()
            Else
                MsgBox("Bạn đang ở dòng đầu tiên!", vbInformation, "Thông báo")
            End If
        End If
    End Sub

    Private Sub SAU_Click(sender As Object, e As EventArgs) Handles SAU.Click
        If Data_DS.Rows.Count > 0 Then
            If viTriHienTai < Data_DS.Rows.Count - 1 Then
                viTriHienTai += 1
                CapNhatDataGridView()
            Else
                MsgBox("Bạn đang ở dòng cuối cùng!", vbInformation, "Thông báo")
            End If
        End If
    End Sub

    Private Sub CUOI_Click(sender As Object, e As EventArgs) Handles CUOI.Click
        If Data_DS.Rows.Count > 0 Then
            viTriHienTai = Data_DS.Rows.Count - 1
            CapNhatDataGridView()
        End If
    End Sub
    Private Sub LUU_Click(sender As Object, e As EventArgs) Handles LUU.Click
        ' Kiểm tra các trường bắt buộc
        If TxtMaChiTiet.Text.Trim() = "" OrElse TxtMaHoaDon.Text.Trim() = "" OrElse TxtMaSanPham.Text.Trim() = "" Then
            MsgBox("Vui lòng điền đầy đủ Mã chi tiết, Mã hóa đơn và Mã sản phẩm!", vbExclamation, "Chú ý")
            Exit Sub
        End If

        ' Kiểm tra số lượng và giá
        Dim soLuong As Integer
        Dim giaBan As Decimal
        If Not Integer.TryParse(TxtSoLuong.Text.Trim(), soLuong) OrElse soLuong <= 0 Then
            MsgBox("Số lượng phải là số nguyên dương.", vbExclamation, "Chú ý")
            Exit Sub
        End If
        If Not Decimal.TryParse(TxtGiaBan.Text.Trim(), giaBan) OrElse giaBan <= 0 Then
            MsgBox("Giá bán phải là số dương.", vbExclamation, "Chú ý")
            Exit Sub
        End If

        Dim thanhTien As Decimal = TinhThanhTien(soLuong, giaBan)

        Try
            If Ket_Noi.State = ConnectionState.Closed Then Ket_Noi.Open()

            ' Kiểm tra mã chi tiết đã tồn tại hay chưa
            Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM CHI_TIET_HOA_DON WHERE MaChiTiet=@MaChiTiet", Ket_Noi)
            checkCmd.Parameters.AddWithValue("@MaChiTiet", TxtMaChiTiet.Text.Trim())
            Dim tonTai As Integer = CInt(checkCmd.ExecuteScalar())

            If tonTai > 0 Then
                ' Nếu đã tồn tại → UPDATE
                Dim sqlUpdate As String = "UPDATE CHI_TIET_HOA_DON SET MaHoaDon=@MaHoaDon, MaSanPham=@MaSanPham, LoaiSanPham=@LoaiSanPham, SoLuong=@SoLuong, GiaBan=@GiaBan, ThanhTien=@ThanhTien WHERE MaChiTiet=@MaChiTiet"
                Using cmd As New SqlCommand(sqlUpdate, Ket_Noi)
                    cmd.Parameters.AddWithValue("@MaChiTiet", TxtMaChiTiet.Text.Trim())
                    cmd.Parameters.AddWithValue("@MaHoaDon", TxtMaHoaDon.Text.Trim())
                    cmd.Parameters.AddWithValue("@MaSanPham", TxtMaSanPham.Text.Trim())

                    cmd.Parameters.AddWithValue("@SoLuong", soLuong)
                    cmd.Parameters.AddWithValue("@GiaBan", giaBan)
                    cmd.Parameters.AddWithValue("@ThanhTien", thanhTien)
                    cmd.ExecuteNonQuery()
                    MsgBox("Cập nhật thành công!", vbInformation, "Thông báo")
                End Using
            Else
                ' Nếu chưa tồn tại → INSERT
                Dim sqlInsert As String = "INSERT INTO CHI_TIET_HOA_DON (MaChiTiet, MaHoaDon, MaSanPham, LoaiSanPham, SoLuong, GiaBan, ThanhTien) VALUES (@MaChiTiet, @MaHoaDon, @MaSanPham, @LoaiSanPham, @SoLuong, @GiaBan, @ThanhTien)"
                Using cmd As New SqlCommand(sqlInsert, Ket_Noi)
                    cmd.Parameters.AddWithValue("@MaChiTiet", TxtMaChiTiet.Text.Trim())
                    cmd.Parameters.AddWithValue("@MaHoaDon", TxtMaHoaDon.Text.Trim())
                    cmd.Parameters.AddWithValue("@MaSanPham", TxtMaSanPham.Text.Trim())

                    cmd.Parameters.AddWithValue("@SoLuong", soLuong)
                    cmd.Parameters.AddWithValue("@GiaBan", giaBan)
                    cmd.Parameters.AddWithValue("@ThanhTien", thanhTien)
                    cmd.ExecuteNonQuery()
                    MsgBox("Thêm chi tiết hóa đơn thành công!", vbInformation, "Thông báo")
                End Using
            End If

            ' Cập nhật lại danh sách
            Load_dulieu()
            XoaDuLieuForm()

        Catch ex As Exception
            MsgBox("Lỗi Lưu: " & ex.Message, vbCritical, "Lỗi")
        Finally
            If Ket_Noi.State = ConnectionState.Open Then Ket_Noi.Close()
        End Try
    End Sub


End Class