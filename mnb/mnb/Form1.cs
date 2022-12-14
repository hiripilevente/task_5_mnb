using mnb.Entities;
using mnb.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace mnb
{

    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();


        public Form1()
        {
            dataGridView1.DataSource= Rates;

            InitializeComponent();
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
        

    }
}
