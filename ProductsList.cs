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

namespace ProductDiagram
{
    public partial class ProductsList : Form
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=qwerty123;Database=ProductsDB";
        public ProductsList()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT name, cost, article FROM product";
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    // Задаем имена столбцам
                    dataTable.Columns["name"].ColumnName = "Название";
                    dataTable.Columns["cost"].ColumnName = "Стоимость";
                    dataTable.Columns["article"].ColumnName = "Артикул";
                    productsGridList.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (productsGridList.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(productsGridList.SelectedRows[0].Cells["id"].Value);
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM product WHERE id = @id";
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", productId);
                            command.ExecuteNonQuery();
                        }
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (productsGridList.Rows.Count > 0)
                {
                    DataGridViewRow lastRow = productsGridList.Rows[productsGridList.Rows.Count - 2];
                    string name = lastRow.Cells["Название"].Value.ToString();
                    decimal cost = Convert.ToDecimal(lastRow.Cells["Стоимость"].Value);
                    string article = lastRow.Cells["Артикул"].Value.ToString();
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO product (name, cost, article) VALUES (@name, @cost, @article)";
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@cost", cost);
                            command.Parameters.AddWithValue("@article", article);
                            command.ExecuteNonQuery();
                        }
                        LoadData();
                    }
                }
                else
                {
                    MessageBox.Show("Нет данных для сохранения.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении записи: " + ex.Message);
            }
        }

        private void buttonMakeDiagram_Click(object sender, EventArgs e)
        {
            Diagram diagramForm = new Diagram();
            diagramForm.ShowDialog();
        }
    }
}
