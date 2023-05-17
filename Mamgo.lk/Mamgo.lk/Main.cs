using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
namespace Mamgo.lk
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        
        
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            
        }
        string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mamgo;sslmode=none;";
        
        private void button7_Click(object sender, EventArgs e)
        {
            
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM customers", connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            dataGridView1.Refresh();


        }
                
        private void Save_Click(object sender, EventArgs e)
        {
            string id = Id.Text;
            string phone = Phone.Text;
            string n = name.Text;
            string town = Town.Text;
            string deleday = Delevery.Text;
            string reseday = "No";
            string item = Item.Text;
            string query = "INSERT INTO customers(`Id`, `Phone`, `Name`, `Town`,`Delevery_day`,`Reseved_day`,`Item`) VALUES ('" + id + "', '" + phone + "', '" + n + "', '" + town + "', '" + deleday + "', '" + reseday + "', '" + item + "')";      
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                string querySearch = "SELECT count(*) FROM customers where Id = @id";
                MySqlCommand command = new MySqlCommand(querySearch, connection);
                command.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                connection.Open();
                string querySearchPhone = "SELECT count(*) FROM customers where Phone = @phone";
                MySqlCommand commandPhone = new MySqlCommand(querySearchPhone, connection);
                commandPhone.Parameters.AddWithValue("@phone", phone);
                int count2= Convert.ToInt32(commandPhone.ExecuteScalar());
                connection.Close();
                if (count > 0)
                {
                    MessageBox.Show("Id already in the table !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (count2 > 0)
                {
                    MessageBox.Show("Phone number already in the table !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    databaseConnection.Open();
                    MySqlDataReader myReader = commandDatabase.ExecuteReader();
                    DialogResult result = MessageBox.Show("Data Saved !", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    databaseConnection.Close();
                    if (result == DialogResult.OK)
                    {

                        connection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM customers", connection);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.Refresh();
                        connection.Close();
                    }
                }

                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("Error while saving !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            string id = Id.Text;
            string reseday = ResevedDate.Text;
            string updateQuery = "UPDATE customers SET Reseved_day = @value1 WHERE Id = @id";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@value1", reseday);
            updateCommand.Parameters.AddWithValue("@id", id);
            int rowsAffected = updateCommand.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                MessageBox.Show("Update success !", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string query = "SELECT * FROM customers where Id =@id";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                command.Parameters.AddWithValue("@id", id);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                dataGridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Error while updating !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
            

        }

        private void Main_Load(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM customers", connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            dataGridView1.Refresh();
            connection.Close();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            string id = Id.Text;
            string phone = Phone.Text;
            if (id == "" && phone=="")
            {
                MessageBox.Show("Please enter Id or Phone number to search!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(id == "" && !(phone == ""))
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                string query = "SELECT * FROM customers where Phone =@phone";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                command.Parameters.AddWithValue("@phone", phone);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                dataGridView1.Refresh();
                connection.Close();
            }
            else if (!(id == "") && phone == "")
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                string query = "SELECT * FROM customers where Id =@id";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                command.Parameters.AddWithValue("@id", id);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                dataGridView1.Refresh();
                connection.Close();
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Id.Text="";
            Phone.Text="";
            name.Text="";
            Town.Text="";
            Item.Text="";
        }

        private void ExportToTextFile(DataGridView dataGridView, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write column headers
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    writer.Write(dataGridView.Columns[i].HeaderText);
                    if (i < dataGridView.Columns.Count - 1)
                    {
                        writer.Write("\t\t\t"); // Use a tab delimiter
                    }
                }
                writer.WriteLine();

                // Write data rows
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    for (int i = 0; i < dataGridView.Columns.Count; i++)
                    {
                        writer.Write(row.Cells[i].Value);
                        if (i < dataGridView.Columns.Count - 1)
                        {
                            writer.Write("\t\t\t"); // Use a tab delimiter
                        }
                    }
                    writer.WriteLine();
                }

                writer.Close();
            }

            MessageBox.Show("Data exported to text file successfully!", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Export_Click(object sender, EventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "Data.txt");
            ExportToTextFile(dataGridView1, filePath);

        }
    }
}
