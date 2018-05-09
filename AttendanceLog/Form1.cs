using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace AttendanceLog
{
    public partial class Form1 : Form
    {
        CDA _db = new CDA();
        int count = 0;
        Thread thread;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Start();

        }

        private void Start()
        {
            thread = new Thread(HitApp);
            thread.Start();

        }

        void HitApp()
        {
            
            lblStatus.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = @"Running...";
            });
            
            
            try
            {
               
                sendPush();
                SleepTime(1000 * 60 * 5);
                HitApp();

            }


            catch (Exception ex)
            {
                if (ex.Message != String.Empty)
                {
                    lblStatus.Invoke((MethodInvoker)delegate { lblStatus.Text = ex.Message; });
                }
            }
        }

        private void ResetStatus()
        {
            lblStatus.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = @"Sleeping...";
            });
        }

        private void InsertResponse(string res)
        {
            if (res.Contains("1"))
            {
                lblStatus.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = @"Data Insert Done...";
                });
            }
        }

        private void SleepTime(int ms)
        {
            lblStatus.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = @"Sleeping for 5 min... ";
            });
            Thread.Sleep(ms);
            

        }
        public void sendPush()
        {
            SendService sendService = new SendService();


            DataSet ds = new CDA().GetDataSet("EXEC [Firebase].dbo.spGetTokenForDesktop_ForAllApps", "MYCHOICE");

            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string res = Post.SendPushNotification(dr["Token"].ToString(), dr["ContentCode"].ToString(), dr["PushTblId"].ToString(),
                        dr["Refid"].ToString());

                    FirebaseResult.Rootobject r = Newtonsoft.Json.JsonConvert.DeserializeObject<FirebaseResult.Rootobject>(res);

                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        token = dr["Token"],
                        Id = dr["RefId"]
                    });
                    new CDA().ExecuteNonQuery("EXEC [Firebase].dbo.spUpdateTokenForDesktop_ForAllApps '" + dr["Id"] + "'", "MYCHOICE");
                    if (r.success == 1)
                    {
                        sendService.Send("https://wap.shabox.mobi/fbandroid/api/TokenManage/SendLog", json);
                    }
                    else
                    {
                        sendService.Send("https://wap.shabox.mobi/fbandroid/api/TokenManage/FailedLog", json);
                        sendService.Send("https://wap.shabox.mobi/fbandroid/api/TokenManage/DeactiveToken", json);
                    }


                }
            }
            


        }

       
        private void btnStop_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
