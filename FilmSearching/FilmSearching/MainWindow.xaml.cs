using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilmSearching
{
    public partial class MainWindow : Window
    {
        //Реквизиты подключения к СУБД
        private string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "localhost", 5432, "postgres",
                    "RtF12346785", "films");

        private NpgsqlConnection conn;
        NpgsqlCommand command;
        DataTable dt;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conn = new NpgsqlConnection(connstring);
        }

        private void Search(string search_text)
        {
            try
            {                 
                string sql = "";                
                string  search_str = "";
                int search_year = 0;
                int num;

                string[] arr_words = search_text.Split(new Char[] {' '});

                foreach (string s in arr_words)
                {
                    if (s.Trim() != "")
                        if (s.Trim().Length == 4 && int.TryParse(s.Trim(),out num))
                        {
                            search_year = Convert.ToInt32(s.Trim());
                        }
                        else
                        {
                            search_str += "%" + s.Trim();
                        }
                }

                //Составляем запрос к базе
                sql = "select id,year,name from movies where name ilike '%" + search_str + "%'" + (search_year == 0 ? "" : " and year::text like '%" + search_year + "%'") + "limit 10";
                conn.Open();
                command = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(command.ExecuteReader());
                conn.Close();
                dtGrid.ItemsSource = null;
                dtGrid.ItemsSource = dt.AsDataView();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (search_box.Text.Length != 0)
            {
                search_btn.IsEnabled = false;
                Search(search_box.Text);
                search_btn.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите строку для поиска");
            }
        }
    }
}
