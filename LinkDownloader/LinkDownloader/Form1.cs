using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Calculate(int i)
        {
            double pow = Math.Pow(i, i);
        }

        public void DoWork(IProgress<int> progress)
        {
            // This method is executed in the context of
            // another thread (different than the main UI thread),
            // so use only thread-safe code
            for (int j = 0; j < 100000; j++)
            {
                Calculate(j);

                // Use progress to notify UI thread that progress has
                // changed
                if (progress != null)
                    progress.Report((j + 1) * 100 / 100000);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if((dateTimePicker1.Value <= dateTimePicker2.Value))
            {
                label7.Visible = true;
                label9.Visible = true;
                String url = "";
                DateTime dt = dateTimePicker1.Value;
                DateTime dt1 = dateTimePicker2.Value;
                string keyValue = textBox1.Text;
                int x = comboBox1.SelectedIndex;
                var calculatedDates = new List<string>(AllDatesBetween(DateTime.Parse(dt.ToString()), DateTime.Parse(dt1.ToString())).Select(d => d.ToString("yyyy-MM-dd")));

                progressBar1.Maximum = 100;
                progressBar1.Step = 1;

                var progress = new Progress<int>(v =>
                {
                    // This lambda is executed in context of UI thread,
                    // so it can safely update form controls
                    progressBar1.Value = v;
                });

                switch (x)
                {
                    case 0: url = "https://gz.blockchair.com/bitcoin/transactions/blockchair_bitcoin_transactions_"; break;
                    case 1: url = "https://gz.blockchair.com/bitcoin-cash/transactions/blockchair_bitcoin-cash_transactions_"; break;
                    case 2: url = "https://gz.blockchair.com/bitcoin-sv/transactions/blockchair_bitcoin-sv_transactions_"; break;
                    case 3: url = "https://gz.blockchair.com/dash/transactions/blockchair_dash_transactions_"; break;
                    case 4: url = "https://gz.blockchair.com/dogecoin/transactions/blockchair_dogecoin_transactions_"; break;
                    case 5: url = "https://gz.blockchair.com/ethereum/transactions/blockchair_ethereum_transactions_"; break;
                    case 6: url = "https://gz.blockchair.com/litecoin/transactions/blockchair_litecoin_transactions_"; break;
                    case 7: url = "https://gz.blockchair.com/zcash/transactions/blockchair_zcash_transactions_"; break;

                }

                try
                {
                    int counter = calculatedDates.Count;

                    foreach (String date in calculatedDates)
                    {
                        counter--;
                        string date1 = date.Replace("-", string.Empty);
                        // Run operation in another thread
                        await Task.Run(() => DoWork(progress));
                        label9.Text = counter.ToString();
                        label7.Text = date1 + ".tsv.gz";
                        label9.Refresh();
                        label7.Refresh();
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(url + date1 + ".tsv.gz?key=" + keyValue, date1 + ".tsv.gz");
                        }

                    }

                    label7.Visible = false;
                    label9.Visible = false;
                    MessageBox.Show("Operations are completed!", "Successful Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Detail:" + ex, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }  else
                    MessageBox.Show("Start Interval date cannot be greater than End Interval Date!", "Opps!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        public static IEnumerable<DateTime> AllDatesBetween(DateTime start, DateTime end)
        {
            for (var day = start.Date; day <= end; day = day.AddDays(1))
                yield return day;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.MaxDate = DateTime.Today;
            dateTimePicker2.MaxDate = DateTime.Today;
            label7.Visible = false;
            label9.Visible = false;
        }
    }
}
