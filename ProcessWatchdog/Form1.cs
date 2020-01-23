using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace ProcessWatchdog {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void watchdogThread() {
            String processName = null;
            String programPath = null;
            String programArgs = null;
            try {
                using (StreamReader sr = new StreamReader("ProcessWatchdog.txt")) {
                    String line;
                    while ((line = sr.ReadLine()) != null) {
                        if (!line.StartsWith("#")) {
                            if (line.StartsWith("Processname")) {
                                String[] nameParts = line.Split('=');
                                processName = nameParts[1].Trim();
                            }
                            if (line.StartsWith("Path")) {
                                String[] pathParts = line.Split('=');
                                programPath = pathParts[1].Trim();
                            }
                            if (line.StartsWith("Args")) {
                                String[] argParts = line.Split('=');
                                programArgs = argParts[1].Trim();
                            }
                        }
                    }
                }
                if (processName != null && programPath != null) {
                    while (true) {
                        Thread.Sleep(500);
                        Process[] processlist = Process.GetProcesses();
                        bool isRunning = false;
                        foreach (Process theprocess in processlist) {
                            if (theprocess.ProcessName.ToLower().Contains(processName.ToLower())) {
                                isRunning = true;
                            }
                        }
                        if (!isRunning) {
                            if (programArgs != null) {
                                Process.Start(programPath, programArgs);
                            }
                            else { 
                                Process.Start(programPath);
                            }
                        }
                    }
                }
                else {
                    MessageBox.Show("Fehler beim Initialisieren");
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Fehler: " + ex.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            Thread t1 = new Thread(new ThreadStart(watchdogThread));
            t1.Start();
        }
    }
}
