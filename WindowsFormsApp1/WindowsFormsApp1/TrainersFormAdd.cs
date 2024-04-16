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
    public partial class TrainersFormAdd : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public TrainersFormAdd()
        {
            InitializeComponent();
        }

        private void TrainersFormAdd_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO trainers (first_name, last_name, specialization) VALUES (@FirstName, @LastName, @Specialization)";

            try
            {

                using (NpgsqlConnection conn = new NpgsqlConnection(sqlCon))
                {
                    conn.Open();
                    string firstName = nameTextBox.Text;
                    string lastName = secondNameTextBox.Text;
                    string specialization = specializationTextBox.Text;
                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(specialization))
                    {
                        MessageBox.Show("Имя, фамилия и специализация не могут быть пустыми.");
                        return;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {

                        cmd.Parameters.AddWithValue("@FirstName", nameTextBox.Text);
                        cmd.Parameters.AddWithValue("@LastName", secondNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@Specialization", specializationTextBox.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            this.DialogResult = DialogResult.OK;
                            MessageBox.Show("Данные успешно добавлены в базу данных.");


                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить данные в базу данных.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
