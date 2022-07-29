using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class frmInvoiceForm : Form
    {
        public frmInvoiceForm()
        {
            InitializeComponent();
        }
        private void setForm()
        {
            this.Size = new Size(946, 549);
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
        }
        private void frmInvoiceForm_Resize(object sender, EventArgs e)
        {
            setForm();
        }
        private void label2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.linkedin.com/feed/");
        }

        private void frmInvoiceForm_Load(object sender, EventArgs e)
        {
            txtInvDate.Text = DateTime.Now.ToString("yyyy/mm/dd");

            Dictionary<int, string> itemsData = new Dictionary<int, string>();
            itemsData.Add(16000, "لاب توب DELL");
            itemsData.Add(17000, "لاب توب HP");
            itemsData.Add(15500, "لاب توب سامسونج");
            itemsData.Add(150, "كيبورد عادي");
            itemsData.Add(80, "ماوس عادي");
            itemsData.Add(250, "كيبورد DELL");
            itemsData.Add(120, "ماوس DELL");
            itemsData.Add(280, "كيبورد HP");
            itemsData.Add(130, "ماوس HP");
            itemsData.Add(850, "طابعة حبر HP");
            itemsData.Add(1650, "طابعة ليزر HP");
            itemsData.Add(400, "راوتر انترنت");

            cbxType.DataSource = new BindingSource(itemsData, null);
            cbxType.DisplayMember = "value";
            cbxType.ValueMember = "key";

            txtPrice.Text = cbxType.SelectedValue.ToString();

            foreach(DataGridViewColumn co in dataGridView1.Columns)
            {
                colKmea.DefaultCellStyle.ForeColor = Color.Navy;
            }

            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Red;

            txtInvCustomerName.Focus();
            txtInvCustomerName.Select();
        }

        private void txtInvDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void txtInvDate_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                txtInvDate.ContextMenu = new ContextMenu();
            }
        }

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtInvCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData==Keys.Enter)
            {
                cbxType.Focus();
            }
        }

        private void cbxType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtKmea.Focus();
                txtKmea.Select();
            }
        }
        ErrorProvider er = new ErrorProvider();
        private void btnAdd_Click(object sender, EventArgs e)
        {
            er.Clear();
            if (cbxType.SelectedIndex <= -1)
            {
                er.SetError(btnAdd, "اختر الشي الذي تريد شراءه");
                cbxType.Focus();
            }
            else
            {
                string strType = cbxType.Text;
                int Kmea = Convert.ToInt32(txtKmea.Text);
                int unitPrice = int.Parse(txtPrice.Text);
                int subTotal = Kmea * unitPrice;

                object[] data = { strType, Kmea, unitPrice, subTotal };

                dataGridView1.Rows.Add(data);

                txtTotal.Text = (int.Parse(txtTotal.Text) + subTotal) + "";
            }
        }

        private void txtKmea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnAdd.PerformClick();
                cbxType.Focus();
            }
        }

        private void cbxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPrice.Text = cbxType.SelectedValue.ToString();
        }

        private void txtInvNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtKmea_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }
        string oldKmea;
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                oldKmea = dataGridView1.CurrentRow.Cells["colKmea"].Value + "";
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string newKmea = dataGridView1.CurrentRow.Cells["colKmea"].Value + "";
                if (oldKmea == newKmea) return;
                if(!Regex.IsMatch(newKmea,"^\\d+$"))
                {
                    dataGridView1.CurrentRow.Cells["colKmea"].Value = oldKmea;
                }
                else
                {

                    int unitPrice = int.Parse(txtPrice.Text);
                    dataGridView1.CurrentRow.Cells["colSubTotal"].Value  = (int.Parse(newKmea) * unitPrice) ;

                    int total=0;
                    foreach(DataGridViewRow r in dataGridView1.Rows )
                    {
                        total += int.Parse(r.Cells["colSubTotal"].Value.ToString());
                            
                    }
                    txtTotal.Text = total.ToString();
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ((Form)printPreviewDialog1).WindowState = FormWindowState.Maximized;
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float margin = 40;
            Font f = new Font("arial", 18, FontStyle.Bold);

            string strNo = "#NO " + txtInvNum.Text;
            string strDate = "التاريخ : " + txtInvDate.Text;
            string strName = "مطلوب من السيد :" + txtInvCustomerName.Text;

            SizeF fontSizeOf=e.Graphics.MeasureString(strNo,f);
            SizeF fontSizeDate = e.Graphics.MeasureString(strDate, f);
            SizeF fontSizeName = e.Graphics.MeasureString(strName, f);


            e.Graphics.DrawImage(Properties.Resources._400230400769_97120, 40, 40, 200, 200);
            e.Graphics.DrawString(strNo, f, Brushes.Red, (e.PageBounds.Width - fontSizeOf.Width) / 2, margin);
            e.Graphics.DrawString(strDate, f, Brushes.Black, e.PageBounds.Width - fontSizeDate.Width - margin, margin + fontSizeOf.Height);
            e.Graphics.DrawString(strName, f, Brushes.Navy, e.PageBounds.Width - fontSizeName.Width - margin, margin + fontSizeOf.Height + fontSizeDate.Height);

            float preHeigits = margin + fontSizeOf.Height + fontSizeDate.Height+fontSizeName.Height+120;
            e.Graphics.DrawRectangle(Pens.Black, margin, preHeigits, e.PageBounds.Width - margin * 2, e.PageBounds.Height-preHeigits-margin);

            e.Graphics.DrawRectangle(Pens.Black, margin, preHeigits, 176, 100);
            e.Graphics.DrawRectangle(Pens.Black, margin+176, preHeigits, 171, 100);
            e.Graphics.DrawRectangle(Pens.Black, margin + 347, preHeigits, 171, 100);
            e.Graphics.DrawRectangle(Pens.Black, margin + 518, preHeigits, 250, 100);

            e.Graphics.DrawString("الصنف", f, Brushes.Black, margin + 600, preHeigits + 40);
            e.Graphics.DrawLine(Pens.Black, margin + 518, preHeigits + 100, margin + 518, e.PageBounds.Height-margin);

            e.Graphics.DrawString("الكميه", f, Brushes.Black, margin + 410, preHeigits + 40);
            e.Graphics.DrawLine(Pens.Black, margin + 347, preHeigits + 100, margin + 347, e.PageBounds.Height - margin);

            e.Graphics.DrawString("السعر", f, Brushes.Black, margin + 230, preHeigits + 40);
            e.Graphics.DrawLine(Pens.Black, margin + 176, preHeigits + 100, margin + 176, e.PageBounds.Height - margin);

            e.Graphics.DrawString("اجمالي", f, Brushes.Black, margin + 60, preHeigits + 40);

            float rowsHeight = 60;
            for(int x=0;x<dataGridView1.Rows.Count;x++)
            {
                e.Graphics.DrawString(dataGridView1.Rows[x].Cells[0].Value.ToString(), f, Brushes.Navy, margin + 600, preHeigits + 60 + rowsHeight);
                e.Graphics.DrawString(dataGridView1.Rows[x].Cells[1].Value.ToString(), f, Brushes.Black, margin + 410, preHeigits + 60 + rowsHeight);
                e.Graphics.DrawString(dataGridView1.Rows[x].Cells[2].Value.ToString(), f, Brushes.Black, margin + 230, preHeigits + 60 + rowsHeight);
                e.Graphics.DrawString(dataGridView1.Rows[x].Cells[3].Value.ToString(), f, Brushes.Black, margin + 60, preHeigits + 60 + rowsHeight);
                e.Graphics.DrawLine(Pens.Black, margin, preHeigits + 110 + rowsHeight , e.PageBounds.Width - margin , preHeigits + 110 + rowsHeight);
                rowsHeight += 60;
            }
            e.Graphics.DrawString("الاجمالي", f, Brushes.Red, margin + 230, preHeigits + 60 + rowsHeight);
            e.Graphics.DrawString(txtTotal.Text, f, Brushes.Navy, margin + 60, preHeigits + 60 + rowsHeight);      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
        }
    }
}
   