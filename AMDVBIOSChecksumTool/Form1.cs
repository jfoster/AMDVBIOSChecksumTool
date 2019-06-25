using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace AMDVBIOSChecksumTool
{
    public partial class Form1 : Form
    {
        byte[] buffer;
        byte currentChecksum;
        byte expectedChecksum;

        public Form1()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ROM files (*.rom)|*.rom|BIN files (*.bin)|*.bin|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Stream openFileStream = null;
                try
                {
                    openFileStream = openFileDialog.OpenFile();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Cannot read file from disk. Original error: " + Ex.Message);
                }

                if (openFileStream != null) {
                    using (BinaryReader br = new BinaryReader(openFileStream))
                    {
                        buffer = br.ReadBytes((int)openFileStream.Length);

                        int atom_rom_header_offs = BitConverter.ToInt16(buffer.Skip(ATOM.ATOM_ROM_HEADER_POINTER).Take(0x2).ToArray(), 0);
                        ATOM.ATOM_ROM_HEADER atom_rom_header = Struct.FromBytes<ATOM.ATOM_ROM_HEADER>(buffer.Skip(atom_rom_header_offs).ToArray());

                        string firmwareSignature = new string(atom_rom_header.uaFirmWareSignature);
                        if (!firmwareSignature.Equals("ATOM"))
                        {
                            TextBox1.Text = TextBox2.Text = "";
                            SaveFileButton.Enabled = false;
                            MessageBox.Show("WARNING! BIOS Signature is not valid!");
                            return;
                        }

                        currentChecksum = buffer[ATOM.ATOM_ROM_CHECKSUM_OFFSET];

                        int size = buffer[ATOM.ATOM_ROM_SIZE_OFFSET] * 512;
                        byte check = 0;
                        for (int i = 0; i < size; i++)
                        {
                            check += buffer[i];
                        }

                        expectedChecksum = (byte)(currentChecksum - check);

                        if (check == 0)
                        {
                            TextBox1.ForeColor = Color.DarkGreen;
                            SaveFileButton.Enabled = false;
                        } else {
                            TextBox1.ForeColor = Color.Red;
                            SaveFileButton.Enabled = true;
                        }

                        TextBox1.Text = "0x" + currentChecksum.ToString("X");
                        TextBox2.Text = "0x" + expectedChecksum.ToString("X");
                    }
                    openFileStream.Close();
                }
            }
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "ROM files (*.rom)|*.rom|BIN files (*.bin)|*.bin|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                DialogResult saveResult = saveFileDialog.ShowDialog();
                if (saveResult != DialogResult.OK)
                {
                    return;
                }

                if (buffer.Length > 0 && currentChecksum != expectedChecksum)
                {
                    currentChecksum = expectedChecksum;
                    buffer[ATOM.ATOM_ROM_CHECKSUM_OFFSET] = expectedChecksum;
                    File.WriteAllBytes(saveFileDialog.FileName, buffer);

                    TextBox1.ForeColor = Color.DarkGreen;
                    TextBox1.Text = "0x" + currentChecksum.ToString("X");
                    TextBox2.Text = "0x" + expectedChecksum.ToString("X");

                    SaveFileButton.Enabled = false;
                }
            }
        }

        ~Form1()
        {

        }
    }
}