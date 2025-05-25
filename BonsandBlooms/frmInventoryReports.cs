using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BonsandBlooms
{
    public partial class frmInventoryReports : Form
    {
        public frmInventoryReports()
        {
            InitializeComponent();
        }
        DatabaseConnect config = new DatabaseConnect();
        usableFunction func = new usableFunction();
        string query;
        int maxrow;

        private void LoadCategories()
        {
            string sql = "SELECT CategoryName FROM tblCategory ORDER BY CategoryName";
            cboCateg.Items.Clear();
            config.Combo(sql, cboCateg);       

            cboCateg.Items.Insert(0, "All");
            cboCateg.SelectedIndex = 0;           
        }






        private void RDOSELECT(RadioButton RDO)
        {
            if (!RDO.Checked) return;

            string search = "%" + txtSearch.Text.Trim() + "%";
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            string categoryFilter = (cboCateg.Text != "All") ? " AND CATEGORY = ?" : "";

            switch (RDO.Text)
            {
                case "All":
                    query = "SELECT P.PROCODE AS [ProductCode], (PRONAME + ' - ' + PRODESC) AS [Product], CATEGORY AS [Category], PROPRICE AS [Price], PROQTY AS [Quantity] " +
                            "FROM tblProductInfo AS P WHERE PRONAME LIKE ?" + categoryFilter;
                    parameters.Add(new OleDbParameter("@search", search));
                    if (cboCateg.Text != "All")
                        parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                    break;

                case "Stock-In":
                    query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATERECEIVED) AS [ReceivedDate], (PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], RECEIVEDQTY AS [Quantity], (RECEIVEDQTY * PROPRICE) AS [TotalPrice] " +
                            "FROM tblStockIn AS S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE" + categoryFilter + " AND PRONAME LIKE ?";
                    if (cboCateg.Text != "All")
                        parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                    parameters.Add(new OleDbParameter("@search", search));
                    break;

                case "Sold":
                    query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATEOUT) AS [TransactionDate], (PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], OUTQTY AS [Quantity], (OUTQTY * PROPRICE) AS [TotalPrice] " +
                            "FROM tblStockOut AS S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE" + categoryFilter + " AND PRONAME LIKE ?";
                    if (cboCateg.Text != "All")
                        parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                    parameters.Add(new OleDbParameter("@search", search));
                    break;
            }

            config.Load_DTG(query, dtglist, parameters.ToArray());
        }

        private void RDOCLEAR(string ACTIONS, RadioButton rdo)
        {
            if (rdo.Checked)
            {
                switch (ACTIONS)
                {
                    case "Product":
                        rdoProduct.Checked = false;
                        break;
                    case "Transaction":
                        rdoStockIn.Checked = false;
                        rdoStockOut.Checked = false;
                        rdoMonthly.Checked = false;
                        rdoToday.Checked = false;
                        break;
                }

                if (rdo.Text != "Today" && rdo.Text != "Monthly")
                {
                    LBLLIST.Text = "List of " + rdo.Text + " (" + cboCateg.Text + ")";
                }
            }
        }

        private void RDOCLEARDATE()
        {
            rdoMonthly.Checked = false;
            rdoToday.Checked = false;
        }

        private void frmInventoryReports_Load(object sender, EventArgs e)
        {
            LoadCategories();
            RDOCLEAR("Transaction", rdoProduct);
            LBLLIST.Text = "List of Products (" + cboCateg.Text + ")";
            RDOSELECT(rdoProduct);
        }


        private void rdoProduct_CheckedChanged(object sender, EventArgs e)
        {
            RDOCLEAR("Transaction", rdoProduct);
            LBLLIST.Text = "List of Products (" + cboCateg.Text + ")";
            RDOSELECT(rdoProduct);
        }

        private void rdoStockIn_CheckedChanged(object sender, EventArgs e)
        {
            RDOCLEAR("Product", rdoStockIn);
            RDOCLEARDATE();
            RDOSELECT(rdoStockIn);
        }

        private void rdoStockOut_CheckedChanged(object sender, EventArgs e)
        {
            RDOCLEAR("Product", rdoStockOut);
            RDOCLEARDATE();
            RDOSELECT(rdoStockOut);
        }

        private void rdoToday_CheckedChanged(object sender, EventArgs e)
        {
            string search = "%" + txtSearch.Text.Trim() + "%";
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            string categoryFilter = (cboCateg.Text != "All") ? " AND CATEGORY = ?" : "";

            if (rdoStockIn.Checked)
            {
                RDOCLEAR("Product", rdoStockIn);
                query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATERECEIVED) AS [ReceivedDate], (PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], RECEIVEDQTY AS [Quantity], (RECEIVEDQTY * PROPRICE) AS [TotalPrice] " +
                        "FROM tblStockIn AS S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE AND DATEVALUE(DATERECEIVED) = DATEVALUE(NOW())" + categoryFilter + " AND PRONAME LIKE ?";
                if (cboCateg.Text != "All")
                    parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                parameters.Add(new OleDbParameter("@search", search));
            }
            else if (rdoStockOut.Checked)
            {
                RDOCLEAR("Product", rdoStockOut);
                query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATEOUT) AS [TransactionDate], (PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], OUTQTY AS [Quantity], (OUTQTY * PROPRICE) AS [TotalPrice] " +
                        "FROM tblStockOut AS S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE AND DATEVALUE(DATEOUT) = DATEVALUE(NOW())" + categoryFilter + " AND PRONAME LIKE ?";
                if (cboCateg.Text != "All")
                    parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                parameters.Add(new OleDbParameter("@search", search));
            }
            config.Load_DTG(query, dtglist, parameters.ToArray());
        }

        private void rdoMonthly_CheckedChanged(object sender, EventArgs e)
        {
            string search = "%" + txtSearch.Text.Trim() + "%";
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            string categoryFilter = (cboCateg.Text != "All") ? " AND CATEGORY = ?" : "";

            if (rdoStockIn.Checked)
            {
                RDOCLEAR("Product", rdoStockIn);
                query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATERECEIVED) AS [ReceivedDate], (PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], RECEIVEDQTY AS [Quantity], (RECEIVEDQTY * PROPRICE) AS [TotalPrice] " +
                        "FROM tblStockIn AS S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE AND MONTH(DATERECEIVED) = MONTH(NOW())" + categoryFilter + " AND PRONAME LIKE ?";
                if (cboCateg.Text != "All")
                    parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                parameters.Add(new OleDbParameter("@search", search));
            }
            else if (rdoStockOut.Checked)
            {
                RDOCLEAR("Product", rdoStockOut);
                query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATEOUT) AS [TransactionDate], (PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], OUTQTY AS [Quantity], (OUTQTY * PROPRICE) AS [TotalPrice] " +
                        "FROM tblStockOut AS S, tblProductInfo AS P WHERE S.PROCODE=P.PROCODE AND MONTH(DATEOUT) = MONTH(NOW())" + categoryFilter + " AND PRONAME LIKE ?";
                if (cboCateg.Text != "All")
                    parameters.Add(new OleDbParameter("@category", cboCateg.Text));
                parameters.Add(new OleDbParameter("@search", search));
            }
            config.Load_DTG(query, dtglist, parameters.ToArray());
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime dfrom = dtpFrom.Value.Date;
            DateTime dto = dtpTo.Value.Date;
            string search = "%" + txtSearch.Text.Trim() + "%";

            List<OleDbParameter> parameters = new List<OleDbParameter>
    {
        new OleDbParameter("@from", OleDbType.Date) { Value = dfrom },
        new OleDbParameter("@to", OleDbType.Date) { Value = dto }
    };

            string categoryFilter = (cboCateg.Text != "All") ? " AND CATEGORY = ?" : "";

            if (rdoStockIn.Checked)
            {
                LBLLIST.Text = $"Stock of {cboCateg.Text} from {dfrom:d} to {dto:d}";
                query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATERECEIVED) AS [ReceivedDate], " +
                        "(PRONAME + ' - ' + PRODESC) AS [Product], PROPRICE AS [Price], RECEIVEDQTY AS [Quantity], " +
                        "(RECEIVEDQTY * PROPRICE) AS [TotalPrice] " +
                        "FROM tblStockIn AS S, tblProductInfo AS P " +
                        "WHERE S.PROCODE = P.PROCODE AND DATEVALUE(DATERECEIVED) BETWEEN ? AND ?" + categoryFilter +
                        " AND (PRODESC LIKE ? OR PRONAME LIKE ?)";

                if (cboCateg.Text != "All")
                    parameters.Add(new OleDbParameter("@category", cboCateg.Text));

                parameters.Add(new OleDbParameter("@search1", search));
                parameters.Add(new OleDbParameter("@search2", search));
            }
            else if (rdoStockOut.Checked)
            {
                LBLLIST.Text = $"Sold {cboCateg.Text} from {dfrom:d} to {dto:d}";
                query = "SELECT P.PROCODE AS [ProductCode], DATEVALUE(DATEOUT) AS [TransactionDate], " +
                        "(PRONAME + ' ' + PRODESC) AS [Product], PROPRICE AS [Price], OUTQTY AS [Quantity], " +
                        "(OUTQTY * PROPRICE) AS [TotalPrice] " +
                        "FROM tblStockOut AS S, tblProductInfo AS P " +
                        "WHERE S.PROCODE = P.PROCODE AND DATEVALUE(DATEOUT) BETWEEN ? AND ?" + categoryFilter +
                        " AND (PRODESC LIKE ? OR PRONAME LIKE ?)";

                if (cboCateg.Text != "All")
                    parameters.Add(new OleDbParameter("@category", cboCateg.Text));

                parameters.Add(new OleDbParameter("@search1", search));
                parameters.Add(new OleDbParameter("@search2", search));
            }

            config.Load_DTG(query, dtglist, parameters.ToArray());
        }


        private void BTNSEARCHGRID_Click(object sender, EventArgs e)
        {
            RDOCLEAR("Transaction", rdoProduct);
            LBLLIST.Text = "List of Products (" + cboCateg.Text + ")";
            RDOSELECT(rdoProduct);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboCateg_SelectedIndexChanged(object sender, EventArgs e)
        {
            RDOSELECT(rdoProduct);
        }

        private void dtglist_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Label1_Click(object sender, EventArgs e)
        {
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
