using SMPMonitor.Scraper;
using System.Data;
using System.Media;

namespace SMPMonitor
{
    public partial class Form1 : Form
    {
        SMPReport latestSMP = new SMPReport();
        List<string> fileList = new List<string>();
        public DataTable table;
        public Form1()
        {
            InitializeComponent();
            Directory.CreateDirectory("C:\\APPS\\SMPReport\\");
            table = new DataTable();
            latestSMP = GetLatest.GetLatestFromApi();
            comboBox1.Items.Add("Latest");
            UpdateSMP();
            timer.Tick += new System.EventHandler(timer_Tick);
            timer.Interval = 5000;
            timer.Start();
            //fileList = Directory.GetFiles("C:\\APPS\\SMPReport\\").OrderByDescending(d => new FileInfo(d).CreationTime).Select(f => Path.GetFileNameWithoutExtension(f)).ToList();

            comboBox1.Items.AddRange(fileList.ToArray());
            comboBox1.SelectedIndex = 0;

            BindingSource sBind = new BindingSource();
            sBind.DataSource = table;

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = table;
            //dataGridView1.DataSource = sBind;

            openFileDialog1.Filter = "Sound Files|*.mp3;*.wav;*.aac;*.flac;";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void timer_Tick(object sender, EventArgs e)
        {
            //textBox1.Text = timer.Interval.ToString();
            try
            {
                SMPReport smp = GetLatest.GetLatestFromApi();
                if (smp.begin_datetime_mpt != latestSMP.begin_datetime_mpt)
                {
                    latestSMP = smp;
                    UpdateSMP();
                    if (!checkBox1.Checked)
                    {
                        if (string.IsNullOrEmpty(textBox1.Text))
                        {
                            SystemSounds.Asterisk.Play();
                        }
                        else
                        {
                            SoundPlayer player = new SoundPlayer(textBox1.Text);
                            player.Play();
                        }
                    }
                }
                label8.Text = "Last Updated On: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                label8.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                label8.Text = "Error with latest pull at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                label8.BackColor = Color.Red;
            }

        }

        private void UpdateSMP()
        {
            dataGridView2.Rows.Clear();
            int hourEnd = latestSMP.begin_datetime_mpt.Hour + 1;
            dataGridView2.Rows.Add(latestSMP.begin_datetime_mpt.ToString("yyyy-MM-dd") + " " + hourEnd, latestSMP.begin_datetime_mpt.ToString("HH:mm:ss"), latestSMP.system_marginal_price.ToString(), latestSMP.volume.ToString());

            //Projected Pool
            ProjectedPool pool = PageScraper.ExportPageData();
            textBox2.Text = pool.hourEnding;
            textBox3.Text = pool.price;
            textBox4.Text = pool.asOfTime;

            var latestFiles = Directory.GetFiles("C:\\APPS\\SMPReport\\").OrderByDescending(d => new FileInfo(d).CreationTime).Select(f => Path.GetFileNameWithoutExtension(f)).ToList();

            var missingFiles = latestFiles.Except(fileList);
            int i = 1;
            foreach (var file in missingFiles)
            {
                comboBox1.Items.Insert(i, file);
                i++;
            }

            fileList = latestFiles;

            string fileName = "";
            if (comboBox1.SelectedIndex == 0)
            {
                fileName = "C:\\APPS\\SMPReport\\" + fileList[0] + ".csv";
                table = readCSV(fileName);
                dataGridView1.DataSource = table;
            }
            //comboBox1.Items.AddRange(Directory.GetFiles(@"C:\\APPS\\SMPReport\\"));
            //GetFileList
            //LoadLatestCSV
        }

        private void GetFileList()
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (timer.Enabled) { timer.Stop(); } else { timer.Start(); }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer.Interval = ((int)numericUpDown1.Value) * 1000;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SMPReport smp = GetLatest.GetLatestFromApi();
            if (smp.begin_datetime_mpt != latestSMP.begin_datetime_mpt)
            {
                latestSMP = smp;
                UpdateSMP();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fileName = "";
            if (comboBox1.SelectedIndex == 0)
            {
                fileName = "C:\\APPS\\SMPReport\\" + fileList[0] + ".csv";
            }
            else
            {
                fileName = "C:\\APPS\\SMPReport\\" + comboBox1.Text + ".csv";
            }
            table = readCSV(fileName);
            dataGridView1.DataSource = table;
        }

        public DataTable readCSV(string filePath)
        {
            var dt = new DataTable();
            // Creating the columns
            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            // Adding the rows
            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split(','))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));
            return dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(openFileDialog1.FileName);
                string filePath = openFileDialog1.FileName;
                textBox1.Text = filePath;
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}