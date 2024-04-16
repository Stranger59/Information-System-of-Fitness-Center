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
    public partial class PaymentsForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public PaymentsForm()
        {
            InitializeComponent();
        }

        private void PaymentsForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
            PopulateClientIdComboBox();
            PopulateServiceIdComboBox();
            PopulateMethodComboBox();
        }
        private void PopulateMethodComboBox()
        {
            methodComboBox.Items.Add("Наличный");
            methodComboBox.Items.Add("Безналичный");
        }
        private void PopulateDataGridView()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT p.id AS ID, " +
                               "c.first_name || ' ' || c.last_name AS Клиент, " +
                               "s.name AS Услуга, " +
                               "p.date AS Дата, " +
                               "p.method AS Метод_оплаты " +
                               "FROM payments p " +
                               "INNER JOIN clients c ON p.client_id = c.id " +
                               "INNER JOIN services s ON p.services_id = s.id";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void PopulateClientIdComboBox()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT id, first_name || ' ' || last_name AS full_name FROM clients";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    connection.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        clientIdComboBox.Items.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
        }

        private void PopulateServiceIdComboBox()
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT id, name FROM services";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    connection.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        serviceIdComboBox.Items.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                idTextBox.Text = row.Cells["ID"].Value.ToString();
                string clientName = row.Cells["Клиент"].Value.ToString();
                string serviceName = row.Cells["Услуга"].Value.ToString();
                dateTextBox.Text = row.Cells["Дата"].Value.ToString();
                methodComboBox.SelectedItem = row.Cells["Метод_оплаты"].Value.ToString();

                foreach (KeyValuePair<int, string> item in clientIdComboBox.Items)
                {
                    if (item.Value == clientName)
                    {
                        clientIdComboBox.SelectedItem = item;
                        break;
                    }
                }

                foreach (KeyValuePair<int, string> item in serviceIdComboBox.Items)
                {
                    if (item.Value == serviceName)
                    {
                        serviceIdComboBox.SelectedItem = item;
                        break;
                    }
                }
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
                        string deleteQuery = "DELETE FROM payments WHERE id = @ID";
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

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;
                if (int.TryParse(idTextBox.Text, out id))
                {
                    int clientId = ((KeyValuePair<int, string>)clientIdComboBox.SelectedItem).Key;
                    int serviceId = ((KeyValuePair<int, string>)serviceIdComboBox.SelectedItem).Key;
                    DateTime date;
                    if (DateTime.TryParse(dateTextBox.Text, out date))
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
                            dataGridView1.Rows[rowIndex].Cells["Клиент"].Value = clientIdComboBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Услуга"].Value = serviceIdComboBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Дата"].Value = dateTextBox.Text;
                            dataGridView1.Rows[rowIndex].Cells["Метод_оплаты"].Value = methodComboBox.SelectedItem.ToString();

                            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                            {
                                connection.Open();
                                string updateQuery = "UPDATE payments SET client_id = @ClientID, services_id = @ServiceID, date = @Date, method = @Method WHERE id = @ID";
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, connection))
                                {
                                    cmd.Parameters.AddWithValue("@ClientID", clientId);
                                    cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                                    cmd.Parameters.AddWithValue("@Date", date);
                                    cmd.Parameters.AddWithValue("@Method", methodComboBox.SelectedItem.ToString());
                                    cmd.Parameters.AddWithValue("@ID", id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            PopulateDataGridView();
                            MessageBox.Show("Значения обновлены успешно.");
                        }
                        else
                        {
                            MessageBox.Show("Запись с указанным ID не найдена.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Некорректный формат даты.");
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
        private void addButton_Click(object sender, EventArgs e)
        {
            PaymentsFormAdd add = new PaymentsFormAdd();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.OK)
            {
                PopulateDataGridView();
            }
        }
    }
}
