using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TelephoneDiary
{
    public partial class Phone : Form
    {
        // Chuỗi kết nối đến cơ sở dữ liệu
        private static string connectionString = "Data Source=CHUHUY;Initial Catalog=PhoneDiary;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        // Đối tượng kết nối SQL
        private readonly SqlConnection conn = new SqlConnection(connectionString);

        public Phone()
        {
            InitializeComponent();
        }

        // Sự kiện Load của Form
        private void Phone_Load(object sender, EventArgs e)
        {
            // Hiển thị dữ liệu ban đầu
            DisplayData();
        }

        // Hiển thị dữ liệu trên DataGridView
        private void DisplayData()
        {
            // Thực hiện truy vấn SQL để lấy dữ liệu
            DataTable dt = ExecuteQuery("SELECT * FROM Mobiles");

            // Đặt dữ liệu vào DataGridView
            dataGridView1.DataSource = dt;
        }

        // Sự kiện Click của nút New
        private void btnNew_Click(object sender, EventArgs e)
        {
            // Xóa trắng các trường nhập liệu
            ClearFields();
        }

        // Xóa trắng các trường nhập liệu
        private void ClearFields()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            cbbCatagory.SelectedIndex = -1;
        }

        // Sự kiện Click của nút Insert
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Câu lệnh SQL chèn dữ liệu
            string query = @"INSERT INTO Mobiles (FirstName, LastName, Mobile, Email, Catagory) 
                    VALUES (@FirstName, @LastName, @Mobile, @Email, @Catagory)";

            // Tạo danh sách tham số cho câu lệnh SQL
            var parameters = new Dictionary<string, object>()
            {
                { "@FirstName", txtFirstName.Text },
                { "@LastName", txtLastName.Text },
                { "@Mobile", txtMobile.Text },
                { "@Email", txtEmail.Text },
                { "@Catagory", cbbCatagory.Text }
            };

            // Thực hiện câu lệnh SQL và nhận số dòng bị ảnh hưởng
            int rowsAffected = ExecuteNonQuery(query, parameters);

            // Kiểm tra số dòng bị ảnh hưởng
            if (rowsAffected > 0)
            {
                MessageBox.Show("Data inserted successfully!");
                // Hiển thị lại dữ liệu sau khi chèn
                DisplayData();
                // Xóa trắng các trường nhập liệu
                ClearFields();
            }
            else
            {
                MessageBox.Show("Failed to insert data.");
            }
        }

        // Sự kiện Click của nút Update
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                // Lấy số điện thoại của dòng được chọn
                string mobileToUpdate = selectedRow.Cells["Mobile"].Value.ToString();

                // Câu lệnh SQL cập nhật dữ liệu
                string query = @"UPDATE Mobiles 
                                 SET FirstName = @FirstName, LastName = @LastName, 
                                     Mobile = @Mobile, Email = @Email, Catagory = @Catagory 
                                 WHERE Mobile = @MobileToUpdate";

                // Tạo danh sách tham số cho câu lệnh SQL
                var parameters = new Dictionary<string, object>()
                {
                    { "@FirstName", txtFirstName.Text },
                    { "@LastName", txtLastName.Text },
                    { "@Mobile", txtMobile.Text },
                    { "@Email", txtEmail.Text },
                    { "@Catagory", cbbCatagory.Text },
                    { "@MobileToUpdate", mobileToUpdate }
                };

                // Thực hiện câu lệnh SQL và nhận số dòng bị ảnh hưởng
                int rowsAffected = ExecuteNonQuery(query, parameters);
                // Kiểm tra số dòng bị ảnh hưởng
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data updated successfully!");
                    // Hiển thị lại dữ liệu sau khi cập nhật
                    DisplayData();
                    // Xóa trắng các trường nhập liệu
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Failed to update data.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        // Sự kiện Click của nút Delete
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                // Lấy số điện thoại của dòng được chọn
                string mobileToDelete = selectedRow.Cells["Mobile"].Value.ToString();
                // Câu lệnh SQL xóa dữ liệu
                string query = "DELETE FROM MOBILES WHERE Mobile = @Mobile";
                // Tạo danh sách tham số cho câu lệnh SQL
                var parameters = new Dictionary<string, object>() { { "@Mobile", mobileToDelete } };
                // Thực hiện câu lệnh SQL và nhận số dòng bị ảnh hưởng
                int rowsAffected = ExecuteNonQuery(query, parameters);
                // Kiểm tra số dòng bị ảnh hưởng
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data deleted successfully!");
                    // Hiển thị lại dữ liệu sau khi xóa
                    DisplayData();
                    // Xóa trắng các trường nhập liệu
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Failed to delete data.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        // Sự kiện TextChanged của ô tìm kiếm
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Lấy giá trị nhập vào từ ô tìm kiếm
            string searchText = txtSearch.Text.Trim();

            // Nếu ô tìm kiếm không rỗng
            if (!string.IsNullOrEmpty(searchText))
            {
                // Tạo câu lệnh SQL để tìm kiếm dựa trên các trường dữ liệu
                string query = @"SELECT * FROM Mobiles WHERE 
                         FirstName LIKE @SearchText OR 
                         LastName LIKE @SearchText OR 
                         Mobile LIKE @SearchText OR 
                         Email LIKE @SearchText OR 
                         Catagory LIKE @SearchText";

                // Tạo danh sách tham số cho câu lệnh SQL
                var parameters = new Dictionary<string, object>()
                {
                    { "@SearchText", "%" + searchText + "%" }
                };

                // Thực hiện truy vấn SQL để lấy dữ liệu vào DataTable
                DataTable dt = ExecuteQuery(query, parameters);

                // Đặt dữ liệu vào DataGridView
                dataGridView1.DataSource = dt;
            }
            else
            {
                // Nếu ô tìm kiếm rỗng, hiển thị lại toàn bộ dữ liệu
                DisplayData();
            }
        }

        // Thực hiện truy vấn SQL và trả về DataTable
        private DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                // Tạo và thực thi truy vấn SQL
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số vào truy vấn SQL nếu có
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    // Sử dụng SqlDataAdapter để lấy dữ liệu vào DataTable
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while executing query: " + ex.Message);
            }
            return dt;
        }

        // Thực hiện truy vấn SQL không trả về dữ liệu
        private int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            int rowsAffected = 0;
            try
            {
                // Tạo và thực thi truy vấn SQL
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số vào truy vấn SQL nếu có
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    // Mở kết nối
                    conn.Open();
                    // Thực hiện truy vấn và lấy số dòng bị ảnh hưởng
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while executing non-query: " + ex.Message);
            }
            finally
            {
                // Đóng kết nối sau khi thực hiện truy vấn
                conn.Close();
            }
            return rowsAffected;
        }

    }
}



