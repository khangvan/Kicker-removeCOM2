namespace Kicker
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProdOrder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSAPSerial = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPSCBarcode = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.chkRework = new System.Windows.Forms.CheckBox();
            this.labelSKModel = new System.Windows.Forms.Label();
            this.labelSKSerial = new System.Windows.Forms.Label();
            this.labelSKProductionOrder = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(128, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model:";
            // 
            // txtModel
            // 
            this.txtModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModel.Location = new System.Drawing.Point(202, 25);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(209, 29);
            this.txtModel.TabIndex = 1;
            this.txtModel.TextChanged += new System.EventHandler(this.txtModel_TextChanged);
            this.txtModel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtModel_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(119, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Serial #:";
            // 
            // txtSerial
            // 
            this.txtSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial.Location = new System.Drawing.Point(202, 82);
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(209, 29);
            this.txtSerial.TabIndex = 2;
            this.txtSerial.TextChanged += new System.EventHandler(this.txtSerial_TextChanged);
            this.txtSerial.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSerial_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(175, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Production Order:#:";
            // 
            // txtProdOrder
            // 
            this.txtProdOrder.Enabled = false;
            this.txtProdOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProdOrder.Location = new System.Drawing.Point(202, 138);
            this.txtProdOrder.Name = "txtProdOrder";
            this.txtProdOrder.Size = new System.Drawing.Size(209, 29);
            this.txtProdOrder.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(108, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "SAP SN::";
            this.label4.Visible = false;
            // 
            // txtSAPSerial
            // 
            this.txtSAPSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSAPSerial.Location = new System.Drawing.Point(202, 193);
            this.txtSAPSerial.Name = "txtSAPSerial";
            this.txtSAPSerial.Size = new System.Drawing.Size(209, 29);
            this.txtSAPSerial.TabIndex = 3;
            this.txtSAPSerial.Visible = false;
            this.txtSAPSerial.TextChanged += new System.EventHandler(this.txtSAPSerial_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(99, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "(barcode):";
            this.label5.Visible = false;
            // 
            // lblPSCBarcode
            // 
            this.lblPSCBarcode.AutoSize = true;
            this.lblPSCBarcode.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblPSCBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPSCBarcode.Location = new System.Drawing.Point(230, 232);
            this.lblPSCBarcode.Margin = new System.Windows.Forms.Padding(3);
            this.lblPSCBarcode.Name = "lblPSCBarcode";
            this.lblPSCBarcode.Size = new System.Drawing.Size(75, 20);
            this.lblPSCBarcode.TabIndex = 14;
            this.lblPSCBarcode.Text = "*789987*";
            this.lblPSCBarcode.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(43, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(190, 49);
            this.button1.TabIndex = 15;
            this.button1.Text = "Interrupt";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkRework
            // 
            this.chkRework.AutoSize = true;
            this.chkRework.Enabled = false;
            this.chkRework.Location = new System.Drawing.Point(435, 313);
            this.chkRework.Name = "chkRework";
            this.chkRework.Size = new System.Drawing.Size(63, 17);
            this.chkRework.TabIndex = 16;
            this.chkRework.Text = "Rework";
            this.chkRework.UseVisualStyleBackColor = true;
            // 
            // labelSKModel
            // 
            this.labelSKModel.AutoSize = true;
            this.labelSKModel.Location = new System.Drawing.Point(112, 58);
            this.labelSKModel.Name = "labelSKModel";
            this.labelSKModel.Size = new System.Drawing.Size(0, 13);
            this.labelSKModel.TabIndex = 17;
            this.labelSKModel.Visible = false;
            // 
            // labelSKSerial
            // 
            this.labelSKSerial.AutoSize = true;
            this.labelSKSerial.Location = new System.Drawing.Point(100, 109);
            this.labelSKSerial.Name = "labelSKSerial";
            this.labelSKSerial.Size = new System.Drawing.Size(35, 13);
            this.labelSKSerial.TabIndex = 18;
            this.labelSKSerial.Text = "label6";
            this.labelSKSerial.Visible = false;
            // 
            // labelSKProductionOrder
            // 
            this.labelSKProductionOrder.AutoSize = true;
            this.labelSKProductionOrder.Location = new System.Drawing.Point(22, 167);
            this.labelSKProductionOrder.Name = "labelSKProductionOrder";
            this.labelSKProductionOrder.Size = new System.Drawing.Size(35, 13);
            this.labelSKProductionOrder.TabIndex = 19;
            this.labelSKProductionOrder.Text = "label6";
            this.labelSKProductionOrder.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(239, 296);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(172, 48);
            this.button2.TabIndex = 20;
            this.button2.Text = "Mở nguồn COM6";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 357);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.labelSKProductionOrder);
            this.Controls.Add(this.labelSKSerial);
            this.Controls.Add(this.labelSKModel);
            this.Controls.Add(this.chkRework);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblPSCBarcode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtSAPSerial);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtProdOrder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSerial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtModel);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "KICKER-WITHOUT MAGICBOX CONTROL";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProdOrder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSAPSerial;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPSCBarcode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkRework;
        private System.Windows.Forms.Label labelSKModel;
        private System.Windows.Forms.Label labelSKSerial;
        private System.Windows.Forms.Label labelSKProductionOrder;
        private System.Windows.Forms.Button button2;
    }
}

