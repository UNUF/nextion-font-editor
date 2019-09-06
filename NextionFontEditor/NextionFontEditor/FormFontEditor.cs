﻿using System;
using System.Linq;
using System.Windows.Forms;
using ZiLib.FileVersion.V3;

namespace NextionFontEditor {

    public partial class FormFontEditor : Form {

        public FormFontEditor() {
            InitializeComponent();
        }

        private ZiLib.IZiFont ziFont;

        private void FormFontEditor_Load(object sender, EventArgs e) {
            cmbZoom.Items.AddRange(Enumerable.Range(1, 30).Select(x => $"{x}x").ToArray());
            cmbZoom.SelectedIndex = 9;
        }

        private void UpdateCharacter() {
            btnAddCharacters.Enabled = ziFont != null;
            btnClear.Enabled = ziFont != null;
            btnCopy.Enabled = ziFont != null;
            btnDeleteCharacter.Enabled = ziFont != null;
            btnMoveDown.Enabled = ziFont != null;
            btnMoveLeft.Enabled = ziFont != null;
            btnMoveUp.Enabled = ziFont != null;

            if (ziFont == null)
            {
                numChar.Value = 0;
                return;
            }

            if (numChar.Value >= ziFont.Characters.Length) {
                numChar.Value = 0;
            }

            if (numChar.Value < 0) {
                numChar.Value = ziFont.Characters.Length - 1;
            }

            var bmp = ziFont.Characters[(int)numChar.Value].GetBitmap();
            if (bmp != null && bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Undefined)
            {
                charEditor1.CharImage = bmp;
                pPreview.Image = charEditor1.CharImage;
            } else {
            }
            txtEncoding.Text = ziFont.CodePage.CodePageIdentifier.GetDescription();
            txtCodePoint.Text = ziFont.Characters[(int)numChar.Value].CodePoint.ToString();

            btnRevertCharacter.Enabled = ziFont.Characters[(int)numChar.Value].CanRevert();
        }

        private void numChar_ValueChanged(object sender, EventArgs e) {
            UpdateCharacter();
        }

        private void btnOpenFont_Click(object sender, EventArgs e)
        {
            var res = ofd.ShowDialog();

            if (res == DialogResult.OK)
            {
                var ziFont2 = ZiLib.FileVersion.Common.ZiFont.FromFile(ofd.FileName);

                if (ziFont2 != null)
                {
                    ziFont = ziFont2;
                    numChar.Maximum = ziFont.CodePage.CharacterCount; // - 1;
                    numChar.Minimum = -1;
                    numChar.Value = 1;
                }
                else {
                    MessageBox.Show("Unsopported file format.","Error",MessageBoxButtons.OK);
                }
                UpdateCharacter();
            }
        }

        private void panel1_Resize(object sender, EventArgs e) {
            charEditor1.Left = (panel1.Width / 2) - (charEditor1.Width / 2);
            charEditor1.Top = (panel1.Height / 2) - (charEditor1.Height / 2);
        }

        private void FormFontEditor_Shown(object sender, EventArgs e) {
            panel1_Resize(sender, e);
        }

        private void cmbZoom_TextChanged(object sender, EventArgs e) {
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e) {
            charEditor1.Zoom = int.Parse(cmbZoom.Text.Replace("x", ""));
            panel1_Resize(sender, e);
        }

        private void btnShowGrid_Click(object sender, EventArgs e) {
            charEditor1.ShowGrid = btnShowGrid.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            var res = sfd.ShowDialog();

            if (res == DialogResult.OK) {

                ziFont.Save(sfd.FileName, ziFont.CodePage);
            }
        }

        private void btnClear_Click(object sender, EventArgs e) {
            charEditor1.Clear();
        }

        private void charEditor1_Paint(object sender, PaintEventArgs e) {
            if (charEditor1.CharImage.PixelFormat != System.Drawing.Imaging.PixelFormat.Undefined)
            {
                pPreview.Image = charEditor1.CharImage;
            }
            else
            {
            }
        }

        private void btnMoveLeft_Click(object sender, EventArgs e) {
            charEditor1.MoveCharacterX(-1);
        }

        private void btnMoveRight_Click(object sender, EventArgs e) {
            charEditor1.MoveCharacterX(1);
        }

        private void btnMoveUp_Click(object sender, EventArgs e) {
            charEditor1.MoveCharacterY(-1);
        }

        private void btnMoveDown_Click(object sender, EventArgs e) {
            charEditor1.MoveCharacterY(1);
        }

        private void CharEditor1_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            ziFont.Characters[(int)numChar.Value].CopyToClipboard();
        }

        private void btnDeleteCharacter_Click(object sender, EventArgs e)
        {
            ziFont.RemoveCharacter((int)numChar.Value);
            UpdateCharacter();
        }

        private void btnRevertCharacter_Click(object sender, EventArgs e)
        {
            ziFont.Characters[(int)numChar.Value].RevertBitmap();
            UpdateCharacter();
        }

        private void BtnUndo_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnAddCharacters_Click(object sender, EventArgs e)
        {
            using (var form = new FormAddCharacters())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {

                    for (ushort ch = 32; ch <= 127; ch++) {
                        if (ziFont.CodePage.CodePoints.Contains(ch)) {
                            if (Array.Exists(ziFont.Characters, character => { return character.CodePoint == ch; })) {
                                if (true) {
                                    // replace existing char
                                }
                            } else
                            {
                                var txt = Char.ConvertFromUtf32((int)ch);
                                var bmp = ZiLib.Extensions.BitmapExtensions.DrawString(txt, "", (byte)ziFont.CharacterHeight);
                                var bytes = ZiLib.FileVersion.V5.BinaryTools.BitmapTo3BppData(bmp);
                                ziFont.AddCharacter(ch, bytes, (byte)bmp.Width);
                            }
                        }
                    }

                }
            }
        }
    }
}