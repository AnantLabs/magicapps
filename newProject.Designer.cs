﻿namespace myWay
{
    partial class newProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(newProject));
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.rt1 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.butAddDirectory = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.butCancel2 = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.butTest = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.butSaveChanges = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(144, 46);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(253, 20);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "ma";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(144, 114);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(253, 20);
            this.txtHost.TabIndex = 2;
            this.txtHost.Text = "192.168.10.135";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Host";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(144, 188);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(253, 20);
            this.txtUser.TabIndex = 4;
            this.txtUser.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 195);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(144, 231);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(253, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // rt1
            // 
            this.rt1.Location = new System.Drawing.Point(403, 46);
            this.rt1.Name = "rt1";
            this.rt1.Size = new System.Drawing.Size(237, 293);
            this.rt1.TabIndex = 10;
            this.rt1.Text = "";
            this.rt1.TextChanged += new System.EventHandler(this.rt1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 238);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Type of database";
            // 
            // cmbDataType
            // 
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Location = new System.Drawing.Point(144, 78);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(253, 21);
            this.cmbDataType.TabIndex = 6;
            this.cmbDataType.SelectedIndexChanged += new System.EventHandler(this.cmbDataType_SelectedIndexChanged);
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(144, 152);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(214, 20);
            this.txtDatabase.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(48, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Database";
            // 
            // butAddDirectory
            // 
            this.butAddDirectory.Location = new System.Drawing.Point(368, 152);
            this.butAddDirectory.Name = "butAddDirectory";
            this.butAddDirectory.Size = new System.Drawing.Size(29, 23);
            this.butAddDirectory.TabIndex = 16;
            this.butAddDirectory.Text = "+";
            this.butAddDirectory.UseVisualStyleBackColor = true;
            this.butAddDirectory.Visible = false;
            this.butAddDirectory.Click += new System.EventHandler(this.butAddDirectory_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // butCancel2
            // 
            this.butCancel2.Location = new System.Drawing.Point(51, 290);
            this.butCancel2.Name = "butCancel2";
            this.butCancel2.Size = new System.Drawing.Size(109, 49);
            this.butCancel2.TabIndex = 20;
            this.butCancel2.Values.Image = global::myWay.Properties.Resources.delete;
            this.butCancel2.Values.Text = "Cancel";
            this.butCancel2.Click += new System.EventHandler(this.butCancel2_Click);
            // 
            // butTest
            // 
            this.butTest.Location = new System.Drawing.Point(178, 290);
            this.butTest.Name = "butTest";
            this.butTest.Size = new System.Drawing.Size(109, 49);
            this.butTest.TabIndex = 21;
            this.butTest.Values.Image = global::myWay.Properties.Resources.database_refresh;
            this.butTest.Values.Text = "Test";
            this.butTest.Click += new System.EventHandler(this.butTest_Click);
            // 
            // butSaveChanges
            // 
            this.butSaveChanges.Location = new System.Drawing.Point(293, 290);
            this.butSaveChanges.Name = "butSaveChanges";
            this.butSaveChanges.Size = new System.Drawing.Size(93, 49);
            this.butSaveChanges.TabIndex = 22;
            this.butSaveChanges.Values.Image = global::myWay.Properties.Resources.database_save;
            this.butSaveChanges.Values.Text = "Save";
            this.butSaveChanges.Click += new System.EventHandler(this.butSaveChanges_Click);
            // 
            // newProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 371);
            this.Controls.Add(this.butSaveChanges);
            this.Controls.Add(this.butTest);
            this.Controls.Add(this.butCancel2);
            this.Controls.Add(this.butAddDirectory);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbDataType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rt1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "newProject";
            this.Text = "newProject";
            this.Load += new System.EventHandler(this.newProject_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.RichTextBox rt1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button butAddDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton butCancel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton butTest;
        private ComponentFactory.Krypton.Toolkit.KryptonButton butSaveChanges;
    }
}