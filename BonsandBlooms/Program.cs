using System;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using ADOX;  

namespace BonsandBlooms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string dbFileName = "BonsandBloomsDatabase.accdb";
            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);

            if (!File.Exists(dbFilePath))
            {
                CreateAccessDatabase(dbFilePath);
                CreateTables(dbFilePath);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BonsandBlooms());
        }

        static void CreateAccessDatabase(string filePath)
        {
            try
            {
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};";
                Catalog catalog = new Catalog();
                catalog.Create(connString);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(catalog);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating database file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        static void CreateTables(string filePath)
        {
            string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                try
                {
                    connection.Open();

                    string[] tableCommands = new string[]
                    {
                @"CREATE TABLE tblStockin (
                    TRANSID AUTOINCREMENT PRIMARY KEY,
                    TRANSNUM LONG,
                    PROID LONG,
                    PROCODE TEXT(50),
                    DATERECEIVED DATETIME,
                    RECEIVEDQTY LONG,
                    RECEIVEDUNIT TEXT(20),
                    RECEIVEDTOTPRICE CURRENCY
                )",

                @"CREATE TABLE tblStockout (
                    TRANSID AUTOINCREMENT PRIMARY KEY,
                    TRANSNUM LONG,
                    PROID LONG,
                    PROCODE TEXT(50),
                    DATEOUT DATETIME,
                    OUTQTY LONG,
                    OUTUNIT TEXT(20),
                    OUTTOTPRICE CURRENCY,
                    RETURNED YESNO
                )",

                @"CREATE TABLE tblUser (
                    USERID AUTOINCREMENT PRIMARY KEY,
                    U_NAME TEXT(100),
                    U_UNAME TEXT(50),
                    U_PASS TEXT(50),
                    U_TYPE TEXT(50)
                )",

                @"CREATE TABLE tblAutonumber (
                    ID AUTOINCREMENT PRIMARY KEY,
                    STARTNUM TEXT(50),
                    INCNUM TEXT(50),
                    ENDNUM TEXT(50),
                    TYPE TEXT(50)
                )",

                @"CREATE TABLE tblCategory (
                    CategoryID AUTOINCREMENT PRIMARY KEY,
                    CategoryName TEXT(100)
                )",

                @"CREATE TABLE tblProductInfo (
                    PROID AUTOINCREMENT PRIMARY KEY,
                    PROCODE TEXT(50),
                    PRONAME TEXT(100),
                    PRODESC TEXT(255),
                    PROPRICE CURRENCY,
                    CATEGORY TEXT(100),
                    PROQTY LONG
                )"
                    };

                    foreach (var cmdText in tableCommands)
                    {
                        using (OleDbCommand cmd = new OleDbCommand(cmdText, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string insertAdmin = @"INSERT INTO tblUser (U_NAME, U_UNAME, U_PASS, U_TYPE) 
                                   VALUES ('Administrator', 'admin', 'admin', 'Administrator')";

                    using (OleDbCommand cmd = new OleDbCommand(insertAdmin, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    string insertAutonumber = @"INSERT INTO tblAutoNumber (STARTNUM, INCNUM, ENDNUM, TYPE) 
                            VALUES (1000, 0, 9999, 'Product')";
                    using (OleDbCommand cmd = new OleDbCommand(insertAutonumber, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating tables or inserting default user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
        }

    }
}
