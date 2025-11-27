using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using static TechChemAnalytica.Authorization;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace TechChemAnalytica
{
    public partial class Interface : Form
    {
        private SqlConnection _sqlConnection = null;
        private int accessLevel;
        private string userName;
        private float b0, b1, b2, b3, b4, b5, b6, b7, b8;
        private int _pressureMin, _pressureMax, _pressureStep, _temperatureMin, _temperatureMax, _temperatureStep;

        
        public Interface(int accessLevel, string userName)
        {
            InitializeComponent();
            SetTooTipExport();
            chartAreaOne.ChartAreas[0].AxisX.Title = "Давление газа, атм/мин";
            chartAreaOne.ChartAreas[0].AxisY.Title = "Прочность твердого сплава, МПА";
            chartAreaOne.ChartAreas[0].AxisY.LabelStyle.Format = "#.##";
            chartAreaTwo.ChartAreas[0].AxisX.Title = "Температура, С";
            chartAreaTwo.ChartAreas[0].AxisY.Title = "Прочность твердого сплава, МПА";
            chartAreaTwo.ChartAreas[0].AxisY.LabelStyle.Format = "#.##";

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Это зафиксирует окно
            this.Size = new Size(1400, 970); // Устанавливаем начальный размер

            this.accessLevel = accessLevel;
            this.userName = userName;
            ConfigureAccess();

            _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString);
            LoadMaterials(); // Загружаем материалы при старте формы
            comboBoxMaterial.SelectedIndexChanged += ComboBoxMaterial_SelectedIndexChanged; // Подписка на событие
        }

        public void ShowForm()
        {
            this.Show(); // Возвращаем Interface
            this.Activate(); // Активируем Interface
        }

        
        private void ConfigureAccess()
        {
            if (accessLevel == 1) // администратор
            {
                functionAdminsToolStripMenuItem.Visible = true;
            }
            else if (accessLevel == 2) // пользователь
            {
                functionAdminsToolStripMenuItem.Visible = false;
            }

            пользовательToolStripMenuItem.Text += userName;

        }

    

        private void SetTooTipExport()
        {
            ToolTip tipExport = new ToolTip();
            tipExport.SetToolTip(this.buttonExport, "Экcпорт результатов в MS Excel");
        }
        
        private void ChangeUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Authorization change = new Authorization();
            change.Show();
        }

        private void Export_ToExelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToExcel(dataGridView1);
        }

        private void СLoseAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CloseAppFormToolStripMenuItem_Click(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void ProfilesUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataUsers data = new DataUsers(); // Передаем метод LoadMaterials
            data.Show();
        }

        private void ChangeMaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MaterialManager materialManager = new MaterialManager(this, LoadMaterials);
            this.Hide(); 
            materialManager.Show();
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help change = new Help();
            change.Show();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            ExportToExcel(dataGridView1);
        }
        private void ExportToExcel(DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!");
                return;
            }

            // Создание нового экземпляра Excel
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;

            // Вывод названия материала из ComboBox в первую строку
            string materialName = comboBoxMaterial.Text; // Предположим, что это ваш ComboBox для выбора названия материала
            var materialCell = worksheet.Cells[1, 1];
            materialCell.Value = "Название материала: " + materialName; // Запись названия в первую ячейку
            materialCell.Font.Bold = true; // Установка жирного шрифта для заголовка

            // Заполнение полей давления и температуры
            worksheet.Cells[2, 1] = "Минимальное давление, атм: " + PgMin.Text;
            worksheet.Cells[3, 1] = "Максимальное давление, атм: " + PgMax.Text;
            worksheet.Cells[4, 1] = "Шаг изменения давления, атм: " + PgStep.Text;
            worksheet.Cells[5, 1] = "Минимальная температура, С: " + tMin.Text;
            worksheet.Cells[6, 1] = "Максимальная температура, С: " + tMax.Text;
            worksheet.Cells[7, 1] = "Шаг изменения температуры, С: " + tStep.Text;

            // Установка жирного шрифта для заголовков полей
            for (int i = 2; i <= 7; i++)
            {
                worksheet.Cells[i, 1].Font.Bold = true;
            }

            // Заполнение заголовков для DataGridView
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                worksheet.Cells[8, i + 1] = dataGridView.Columns[i].HeaderText; // Заполняем заголовки со строки 8
            }

            // Заполнение данных
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    worksheet.Cells[i + 9, j + 1] = dataGridView.Rows[i].Cells[j].Value?.ToString(); // Данные начинаются с 9 строки
                }
            }

            // Установка ширины ячеек (опционально)
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                worksheet.Columns[i + 1].ColumnWidth = 10; // Установка ширины ячеек
            }

            // Делает Excel видимым
            excelApp.Visible = true;

            // Освобождение ресурсов
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }
        private void buttonСalculate1_Click(object sender, EventArgs e)
        {

            if (comboBoxMaterial.SelectedIndex.ToString() == "-1")
            {
                MessageBox.Show("Не выбран материал!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ValidateTextBoxes())
            {
                return;
            }

            _pressureMin = Convert.ToInt32(PgMin.Text);
            _pressureMax = Convert.ToInt32(PgMax.Text);
            _pressureStep = Convert.ToInt32(PgStep.Text);
            _temperatureMin = Convert.ToInt32(tMin.Text);
            _temperatureMax = Convert.ToInt32(tMax.Text);
            _temperatureStep = Convert.ToInt32(tStep.Text);

            float ColCount = (_pressureMax - _pressureMin) / _pressureStep + 2;
            int ColCountT = (int)ColCount;
            float RowCount = (_temperatureMax - _temperatureMin) / _temperatureStep + 2;

            double tempPgMin = _pressureMin;
            double T = _temperatureMin;
            double Pg = _pressureMin;
            int countOperations = 0;
            double max = -0.2, min = 2;

            // Измерение времени выполнения
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Измерение памяти с помощью Process
            Process currentProcess = Process.GetCurrentProcess();
            long memoryBefore = currentProcess.PrivateMemorySize64;

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.ColumnCount = ColCountT;

            chartAreaOne.Series["Series1"].Points.Clear();
            chartAreaOne.Series["Series2"].Points.Clear();
            chartAreaOne.Series["Series3"].Points.Clear();
            chartAreaTwo.Series["Series1"].Points.Clear();
            chartAreaTwo.Series["Series2"].Points.Clear();
            chartAreaTwo.Series["Series3"].Points.Clear();

            for (int k = 1; k < ColCountT; k++)
            {
                dataGridView1.Columns[k].Name = "Pg = " + tempPgMin.ToString() + " атм";
                tempPgMin += _pressureStep;
            }

            for (int j = 1; j < RowCount; j++)
            {
                double PgChart = _pressureMin;
                double Tchart = _temperatureMin;
                string[] row = new string[ColCountT];
                row[0] = "T =" + T.ToString() + " ,C";
                for (int i = 1; i < ColCountT; i++)
                {
                    // Ваши вычисления
                    double result = b0 + b1 * PgChart + b2 * T + b3 * PgChart * T + b4 * (PgChart * PgChart) + b5 * (T * T) +
                                    b6 * (PgChart * PgChart) * T + b7 * PgChart * (T * T) + b8 * (PgChart * PgChart) * (T * T);
                    double result2 = b0 + b1 * Pg + b2 * Tchart + b3 * Pg * Tchart + b4 * (Pg * Pg) + b5 * (Tchart * Tchart) +
                                    b6 * (Pg * Pg) * Tchart + b7 * Pg * (Tchart * Tchart) + b8 * (Pg * Pg) * (Tchart * Tchart);

                    countOperations += 10;

                    // Добавление данных в графики
                    if (j == 1)
                    {
                        chartAreaOne.Series["Series1"].Points.AddXY(PgChart, result);
                        chartAreaOne.Series["Series1"].BorderWidth = 5;
                        chartAreaTwo.Series["Series1"].Points.AddXY(Tchart, result2);
                        chartAreaTwo.Series["Series1"].BorderWidth = 5;
                    }
                    else if (j == RowCount / 2)
                    {
                        chartAreaOne.Series["Series2"].Points.AddXY(PgChart, result);
                        chartAreaOne.Series["Series2"].BorderWidth = 5;
                        chartAreaTwo.Series["Series2"].Points.AddXY(Tchart, result2);
                        chartAreaTwo.Series["Series2"].BorderWidth = 5;
                    }
                    else if (j == RowCount - 1)
                    {
                        chartAreaOne.Series["Series3"].Points.AddXY(PgChart, result);
                        chartAreaOne.Series["Series3"].BorderWidth = 5;
                        chartAreaTwo.Series["Series3"].Points.AddXY(Tchart, result2);
                        chartAreaTwo.Series["Series3"].BorderWidth = 5;
                    }

                    row[i] = string.Format("{0:N2}", result);
                    PgChart += _pressureStep;
                    Tchart += _temperatureStep;
                }

                dataGridView1.Rows.Add(row);
                Pg += _pressureStep;
                T += _temperatureStep;
            }

            chartAreaOne.Invalidate();
            chartAreaTwo.Invalidate();

            // Завершаем измерение времени
            stopwatch.Stop();

            // Задержка для подтверждения выделения памяти
            System.Threading.Thread.Sleep(100); // Небольшая задержка

            // Измерение памяти после выполнения
            long memoryAfter = currentProcess.PrivateMemorySize64;
            //long memoryUsed = memoryAfter - memoryBefore;

            // Конвертация используемой памяти в КБ
            double memoryUsedKB = memoryAfter / 1024.0;

            // Конвертация времени выполнения в секунды
            double elapsedTimeInSeconds = stopwatch.ElapsedMilliseconds / 1000.0;

            // Формируем сообщение
            string details = $"Значения математический коэфф.\nдля расчета:\n" +
                             $"b0: {b0}" +
                             $"  b1: {b1}\n" +
                             $"b2: {b2}" +
                             $"  b3: {b3}\n" +
                             $"b4: {b4}" +
                             $"  b5: {b5}\n" +
                             $"b6: {b6}" +
                             $"  b7: {b7}\n" +
                             $"b8: {b8}";

            StringBuilder message = new StringBuilder();
            message.Append(details);
            message.Append($"\n\n\nПоказателей экономичности :\n");
            message.Append($"\nВремя выполнения: {elapsedTimeInSeconds} секунд\n");
            message.Append($"Используемая память: {memoryUsedKB} КБ\n");
            message.Append($"Количество арифметических \nопераций: {countOperations}");

            Label_Info.Text = message.ToString();
        }

        private void buttonClearBox1_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(this);
            Label_Info.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            this.comboBoxMaterial.SelectedIndex = -1;
            chartAreaOne.Series["Series1"].Points.Clear();
            chartAreaOne.Series["Series2"].Points.Clear();
            chartAreaOne.Series["Series3"].Points.Clear();
            chartAreaTwo.Series["Series1"].Points.Clear();
            chartAreaTwo.Series["Series2"].Points.Clear();
            chartAreaTwo.Series["Series3"].Points.Clear();
        }
        private void ClearTextBoxes(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
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


        public class ComboBoxItem
        {
            public int Id { get; set; } // Идентификатор материала
            public string Name { get; set; } // Имя материала

            public override string ToString()
            {
                return Name; // Возвращает имя для отображения в ComboBox
            }
        }
        private void LoadMaterials()
        {


            string query = "SELECT Id, NameMaterial FROM AllSpeSpecifications"; // Измените SQL-запрос по необходимости

            _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString);

            try
            {
                _sqlConnection.Open();

                using (SqlCommand command = new SqlCommand(query, _sqlConnection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    comboBoxMaterial.Items.Clear();
                    while (reader.Read())
                    {
                        comboBoxMaterial.Items.Add(new ComboBoxItem
                        {
                            Id = (int)reader["Id"],
                            Name = reader["NameMaterial"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке материалов: " + ex.Message);
            }
            _sqlConnection.Close();
        }

        private void ComboBoxMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMaterial.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)comboBoxMaterial.SelectedItem; // Получаем выбранный элемент
                LoadMaterialDetails(selectedItem.Id); // Загружаем детали материала
            }
        }

        private void LoadMaterialDetails(int materialId)
        {
            string query = "SELECT PgMin, PgMax, PgStep, tMin, tMax, tStep, b0, b1, b2, b3, b4, b5, b6, b7, b8 " +
                "FROM AllSpeSpecifications WHERE Id = @NameMaterial";
            try
            {
                _sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(query, _sqlConnection))
                {
                    command.Parameters.AddWithValue("@NameMaterial", materialId);

                    // Здесь мы используем using для SqlDataReader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            PgMin.Text = reader["PgMin"].ToString(); // Параметры, которые вы хотите отобразить
                            PgMax.Text = reader["PgMax"].ToString();
                            PgStep.Text = reader["PgStep"].ToString();
                            tMin.Text = reader["tMin"].ToString();
                            tMax.Text = reader["tMax"].ToString();
                            tStep.Text = reader["tStep"].ToString();

                            //PgMinV = reader.IsDBNull(reader.GetOrdinal("PgMin")) ? 0 : Convert.ToSingle(reader["PgMin"]);
                            //PgMaxV = reader.IsDBNull(reader.GetOrdinal("PgMax")) ? 0 : Convert.ToSingle(reader["PgMax"]);
                            //PgStepV = reader.IsDBNull(reader.GetOrdinal("PgStep")) ? 0 : Convert.ToSingle(reader["PgStep"]);
                            //tMinV = reader.IsDBNull(reader.GetOrdinal("tMin")) ? 0 : Convert.ToSingle(reader["tMin"]);
                            //tMaxV = reader.IsDBNull(reader.GetOrdinal("tMax")) ? 0 : Convert.ToSingle(reader["tMax"]);
                            //tStepV = reader.IsDBNull(reader.GetOrdinal("tStep")) ? 0 : Convert.ToSingle(reader["tStep"]);

                            // Присваиваем значения b0 до b8
                            b0 = reader.IsDBNull(reader.GetOrdinal("b0")) ? 0 : Convert.ToSingle(reader["b0"]);
                            b1 = reader.IsDBNull(reader.GetOrdinal("b1")) ? 0 : Convert.ToSingle(reader["b1"]);
                            b2 = reader.IsDBNull(reader.GetOrdinal("b2")) ? 0 : Convert.ToSingle(reader["b2"]);
                            b3 = reader.IsDBNull(reader.GetOrdinal("b3")) ? 0 : Convert.ToSingle(reader["b3"]);
                            b4 = reader.IsDBNull(reader.GetOrdinal("b4")) ? 0 : Convert.ToSingle(reader["b4"]);
                            b5 = reader.IsDBNull(reader.GetOrdinal("b5")) ? 0 : Convert.ToSingle(reader["b5"]);
                            b6 = reader.IsDBNull(reader.GetOrdinal("b6")) ? 0 : Convert.ToSingle(reader["b6"]);
                            b7 = reader.IsDBNull(reader.GetOrdinal("b7")) ? 0 : Convert.ToSingle(reader["b7"]);
                            b8 = reader.IsDBNull(reader.GetOrdinal("b8")) ? 0 : Convert.ToSingle(reader["b8"]);


                            
                            
                        }
                        else
                        {
                            MessageBox.Show("Детали для выбранного материала не найдены.");

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при загрузке деталей материала: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            _sqlConnection.Close();
        }

        private bool ValidateTextBoxes()
        {
            // Получаем значения из текстовых полей
            string pgMin = PgMin.Text.Trim();
            string pgMax = PgMax.Text.Trim();
            string pgStep = PgStep.Text.Trim();
            string TMin = tMin.Text.Trim();
            string TMax = tMax.Text.Trim();
            string TStep = tStep.Text.Trim();

            // Проверяем каждое текстовое поле на пустоту и значение
            if (!ValidateField(pgMin, "Поле значения Минимальное давление не должно быть пустым и должно быть положительным числом."))
                return false;

            if (!ValidateField(pgMax, "Поле PgMax не должно быть пустым и должно быть положительным числом."))
                return false;

            if (!ValidateField(pgStep, "Поле PgStep не должно быть пустым и должно быть положительным числом."))
                return false;

            if (!ValidateField(TMin, "Поле tMin не должно быть пустым и должно быть положительным числом."))
                return false;

            if (!ValidateField(TMax, "Поле tMax не должно быть пустым и должно быть положительным числом."))
                return false;

            if (!ValidateField(TStep, "Поле tStep не должно быть пустым и должно быть положительным числом."))
                return false;

            // Если все поля заполнены и корректны, возвращаем true
            return true;
        }

        private bool ValidateField(string value, string errorMessage)
        {
            // Проверка на пустоту
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            // Проверка на наличие правильного числа
            if (!double.TryParse(value, out double numericValue) || numericValue < 0)
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            return true; // Если поле валидное
        }
    }
}
    

