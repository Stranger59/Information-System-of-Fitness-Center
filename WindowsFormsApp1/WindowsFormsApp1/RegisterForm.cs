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
using Npgsql;

namespace WindowsFormsApp1
{
    public partial class RegisterForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            textBoxPasswordReg.PasswordChar = '*';

            textBoxLoginReg.KeyPress += new KeyPressEventHandler(TextBoxLoginReg_KeyPress);

            textBoxPasswordReg.KeyPress += new KeyPressEventHandler(TextBoxPasswordReg_KeyPress);
        }

        private void TextBoxLoginReg_KeyPress(object sender, KeyPressEventArgs e)
        {
      
            if (e.KeyChar == (char)Keys.Enter)
            {
                
                RegisterUser();
            }
        }

        private void TextBoxPasswordReg_KeyPress(object sender, KeyPressEventArgs e)
        {
          
            if (e.KeyChar == (char)Keys.Enter)
            {
                
                RegisterUser();
            }
        }

        private void regButton_Click(object sender, EventArgs e)
        {
            
            RegisterUser();
        }

        private void RegisterUser()
        {
            string login = textBoxLoginReg.Text;
            string password = textBoxPasswordReg.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми");
                return;
            }
            if (UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return;
            }

            if (RegisterUser(login, password))
            {
                MessageBox.Show("Пользователь успешно зарегистрирован, ожидайте подтверждения администратора");
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка регистрации пользователя");
            }
        }

        private bool UserExists(string login)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT COUNT(*) FROM users WHERE login = @Login";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int rowCount = Convert.ToInt32(result);
                        return rowCount > 0;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private bool RegisterUser(string login, string password)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "INSERT INTO users (login, password, \"isAdmin\", \"isAllowed\") VALUES (@Login, @Password, false, false)";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
