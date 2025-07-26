using System;
using System.Windows.Forms;
using AWS_DB_Update.Modals;
using AWS_DB_Update.Helper;

namespace AWS_DB_Update
{
    public partial class frmEnviornmentSetup : Form
    {
        public frmEnviornmentSetup()
        {
            InitializeComponent();
            try
            {
                var objUser = ConfigHelper.ReadLoginConfigJson();
                if (objUser != null)
                {
                    txtCompany.Text = objUser.Company;
                    txtPassword.Text = objUser.Password;
                    txtUser.Text = objUser.User;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("frmEnviornmentSetup Error: " + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCompany.Text = "";
            txtPassword.Text = "";
            txtUser.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            frmEnviornmentSetup.ActiveForm.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var strCompanyName = txtCompany.Text;
                var strUserName = txtUser.Text;
                var strPassword = txtPassword.Text;
                if (strCompanyName == string.Empty)
                {
                    MessageBox.Show("Please enter a valid company!");
                    txtCompany.Focus();
                    return;
                }
                if (strUserName == string.Empty)
                {
                    MessageBox.Show("Please enter a valid user!");
                    txtUser.Focus();
                    return;
                }
                if (strPassword == string.Empty)
                {
                    MessageBox.Show("Please enter a valid password!");
                    txtPassword.Focus();
                    return;
                }
                var objUser = new LoginModal
                {
                    Company = strCompanyName.Trim(),
                    Password = strPassword.Trim(),
                    User = strUserName.Trim()
                };

                if (ConfigHelper.WriteLoginConfigJson(objUser))
                    MessageBox.Show("Data saved successfully!");
                else
                    MessageBox.Show("Falied to save data!");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("btnSave_Click Error: " + ex.Message);                
            }
        }
    }
}
