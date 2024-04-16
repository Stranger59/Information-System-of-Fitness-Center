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
    public partial class PassesForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public PassesForm()
        {
            InitializeComponent();
            isActivatedComboBox.Items.AddRange(new string[] { "True", "False" });

        }

        private void PassesForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void PopulateDataGridView()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT Passes.id AS ID, CONCAT(Clients.first_name, ' ', Clients.last_name) AS Клиент, Passes.number AS Номер, \"isActivated\" AS Активирован " +
                               "FROM Passes " +
                               "INNER JOIN Clients ON Passes.client_id = Clients.id";

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
                numberTextBox.Text = row.Cells["Номер"].Value.ToString();
                isActivatedComboBox.SelectedItem = Convert.ToBoolean(row.Cells["Активирован"].Value) ? "True" : "False";
            }
        }

        private void updateButton_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;

                if (int.TryParse(idTextBox.Text, out id))
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
                        int number;
                        if (int.TryParse(numberTextBox.Text, out number) && numberTextBox.Text.Length == 6)
                        {
                            dataGridView1.Rows[rowIndex].Cells["Номер"].Value = numberTextBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Активирован"].Value = isActivatedComboBox.SelectedItem.ToString();

                            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                            {
                                connection.Open();
                                string updateQuery = "UPDATE Passes SET number = @Number, \"isActivated\" = @IsActivated WHERE id = @ID";
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, connection))
                                {
                                    cmd.Parameters.AddWithValue("@Number", numberTextBox.Text);
                                    cmd.Parameters.AddWithValue("@IsActivated", isActivatedComboBox.SelectedItem.ToString() == "True");
                                    cmd.Parameters.AddWithValue("@ID", id);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show("Значения обновлены успешно.");
                        }
                        else
                        {
                            MessageBox.Show("Номер пропуска должен быть числом и его длина должна быть равна 6.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Запись с указанным ID не найдена.");
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

        private void deleteButton_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;

                if (int.TryParse(idTextBox.Text, out id))
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                    {
                        connection.Open();
                        string deleteQuery = "DELETE FROM Passes WHERE id = @ID";
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
            PassesFormAdd add = new PassesFormAdd();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.OK)
            {
                PopulateDataGridView();
            }
        }
    }
}
