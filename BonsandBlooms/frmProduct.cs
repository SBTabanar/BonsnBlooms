using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BonsandBlooms
{
    public partial class frmProduct : Form
    {
        public frmProduct()
        {
            InitializeComponent();
        }
        DatabaseConnect config = new DatabaseConnect();
        usableFunction func = new usableFunction();
        string sql;
        int maxrow;

        private void Button1_Click(object sender, EventArgs e)
        {
            func.clearTxt(this);
            cboCateg.Text = "Select";
            config.autonumber(2, txtPROCODE);
        }

        private void frmProduct_Load(object sender, EventArgs e)
        {
            LoadCategories();
            Button1_Click(sender, e);
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnManageCategories_Click(object sender, EventArgs e)
        {
            using (frmCategoryManager frm = new frmCategoryManager())
            {
                frm.ShowDialog();
                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            string sql = "SELECT CategoryName FROM tblCategory ORDER BY CategoryName";
            config.combo(sql, cboCateg);
        }




        private void BTNSAVE_Click(object sender, EventArgs e)
        {
            string selectedCategory = cboCateg.Text?.Trim();

            if (string.IsNullOrEmpty(selectedCategory) || selectedCategory == "Select")
            {
                MessageBox.Show("Please select a valid category.");
                cboCateg.Focus();
                return;
            }

            if (TXTPRONAME.Text == "" || TXTDESC.Text == "" || TXTPRICE.Text == "" || txtPROQTY.Text == "")
            {
                func.messagerequired();
                return;
            }

            int qty;
            decimal price;

            if (!int.TryParse(txtPROQTY.Text, out qty) || qty < 0)
            {
                MessageBox.Show("Please enter a valid non-negative integer for Quantity.");
                txtPROQTY.Focus();
                return;
            }

            if (!decimal.TryParse(TXTPRICE.Text, out price) || price < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Price.");
                TXTPRICE.Focus();
                return;
            }

            sql = "SELECT * FROM tblProductInfo WHERE PROCODE='" + txtPROCODE.Text + "'";
            maxrow = config.maxrow(sql);

            if (maxrow > 0)
            {
                sql = "UPDATE tblProductInfo SET PRONAME='" + TXTPRONAME.Text +
                      "', PRODESC='" + TXTDESC.Text +
                      "', CATEGORY='" + selectedCategory +
                      "', PROPRICE=" + price +
                      ", PROQTY=" + qty +
                      " WHERE PROCODE='" + txtPROCODE.Text + "'";
                config.Execute_CUD(sql, "Error to update Product", "Product has been updated.");
            }
            else
            {
                sql = "SELECT * FROM tblProductInfo WHERE PRONAME='" + TXTPRONAME.Text + "'";
                maxrow = config.maxrow(sql);

                if (maxrow > 0)
                {
                    MessageBox.Show("A product with the same name already exists. Please use a different product name.");
                    TXTPRONAME.Focus();
                    return;
                }

                sql = "INSERT INTO tblProductInfo (PROCODE, PRONAME, PRODESC, CATEGORY, PROPRICE, PROQTY) " +
                      "VALUES ('" + txtPROCODE.Text + "', '" + TXTPRONAME.Text + "', '" + TXTDESC.Text + "', '" + selectedCategory + "', " + price + ", " + qty + ")";
                config.Execute_CUD(sql, "Error to save Product.", "Product added successfully!");

                config.update_Autonumber(2);
            }

            Button1_Click(sender, e);
        }




        private void cboCateg_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void TXTDESC_TextChanged(object sender, EventArgs e)
        {

        }

        }
    }
