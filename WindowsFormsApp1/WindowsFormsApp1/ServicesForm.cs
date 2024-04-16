using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ServicesForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public ServicesForm()
        {
            InitializeComponent();
        }
        
        private void ServicesForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }
        private void PopulateDataGridView()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT id AS ID, name AS Название, description AS Описание, price AS Цена FROM Services";

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
                nameTextBox.Text = row.Cells["Название"].Value.ToString();
                descriptionTextBox.Text = row.Cells["Описание"].Value.ToString();
                priceTextBox.Text = row.Cells["Цена"].Value.ToString();
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^\d+(\,\d{0,2})?$");
            if (!regex.IsMatch(priceTextBox.Text))
            {
                MessageBox.Show("Цена должна быть числом с максимум двумя знаками после запятой.");
                return;
            }

            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;

                if (int.TryParse(idTextBox.Text, out id))
                {
                    decimal price;

                    if (decimal.TryParse(priceTextBox.Text, out price))
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
                            dataGridView1.Rows[rowIndex].Cells["Название"].Value = nameTextBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Описание"].Value = descriptionTextBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Цена"].Value = priceTextBox.Text;

                            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                            {
                                connection.Open();
                                string updateQuery = "UPDATE Services SET name = @Name, description = @Description, price = @Price WHERE id = @ID";
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, connection))
                                {
                                    cmd.Parameters.AddWithValue("@Name", nameTextBox.Text);
                                    cmd.Parameters.AddWithValue("@Description", descriptionTextBox.Text);
                                    cmd.Parameters.AddWithValue("@Price", price);
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
                        MessageBox.Show("Цена должна быть числом.");
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
                        string deleteQuery = "DELETE FROM Services WHERE id = @ID";
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
            ServicesFormAdd add = new ServicesFormAdd();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.OK)
            {
                PopulateDataGridView();
            }
        }
    }
}
