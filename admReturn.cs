using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace LibraryManagementSystem
{
    public partial class admReturn : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        public admReturn()
        {
            InitializeComponent();
        }

        private void admReturn_Load(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LibraryManagementSystem.Properties.Settings.LibraryDB"].ToString();
            con = new SqlConnection(connectionString);

            display();
        }

        private void clear()
        {
            admReturnTbxSearchQuery.Text = string.Empty;
            admReturnTbxUserID.Text = string.Empty;
            amdReturnTbxBookID.Text = string.Empty;

            admReturnBooksLblFine.Text = string.Empty;
        }

        public void display()
        {
            cmd = new SqlCommand("select user_id as 'User ID', name as 'Name', book_id as 'Book ID', title as 'Title', date_issued as 'Date Issued', DATEDIFF(day, date_issued, CONVERT(date, GETDATE())) as 'Days Passed' from issue, users, books where i_user_id = user_id and i_book_id = book_id", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);

            admReturnDgvTable.DataSource = ds.Tables[0];
            admReturnDgvTable.ReadOnly = true;
        }

        private void admReturnDgvTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1 && e.RowIndex != admReturnDgvTable.Rows.Count - 1)
            {
                int book_id = Convert.ToInt32(admReturnDgvTable.Rows[e.RowIndex].Cells[2].Value);
                int user_id = Convert.ToInt32(admReturnDgvTable.Rows[e.RowIndex].Cells[0].Value);

                amdReturnTbxBookID.Text = Convert.ToString(book_id);
                admReturnTbxUserID.Text = Convert.ToString(user_id);

                int fine = 0;
                int days_passed = Convert.ToInt32(admReturnDgvTable.Rows[e.RowIndex].Cells[5].Value);

                if(days_passed > 15)
                {
                    fine = days_passed - 15;
                }

                admReturnBooksLblFine.Text = Convert.ToString(fine + " Rs.");
            }
        }

        private void admReturnBtnSearch_Click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            try
            {
                if(admReturnTbxSearchQuery.Text == string.Empty)
                {
                    display();
                }
                else
                {
                    cmd = new SqlCommand("select user_id as 'User ID', name as 'Name', book_id as 'Book ID', title as 'Title', date_issued as 'Date Issued', DATEDIFF(day, date_issued, CONVERT(date, GETDATE())) as 'Days Passed' from issue, users, books where i_user_id = user_id and i_book_id = book_id and ( book_id = @searchQuery or user_id = @searchQuery )", con);
                    cmd.Parameters.AddWithValue("@searchQuery", Convert.ToInt32(admReturnTbxSearchQuery.Text));

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    admReturnDgvTable.DataSource = ds.Tables[0];
                }
            }
            catch
            {
                MessageBox.Show("Please make sure that you enter only an integer in the search box.");
            }
                
        }

        private void admReturnBtnReturn_Click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();

            int book_id = 0;
            int user_id = 0;
            bool preliminaryAcceptedState = true;

            try
            {
                book_id = int.Parse(amdReturnTbxBookID.Text);
            }
            catch
            {
                MessageBox.Show("Book ID should be an integer.");
                preliminaryAcceptedState = false;
            }
            try
            {
                user_id = int.Parse(admReturnTbxUserID.Text);
            }
            catch
            {
                MessageBox.Show("User ID should be an interger.");
                preliminaryAcceptedState = false;
            }

            if(preliminaryAcceptedState == true)
            {
                try
                {
                    cmd = new SqlCommand("delete from issue where i_book_id = @book_id and i_user_id = @user_id", con);
                    cmd.Parameters.AddWithValue("@book_id", book_id);
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    int result = cmd.ExecuteNonQuery();

                    if (result == 0)
                    {
                        MessageBox.Show("Please enter a valid User ID and Book ID.\nInvalid Input.");
                    }

                    if(result == 1)
                    {
                        MessageBox.Show("Book successfully returned.");
                        clear();
                        display();
                    }
                }
                catch
                {
                    MessageBox.Show("Internal system error.\nPlease contact the developer.");
                }
            }
        }

        private void admReturnBooksBtnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            admStartPage asp = new admStartPage();
            asp.Show();
        }

        private void admReturnBooksBtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            login lg = new login();
            lg.Show();
        }
    }
}
