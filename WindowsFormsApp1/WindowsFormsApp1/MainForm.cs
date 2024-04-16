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
using System.Diagnostics.Eventing.Reader;

namespace WindowsFormsApp1
{

    public partial class MainForm : Form
    {
        string sqlCon = ConfigurationManager.ConnectionStrings["Name"].ConnectionString;
        private string userLogin;
        public MainForm(string login)
        {
            InitializeComponent();
            userLogin = login;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if(activeForm != null)
            {
                activeForm.Close();
            }
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panel2.Controls.Add(childForm);
            panel2.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        private void clientsButton_Click(object sender, EventArgs e)
        {
            openChildForm(new ClientForm());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openChildForm(new PassesForm());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openChildForm(new ServicesForm());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openChildForm(new PaymentsForm());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openChildForm(new TrainersForm());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openChildForm(new TrainingsScheduleForm());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openChildForm(new VisitsForm());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            bool isAdmin = IsAdmin(userLogin);

            if (isAdmin)
            {
                openChildForm(new AllowedForm());
            }
            else
            {
                MessageBox.Show("Пользователь не является администратором.");
            }
        }
        private bool IsAdmin(string userLogin)
        {
            using (var connection = new NpgsqlConnection(sqlCon))
            {
                connection.Open();

                string sql = "SELECT \"isAdmin\" FROM users WHERE login = @userLogin";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("userLogin", userLogin);

                    bool isAdmin = (bool)cmd.ExecuteScalar();
                    return isAdmin;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
