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
    public partial class PassesFormAdd : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public PassesFormAdd()
        {
            InitializeComponent();
            isActivatedComboBox.Items.AddRange(new string[] { "True", "False" });
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string firstName = nameTextBox.Text;
            string lastName = secondNameTextBox.Text;
            string number = numberTextBox.Text;
            string isActivated = isActivatedComboBox.SelectedItem.ToString();

            if (number.Length != 6)
            {
                MessageBox.Show("Номер должен содержать 6 символов.");
                return;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();

                string checkClientQuery = "SELECT id FROM Clients WHERE first_name = @FirstName AND last_name = @LastName";
                using (NpgsqlCommand command = new NpgsqlCommand(checkClientQuery, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);

                    object clientId = command.ExecuteScalar();

                    if (clientId != null)
                    {
                        int clientIdValue = Convert.ToInt32(clientId);

                        string checkPassQuery = "SELECT COUNT(*) FROM Passes WHERE number = @Number";
                        using (NpgsqlCommand checkPassCommand = new NpgsqlCommand(checkPassQuery, connection))
                        {
                            checkPassCommand.Parameters.AddWithValue("@Number", number);

                            int count = Convert.ToInt32(checkPassCommand.ExecuteScalar());

                            if (count > 0)
                            {
                                MessageBox.Show("Пропуск с таким номером уже существует.");
                                return;
                            }
                        }

                        string insertPassQuery = "INSERT INTO Passes (client_id, number, \"isActivated\") VALUES (@ClientId, @Number, @IsActivated)";
                        using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertPassQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@ClientId", clientIdValue);
                            insertCommand.Parameters.AddWithValue("@Number", number);
                            insertCommand.Parameters.AddWithValue("@IsActivated", isActivated == "True"); 

                            insertCommand.ExecuteNonQuery();
                        }
                        this.DialogResult = DialogResult.OK;
                        MessageBox.Show("Пропуск успешно добавлен.");
                    }
                    else
                    {
                        MessageBox.Show("Клиент с таким именем и фамилией не найден.");
                    }
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PassesFormAdd_Load(object sender, EventArgs e)
        {

        }
    }
}
