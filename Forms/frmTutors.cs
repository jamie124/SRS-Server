using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace srs_server
{
    public partial class frmTutors : Form
    {
        UserManager mUserManager;
        bool mTutorsListModified;       // Tutors list has been modified
        TutorDetails mTutorDetails;
        public frmTutors(UserManager prUserManager)
        {
            InitializeComponent();

            mUserManager = prUserManager;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (mTutorsListModified)
            {
                if (MessageBox.Show("Save changes made to tutors list?", "Save confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    XmlHandler iXMLHandler = new XmlHandler();
                    iXMLHandler.SaveTutorList(mUserManager.Tutors);
                    Close();
                }
            }
            
            Close();
        }

        private void frmTutors_Load(object sender, EventArgs e)
        {
            if (mUserManager.Tutors != null)
            {
                if (mUserManager.Tutors.Count > 0)
                {
                    DisplayTutors();
                }
            }
        }

        private void DisplayTutors()
        {
            Dictionary<int, TutorDetails> iTutors = mUserManager.Tutors;

            // Clear list
            lstTutors.Items.Clear();

            foreach (KeyValuePair<int, TutorDetails> iTutor in iTutors)
            {
                lstTutors.Items.Add(iTutor.Value.Name);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Make sure the name is is not already in use
            if (txtName.Text != "")
                if (!mUserManager.IsUserATutor(txtName.Text))
                    if (txtPassword.Text != "")
                        if (txtClass.Text != "")
                        {
                            // Add a new tutor
                            TutorDetails iNewTutor = new TutorDetails();
                            iNewTutor.Name = txtName.Text;
                            iNewTutor.Password = txtPassword.Text;
                            iNewTutor.Class = txtClass.Text;
                            mUserManager.AddTutor(iNewTutor);

                            DisplayTutors();    // Redisplay tutor list
                            mTutorsListModified = true;
                        }
                        else
                            MessageBox.Show("Tutors class is missing, use NA if unknown", "Class missing");
                    else
                        MessageBox.Show("Starting password is missing", "Password missing");
                else
                    MessageBox.Show("Name is already in use", "Name taken");
            else
                MessageBox.Show("Tutor name is missing", "Name missing");
        }

        private void lstTutors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTutors.SelectedIndex > -1)
            {
                btnModify.Enabled = true;
                btnDelete.Enabled = true;
                txtName.Enabled = false;
                // Get the tutor
                mTutorDetails = mUserManager.GetTutorDetails(lstTutors.SelectedItem.ToString());

                if (mTutorDetails != null)
                {
                    // Display the details
                    txtName.Text = mTutorDetails.Name;
                    txtPassword.Text = mTutorDetails.Password;
                    txtClass.Text = mTutorDetails.Class;
                }
            }
            else
            {
                btnModify.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstTutors.SelectedItem != null)
            {
            string iTutorToDelete = lstTutors.SelectedItem.ToString();     // Ensure that the user deleted is the one selected.  
                if (MessageBox.Show("Are you sure you want to delete tutor: " + iTutorToDelete + "?", "Confirm Delete",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    mUserManager.RemoveTutor(iTutorToDelete);

                    DisplayTutors();    // Redisplay tutor list
                    mTutorsListModified = true;
                }
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (lstTutors.SelectedItem != null)
            {

                mTutorDetails.Password = txtPassword.Text;
                mTutorDetails.Class = txtClass.Text;

                if (MessageBox.Show("Are you sure you want to modify tutor: " + txtName.Text + "?", "Confirm Delete",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Delete and then insert the modified tutor
                    mUserManager.RemoveTutor(txtName.Text);
                    mUserManager.AddTutor(mTutorDetails);

                    DisplayTutors();    // Redisplay tutor list
                    mTutorsListModified = true;

                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Enabled = true;
            txtName.Text = "";
            txtPassword.Text = "";
            txtClass.Text = "";
        }
    }
}
