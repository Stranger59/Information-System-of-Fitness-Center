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
    public partial class TrainingsScheduleForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public TrainingsScheduleForm()
        {
            InitializeComponent();
        }

        private void TrainingsScheduleForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
            PopulateClientIdComboBox();
            PopulateTrainerIdComboBox();
            PopulateServiceIdComboBox();
        }

        private void PopulateDataGridView()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT ts.id AS ID, " +
                               "c.first_name || ' ' || c.last_name AS Клиент, " +
                               "t.first_name || ' ' || t.last_name AS Тренер, " +
                               "s.name AS Услуга, " +
                               "ts.date AS Дата " +
                               "FROM trainings_schedule ts " +
                               "INNER JOIN clients c ON ts.client_id = c.id " +
                               "INNER JOIN trainers t ON ts.trainer_id = t.id " +
                               "INNER JOIN services s ON ts.service_id = s.id";

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

        private void PopulateTrainerIdComboBox()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT id, first_name || ' ' || last_name AS full_name FROM trainers";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    connection.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        trainerIdcomboBox.Items.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
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

        private void TrainingsScheduleFormAdd_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
            PopulateClientIdComboBox();
            PopulateTrainerIdComboBox();
            PopulateServiceIdComboBox();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;
                if (int.TryParse(idTextBox.Text, out id))
                {
                    int clientId = ((KeyValuePair<int, string>)clientIdComboBox.SelectedItem).Key;
                    int trainerId = ((KeyValuePair<int, string>)trainerIdcomboBox.SelectedItem).Key;
                    int serviceId = ((KeyValuePair<int, string>)serviceIdComboBox.SelectedItem).Key;
                    DateTime date;
                    if (DateTime.TryParse(dateTextBox.Text, out date))
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                        {
                            connection.Open();
                            string updateQuery = "UPDATE trainings_schedule SET client_id = @ClientID, trainer_id = @TrainerID, service_id = @ServiceID, date = @Date WHERE id = @ID";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", clientId);
                                cmd.Parameters.AddWithValue("@TrainerID", trainerId);
                                cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                                cmd.Parameters.AddWithValue("@Date", date);
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        PopulateDataGridView(); 
                        MessageBox.Show("Значения обновлены успешно.");
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
                        string deleteQuery = "DELETE FROM trainings_schedule WHERE id = @ID";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@ID", id);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Запись удалена успешно из базы данных.");
                                PopulateDataGridView();
                            }
                            else
                            {
                                MessageBox.Show("Запись с указанным ID не найдена в базе данных.");
                            }
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

        

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                idTextBox.Text = row.Cells["ID"].Value.ToString();
                string clientName = row.Cells["Клиент"].Value.ToString();
                string trainerName = row.Cells["Тренер"].Value.ToString();
                string serviceName = row.Cells["Услуга"].Value.ToString();
                dateTextBox.Text = row.Cells["Дата"].Value.ToString();

                foreach (KeyValuePair<int, string> item in clientIdComboBox.Items)
                {
                    if (item.Value == clientName)
                    {
                        clientIdComboBox.SelectedItem = item;
                        break;
                    }
                }

                foreach (KeyValuePair<int, string> item in trainerIdcomboBox.Items)
                {
                    if (item.Value == trainerName)
                    {
                        trainerIdcomboBox.SelectedItem = item;
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
        private void addButton_Click(object sender, EventArgs e)
        {
            TrainingsScheduleFormAdd add = new TrainingsScheduleFormAdd();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.OK)
            {
                PopulateDataGridView();
            }
        }
    }
}
