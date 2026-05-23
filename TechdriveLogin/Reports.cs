using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public partial class Reports : TechdriveLogin.tdLogin.Template
    {
        public Reports()
        {
            InitializeComponent();
            this.Load += (s, e) => LoadReportsData();
        }

        private void LoadReportsData()
        {
            try
            {
                var reports = DatabaseHelper.GetVehicleReports(8);

                Label[] vmLabels = { reportVMLbl1, reportVMLbl2, reportVMLbl3, reportVMLbl4, reportVMLbl5, reportVMLbl6, reportVMLbl7, reportVMLbl8 };
                Label[] pnLabels = { reportPNLbl1, reportPNLbl2, reportPNLbl3, reportPNLbl4, reportPNLbl5, reportPNLbl6, reportPNLbl7, reportPNLbl8 };
                Label[] lmdLabels = { lblLmd1, lblLmd2, lblLmd3, lblLmd4, lblLmd5, lblLmd6, lblLmd7, lblLmd8 };
                Label[] tbtmLabels = { lblTBTM1, lblTBTM2, lblTBTM3, lblTBTM4, lblTBTM5, lblTBTM6, lblTBTM7, lblTBTM8 };
                Label[] netLabels = { lblNetEarnings1, lblNetEarnings2, lblNetEarnings3, lblNetEarnings4, lblNetEarnings5, lblNetEarnings6, lblNetEarnings7, lblNetEarnings8 };

                for (int i = 0; i < 8; i++)
                {
                    if (i < reports.Count)
                    {
                        var rep = reports[i];
                        if (vmLabels[i] != null) vmLabels[i].Text = rep.VehicleModel;
                        if (pnLabels[i] != null) pnLabels[i].Text = rep.PlateNumber;
                        if (lmdLabels[i] != null) lmdLabels[i].Text = rep.LastMaintenance;
                        if (tbtmLabels[i] != null) tbtmLabels[i].Text = rep.TotalTrips.ToString();
                        if (netLabels[i] != null) netLabels[i].Text = $"₱{rep.NetEarnings:N2}";

                        if (vmLabels[i] != null) vmLabels[i].Visible = true;
                        if (pnLabels[i] != null) pnLabels[i].Visible = true;
                        if (lmdLabels[i] != null) lmdLabels[i].Visible = true;
                        if (tbtmLabels[i] != null) tbtmLabels[i].Visible = true;
                        if (netLabels[i] != null) netLabels[i].Visible = true;
                    }
                    else
                    {
                        // Hide extra slots if we don't have enough data
                        if (vmLabels[i] != null) vmLabels[i].Visible = false;
                        if (pnLabels[i] != null) pnLabels[i].Visible = false;
                        if (lmdLabels[i] != null) lmdLabels[i].Visible = false;
                        if (tbtmLabels[i] != null) tbtmLabels[i].Visible = false;
                        if (netLabels[i] != null) netLabels[i].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
