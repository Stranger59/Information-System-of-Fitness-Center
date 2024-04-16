using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ServicesFormAdd : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;

        public ServicesFormAdd()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO services (name, description, price) VALUES (@Name, @Description, @Price)";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(sqlCon))
                {
                    conn.Open();
                    string name = nameTextBox.Text;
                    string description = descriptionTextBox.Text;
                    string price = priceTextBox.Text;

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(price))
                    {
                        MessageBox.Show("Имя, описание и цена не могут быть пустыми.");
                        return;
                    }

                    Regex regex = new Regex(@"^\d+(\,\d{0,2})?$");
                    if (!regex.IsMatch(price))
                    {
                        MessageBox.Show("Цена должна быть числом с максимум двумя знаками после запятой.");
                        return;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", nameTextBox.Text);
                        cmd.Parameters.AddWithValue("@Description", descriptionTextBox.Text);
                        cmd.Parameters.AddWithValue("@Price", decimal.Parse(priceTextBox.Text));

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
