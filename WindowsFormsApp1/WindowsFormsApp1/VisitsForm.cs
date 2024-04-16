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
    public partial class VisitsForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        public VisitsForm()
        {
            InitializeComponent();
        }


        private void VisitsForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();

        }

        private void PopulateDataGridView()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                string query = "SELECT visits.id, Passes.number AS pass_number, visits.date " +
                               "FROM visits " +
                               "INNER JOIN Passes ON visits.pass_id = Passes.id";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                    dataGridView1.DataSource = table;
                    dataGridView1.Columns["ID"].HeaderText = "ID";
                    dataGridView1.Columns["pass_number"].HeaderText = "НомерПропуска";
                    dataGridView1.Columns["date"].HeaderText = "Дата";
                }
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idTextBox.Text))
            {
                int id;
                if (int.TryParse(idTextBox.Text, out id))
                {
                    bool passIsActive = CheckPassIsActive();
                    if (!passIsActive)
                    {
                        MessageBox.Show("Пользователь с неактивным пропуском.");
                        return;
                    }

                    int passId = FindPassId();

                    if (passId == -1)
                    {
                        MessageBox.Show("Пропуск с указанным номером не найден.");
                        return;
                    }

                    DateTime visitDate;
                    if (DateTime.TryParse(dateTextBox.Text, out visitDate))
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
                        {
                            connection.Open();
                            string updateQuery = "UPDATE visits SET pass_id = @PassId, date = @Date WHERE id = @ID";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@PassId", passId);
                                cmd.Parameters.AddWithValue("@Date", visitDate);
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Значения обновлены успешно.");
                        PopulateDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат даты.");
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

        private bool CheckPassIsActive()
        {
            bool passIsActive = false;
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();
                string query = "SELECT \"isActivated\" FROM Passes WHERE number = @Number";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", numberTextBox.Text);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        passIsActive = Convert.ToBoolean(result);
                    }
                }
            }
            return passIsActive;
        }

        private int FindPassId()
        {
            int passId = -1;
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();
                string query = "SELECT id FROM Passes WHERE number = @Number";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", numberTextBox.Text);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        passId = Convert.ToInt32(result);
                    }
                }
            }
            return passId;
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
                        string deleteQuery = "DELETE FROM visits WHERE id = @ID";
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

                idTextBox.Text = row.Cells["id"].Value.ToString();
                numberTextBox.Text = row.Cells["pass_number"].Value.ToString();
                dateTextBox.Text = row.Cells["date"].Value.ToString();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            VisitsFormAdd add = new VisitsFormAdd();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.OK)
            {
                PopulateDataGridView();
            }
        }
    }
}
