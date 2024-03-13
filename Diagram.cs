using Npgsql;
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
using System.Windows.Forms.DataVisualization.Charting;

namespace ProductDiagram
{
    public partial class Diagram : Form
    {
        private List<String> products = new List<String>();
        public Diagram()
        {
            InitializeComponent();
            LoadProductsFromDatabase();
        }
        private void LoadProductsFromDatabase()
        {
            try
            {
                string connectionString = "Host=localhost;Username=postgres;Password=qwerty123;Database=ProductsDB";
                string query = "SELECT name, cost FROM product"; 

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string productName = reader["name"].ToString();
                                decimal productCost = Convert.ToDecimal(reader["cost"]);
                                string productString = $"{productName}: {productCost}";
                                products.Add(productString);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных из базы данных: " + ex.Message);
            }
        }
        private void btnMakeDiagram_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedRange = comboBox1.SelectedItem.ToString();
                int minPrice, maxPrice;
                switch (selectedRange)
                {
                    case "От 100 до 500":
                        minPrice = 100;
                        maxPrice = 500;
                        break;

                    case "От 500 до 1000":
                        minPrice = 500;
                        maxPrice = 1000;
                        break;

                    case "От 1000 до 2000":
                        minPrice = 1000;
                        maxPrice = 2000;
                        break;

                    case "От 2000 до 5000":
                        minPrice = 2000;
                        maxPrice = 5000;
                        break;

                    case "От 5000":
                        minPrice = 5000;
                        maxPrice = int.MaxValue; 
                        break;

                    default:
                        MessageBox.Show("Выберите диапазон цен.");
                        return;
                }
                BuildChart(minPrice, maxPrice);
            }
            else
            {
                MessageBox.Show("Выберите диапазон цен.");
            }
        }
        private void BuildChart(int minPrice, int maxPrice)
        {
            chart1.ChartAreas.Clear();
            chart1.Series.Clear();

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "Ценовые категории";
            chartArea.AxisY.Title = "Количество продуктов";
            chart1.ChartAreas.Add(chartArea);

            Series series = new Series();
            series.ChartType = SeriesChartType.Column;
            series.Name = "Ценовые категории";

            int categorySize = 500; 
            int numberOfCategories = (maxPrice - minPrice) / categorySize + 1;
            int[] categoryCounts = new int[numberOfCategories];
            foreach (var productString in products)
            {
                string[] parts = productString.Split(':');
                if (parts.Length == 2)
                {
                    string productName = parts[0].Trim();
                    decimal productCost;
                    if (decimal.TryParse(parts[1].Trim(), out productCost))
                    {
                        if (productCost >= minPrice && productCost <= maxPrice)
                        {
                            int categoryIndex = (int)Math.Ceiling((double)(productCost - minPrice + 1) / categorySize);
                            categoryCounts[categoryIndex - 1]++;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при преобразовании стоимости продукта: {parts[1].Trim()}");
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка при разборе строки продукта: {productString}");
                }
            }
            for (int i = 0; i < numberOfCategories; i++)
            {
                series.Points.AddXY($"Категория {i + 1}", categoryCounts[i]);
            }
            chart1.Series.Add(series);
            chart1.ChartAreas[0].AxisY.Interval = 1;
        }
    }
}   

