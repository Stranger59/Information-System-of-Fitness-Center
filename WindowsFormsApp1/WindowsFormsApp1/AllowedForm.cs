using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    public partial class AllowedForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        private int userId;

        public AllowedForm()
        {
            InitializeComponent();
        }

        private void AllowedForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
            
        }
        private void LoadUsers()
        {
            using (var connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();

                string sql = "SELECT id, login, password FROM users WHERE \"isAllowed\" = false";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void buttonAllow_Click(object sender, EventArgs e)
        {
            if (userId != 0)
            {
                using (var connection = new NpgsqlConnection(sqlCon))
                {
                    connection.Open();

                    string sql = "UPDATE users SET \"isAllowed\" = true WHERE id = @userId";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("userId", userId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Разрешение пользователя обновлено.");
                            LoadUsers();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить разрешение пользователя.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для разрешения.");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                userId = Convert.ToInt32(row.Cells["id"].Value); 
            }
        }
    }
}
