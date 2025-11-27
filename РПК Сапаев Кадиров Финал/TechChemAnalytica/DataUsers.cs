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

namespace TechChemAnalytica
{
    public partial class DataUsers : Form
    {
        private SqlConnection sqlConnection = null;
        public DataUsers()
        {
            InitializeComponent();
            this.dataGridUsers.CellClick += new DataGridViewCellEventHandler(this.dataGridUsers_CellClick);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Это зафиксирует окно
            this.Size = new Size(778, 258); // Устанавливаем начальный размер
        }

        private void DataUsers_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString);
            sqlConnection.Open();
            if (sqlConnection.State == ConnectionState.Open)
            {
                MessageBox.Show("Загрузка формы. Подождите...");
            }
            LoadData();
        }

        public void LoadData()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM DataUsersDB", sqlConnection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridUsers.DataSource = dataSet.Tables[0];
            dataGridUsers.Columns[0].HeaderText = "ID";          // Первое название столбца
            dataGridUsers.Columns[1].HeaderText = "Имя пользователя";        // Второе название столбца
            dataGridUsers.Columns[2].HeaderText = "Пароль";     // Третье название столбца
            dataGridUsers.Columns[3].HeaderText = "Уровень доступа"; // Четвертое название столбца
            
        }

        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            

            
            SqlCommand command = new SqlCommand(
                $"INSERT INTO [DataUsersDB] (UserName, UserPassword, AccessLevel) VALUES (@UserName, @UserPassword, @AccessLevel)",
                sqlConnection
                );

            command.Parameters.AddWithValue("UserName", boxUserName.Text);
            command.Parameters.AddWithValue("UserPassword", boxUserPass.Text);
            command.Parameters.AddWithValue("AccessLevel", comboAccessLVL.Text);
            



            if (command.ExecuteNonQuery() != 0)
            {
                LoadData();
                MessageBox.Show("Пользователь добавлен!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            else
            {
                MessageBox.Show("Ошибка добавления пользователя!","Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUpdUser_Click(object sender, EventArgs e)
        {
            // Убедитесь, что выбрана строка в DataGridView
            if (dataGridUsers.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridUsers.SelectedRows[0];

                // Получаем идентификатор (или ключ) выбранной строки
                int id = Convert.ToInt32(selectedRow.Cells["ID"].Value); // Замените "ID" на имя вашего столбца с ключом

                // Подготовка команды обновления
                SqlCommand command = new SqlCommand(
                "UPDATE DataUsersDB SET UserName = @UserName, UserPassword = @UserPassword, AccessLevel = @AccessLevel WHERE ID = @ID", // Замените "ID" на имя вашего столбца с ключом
                sqlConnection
                );

                // Заполнение параметров
                command.Parameters.AddWithValue("UserName", boxUserName.Text);
                command.Parameters.AddWithValue("UserPassword", boxUserPass.Text);
                command.Parameters.AddWithValue("AccessLevel", comboAccessLVL.Text);
                command.Parameters.AddWithValue("@ID", id); // Передаем ID записи для обновления

                // Выполнение команды
                if (command.ExecuteNonQuery() != 0)
                {
                    LoadData(); // Обновляем DataGridView
                    MessageBox.Show("Пользователь обновлен!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Ошибка обновления пользователь!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для обновления!","Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonDelUser_Click(object sender, EventArgs e)
        {
            if (dataGridUsers.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridUsers.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value); // Получаем Id выбранной строки

                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить Пользователя?",
                                                     "Подтверждение удаления",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    // SQL-команда для удаления
                    using (SqlCommand command = new SqlCommand("DELETE FROM [DataUsersDB] WHERE Id = @Id", sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Id", id); // Добавляем Id в параметры команды

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                LoadData(); // Обновляем данные в grid
                                MessageBox.Show("Пользователь удален!","Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                MessageBox.Show("Ошибка: ни одна строка не была удалена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, выбрана ли правильная ячейка (не заголовок строки и не заголовок столбца)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridUsers.Rows[e.RowIndex];

                // Заполняем TextBox значением из ячейки UserName (предполагая, что первый столбец - UserName)
                boxUserName.Text = selectedRow.Cells["UserName"].Value.ToString();

                // Если у вас есть другие поля, также можно заполнять их тут
                boxUserPass.Text = selectedRow.Cells["UserPassword"].Value.ToString();
                comboAccessLVL.Text = selectedRow.Cells["AccessLevel"].Value.ToString();
            }
        }
    }
}
