using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Timers;
using System.Threading;

namespace HuyangForm
{
    public partial class Form1 : Form
    {
        private string id = string.Empty;
        private string pw = string.Empty;

        private string availMonth = string.Empty;
        private System.Timers.Timer timer = new System.Timers.Timer();

        //Huyang huyang = new Huyang();
        Huyang[] huyang = {
                              new Huyang(),
                              new Huyang(),
                              new Huyang(),
                              new Huyang(),
                              new Huyang()
                          };

        public Form1()
        {
            InitializeComponent();

            LoadDB();

            List<Object> items = new List<Object>();
            items.Add(new { Text = "1박2일", Value = 1 });
            items.Add(new { Text = "2박3일", Value = 2 });
            items.Add(new { Text = "3박4일", Value = 3 });
            cbxPeriod.DisplayMember = "Text";
            cbxPeriod.ValueMember = "Value";
            cbxPeriod.DataSource = items;
        }

        private void cbxDprtmInit()
        {
            List<Object> items = new List<Object>();

            int ret = huyang[0].GetSelectableDprtm(ref items, ref availMonth);
            if (ret < 0)
            {
                logListBox.Items.Add("휴양림 코드를 읽어올 수 없습니다.");
                return;
            }

            cbxDprtm.DisplayMember = "Text";
            cbxDprtm.ValueMember = "Value";
            cbxDprtm.DataSource = items;
        }
        private void dprtmIndexChanged(object sender, EventArgs e)
        {
            string dprtm = cbxDprtm.SelectedValue.ToString();
            List<Object> items = new List<Object>();

            int ret = huyang[0].GetSelectablefcltMdcls(ref items, dprtm);
            if (ret < 0)
            {
                logListBox.Items.Add("시설 코드를 읽어올 수 없습니다.");
                return;
            }

            cbxfcltMdclsCd.DisplayMember = "Text";
            cbxfcltMdclsCd.ValueMember = "Value";
            cbxfcltMdclsCd.DataSource = items;            
        }
        private void fcltMdclsIndexChanged(object sender, EventArgs e)
        {
            string dprtm = cbxDprtm.SelectedValue.ToString();
            string fcltMdcls = cbxfcltMdclsCd.SelectedValue.ToString();
            List<Object> items = new List<Object>();

            int ret = huyang[0].GetSelectableRooms(ref items, dprtm, fcltMdcls, availMonth);
            if (ret < 0)
            {
                logListBox.Items.Add("방 코드를 읽어올 수 없습니다.");
                cbxRoom.Enabled = false;
                return;
            }

            cbxRoom.Enabled = true;
            cbxRoom.DisplayMember = "Text";
            cbxRoom.ValueMember = "Value";
            cbxRoom.DataSource = items;
        }
        private void roomIndexChanged(object sender, EventArgs e)
        {
            cbxDprtm.Enabled = true;
            cbxfcltMdclsCd.Enabled = true;
            cbxRoom.Enabled = true;
            cbxPeriod.Enabled = true;
            dateReserve.Enabled = true;
            btnAdd.Enabled = true;
        }

        private void addListViewItem(string dprtmName, string roomName, string periodText, string dateText, string rsrvtQntt, string dprtmId, string fcltId)
        {
            ListViewItem lvi = new ListViewItem(dprtmName);
            lvi.SubItems.Add(roomName);
            lvi.SubItems.Add(periodText);
            lvi.SubItems.Add(dateText);

            lvi.SubItems.Add(rsrvtQntt);
            lvi.SubItems.Add(dprtmId);
            lvi.SubItems.Add(fcltId);
            listView1.Items.Add(lvi);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string YYYY = dateReserve.Value.Year.ToString("D4");
            string MM = dateReserve.Value.Month.ToString("D2");
            string DD = dateReserve.Value.Day.ToString("D2");

            addListViewItem(
                /*휴양*/ cbxDprtm.Text, 
                /*숙소*/ cbxRoom.Text, 
                /*기간*/ cbxPeriod.Text, 
                /*날짜*/ YYYY + MM + DD,
                /*rsrvtQntt*/   cbxPeriod.SelectedValue.ToString(),
                /*dprtmId*/     cbxDprtm.SelectedValue.ToString(),
                /*fcltId*/      cbxRoom.SelectedValue.ToString()
            );
        }
        private void LoadDB()
        {
            FileInfo fi = new FileInfo(@"db.txt");
            if (fi.Exists == false)
                return;

            bool isFirst = true;
            string pattern = string.Empty;
            Regex regex;
            Match match;
            string[] lines = System.IO.File.ReadAllLines(@"db.txt");

            foreach (string line in lines)
            {
                /* parse id,pw */
                if (isFirst)
                {
                    pattern = "(.*?),(.*?)$";
                    regex = new Regex(pattern);
                    match = regex.Match(line);
                    if (match.Success == false)
                        return;

                    txtID.Text = match.Groups[1].Value;
                    txtPW.Text = match.Groups[2].Value;

                    isFirst = false;
                    continue;
                }

                /* parse rooms */
                pattern = "(.*?),(.*?),(.*?),(.*?),(.*?),(.*?),(.*?)$";
                regex = new Regex(pattern);
                match = regex.Match(line);
                if (match.Success == false)
                    return;

                for (; match.Success; match = match.NextMatch())
                {
                    addListViewItem(
                        match.Groups[1].Value,
                        match.Groups[2].Value,
                        match.Groups[3].Value,
                        match.Groups[4].Value,
                        match.Groups[5].Value,
                        match.Groups[6].Value,
                        match.Groups[7].Value
                    );
                }

                
            }
        }
        private void SaveDB()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("db.txt");
            string db = string.Empty;

            db = string.Format("{0},{1}", txtID.Text, txtPW.Text);
            file.WriteLine(db);

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                db = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    /*휴양*/ listView1.Items[i].Text,
                    /*숙소*/ listView1.Items[i].SubItems[1].Text,
                    /*기간*/ listView1.Items[i].SubItems[2].Text,
                    /*날짜*/ listView1.Items[i].SubItems[3].Text,
                    /*rsrvtQntt*/   listView1.Items[i].SubItems[4].Text,
                    /*dprtmId*/     listView1.Items[i].SubItems[5].Text,
                    /*fcltId*/      listView1.Items[i].SubItems[6].Text
                    );

                file.WriteLine(db);
            }
            file.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Length == 0 || txtPW.Text.Length == 0)
            {
                MessageBox.Show("아이디 또는 비밀번호를 입력하세요.");
                return;
            }

            foreach (Huyang hy in huyang)
            {
                if (hy.Login(txtID.Text, txtPW.Text) < 0)
                {
                    MessageBox.Show("로그인 실패");
                    return;
                }
            }

            logListBox.Items.Add("로그인 성공");
            txtID.Enabled = false;
            txtPW.Enabled = false;
            btnLogin.Enabled = false;

            btnChoose.Enabled = true;
            btnDelete.Enabled = true;
            btnSave.Enabled = true;
            btnReserve.Enabled = true;
            //btnAutoReserve.Enabled = true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDB();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].Remove();
                listView1.Update();
            }
        }

        private void runReserve(object args)
        {
            Array argArray = new object[5];
            argArray = (Array)args;

            int threadIdx = (int)argArray.GetValue(0);
            string dprtmName = (string)argArray.GetValue(1);
            string roomName = (string)argArray.GetValue(2);
            string paramDprtmId = (string)argArray.GetValue(3);
            string fcltId = (string)argArray.GetValue(4);

            int ret = huyang[threadIdx].Select(paramDprtmId, fcltId);
            logListBox.Items.Add(dprtmName + " : " + roomName + " 예약 " + (ret < 0 ? "실패" : "성공"));
        }
        private void btnReserve_Click(object sender, EventArgs e)
        {
            AutoReserve_Cancel();

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (i > huyang.Length - 1)
                    break;

                string dprtmName = listView1.Items[i].Text;
                string roomName = listView1.Items[i].SubItems[1].Text;

                string rsrvtQntt = listView1.Items[i].SubItems[4].Text;
                string dprtmId = listView1.Items[i].SubItems[5].Text;
                string fcltId = listView1.Items[i].SubItems[6].Text;

                string YYYY = listView1.Items[i].SubItems[3].Text.Substring(0, 4);
                string MM = listView1.Items[i].SubItems[3].Text.Substring(4, 2);
                string DD = listView1.Items[i].SubItems[3].Text.Substring(6, 2);

                huyang[i].SetParams(rsrvtQntt, YYYY, MM, DD, dprtmId, fcltId); //TODO: 방번호(fcltId) -> 휴양림타입(fcltMdclsCd)

                /* reserve as thread */
                Thread th = new Thread(new ParameterizedThreadStart(runReserve));
                th.Start(new object[5] { i, dprtmName, roomName, dprtmId, fcltId });
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            cbxDprtmInit();
            btnChoose.Enabled = false;
        }

        private string txtCancel = "예약취소";
        private string txtReserve = "자동예약";
        private bool AutoReserve_Cancel()
        {
            if (string.Compare(btnAutoReserve.Text, txtCancel) == 0)
            {
                timer.Stop();
                btnAutoReserve.Text = txtReserve;
                logListBox.Items.Add("자동예약이 취소되었습니다.");
                return true;
            }
            return false;
        }
        private void btnAutoReserve_Click(object sender, EventArgs e)
        {
            if (AutoReserve_Cancel())
                return;

            DateTime dtNow = DateTime.Now;
            DateTime dtTarget = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 9, 0, 0);
            if (dtTarget < dtNow)
                dtTarget = dtTarget.AddDays(1);

            TimeSpan ts = dtTarget - dtNow;
            int margin_ms = 450;
            int diff_ms = ((ts.Hours * 60 * 60) + (ts.Minutes * 60) + ts.Seconds) * 1000 + (ts.Milliseconds) + margin_ms;

            timer.Interval = diff_ms;
            timer.Elapsed += new ElapsedEventHandler(btnReserve_Click);
            timer.Start();

            string msg = string.Format("{0}시간 {1}분 {2}.{3}초 후 자동예약이 시작됩니다."
                                        , ts.Hours.ToString()
                                        , ts.Minutes.ToString()
                                        , ts.Seconds.ToString()
                                        , ts.Milliseconds.ToString());
            logListBox.Items.Add(msg);
            btnAutoReserve.Text = txtCancel;
        }
    }
}
