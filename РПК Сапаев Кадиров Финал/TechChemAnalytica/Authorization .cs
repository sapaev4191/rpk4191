using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TechChemAnalytica
{
    public partial class Authorization : Form
    {
        private string placeholderTextLogin = "Пользователь";
        private string placeholderTextPass = "Пароль";
       // private SqlConnection sqlConnection = null;
        public Authorization()
        {
            InitializeComponent();
            InitializePlaceholders();
            this.Shown += new EventHandler(Authorization_Shown); // Подписка на событие Shown
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Это зафиксирует окно
            this.Size = new Size(333, 288); // Устанавливаем начальный размер
        }

        private void Authorization_Shown(object sender, EventArgs e)
        {
            // Убираем фокус с полей ввода, устанавливая его на кнопку
            AuthButton.Focus();
        }

        private void InitializePlaceholders()
        {
            // Установка предтекстов и цветового оформления
            SetPlaceholder(BoxLogin, placeholderTextLogin);
            SetPlaceholder(BoxPass, placeholderTextPass);
        }

        private void SetPlaceholder(System.Windows.Forms.TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.ForeColor = Color.Gray;

            // Подписка на события
            textBox.Leave += (sender, e) => HandleLeave(textBox, placeholderText);
            textBox.Enter += (sender, e) => HandleEnter(textBox, placeholderText);
        }

        private void HandleEnter(System.Windows.Forms.TextBox textBox, string placeholderText)
        {
            // Если текст - это предтекст, очистить поле
            if (textBox.Text == placeholderText)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }

        private void HandleLeave(System.Windows.Forms.TextBox textBox, string placeholderText)
        {
            // Если поле пустое, вернуть предтекст
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholderText;
                textBox.ForeColor = Color.Gray;
            }
        }
        public static class Admin
        {
            public static bool Change {get; set;}
        }
        private void AuthButton_Click(object sender, EventArgs e)
        {
            
            string username = BoxLogin.Text;
            string password = BoxPass.Text;

            int accessLevel = CheckUserCredentials(username, password);

            if (accessLevel != 0)
            {
                Interface interfaceOpen = new Interface(accessLevel, username);
                interfaceOpen.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private int CheckUserCredentials(string username, string password)
        {
            int accessLevel = 0;

            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString))
            {
                string query = "SELECT AccessLevel FROM DataUsersDB WHERE UserName = @UserName AND UserPassword = @UserPassword";
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.AddWithValue("@UserName", username);
                command.Parameters.AddWithValue("@UserPassword", password);

                sqlConnection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    accessLevel = Convert.ToInt32(result);
                }
                return accessLevel;
            }

        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Authorization_Resize(object sender, EventArgs e)
        {

        }
    }
}
