using DGVPrinterHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Burgerownia
{
    public partial class UC_PlaceOrder : UserControl
    {
        function fn = new function();
        String query;
        public UC_PlaceOrder()
        {
            InitializeComponent();
        }

        private void comboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            String category = comboCategory.Text;
            query = "select nazwa from produkty where kategoria = '"+category+"'";
            showItemList(query);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            
            String category = comboCategory.Text;
            query = "select nazwa from produkty where kategoria = '" + category + "' and nazwa like '"+txtSearch.Text+"%'";
            showItemList(query);
        }

        private void showItemList(String query)
        {
            listBox1.Items.Clear();
            DataSet ds = fn.getData(query);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listBox1.Items.Add(ds.Tables[0].Rows[i][0].ToString());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtQuantityUpDown.ResetText();
            txtTotal.Clear();

            String text = listBox1.GetItemText(listBox1.SelectedItem);
            txtItemName.Text = text;
            query = "select cena from produkty where nazwa = '"+text+"'";
            DataSet ds = fn.getData(query);

            try
            {
                txtPrice.Text = ds.Tables[0].Rows[0][0].ToString();
            }
            catch
            {

            }
            }

        private void txtQuantityUpDown_ValueChanged(object sender, EventArgs e)
        {
            Int64 quan = Int64.Parse(txtQuantityUpDown.Value.ToString());
            Int64 price = Int64.Parse(txtPrice.Text);
            txtTotal.Text = (quan * price).ToString();
        }

        protected int n, total = 0;
        int amount;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                amount = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString());
            }
            catch { }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.RemoveAt(this.dataGridView1.SelectedRows[0].Index);
            }
            catch
            { }
            total -= amount;
            if(total<0)
                labelTotalAmount.Text = "0.00 zł";
            else
                labelTotalAmount.Text = total + " zł";
        }

        

        private void btnAddtoCart_Click(object sender, EventArgs e)
        {
            if (txtTotal.Text != "0" && txtTotal.Text != "")
            {
                n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = txtItemName.Text;
                dataGridView1.Rows[n].Cells[1].Value = txtPrice.Text;
                dataGridView1.Rows[n].Cells[2].Value = txtQuantityUpDown.Value;
                dataGridView1.Rows[n].Cells[3].Value = txtTotal.Text;
                
                total += int.Parse(txtTotal.Text);
                labelTotalAmount.Text = total + " zł";
            }
            else
            {
                MessageBox.Show("Minimalna Ilość musi być 1", "Informacja", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = new DGVPrinter();
            printer.Title = "Rachunek Klienta";
            printer.SubTitle = string.Format("Data: {0}", DateTime.Now.Date);
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.Footer = "Całkowita Kwota do Zapłaty: " + labelTotalAmount.Text;
            printer.FooterSpacing = 15;
            printer.PrintDataGridView(dataGridView1);

            total = 0;
            dataGridView1.Rows.Clear();
            labelTotalAmount.Text = total + " zł";
        }
    }
}
