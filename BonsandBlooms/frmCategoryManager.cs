using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace BonsandBlooms
{
    public partial class frmCategoryManager : Form
    {
        public frmCategoryManager()
        {
            InitializeComponent();
            LoadCategories();
        }

        private DatabaseConnect config = new DatabaseConnect();

        private void LoadCategories()
        {
            string sql = "SELECT CategoryID, CategoryName FROM tblCategory ORDER BY CategoryName";
            config.Load_DTG(sql, dgvCategories);

            if (dgvCategories.Columns.Contains("CategoryID"))
                dgvCategories.Columns["CategoryID"].Visible = false;
        }

        public static string ShowInputDialog(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 340 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Text = defaultValue };
            Button confirmation = new Button() { Text = "OK", Left = 270, Width = 90, Top = 80, DialogResult = DialogResult.OK };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newCategory = ShowInputDialog("Enter new category name:", "Add Category");

            if (string.IsNullOrWhiteSpace(newCategory))
                return;

            newCategory = newCategory.Trim();

            string checkSql = "SELECT * FROM tblCategory WHERE CategoryName = ?";
            var checkParams = new OleDbParameter[] { new OleDbParameter("@CategoryName", newCategory) };
            int exists = config.maxrow(checkSql, checkParams);

            if (exists > 0)
            {
                MessageBox.Show("Category already exists.");
                return;
            }

            string insertSql = "INSERT INTO tblCategory (CategoryName) VALUES (?)";
            var insertParams = new OleDbParameter[] { new OleDbParameter("@CategoryName", newCategory) };
            config.Execute_CUD(insertSql, "Error adding category.", "Category added successfully!", insertParams);

            LoadCategories();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null)
            {
                MessageBox.Show("Please select a category to edit.");
                return;
            }

            int categoryId = Convert.ToInt32(dgvCategories.CurrentRow.Cells["CategoryID"].Value);
            string oldName = dgvCategories.CurrentRow.Cells["CategoryName"].Value.ToString();

            string newCategory = ShowInputDialog("Edit category name:", "Edit Category", oldName);

            if (string.IsNullOrWhiteSpace(newCategory) || newCategory == oldName)
                return;

            newCategory = newCategory.Trim();

            string checkSql = "SELECT * FROM tblCategory WHERE CategoryName = ? AND CategoryID <> ?";
            var checkParams = new OleDbParameter[]
            {
                new OleDbParameter("@CategoryName", newCategory),
                new OleDbParameter("@CategoryID", categoryId)
            };
            int exists = config.maxrow(checkSql, checkParams);

            if (exists > 0)
            {
                MessageBox.Show("Category name already exists.");
                return;
            }

            string updateSql = "UPDATE tblCategory SET CategoryName = ? WHERE CategoryID = ?";
            var updateParams = new OleDbParameter[]
            {
                new OleDbParameter("@CategoryName", newCategory),
                new OleDbParameter("@CategoryID", categoryId)
            };
            config.Execute_CUD(updateSql, "Error updating category.", "Category updated successfully!", updateParams);

            LoadCategories();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null)
            {
                MessageBox.Show("Please select a category to delete.");
                return;
            }

            int categoryId = Convert.ToInt32(dgvCategories.CurrentRow.Cells["CategoryID"].Value);
            string categoryName = dgvCategories.CurrentRow.Cells["CategoryName"].Value.ToString();

            var confirm = MessageBox.Show($"Are you sure you want to delete the category '{categoryName}'?", "Confirm Delete", MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                string usageSql = "SELECT COUNT(*) FROM tblProductInfo WHERE CATEGORY = ?";
                var usageParams = new OleDbParameter[] { new OleDbParameter("@CategoryName", categoryName) };
                int usageCount = 0;

                try
                {
                    DataTable dt = config.Execute_Query(usageSql, usageParams);
                    if (dt.Rows.Count > 0)
                        usageCount = Convert.ToInt32(dt.Rows[0][0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                if (usageCount > 0)
                {
                    MessageBox.Show("Cannot delete category because it is assigned to existing products.");
                    return;
                }

                string deleteSql = "DELETE FROM tblCategory WHERE CategoryID = ?";
                var deleteParams = new OleDbParameter[] { new OleDbParameter("@CategoryID", categoryId) };
                config.Execute_CUD(deleteSql, "Error deleting category.", "Category deleted successfully!", deleteParams);

                LoadCategories();
            }
        }

        private void frmCategoryManager_Load(object sender, EventArgs e)
        {

        }

        private void dgvCategories_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
