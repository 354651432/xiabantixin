using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Windows.Forms.NotifyIcon notifyIcon;
        public App()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = "托盘图标",
                Icon = WpfApp1.Properties.Resources._163_com,
                Visible = true,
            };

            var isHide = false;
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] {
            new System.Windows.Forms.MenuItem("隐藏",(Object t,EventArgs e)=>{
                var item = t as System.Windows.Forms.MenuItem;
                if(isHide)
                {
                    item.Text = "隐藏";
                    MainWindow.Show();
                }else
                {
                     item.Text = "隐藏";
                    MainWindow.Hide();
                }

                isHide = !isHide;
            }),
            new System.Windows.Forms.MenuItem("退出",(e,t)=>Shutdown()),
            });
        }



        DateTime startup;
        Label span;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            startup = getStartupTime();
            Label time = MainWindow.FindName("time") as Label;
            time.Content = startup.ToString();
            span = MainWindow.FindName("span") as Label;

            new Timer(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    var spanTime = DateTime.Now - startup;
                    span.Content = String.Format("{0}小时{1}分钟", spanTime.Hours, spanTime.Minutes);
                    if (spanTime.Hours >= 9)
                    {
                        span.Foreground = Brushes.Red;
                        span.FontWeight = FontWeights.Bold;
                        //notifyIcon.ShowBalloonTip(2000, "", "可以下班了", System.Windows.Forms.ToolTipIcon.None);
                    }

                });
            }, null, 0, 60 * 1000);
        }

        static private DateTime getStartupTime()
        {
            var logs = EventLog.GetEventLogs();
            var list = new List<DateTime>();
            foreach (EventLog log in logs)
            {
                if (log.Log != "Application")
                {
                    break;
                }
                foreach (EventLogEntry entity in log.Entries)
                {
                    if (entity.TimeGenerated.Date != DateTime.Today)
                    {
                        continue;
                    }
                    if (list.Count > 100)
                    {
                        break;
                    }
                    list.Add(entity.TimeGenerated);
                }
            }

            list.Sort((a, b) => a > b ? 1 : 0);
            return list.Find(_ => true);
        }
    }
}
