﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace shadowsocks_csharp
{
    public partial class ConfigForm : Form
    {
        ShadowsocksController controller;

        public ConfigForm(ShadowsocksController controller)
        {
            InitializeComponent();
            notifyIcon1.ContextMenu = contextMenu1;

            this.controller = controller;
            controller.EnableStatusChanged += controller_EnableStatusChanged;
            controller.ConfigChanged += controller_ConfigChanged;

            updateUI();
        }

        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        private void controller_EnableStatusChanged(object sender, EventArgs e)
        {
            updateUI();
        }
        
        private void showWindow()
        {
            this.Opacity = 1;
            this.Show();
        }

        private void updateUI()
        {
            Config config = controller.GetConfig();

            textBox1.Text = config.server;
            textBox2.Text = config.server_port.ToString();
            textBox3.Text = config.password;
            textBox4.Text = config.local_port.ToString();
            comboBox1.Text = config.method == null ? "aes-256-cfb" : config.method;

            enableItem.Checked = config.enabled;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!controller.GetConfig().isDefault)
            {
                this.Opacity = 0;
                BeginInvoke(new MethodInvoker(delegate
                {
                    this.Hide();
                }));
            }
        }

        private void Config_Click(object sender, EventArgs e)
        {
            showWindow();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                Config config = new Config
                {
                    server = textBox1.Text,
                    server_port = int.Parse(textBox2.Text),
                    password = textBox3.Text,
                    local_port = int.Parse(textBox4.Text),
                    method = comboBox1.Text,
                    isDefault = false
                };
                controller.SaveConfig(config);
                this.Hide();
            }
            catch (FormatException)
            {
                MessageBox.Show("illegal port number format");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            updateUI();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.Stop();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/clowwindy/shadowsocks-csharp");
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            showWindow();
        }


        private void EnableItem_Click(object sender, EventArgs e)
        {
            enableItem.Checked = !enableItem.Checked;
            controller.ToggleEnable(enableItem.Checked);
        }

    }
}
