﻿using System;
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
    public partial class frmListStockin : Form
    {
        public string SelectedProductCode { get; private set; }
        public string SelectedProductName { get; private set; }
        public string SelectedCategory { get; private set; }
        public string SelectedDesc { get; private set; }
        public string SelectedPrice { get; private set; }
        public string SelectedQty { get; private set; }

        public frmListStockin()
        {
            InitializeComponent();
            DTGLIST.CellDoubleClick += DTGLIST_CellDoubleClick;

        }
        DatabaseConnect config = new DatabaseConnect();
        usableFunction func = new usableFunction();
        string sql;
        int maxrow;
        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DTGLIST_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = DTGLIST.Rows[e.RowIndex];
                SelectedProductCode = row.Cells["ProductCode"].Value.ToString();
                SelectedProductName = row.Cells["Product"].Value.ToString();
                SelectedCategory = ""; 
                SelectedDesc = row.Cells["Description"].Value.ToString();
                SelectedPrice = row.Cells["Price"].Value.ToString();
                SelectedQty = row.Cells["Quantity"].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            sql = "SELECT TRANSNUM AS [Transaction#], P.PROCODE as [ProductCode],PRONAME AS [Product],  PROPRICE  as Price, (PRODESC + ' [' + CATEGORY + ']') AS [Description],DATERECEIVED AS [TransactionDate],RECEIVEDQTY AS [Quantity], RECEIVEDTOTPRICE AS [TotalPrice]  FROM tblStockIn as S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE";
            config.Load_DTG(sql, DTGLIST);
        }

        private void frmListStockin_Load(object sender, EventArgs e)
        {
            btnRefresh_Click(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string numtrans = DTGLIST.CurrentRow.Cells[0].Value.ToString(); 
           int transnum = int.Parse(numtrans);
            sql = "UPDATE tblProductInfo AS P, tblStockIn AS S SET PROQTY = PROQTY - RECEIVEDQTY  WHERE P.PROCODE=S.PROCODE AND TRANSNUM =" + transnum ;
            config.Execute_Query(sql);

            sql = "DELETE * FROM tblStockIn WHERE TRANSNUM = " + transnum ; 
            config.Execute_CUD(sql,"error to execute the query.","Transaction has been deleted.");

            btnRefresh_Click(sender, e);
        }

        private void TXTSEARCH_TextChanged(object sender, EventArgs e)
        { 
                sql = "SELECT TRANSNUM AS [Transaction#], P.PROCODE as [ProductCode], PRONAME  AS [Product],(PRODESC + ' [' + CATEGORY + ']') AS [Description],PROPRICE as [Price],DATERECEIVED AS [TransactionDate],RECEIVEDQTY AS [Quantity], RECEIVEDTOTPRICE AS [TotalPrice]  FROM tblStockIn as S, tblProductInfo AS P  WHERE S.PROCODE=P.PROCODE AND " +
                      " (P.PROCODE   + ' '  + PRONAME + ' '  + PRODESC + ' '  + CATEGORY  ) LIKE '%" + TXTSEARCH.Text + "%'";
                config.Load_DTG(sql, DTGLIST); 
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DTGLIST_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
