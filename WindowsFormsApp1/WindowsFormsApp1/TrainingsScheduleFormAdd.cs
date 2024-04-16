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

    public partial class TrainingsScheduleFormAdd : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public TrainingsScheduleFormAdd()
        {
            InitializeComponent();
        }
        private void TrainingsScheduleFormAdd_Load(object sender, EventArgs e)
        {
            PopulateClientIdComboBox();
            PopulateTrainerIdComboBox();
            PopulateServiceIdComboBox();
            dateTextBox.Text = DateTime.Now.ToString();
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

        

        

        private void okButton_Click(object sender, EventArgs e)
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
                    string insertQuery = "INSERT INTO trainings_schedule (client_id, trainer_id, service_id, date) VALUES (@ClientID, @TrainerID, @ServiceID, @Date)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientId);
                        cmd.Parameters.AddWithValue("@TrainerID", trainerId);
                        cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Данные успешно добавлены в базу данных.");
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Некорректный формат даты.");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
