using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using WinFormsEditExample;

using WW.Cad.IO;
using WW.Cad.Model;
using WW.Cad.Model.Entities;
using WW.Math;

namespace WinFormsEditExample {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// Opens a DXF file and passes it to the MainForm.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string filename = null;
            if (args.Length > 0) {
                filename = args[0];
            } else {
                // Uncomment to open an existing file.
//                filename = OpenFile();
            }

            DxfModel model;
            if (!string.IsNullOrEmpty(filename)) {
                string extension = Path.GetExtension(filename);
                if (string.Compare(extension, ".dwg", true) == 0) {
                    model = DwgReader.Read(filename);
                } else {
                    model = DxfReader.Read(filename);
                }
            } else {
                model = new DxfModel();
            }
            Application.Run(new MainForm(model));
        }

      

        private static string OpenFile() {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "AutoCad files (*.dxf, *.dwg)|*.dxf;*.dwg";
            string filename = null;
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    filename = openFileDialog.FileName;
                } catch (Exception ex) {
                    MessageBox.Show("Error occurred: " + ex.Message);
                    Environment.Exit(1);
                }
            } else {
                Environment.Exit(0);
            }
            return filename;
        }
    }
}
