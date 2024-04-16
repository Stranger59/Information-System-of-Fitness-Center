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
    public partial class VisitsFormAdd : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public VisitsFormAdd()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(numberTextBox.Text) && !string.IsNullOrEmpty(dateTextBox.Text))
            {
              
                bool passExists = CheckPassExists(numberTextBox.Text);
                bool passIsActive = passExists ? CheckPassIsActive(numberTextBox.Text) : false;

                if (passExists && passIsActive)
                {
                    
                    int passId = FindPassId(numberTextBox.Text);
                    DateTime visitDate;
                    if (DateTime.TryParse(dateTextBox.Text, out visitDate))
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                        {
                            connection.Open();
                            string insertQuery = "INSERT INTO visits (pass_id, date) VALUES (@PassId, @Date)";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@PassId", passId);
                                cmd.Parameters.AddWithValue("@Date", visitDate);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Запись успешно добавлена.");
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат даты.");
                    }
                }
                else
                {
                    MessageBox.Show("Пропуск с указанным номером не существует или не активен.");
                }
            }
            else
            {
                MessageBox.Show("Введите номер пропуска и дату.");
            }
        }
        private bool CheckPassExists(string passNumber)
        {
            bool passExists = false;
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Passes WHERE number = @Number";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", passNumber);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    passExists = count > 0;
                }
            }
            return passExists;
        }

        private bool CheckPassIsActive(string passNumber)
        {
            bool passIsActive = false;
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();
                string query = "SELECT \"isActivated\" FROM Passes WHERE number = @Number";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", passNumber);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        passIsActive = Convert.ToBoolean(result);
                    }
                }
            }
            return passIsActive;
        }

        private int FindPassId(string passNumber)
        {
            int passId = -1;
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();
                string query = "SELECT id FROM Passes WHERE number = @Number";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", passNumber);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        passId = Convert.ToInt32(result);
                    }
                }
            }
            return passId;
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
