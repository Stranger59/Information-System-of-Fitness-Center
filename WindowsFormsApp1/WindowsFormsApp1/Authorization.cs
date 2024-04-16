using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Configuration;
using System.Collections;

namespace WindowsFormsApp1
{
    public partial class Authorization : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public Authorization()
        {
            InitializeComponent();
        }

        private void Authorization_Load(object sender, EventArgs e)
        {
            textBoxPassword.PasswordChar = '*';

         
            textBoxLogin.KeyPress += new KeyPressEventHandler(TextBoxLogin_KeyPress);

            textBoxPassword.KeyPress += new KeyPressEventHandler(TextBoxPassword_KeyPress);
        }

        private void TextBoxLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PerformLogin();
            }
        }

        private void TextBoxPassword_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {
                
                PerformLogin();
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;

            if (ValidateUser(login, password))
            {
                MainForm main = new MainForm(login);
                main.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Такой пользователь не существует или не одобрен администратором");
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool ValidateUser(string login, string password)
        {
            string query = "SELECT \"isAllowed\" FROM users WHERE login = @Login AND password = @Password";

            bool isAllowed = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isAllowed = reader.GetBoolean(0);
                        }
                    }
                }
            }
            return isAllowed;
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterForm reg = new RegisterForm();
            reg.ShowDialog();
        }
    }
}
