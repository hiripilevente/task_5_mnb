using mnb.Entities;
using mnb.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace mnb
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currencies = new BindingList<string>();

        public Form1()
        {
            InitializeComponent();
            dataGridView1.DataSource = Rates;
            cbx1.DataSource = Currencies;
            //GetRates();
            GetCurrencies();
            RefreshData();
        }

        void GetCurrencies()
        {
            MNBArfolyamServiceSoapClient m = new MNBArfolyamServiceSoapClient();
            GetCurrenciesRequestBody request = new GetCurrenciesRequestBody();
            GetCurrenciesResponseBody response = m.GetCurrencies(request);
            string result = response.GetCurrenciesResult;
            XmlDocument x = new XmlDocument();
            x.LoadXml(result);
            MessageBox.Show(result);
            XmlElement item = x.DocumentElement;
            int i = 0;
            while (item.ChildNodes[0].ChildNodes[i] != null)
            {
                Currencies.Add(item.ChildNodes[0].ChildNodes[i].InnerText);
                i++;
            }
            m.Close();
        }
        
        private void RefreshData()
        {
            Rates.Clear();
            ReadXml();
            chartRateData.DataSource = Rates;
            chartRateData.Series[0].ChartType = SeriesChartType.Line;
            chartRateData.Series[0].XValueMember = "date";
            chartRateData.Series[0].YValueMembers = "value";
            chartRateData.Series[0].BorderWidth = 2;
            chartRateData.Legends[0].Enabled = false;
            chartRateData.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chartRateData.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chartRateData.ChartAreas[0].AxisY.IsStartedFromZero = false;
        }

        private void ReadXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(GetRates());
            foreach (XmlElement item in xml.DocumentElement)
            {
                if (item.ChildNodes[0] != null)
                {
                    RateData rd = new RateData();
                    Rates.Add(rd);
                    rd.Currency = item.ChildNodes[0].Attributes["curr"].Value;
                    rd.Date = Convert.ToDateTime(item.Attributes["date"].Value);
                    decimal unit = Convert.ToDecimal(item.ChildNodes[0].Attributes["unit"].Value);
                    decimal value = Convert.ToDecimal(item.ChildNodes[0].InnerText);
                    if (unit != 0)
                    {
                        rd.Value = value / unit;
                    }
                    else
                    {
                        rd.Value = value;
                    }
                }
            }
        }

        private string GetRates()
        {
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetExchangeRatesRequestBody request = new GetExchangeRatesRequestBody()
            {
                currencyNames = cbx1.SelectedItem.ToString(),
                startDate = dateTimePicker1.Value.ToString(),
                endDate = dateTimePicker2.Value.ToString()
            };
            GetExchangeRatesResponseBody response = mnbService.GetExchangeRates(request);
            string result = response.GetExchangeRatesResult;
            //MessageBox.Show(result);

            mnbService.Close();
            return result;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void cbx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }

}
