using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Tekla.Structures;
using Tekla.Structures.Dialog;

namespace cs_net_lib
{
    public class WarningDialog : Form
    {
        private readonly static Dictionary<string, string> LanguageCodeLookup;

        private IContainer components;

        private PictureBox pictureBox;

        private CheckBox chkDoNotShow;

        private Label txtMessage;

        private Button OkBtn;

        public bool DoNotShow
        {
            get;
            private set;
        }

        static WarningDialog()
        {
            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { "ENGLISH", "enu" },
                { "DUTCH", "nld" },
                { "FRENCH", "fra" },
                { "GERMAN", "deu" },
                { "ITALIAN", "ita" },
                { "SPANISH", "esp" },
                { "JAPANESE", "jpn" },
                { "CHINESE SIMPLIFIED", "chs" },
                { "CHINESE TRADITIONAL", "cht" },
                { "CZECH", "csy" },
                { "PORTUGUESE BRAZILIAN", "ptb" },
                { "HUNGARIAN", "hun" },
                { "POLISH", "plk" },
                { "RUSSIAN", "rus" }
            };
            WarningDialog.LanguageCodeLookup = strs;
        }

        public WarningDialog(string Caption, string Message)
        {
            this.InitializeComponent();
            base.Text = Caption;
            this.txtMessage.Text = Message;
            this.LocalizeForm("WarningDialog.xml");
            base.Location = System.Windows.Forms.Cursor.Position;
        }

        private void CheckDoNotShow(object sender, EventArgs e)
        {
            this.DoNotShow = this.chkDoNotShow.Checked;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private static string GetCurrentLanguage()
        {
            string str;
            string empty = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XS_LANGUAGE", ref empty);
            if (empty == null)
            {
                str = "enu";
                Trace.WriteLine("GetCurrentLanguage : XS_LANGUAGE option returns null");
            }
            else if (!WarningDialog.LanguageCodeLookup.TryGetValue(empty, out str))
            {
                str = "enu";
                Trace.WriteLine(string.Format("XS_LANGUAGE ({0}) cannot be mapped to language code", empty));
            }
            return str;
        }

        private void InitializeComponent()
        {
            this.pictureBox = new PictureBox();
            this.chkDoNotShow = new CheckBox();
            this.txtMessage = new Label();
            this.OkBtn = new Button();
            ((ISupportInitialize)this.pictureBox).BeginInit();
            base.SuspendLayout();
            this.pictureBox.Location = new Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(40, 40);
            this.pictureBox.TabIndex = 7;
            this.pictureBox.TabStop = false;
            this.chkDoNotShow.AutoSize = true;
            this.chkDoNotShow.Location = new Point(61, 35);
            this.chkDoNotShow.Name = "chkDoNotShow";
            this.chkDoNotShow.Size = new System.Drawing.Size(214, 17);
            this.chkDoNotShow.TabIndex = 6;
            this.chkDoNotShow.Text = "albl_do_not_show_this_message_again";
            this.chkDoNotShow.UseVisualStyleBackColor = true;
            this.chkDoNotShow.CheckedChanged += new EventHandler(this.CheckDoNotShow);
            this.txtMessage.AutoSize = true;
            this.txtMessage.Location = new Point(58, 12);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(367, 13);
            this.txtMessage.TabIndex = 5;
            this.txtMessage.Text = "Illegal cast unit type of main part. Only cast in place is allowed in this module!";
            this.OkBtn.Anchor = AnchorStyles.Bottom;
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Location = new Point(168, 80);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 4;
            this.OkBtn.Text = "albl_OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(449, 115);
            base.Controls.Add(this.pictureBox);
            base.Controls.Add(this.chkDoNotShow);
            base.Controls.Add(this.txtMessage);
            base.Controls.Add(this.OkBtn);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            base.Name = "WarningDialog";
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "Cast unit";
            ((ISupportInitialize)this.pictureBox).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LocalizeForm(string PrivateLocalizationFile)
        {
            try
            {
                string currentLanguage = WarningDialog.GetCurrentLanguage();
                Localization localization = new Localization(Localization.DefaultLocalizationFile, currentLanguage);
                if (PrivateLocalizationFile != null)
                {
                    string str = string.Concat(Localization.DefaultLocalizationPath, PrivateLocalizationFile);
                    if (!File.Exists(str))
                    {
                        Trace.WriteLine(string.Format("Localization file {0} is missing", str));
                    }
                    else
                    {
                        localization.LoadXMLFile(str);
                    }
                }
                localization.Localize(this);
                this.pictureBox.Image = Bitmap.FromHicon(SystemIcons.Warning.Handle);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Concat("Exception: ", exception.Message));
            }
        }
    }
}