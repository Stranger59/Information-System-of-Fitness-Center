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
using System.Numerics;
namespace WindowsFormsApp1
{
    public partial class ClientFormAdd : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public ClientFormAdd()
        {
            InitializeComponent();
        }

        private void ClientFormAdd_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO clients (first_name, last_name, phone) VALUES (@FirstName, @LastName, @Phone)";

            try
            {
                
                using (NpgsqlConnection conn = new NpgsqlConnection(sqlCon))
                {
                    conn.Open();
                    string firstName = nameTextBox.Text;
                    string lastName = secondNameTextBox.Text;
                    string phoneNumber = numberTextBox.Text;
                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phoneNumber))
                    {
                        MessageBox.Show("Имя, фамилия и номер телефона не могут быть пустыми.");
                        return; 
                    }

                    if (!BigInteger.TryParse(phoneNumber, out _) || phoneNumber.Length != 10)
                        
                    {
                        MessageBox.Show("Номер телефона должен состоять из ровно 10 цифр и не иметь букв/символов");
                        return; 
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@FirstName", nameTextBox.Text);
                        cmd.Parameters.AddWithValue("@LastName", secondNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@Phone", numberTextBox.Text);

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
