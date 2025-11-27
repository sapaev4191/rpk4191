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
using static TechChemAnalytica.Authorization;

namespace TechChemAnalytica
{

    public partial class MaterialManager : Form
    {
        private SqlConnection sqlConnection = null;

        private Interface interfaceForm;
        private Action loadMaterials;
        public MaterialManager(Interface interfaceForm, Action loadMaterials)
        {
            InitializeComponent();
            this.interfaceForm = interfaceForm;
            this.loadMaterials = loadMaterials; // Сохраняем делегат
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            this.dataGridMaterial.CellClick += new DataGridViewCellEventHandler(this.dataGridMaterial_CellClick);
            this.Size = new Size(796, 707); // Устанавливаем начальный размер

            BoxNameMaterial.TextChanged += Box_TextChanged;
            BoxPgMin.TextChanged += Box_TextChanged;
            BoxPgMax.TextChanged += Box_TextChanged;
            BoxPgStep.TextChanged += Box_TextChanged;
            BoxtMin.TextChanged += Box_TextChanged;
            BoxtMax.TextChanged += Box_TextChanged;
            BoxtStep.TextChanged += Box_TextChanged;
            BoxNameMaterial.TextChanged += Box_TextChanged;
            BoxB0.TextChanged += Box_TextChanged;
            BoxB1.TextChanged += Box_TextChanged;
            BoxB2.TextChanged += Box_TextChanged;
            BoxB3.TextChanged += Box_TextChanged;
            BoxB4.TextChanged += Box_TextChanged;
            BoxB5.TextChanged += Box_TextChanged;
            BoxB6.TextChanged += Box_TextChanged;
            BoxB7.TextChanged += Box_TextChanged;
            BoxB8.TextChanged += Box_TextChanged;

            BBoxNameMaterial.TextChanged += Box_TextChanged;
            BBoxPgMin.TextChanged += Box_TextChanged;
            BBoxPgMax.TextChanged += Box_TextChanged;
            BBoxPgStep.TextChanged += Box_TextChanged;
            BBoxtMin.TextChanged += Box_TextChanged;
            BBoxtMax.TextChanged += Box_TextChanged;
            BBoxtStep.TextChanged += Box_TextChanged;
            BBoxB0.TextChanged += Box_TextChanged;
            BBoxB1.TextChanged += Box_TextChanged;
            BBoxB2.TextChanged += Box_TextChanged;
            BBoxB3.TextChanged += Box_TextChanged;
            BBoxB4.TextChanged += Box_TextChanged;
            BBoxB5.TextChanged += Box_TextChanged;
            BBoxB6.TextChanged += Box_TextChanged;
            BBoxB7.TextChanged += Box_TextChanged;
            BBoxB8.TextChanged += Box_TextChanged;

        }

        private void MaterialManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            loadMaterials?.Invoke();
            interfaceForm.ShowForm(); // Возвращаем Interface

        }



        public void LoadData()
        {
            //SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT NameMaterial, PgMin, PgMax, PgStep, tMin, tMax, tStep, b0,  b1, b2, b3, b4, b5, b6, b7, b8 FROM AllSpeSpecifications", sqlConnection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM AllSpeSpecifications", sqlConnection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridMaterial.DataSource = dataSet.Tables[0];
        }

        private void MaterialManager_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString);
            sqlConnection.Open();
            this.FormClosed += MaterialManager_FormClosed; // Подписка на событие закрытия
            if (sqlConnection.State == ConnectionState.Open)
            {
                MessageBox.Show("Загрузка формы. Подождите...");
            }

            LoadData();

        }

        private void InsertDB_Click(object sender, EventArgs e)
        {
            
            
            SqlCommand command = new SqlCommand(
                $"INSERT INTO [AllSpeSpecifications] (NameMaterial, PgMin, PgMax, PgStep, tMin, tMax, tStep, b0,  b1, b2, b3, b4, b5, b6, b7, b8) VALUES (@NameMaterial, @PgMin, @PgMax, @PgStep, @tMin, @tMax, @tStep, @b0, @b1, @b2, @b3, @b4, @b5, @b6, @b7, @b8)",
                sqlConnection
                );

            command.Parameters.AddWithValue("NameMaterial", BoxNameMaterial.Text);
            command.Parameters.AddWithValue("PgMin", BoxPgMin.Text);
            command.Parameters.AddWithValue("PgMax", BoxPgMax.Text);
            command.Parameters.AddWithValue("PgStep", BoxPgStep.Text);
            command.Parameters.AddWithValue("tMin", BoxtMin.Text);
            command.Parameters.AddWithValue("tMax", BoxtMax.Text);
            command.Parameters.AddWithValue("tStep", BoxtStep.Text);
            command.Parameters.AddWithValue("b0", BoxB0.Text);
            command.Parameters.AddWithValue("b1", BoxB1.Text);
            command.Parameters.AddWithValue("b2", BoxB2.Text);
            command.Parameters.AddWithValue("b3", BoxB3.Text);
            command.Parameters.AddWithValue("b4", BoxB4.Text);
            command.Parameters.AddWithValue("b5", BoxB5.Text);
            command.Parameters.AddWithValue("b6", BoxB6.Text);
            command.Parameters.AddWithValue("b7", BoxB7.Text);
            command.Parameters.AddWithValue("b8", BoxB8.Text);



            if (command.ExecuteNonQuery() != 0)
            {
                LoadData();
                MessageBox.Show("Материал добавлен!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            else
            {
                MessageBox.Show("Ошибка добавления материала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(this);
        }
        private void ClearButton2_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(this);
        }
        public void ClearTextBoxes(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is System.Windows.Forms.TextBox textBox)
                {
                    textBox.Clear(); // Очищаем TextBox
                }
                // Если есть вложенные элементы управления, рекурсивно очищаем их
                else if (control.HasChildren)
                {
                    ClearTextBoxes(control);
                }
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            // Убедитесь, что выбрана строка в DataGridView
            if (dataGridMaterial.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridMaterial.SelectedRows[0];

                // Получаем идентификатор (или ключ) выбранной строки
                int id = Convert.ToInt32(selectedRow.Cells["ID"].Value); // Замените "ID" на имя вашего столбца с ключом

                // Подготовка команды обновления
                SqlCommand command = new SqlCommand(
                    "UPDATE AllSpeSpecifications SET NameMaterial = @NameMaterial, PgMin = @PgMin, PgMax = @PgMax, PgStep = @PgStep, " +
                    "tMin = @tMin, tMax = @tMax, tStep = @tStep, b0 = @b0, b1 = @b1, b2 = @b2, b3 = @b3, b4 = @b4, b5 = @b5, " +
                    "b6 = @b6, b7 = @b7, b8 = @b8 WHERE ID = @ID", // Замените "ID" на имя вашего столбца с ключом
                    sqlConnection
                );

                // Заполнение параметров
                command.Parameters.AddWithValue("@NameMaterial", BBoxNameMaterial.Text);
                command.Parameters.AddWithValue("@PgMin", BBoxPgMin.Text);
                command.Parameters.AddWithValue("@PgMax", BBoxPgMax.Text);
                command.Parameters.AddWithValue("@PgStep", BBoxPgStep.Text);
                command.Parameters.AddWithValue("@tMin", BBoxtMin.Text);
                command.Parameters.AddWithValue("@tMax", BBoxtMax.Text);
                command.Parameters.AddWithValue("@tStep", BBoxtStep.Text);
                command.Parameters.AddWithValue("@b0", BBoxB0.Text);
                command.Parameters.AddWithValue("@b1", BBoxB1.Text);
                command.Parameters.AddWithValue("@b2", BBoxB2.Text);
                command.Parameters.AddWithValue("@b3", BBoxB3.Text);
                command.Parameters.AddWithValue("@b4", BBoxB4.Text);
                command.Parameters.AddWithValue("@b5", BBoxB5.Text);
                command.Parameters.AddWithValue("@b6", BBoxB6.Text);
                command.Parameters.AddWithValue("@b7", BBoxB7.Text);
                command.Parameters.AddWithValue("@b8", BBoxB8.Text);
                command.Parameters.AddWithValue("@ID", id); // Передаем ID записи для обновления

                // Выполнение команды
                if (command.ExecuteNonQuery() != 0)
                {
                    LoadData(); // Обновляем DataGridView
                    MessageBox.Show("Материал обновлен!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Ошибка обновления материала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для обновления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridMaterial.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridMaterial.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value); // Получаем Id выбранной строки

                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить этот материал?",
                                                     "Подтверждение удаления",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    // SQL-команда для удаления
                    using (SqlCommand command = new SqlCommand("DELETE FROM [AllSpeSpecifications] WHERE Id = @Id", sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Id", id); // Добавляем Id в параметры команды

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                LoadData(); // Обновляем данные в grid
                                MessageBox.Show("Материал удален!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                MessageBox.Show("Пожалуйста, выберите материал для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridMaterial_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, выбрана ли правильная ячейка (не заголовок строки и не заголовок столбца)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridMaterial.Rows[e.RowIndex];

                BBoxNameMaterial.Text = selectedRow.Cells["NameMaterial"].Value.ToString(); ;
                BBoxPgMin.Text = selectedRow.Cells["PgMin"].Value.ToString(); ;
                BBoxPgMax.Text = selectedRow.Cells["PgMax"].Value.ToString(); ;
                BBoxPgStep.Text = selectedRow.Cells["PgStep"].Value.ToString(); ;
                BBoxtMin.Text = selectedRow.Cells["tMin"].Value.ToString();
                BBoxtMax.Text = selectedRow.Cells["tMax"].Value.ToString();
                BBoxtStep.Text = selectedRow.Cells["tStep"].Value.ToString();
                BBoxB0.Text = selectedRow.Cells["b0"].Value.ToString();
                BBoxB1.Text = selectedRow.Cells["b1"].Value.ToString();
                BBoxB2.Text = selectedRow.Cells["b2"].Value.ToString();
                BBoxB3.Text = selectedRow.Cells["b3"].Value.ToString();
                BBoxB4.Text = selectedRow.Cells["b4"].Value.ToString();
                BBoxB5.Text = selectedRow.Cells["b5"].Value.ToString();
                BBoxB6.Text = selectedRow.Cells["b6"].Value.ToString();
                BBoxB7.Text = selectedRow.Cells["b7"].Value.ToString();
                BBoxB8.Text = selectedRow.Cells["b8"].Value.ToString();
            }
        }

        private void Box_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox currentTextBox = sender as System.Windows.Forms.TextBox;
            if (currentTextBox != null)
            {
                ReplaceCommasWithDots(currentTextBox);
            }
        }

        private void ReplaceCommasWithDots(System.Windows.Forms.TextBox textBox)
        {
            // Заменяет все запятые на точки
            textBox.Text = textBox.Text.Replace(',', '.');

            // Перемещаем курсор в конец текста после замены
            textBox.Select(textBox.Text.Length, 0);
        }


    }
}
