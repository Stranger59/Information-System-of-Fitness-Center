using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace WindowsFormsApp1
{
    public partial class ClientForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }
        private void PopulateDataGridView()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT id AS ID, first_name AS Имя, last_name AS Фамилия, phone AS Телефон FROM Clients";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                idTextBox.Text = row.Cells["ID"].Value.ToString();
                nameTextBox.Text = row.Cells["Имя"].Value.ToString();
                secondNameTextBox.Text = row.Cells["Фамилия"].Value.ToString();
                numberTextBox.Text = row.Cells["Телефон"].Value.ToString();
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;
                
                if (int.TryParse(idTextBox.Text, out id))
                {
                    
                    if (numberTextBox.Text.Length == 10)
                    {
                        
                        int rowIndex = -1;
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["ID"].Value != null && Convert.ToInt32(row.Cells["ID"].Value) == id)
                            {
                                rowIndex = row.Index;
                                break;
                            }
                        }

                        
                        if (rowIndex != -1)
                        {
                           
                            dataGridView1.Rows[rowIndex].Cells["Имя"].Value = nameTextBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Фамилия"].Value = secondNameTextBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Телефон"].Value = numberTextBox.Text;

                            
                            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                            {
                                connection.Open();
                                string updateQuery = "UPDATE Clients SET first_name = @FirstName, last_name = @LastName, phone = @Phone WHERE id = @ID";
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, connection))
                                {
                                    cmd.Parameters.AddWithValue("@FirstName", nameTextBox.Text);
                                    cmd.Parameters.AddWithValue("@LastName", secondNameTextBox.Text);
                                    cmd.Parameters.AddWithValue("@Phone", numberTextBox.Text);
                                    cmd.Parameters.AddWithValue("@ID", id);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show("Значения обновлены успешно.");
                        }
                        else
                        {
                            MessageBox.Show("Запись с указанным ID не найдена.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Номер телефона должен состоять из 10 цифр.");
                    }
                }
                else
                {
                    MessageBox.Show("ID должен быть числом.");
                }
            }
            else
            {
                MessageBox.Show("Введите ID записи для обновления.");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;
                
                if (int.TryParse(idTextBox.Text, out id))
                {
                    
                    using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                    {
                        connection.Open();
                        string deleteQuery = "DELETE FROM Clients WHERE id = @ID";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@ID", id);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Запись удалена успешно из базы данных.");
                            }
                            else
                            {
                                MessageBox.Show("Запись с указанным ID не найдена в базе данных.");
                            }
                        }
                    }

                    
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["ID"].Value != null && Convert.ToInt32(row.Cells["ID"].Value) == id)
                        {
                            dataGridView1.Rows.Remove(row);
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("ID должен быть числом.");
                }
            }
            else
            {
                MessageBox.Show("Введите ID записи для удаления.");
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ClientFormAdd add = new ClientFormAdd();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.OK)
            {
                PopulateDataGridView();
            }
        }
        
    }
}
